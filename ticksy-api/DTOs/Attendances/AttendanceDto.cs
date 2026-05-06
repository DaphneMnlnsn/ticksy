using ticksy_api.Models;

public class AttendanceDto {

    public DateOnly Date { get; set; }
    public TimeOnly TimeIn { get; set; }
    public TimeSpan BreakDuration { get; set; } = TimeSpan.Zero;
    public TimeOnly? TimeOut { get; set; }
    public int LateMinutes  { get; set; }
    public int TotalWorkMinutes { get; set; }
    public string? Notes { get; set; }
    
    public Attendance.ClockInSourceEnum ClockInSource { get; set; }
    public Attendance.AttendanceStatus Status { get; set; }

}