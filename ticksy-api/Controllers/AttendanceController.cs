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

        [Authorize]
        [HttpGet("today-status")]
        public async Task<IActionResult> GetTodayStatus()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var attendance = await _context.Attendances
                .Where(a => a.UserId == userId && a.TimeOut == null)
                .OrderByDescending(a => a.Date)
                .FirstOrDefaultAsync();

            if (attendance == null) return Ok(new { isClockedIn = false });

            return Ok(new {
                isClockedIn = true,
                isOnBreak = attendance.BreakStart != null,
                timeIn = attendance.CreatedAt,
                breakStart = attendance.BreakStart
            });
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
                .ToListAsync();

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

        [Authorize]
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyAttendance(DateTime start, DateTime end, int? userId = null)
        {
            var requesterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                userId = requesterId; 
            }
            var startDate = DateOnly.FromDateTime(start);
            var endDate = DateOnly.FromDateTime(end);

            var query = _context.Attendances
                .Include(a => a.User)
                .Where(a => a.Date >= startDate && a.Date <= endDate);

            if (userId != null)
            {
                query = query.Where(a => a.UserId == userId);
            }

            var data = await query.ToListAsync();

            var result = data
                .GroupBy(a => new
                {
                    a.UserId,
                    FullName = string.Join(" ", new[]
                    {
                        a.User.FirstName,
                        a.User.MiddleName,
                        a.User.LastName
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),

                    AvatarUrl = a.User.AvatarUrl
                })
                .Select(group =>
                {
                    var days = Enumerable.Repeat("0", 7).ToArray();
                    int total = 0;

                    foreach (var a in group)
                    {
                        var day = a.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek;

                        int index = day == DayOfWeek.Sunday ? 6 : (int)day - 1;

                        if (a.TotalWorkMinutes > 0)
                            days[index] = FormatMinutes(a.TotalWorkMinutes);

                        total += a.TotalWorkMinutes;
                    }

                    return new
                    {
                        userId = group.Key.UserId,
                        name = group.Key.FullName,
                        avatar = group.Key.AvatarUrl,
                        days,
                        total = FormatMinutes(total)
                    };
                })
                .ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyAttendance(DateTime date, int? userId = null)
        {
            var requesterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                userId = requesterId; 
            }
            var selectedDate = DateOnly.FromDateTime(date);

            var query = _context.Attendances
                .Include(a => a.User)
                .Where(a => a.Date == selectedDate);

            if (userId != null)
            {
                query = query.Where(a => a.UserId == userId);
            }

            var data = await query.ToListAsync();

            var result = data
                .GroupBy(a => new
                {
                    a.UserId,
                    FullName = string.Join(" ", new[]
                    {
                        a.User.FirstName,
                        a.User.MiddleName,
                        a.User.LastName
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),

                    AvatarUrl = a.User.AvatarUrl
                })
                .Select(group =>
                {
                    var firstIn = group
                        .Select(a => (TimeOnly?)a.TimeIn)
                        .Min();

                    var lastOut = group
                        .Where(a => a.TimeOut.HasValue)
                        .Select(a => a.TimeOut)
                        .DefaultIfEmpty()
                        .Max();

                    var totalMinutes = group.Sum(a => a.TotalWorkMinutes);

                    return new
                    {
                        userId = group.Key.UserId,
                        name = group.Key.FullName,
                        avatar = group.Key.AvatarUrl,

                        firstIn = firstIn.HasValue
                            ? new DateTime(
                                selectedDate.Year,
                                selectedDate.Month,
                                selectedDate.Day,
                                firstIn.Value.Hour,
                                firstIn.Value.Minute,
                                firstIn.Value.Second
                            ).ToString("hh:mm tt")
                            : "--",

                        lastOut = lastOut.HasValue
                            ? new DateTime(
                                selectedDate.Year,
                                selectedDate.Month,
                                selectedDate.Day,
                                lastOut.Value.Hour,
                                lastOut.Value.Minute,
                                lastOut.Value.Second
                            ).ToString("hh:mm tt")
                            : "--",

                        total = FormatMinutes(totalMinutes)
                    };
                })
                .ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyAttendance(DateTime date, int? userId = null)
        {
            var requesterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                userId = requesterId; 
            }
            var selectedDate = DateOnly.FromDateTime(date);

            var startOfMonth = new DateOnly(selectedDate.Year, selectedDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var query = _context.Attendances
                .Include(a => a.User)
                .Where(a =>
                    a.Date.ToDateTime(TimeOnly.MinValue) >= startOfMonth.ToDateTime(TimeOnly.MinValue) &&
                    a.Date.ToDateTime(TimeOnly.MinValue) <= endOfMonth.ToDateTime(TimeOnly.MinValue)
                );

            if (userId != null)
            {
                query = query.Where(a => a.UserId == userId);
            }

            var data = await query.ToListAsync();

            var userSchedules = await _context.UserWorkSchedules
                .Where(us =>
                    us.WorkSchedule.DeletedAt == null &&
                    us.UserId != null
                )
                .Select(us => new
                {
                    us.UserId,
                    Days = us.WorkSchedule.ScheduleDays.Select(sd => new ScheduleDayDto
                    {
                        Day = sd.Day,
                        StartTime = sd.StartTime,
                        EndTime = sd.EndTime,
                        Duration = sd.Duration,
                        IsRestDay = sd.IsRestDay,
                        Breaks = new List<ScheduleBreakDisplayDto>()
                    })
                })
                .ToListAsync();

            var scheduleMap = userSchedules
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(x => x.Days)
                        .GroupBy(d => new { d.Day, d.IsRestDay })
                        .Select(g => g.First())
                        .ToList()
                );

            var leaves = await _context.LeaveRequests
                .Where(l =>
                    l.Status == LeaveRequest.RequestStatus.Approved &&
                    (userId == null || l.UserId == userId) &&
                    l.StartDate <= endOfMonth &&
                    l.EndDate >= startOfMonth
                )
                .ToListAsync();

            var result = data
                .GroupBy(a => new
                {
                    a.UserId,
                    FullName = string.Join(" ", new[]
                    {
                        a.User.FirstName,
                        a.User.MiddleName,
                        a.User.LastName
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    AvatarUrl = a.User.AvatarUrl
                })
                .Select(group =>
                {
                    var daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
                    var days = new string[daysInMonth];
                    var dailyMinutes = new int[daysInMonth];
                    int totalMinutes = 0;

                    var lateDays = new bool[daysInMonth];

                    foreach (var a in group)
                    {
                        int dayIndex = a.Date.Day - 1;
                        dailyMinutes[dayIndex] += a.TotalWorkMinutes;
                        totalMinutes += a.TotalWorkMinutes;

                        if (a.LateMinutes > 0)
                        {
                            lateDays[dayIndex] = true;
                        }
                    }

                    var schedule = scheduleMap.ContainsKey(group.Key.UserId)
                        ? scheduleMap[group.Key.UserId]
                        : new List<ScheduleDayDto>();

                    var userLeaves = leaves
                        .Where(l => l.UserId == group.Key.UserId)
                        .ToList();

                    var leaveMap = new HashSet<string>();

                    foreach (var leave in userLeaves)
                    {
                        for (var d = leave.StartDate; d <= leave.EndDate; d = d.AddDays(1))
                        {
                            leaveMap.Add(d.ToString("yyyy-MM-dd"));
                        }
                    }

                    for (int i = 0; i < daysInMonth; i++)
                    {
                        var currentDate = new DateOnly(selectedDate.Year, selectedDate.Month, i + 1);
                        var key = currentDate.ToString("yyyy-MM-dd");

                        var isRestDay = schedule.Any(s =>
                            s.Day == currentDate.DayOfWeek &&
                            s.IsRestDay
                        );

                        if (leaveMap.Contains(key))
                        {
                            days[i] = "LEAVE";
                        }
                        else if (isRestDay)
                        {
                            days[i] = "REST";
                        }
                        else if (dailyMinutes[i] > 0)
                        {
                            days[i] = FormatMinutes(dailyMinutes[i]);
                        }
                        else
                        {
                            days[i] = "0h";
                        }
                    }

                    return new
                    {
                        userId = group.Key.UserId,
                        name = group.Key.FullName,
                        avatar = group.Key.AvatarUrl,
                        days,
                        total = FormatMinutes(totalMinutes),
                        lateCount = lateDays.Count(ld => ld)
                    };
                })
                .ToList();

            return Ok(result);
        }

        private static string FormatMinutes(int totalMinutes)
        {
            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;

            return $"{hours}h {minutes}m";
        }
    }