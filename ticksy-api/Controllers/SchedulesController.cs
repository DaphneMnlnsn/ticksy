using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class SchedulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SchedulesController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _context.WorkSchedules.AnyAsync(s => s.ScheduleName.ToLower() == dto.ScheduleName.ToLower() && s.DeletedAt == null))
            return BadRequest("A schedule with this name already exists.");
        
        if (dto.Days == null || dto.Days.Count != 7)
            return BadRequest("All 7 days must be provided.");

        var hasDuplicateDays = dto.Days
            .GroupBy(d => d.Day)
            .Any(g => g.Count() > 1);

        if (hasDuplicateDays)
            return BadRequest("Duplicate days are not allowed.");

        var schedule = new WorkSchedule
        {
            ScheduleName =  dto.ScheduleName,
            WorkArrangement = dto.WorkArrangement,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WorkSchedules.Add(schedule);

        var daysToAdd = dto.Days.Select(day => new WorkScheduleDay
        {
            WorkSchedule = schedule,
            Day = day.Day,
            StartTime = day.IsRestDay ? null : day.StartTime,
            EndTime = day.IsRestDay ? null : day.EndTime,
            IsRestDay = day.IsRestDay
        }).ToList();

        _context.WorkScheduleDays.AddRange(daysToAdd);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSchedule),new { id = schedule.Id }, new {
            schedule.Id,
            schedule.ScheduleName,
            Days = daysToAdd.Select(d => new {
                d.Day,
                d.StartTime,
                d.EndTime,
                d.IsRestDay
            })
        });
    }

    [Authorize]
    [HttpPost("add-break")]
    public async Task<IActionResult> AddBreak([FromBody] ScheduleBreakDto dto)
    {
        if (dto.Days == null || !dto.Days.Any())
            return BadRequest("At least one day must be selected.");

        var scheduleDays = await _context.WorkScheduleDays
            .Where(sd => 
                sd.WorkScheduleId == dto.ScheduleId &&
                dto.Days.Contains(sd.Day) &&
                !sd.IsRestDay)
            .ToListAsync();

        if (!scheduleDays.Any())
            return NotFound("No valid schedule days found.");

        var scheduleDayIds = scheduleDays.Select(sd => sd.Id).ToList();

        var exists = await _context.WorkScheduleBreaks.AnyAsync(b =>
            scheduleDayIds.Contains(b.WorkScheduleDayId) &&
            b.BreakName.ToLower() == dto.BreakName.ToLower());

        if (exists)
            return BadRequest("Break already exists for one or more selected days.");

        var breaks = scheduleDays.Select(sd => new WorkScheduleBreak
        {
            WorkScheduleDayId = sd.Id,
            BreakName = dto.BreakName,
            BreakDuration = dto.BreakDuration
        }).ToList();

        _context.WorkScheduleBreaks.AddRange(breaks);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Break added successfully." });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetSchedules()
    {
        var schedules = await _context.WorkSchedules
            .Where(sd => sd.DeletedAt == null)
            .Select(sd => new ScheduleListDto
            {
                Id = sd.Id,
                ScheduleName = sd.ScheduleName
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchedule(int id)
    {
        var data = await _context.WorkSchedules
            .Where(s => s.Id == id && s.DeletedAt == null)
            .Select(schedule => new
            {
                schedule.Id,
                schedule.ScheduleName,
                schedule.CreatedBy,

                Days = schedule.ScheduleDays.Select(sd => new
                {
                    sd.Day,
                    sd.StartTime,
                    sd.EndTime,
                    sd.IsRestDay,

                    Breaks = sd.Breaks.Select(sb => new
                    {
                        sb.BreakName,
                        sb.BreakDuration
                    })
                }),

                Users = schedule.UserWorkSchedules.Select(us => new
                {
                    us.UserId,
                    us.User.FirstName,
                    us.User.MiddleName,
                    us.User.LastName
                })
            })
            .FirstOrDefaultAsync();

        if (data == null) return NotFound();

        var result = new ScheduleDetailsDto
        {
            Id = data.Id,
            ScheduleName = data.ScheduleName,
            CreatedBy = data.CreatedBy,

            Days = data.Days.Select(sd => new ScheduleDayDto
            {
                Day = sd.Day,
                StartTime = sd.StartTime,
                EndTime = sd.EndTime,
                IsRestDay = sd.IsRestDay,

                Breaks = sd.Breaks.Select(sb => new ScheduleBreakDisplayDto
                {
                    BreakName = sb.BreakName,
                    BreakDuration = sb.BreakDuration
                }).ToList()
            }).ToList(),

            UserWorkSchedules = data.Users.Select(us => new UserWorkScheduleDto
            {
                UserId = us.UserId,
                FullName = string.Join(" ", new[]
                {
                    us.FirstName,
                    us.MiddleName,
                    us.LastName
                }.Where(x => !string.IsNullOrWhiteSpace(x)))
            }).ToList()
        };

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/assign-members")]
    public async Task<IActionResult> AssignMembers(int id, [FromBody] ScheduleAssignDto dto)
    {
        var schedule = await _context.WorkSchedules.FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
            return NotFound("Schedule not found.");

        var existingMembers = await _context.UserWorkSchedules
            .Where(us => us.WorkScheduleId == id)
            .Select(tm => tm.UserId)
            .ToListAsync();

        var membersToAdd = new List<UserWorkSchedule>();
        var validUserIds = await _context.Users
            .Where(u => dto.MemberIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var memberId in validUserIds.Distinct())
        {
            if (existingMembers.Contains(memberId)) continue;

            membersToAdd.Add(new UserWorkSchedule
            {
                WorkSchedule = schedule,
                UserId = memberId,
                AssignedAt = DateTime.UtcNow
            });
        }

        if (!membersToAdd.Any())
            return BadRequest("No valid members to add.");

        _context.UserWorkSchedules.AddRange(membersToAdd);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employees assigned successfully.", added = membersToAdd.Count });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleUpdateDto dto)
    {
        var schedule = await _context.WorkSchedules
            .Include(s => s.ScheduleDays)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (schedule == null) return NotFound();

        if (schedule.DeletedAt != null)
            return BadRequest("Cannot update a deleted schedule.");

        if(!string.IsNullOrWhiteSpace(dto.ScheduleName)) schedule.ScheduleName = dto.ScheduleName;
        schedule.WorkArrangement = dto.WorkArrangement;
        schedule.UpdatedAt = DateTime.UtcNow;

        if (dto.Days == null || dto.Days.Count != 7)
            return BadRequest("All 7 days must be provided.");

        if (dto.Days.GroupBy(d => d.Day).Any(g => g.Count() > 1))
            return BadRequest("Duplicate days are not allowed.");

        _context.WorkScheduleDays.RemoveRange(schedule.ScheduleDays);

        schedule.ScheduleDays = dto.Days.Select(d => new WorkScheduleDay
        {
            Day = d.Day,
            IsRestDay = d.IsRestDay,
            StartTime = d.IsRestDay ? null : d.StartTime,
            EndTime = d.IsRestDay ? null : d.EndTime
        }).ToList();

        await _context.SaveChangesAsync();

        return Ok(new { message = "Schedule updated successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{scheduleId}/members/{memberId}")]
    public async Task<IActionResult> UnassignMember(int scheduleId, int memberId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var member = await _context.UserWorkSchedules
            .FirstOrDefaultAsync(us => us.WorkScheduleId == scheduleId && us.UserId == memberId);

        if (member == null)
            return NotFound("Member not found in this schedule.");

        _context.UserWorkSchedules.Remove(member);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Member unassigned successfully." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var schedule = await _context.WorkSchedules.FirstOrDefaultAsync(s => s.Id == id);
        if (schedule == null) return NotFound();

        if (schedule.DeletedAt != null)
            return BadRequest("Schedule is already deleted.");

        schedule.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Schedule deleted successfully."});
    }
}