using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttendanceController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn([FromBody] AttendanceDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users
            .Include(u => u.UserWorkSchedules)
                .ThenInclude(s => s.WorkSchedule)
                .ThenInclude(ws => ws.ScheduleDays)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound("User not found.");

        var nowLocal = GetUserLocalTime(user);
        var today = DateOnly.FromDateTime(nowLocal);
        var todayDayOfWeek = nowLocal.DayOfWeek;

        var existing = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (existing != null) return BadRequest("Already clocked in today.");
        
        var attendance = new Attendance
        {
            UserId = userId,
            Date = today,
            TimeIn = TimeOnly.FromDateTime(nowLocal),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClockInSource = Attendance.ClockInSourceEnum.Manual,
            Notes = dto.Notes
        };

        var scheduleDay = user.UserWorkSchedules
            .SelectMany(us => us.WorkSchedule.ScheduleDays)
            .FirstOrDefault(sd => sd.Day == todayDayOfWeek);

        if (scheduleDay?.StartTime != null)
        {
            var actualInTime = TimeOnly.FromDateTime(nowLocal);
            
            if (actualInTime > scheduleDay.StartTime.Value)
            {
                var difference = actualInTime - scheduleDay.StartTime.Value;
                attendance.LateMinutes = (int)difference.TotalMinutes;
            }
        }
        
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Clocked in successfully." });
    }

    [Authorize]
    [HttpPatch("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users
            .Include(u => u.UserWorkSchedules)
                .ThenInclude(s => s.WorkSchedule)
                .ThenInclude(ws => ws.ScheduleDays) 
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound("User not found.");

        var attendance = await _context.Attendances
            .Where(a => a.UserId == userId && a.TimeOut == null)
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.TimeIn)
            .FirstOrDefaultAsync();

        if (attendance == null)
            return BadRequest("You are not currently clocked in.");

        var nowLocal = GetUserLocalTime(user);
        var nowUtc = DateTime.UtcNow;

        var duration = TimeOnly.FromDateTime(nowLocal) - attendance.TimeIn;
        
        var totalMinutes = (int)duration.TotalMinutes - (int)attendance.BreakDuration.TotalMinutes;

        attendance.TimeOut = TimeOnly.FromDateTime(nowLocal);
        attendance.TotalWorkMinutes = Math.Max(0, totalMinutes); // Ensure no negative minutes
        attendance.UpdatedAt = nowUtc;

        if (attendance.LateMinutes == 0)
            attendance.Status = Attendance.AttendanceStatus.Present;
        else
            attendance.Status = Attendance.AttendanceStatus.Late;

        var todayDayOfWeek = nowLocal.DayOfWeek;
        var scheduleDay = user.UserWorkSchedules
            .SelectMany(us => us.WorkSchedule.ScheduleDays)
            .FirstOrDefault(sd => sd.Day == todayDayOfWeek);

        if (scheduleDay?.StartTime != null && scheduleDay?.EndTime != null)
        {
            var expectedMinutes = (int)(scheduleDay.EndTime.Value - scheduleDay.StartTime.Value).TotalMinutes;
            if (attendance.TotalWorkMinutes < (expectedMinutes / 2))
                attendance.Status = Attendance.AttendanceStatus.HalfDay;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Clocked out successfully." });
    }

    [Authorize]
    [HttpPatch("start-break")]
    public async Task<IActionResult> StartBreak()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found.");

        var attendance = await _context.Attendances
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.TimeIn)
            .FirstOrDefaultAsync();

        if (attendance == null)
            return BadRequest("No attendance record found. Did you clock in?");

        if (attendance.TimeOut != null)
            return BadRequest("You have already clocked out for your last session.");

        if (attendance.BreakStart != null)
            return BadRequest("You are already on a break.");
        
        var nowLocal = GetUserLocalTime(user);
        attendance.BreakStart = TimeOnly.FromDateTime(nowLocal);
        attendance.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Break started successfully.", breakStart = attendance.BreakStart });
    }

    [Authorize]
    [HttpPatch("end-break")]
    public async Task<IActionResult> EndBreak()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await _context.Users
            .Include(u => u.UserWorkSchedules)
                .ThenInclude(s => s.WorkSchedule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found.");

        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (attendance == null)
            return BadRequest("No attendance found.");

        if (attendance.BreakStart == null)
            return BadRequest("Break not started!");
        
        var now = DateTime.UtcNow;
        var nowLocal = GetUserLocalTime(user);
        
        var breakTime = TimeOnly.FromDateTime(nowLocal) - attendance.BreakStart.Value;

        attendance.BreakDuration = attendance.BreakDuration + breakTime;
        attendance.BreakStart = null;
        attendance.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Break ended." });
    }
    private DateTime GetUserLocalTime(User user)
    {
        if (string.IsNullOrEmpty(user.TimeZone))
            return DateTime.UtcNow;

        try
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, user.TimeZone);
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserAttendance(int id)
    {
        var result = await _context.Attendances
            .Where(a => a.UserId == id)
            .Select(a => new
            {
                a.Date,
                a.TimeIn,
                a.TimeOut,
                BreakDuration = a.BreakDuration.TotalMinutes == 0
                    ? "0 m"
                    : a.BreakDuration.TotalHours < 1
                        ? $"{a.BreakDuration.Minutes} m"
                        : a.BreakDuration.Minutes == 0
                            ? $"{(int)a.BreakDuration.TotalHours} hr"
                            : $"{(int)a.BreakDuration.TotalHours} hr {a.BreakDuration.Minutes} m",
                a.LateMinutes,
                a.TotalWorkMinutes,
                TotalWorkTime = a.TotalWorkMinutes == 0
                    ? "0 m"
                    : a.TotalWorkMinutes < 60
                        ? $"{a.TotalWorkMinutes} m"
                        : a.TotalWorkMinutes % 60 == 0
                            ? $"{a.TotalWorkMinutes / 60} hr"
                            : $"{a.TotalWorkMinutes / 60} hr {a.TotalWorkMinutes % 60} m",
                a.ClockInSource,
                a.Status,
                a.Notes
            })
            .FirstOrDefaultAsync();

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAttendance()
    {
        var data = await _context.Attendances
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.User.FirstName,
                a.User.MiddleName,
                a.User.LastName,
                a.Date,
                a.TimeIn,
                a.TimeOut,
                a.BreakDuration,
                a.LateMinutes,
                a.TotalWorkMinutes,
                a.ClockInSource,
                a.Status,
                a.Notes
            })
            .ToListAsync();

        var attendances = data.Select(a => new
            {
                a.Id,
                a.UserId,
                FullName = string.Join(" ", new[]
                {
                    a.FirstName,
                    a.MiddleName,
                    a.LastName
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                a.Date,
                TimeIn = a.TimeIn.ToString("HH:mm"),
                TimeOut = a.TimeOut.HasValue ? a.TimeOut.Value.ToString("HH:mm") : null,
                BreakDuration = a.BreakDuration.TotalMinutes == 0
                    ? "0 m"
                    : a.BreakDuration.TotalHours < 1
                        ? $"{a.BreakDuration.Minutes} m"
                        : a.BreakDuration.Minutes == 0
                            ? $"{(int)a.BreakDuration.TotalHours} hr"
                            : $"{(int)a.BreakDuration.TotalHours} hr {a.BreakDuration.Minutes} m",
                a.LateMinutes,
                a.TotalWorkMinutes,
                TotalWorkTime = a.TotalWorkMinutes == 0
                    ? "0 m"
                    : a.TotalWorkMinutes < 60
                        ? $"{a.TotalWorkMinutes} m"
                        : a.TotalWorkMinutes % 60 == 0
                            ? $"{a.TotalWorkMinutes / 60} hr"
                            : $"{a.TotalWorkMinutes / 60} hr {a.TotalWorkMinutes % 60} m",
                a.ClockInSource,
                a.Status,
                a.Notes
            });

        return Ok(attendances);
    }
}