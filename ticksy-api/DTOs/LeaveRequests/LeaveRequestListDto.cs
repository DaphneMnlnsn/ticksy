using ticksy_api.Models;

public class LeaveRequestListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string LeaveType { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public LeaveRequest.RequestStatus Status { get; set; }
}