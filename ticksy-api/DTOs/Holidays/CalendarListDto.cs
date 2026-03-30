using ticksy_api.Models;

public class CalendarListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Holiday.HolidaySource Source { get; set; }
}