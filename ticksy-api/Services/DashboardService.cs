using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Attendance> GetUserAttendance(int userId, DateOnly start, DateOnly end)
    {
        return _context.Attendances
            .Where(a => a.UserId == userId &&
                    a.Date >= start &&
                    a.Date <= end);
    }

    public async Task<DashboardSummaryDto> GetDashboardSummary(int userId, DateOnly start, DateOnly end)
    {
        var data = await GetUserAttendance(userId, start, end).ToListAsync();

        if (data == null || !data.Any()) 
        {
            return new DashboardSummaryDto();
        }

        double totalMinutes = 0;
        double breakMinutes = 0;

        foreach (var a in data)
        {
            if (a.TimeOut == null)
            {
                var nowLocal = TimeOnly.FromDateTime(DateTime.Now);
                var currentElapsed = (nowLocal - a.TimeIn).TotalMinutes;
                totalMinutes += Math.Max(0, currentElapsed);
            }
            else
            {
                totalMinutes += a.TotalWorkMinutes;
            }

            if (a.BreakDuration != TimeSpan.Zero)
            {
                breakMinutes += a.BreakDuration.TotalMinutes;
            }
        }

        var overtimeMinutes = data
            .Where(a => a.TotalWorkMinutes > 480)
            .Sum(a => a.TotalWorkMinutes - 480);

        return new DashboardSummaryDto
        {
            TotalWorkHours = Math.Round(totalMinutes / 60.0, 2),
            TotalBreakHours = Math.Round(breakMinutes / 60.0, 2),
            TotalOvertimeHours = Math.Round(overtimeMinutes / 60.0, 2)
        };
    }

    public async Task<DailyTrackedHoursDto> GetDailyTrackedHours(int userId, DateOnly start, DateOnly end)
    {
        var today = start;

        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (attendance == null)
        {
            return new DailyTrackedHoursDto();
        }

        return new DailyTrackedHoursDto
        {
            Date = today,
            WorkHours = Math.Round(attendance.TotalWorkMinutes / 60.0, 2),
            BreakHours = Math.Round(attendance.BreakDuration.TotalMinutes / 60.0, 2),
            OvertimeHours = Math.Round(attendance.TotalWorkMinutes > 480
                ? (attendance.TotalWorkMinutes - 480) / 60.0
                : 0, 2)
        };
    }

    public async Task<List<DailyTrackedHoursDto>> GetWeeklyTrackedHours(int userId, DateOnly start, DateOnly end)
    {
        var data = await GetUserAttendance(userId, start, end).ToListAsync();

        var result = data
            .GroupBy(a => a.Date)
            .Select(g => new DailyTrackedHoursDto
            {
                Date = g.Key,
                WorkHours = Math.Round(g.Sum(a => a.TotalWorkMinutes) / 60.0, 2),
                BreakHours = Math.Round(g.Sum(a => a.BreakDuration.TotalMinutes) / 60.0, 2),
                OvertimeHours = Math.Round(g.Sum(a => a.TotalWorkMinutes > 480 ? a.TotalWorkMinutes - 480 : 0) / 60.0, 2)
            })
            .OrderBy(x => x.Date)
            .ToList();

        return result;
    }

    public async Task<List<DailyTrackedHoursDto>> GetMonthlyTrackedHours(int userId, DateOnly start, DateOnly end)
    {
        var data = await GetUserAttendance(userId, start, end).ToListAsync();

        var result = data
            .GroupBy(a => a.Date)
            .Select(g => new DailyTrackedHoursDto
            {
                Date = g.Key,
                WorkHours = Math.Round(g.Sum(a => a.TotalWorkMinutes) / 60.0, 2),
                BreakHours = Math.Round(g.Sum(a => a.BreakDuration.TotalMinutes) / 60.0, 2),
                OvertimeHours = Math.Round(g.Sum(a => a.TotalWorkMinutes > 480 ? a.TotalWorkMinutes - 480 : 0) / 60.0, 2)
            })
            .OrderBy(x => x.Date)
            .ToList();

        return result;
    }

    public async Task<List<MostAbsencesDto>> GetMostAbsences(int adminId, DateOnly start, DateOnly end)
    {

        var defaultCalendarId = await _context.HolidayCalendars
            .Where(c => c.IsDefault)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        var users = await _context.Users
            .Include(u => u.UserWorkSchedules)
                .ThenInclude(us => us.WorkSchedule)
                .ThenInclude(ws => ws.ScheduleDays)
            .ToListAsync();

        var allAttendances = await _context.Attendances
            .Where(a => a.Date >= start && a.Date <= end)
            .ToListAsync();

        var holidays = (await _context.Holidays
            .Where(h =>
                h.CalendarId == defaultCalendarId &&
                h.Date >= start &&
                h.Date <= end &&
                h.DeletedAt == null
            )
            .Select(h => h.Date)
            .ToListAsync())
            .ToHashSet();

        var approvedLeaves = await _context.LeaveRequests
            .Where(l =>
                l.Status == LeaveRequest.RequestStatus.Approved &&
                l.EndDate >= start &&
                l.StartDate <= end
            )
            .ToListAsync();

        var result = new List<MostAbsencesDto>();

        foreach (var user in users)
        {
            int absenceCount = 0;

            var userAttendanceDates = allAttendances
                .Where(a => a.UserId == user.Id)
                .Select(a => a.Date)
                .ToHashSet();

            var userLeaves = approvedLeaves
                .Where(l => l.UserId == user.Id)
                .ToList();

            var scheduleDays = user.UserWorkSchedules
                .SelectMany(us => us.WorkSchedule.ScheduleDays)
                .Where(sd => !sd.IsRestDay)
                .ToList();

            if (!scheduleDays.Any()) continue;

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;

                var isScheduled = scheduleDays.Any(sd => sd.Day == dayOfWeek);
                if (!isScheduled) continue;

                if (holidays.Contains(date)) continue;

                var isOnLeave = userLeaves.Any(l =>
                    date >= l.StartDate && date <= l.EndDate
                );
                if (isOnLeave) continue;

                var hasAttendance = userAttendanceDates.Contains(date);

                if (!hasAttendance)
                {
                    absenceCount++;
                }
            }

            if (absenceCount > 0)
            {
                result.Add(new MostAbsencesDto
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    AvatarUrl = user.AvatarUrl,
                    AbsenceCount = absenceCount
                });
            }
        }

        return result
            .OrderByDescending(r => r.AbsenceCount)
            .Take(5)
            .ToList();
    }

    public async Task<List<UserLiveStatusDto>> GetUsersLiveStatusAsync()
    {
        var users = await _context.Users
                        .Where(u => u.DeletedAt == null)
                        .ToListAsync();
        var result = new List<UserLiveStatusDto>();

        foreach (var user in users)
        {
            var latest = await _context.Attendances
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.TimeIn)
                .FirstOrDefaultAsync();

            string status = "Out";
            DateOnly? lastDate = latest?.Date;
            TimeOnly? lastTime = null;

            if (latest != null)
            {
                if (latest.BreakStart != null)
                {
                    status = "On Break";
                    lastTime = latest.BreakStart;
                }
                else if (latest.TimeOut != null)
                {
                    status = "Out";
                    lastTime = latest.TimeOut;
                }
                else if (latest.TimeIn != default)
                {
                    status = "In";
                    lastTime = latest.TimeIn;
                }
            }

            result.Add(new UserLiveStatusDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                AvatarUrl = user.AvatarUrl,
                Status = status,
                LastActionDate = lastDate,
                LastActionTime = lastTime
            });

            Console.WriteLine($"User {user.Id} | In: {latest?.TimeIn} | Out: {latest?.TimeOut} | Break: {latest?.BreakStart}");
        }

        return result;
    }
}