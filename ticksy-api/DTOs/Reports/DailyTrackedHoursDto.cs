public class DailyTrackedHoursDto
{
    public DateOnly Date { get; set; }
    public double WorkHours { get; set; }
    public double BreakHours { get; set; }
    public double OvertimeHours { get; set; }
}