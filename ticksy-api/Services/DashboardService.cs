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
}