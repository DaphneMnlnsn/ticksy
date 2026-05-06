using ticksy_api.Models;

public class HolidayCalendar
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsDefault { get; set; } = false; 
    public Holiday.HolidaySource Source { get; set; } = Holiday.HolidaySource.Manual;
    public string? ExternalCalendarId { get; set; }
    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
}