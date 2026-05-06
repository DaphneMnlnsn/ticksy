using ticksy_api.Models;

public class HolidayUpdateDto
{
    public string? Name { get; set; }
    public DateOnly? Date { get; set; }
    public Holiday.HolidayType? Type { get; set; }
}