public class AttendanceSummaryDto
{
    public int Present { get; set; }
    public int Late { get; set; }
    public int HalfDay { get; set; }

    public int NoClockIn { get; set; }
    public int NoClockOut { get; set; }

    public int Absent { get; set; }
    public int OnLeave { get; set; }
    public int DayOff { get; set; }
}