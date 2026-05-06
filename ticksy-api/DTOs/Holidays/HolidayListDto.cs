public class HolidayListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly Date { get; set; }

    public string Day => Date.ToString("dddd");
}