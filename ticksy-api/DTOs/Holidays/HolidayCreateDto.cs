using ticksy_api.Models;

public class HolidayCreateDto
{
    public string Name { get; set; } = null!;
    public DateOnly Date { get; set; }
    public Holiday.HolidayType Type { get; set; }

    public int CalendarId { get; set; }
}