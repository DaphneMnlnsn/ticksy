public class PolicyCreateDto
{
    public string Name { get; set; } = null!;
    public int MaxDays { get; set; }
    public string Rules { get; set; } = null!;
}