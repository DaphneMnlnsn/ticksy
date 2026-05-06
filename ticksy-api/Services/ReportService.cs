using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<AttendanceSummaryDto> GetAttendanceSummary(DateOnly start, DateOnly end)
    {
        var data = await _context.Attendances
            .Where(a => a.Date >= start && a.Date <= end)
            .ToListAsync();

        return new AttendanceSummaryDto
        {
            Present = data.Count(a => a.Status == Attendance.AttendanceStatus.Present),
            Late = data.Count(a => a.Status == Attendance.AttendanceStatus.Late),
            HalfDay = data.Count(a => a.Status == Attendance.AttendanceStatus.HalfDay),

            NoClockIn = data.Count(a => a.TimeIn == default),
            NoClockOut = data.Count(a => a.TimeOut == null),

            Absent = 0,
            OnLeave = 0,
            DayOff = 0
        };
    }

    public async Task<List<AttendanceReportRowDto>> GetAttendanceReport(DateOnly start, DateOnly end, string? search = null)
    {
        var query = _context.Attendances
            .Include(a => a.User)
            .Where(a => a.Date >= start && a.Date <= end);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a =>
                (a.User.FirstName + " " + a.User.LastName)
                    .ToLower()
                    .Contains(search.ToLower()));
        }

        var data = await query
        .OrderByDescending(a => a.Date)
        .Select(a => new
        {
            a.Date,
            a.TimeIn,
            a.TimeOut,
            a.TotalWorkMinutes,
            a.Status,
            a.Notes,
            a.User.FirstName,
            a.User.MiddleName,
            a.User.LastName,
            a.User.AvatarUrl
        })
        .ToListAsync();

        var result = data.Select(a => new AttendanceReportRowDto
        {
            EmployeeName = string.Join(" ", new[]
            {
                a.FirstName,
                a.MiddleName,
                a.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            Date = a.Date,
            ClockIn = a.TimeIn.ToString("HH:mm"),
            ClockOut = a.TimeOut.HasValue ? a.TimeOut.Value.ToString("HH:mm") : "-",

            TotalHours = Math.Round(a.TotalWorkMinutes / 60.0, 2),
            OvertimeHours = Math.Round(a.TotalWorkMinutes > 480
                ? (a.TotalWorkMinutes - 480) / 60.0
                : 0, 2),

            Status = a.Status.ToString(),
            Notes = a.Notes,
            AvatarUrl = a.AvatarUrl
        }).ToList();

        return result;
    }
}