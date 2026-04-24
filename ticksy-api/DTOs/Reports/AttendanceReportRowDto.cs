public class AttendanceReportRowDto
{
    public string EmployeeName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public DateOnly Date { get; set; }

    public string ClockIn { get; set; } = null!;
    public string ClockOut { get; set; } = null!;

    public double TotalHours { get; set; }
    public double OvertimeHours { get; set; }

    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
}