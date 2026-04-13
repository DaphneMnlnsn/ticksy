using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/requests")]
[ApiController]
public class LeaveRequestController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;

    public LeaveRequestController(AppDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequestCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var admins = await _context.Users
            .Where(u => u.Role == ticksy_api.Models.User.UserRole.Admin && u.DeletedAt == null)
            .ToListAsync();

        if (await _context.LeaveRequests.AnyAsync(r =>
            r.UserId == userId &&
            dto.StartDate <= r.EndDate &&
            dto.EndDate >= r.StartDate &&
            r.Status != LeaveRequest.RequestStatus.Cancelled))
        {
            return BadRequest("You already have a leave request overlapping these dates.");
        }

        var leaveType = await _context.TimeOffPolicies
            .Where(p => p.Name.ToLower() == dto.LeaveType.ToLower())
            .FirstOrDefaultAsync();

        if (leaveType == null)
            return BadRequest("Leave type does not exist.");

        if (dto.EndDate < dto.StartDate)
            return BadRequest("End date cannot be earlier than start date.");
        
        var request = new LeaveRequest
        {
            UserId = userId,
            LeaveTypeId =  leaveType.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveRequest.RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.LeaveRequests.Add(request);

        await _context.SaveChangesAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateAsync(
                admin.Id,
                Notification.NotifType.LeaveRequest,
                $"New leave request from user ID {userId}.",
                true
            );
        }

        return Ok(new { message = "Leave request created successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetRequests()
    {
        var requests = await _context.LeaveRequests
            .Select(pl => new
            {
                pl.Id,
                pl.User.FirstName,
                pl.User.MiddleName,
                pl.User.LastName,
                LeaveType = pl.TimeOffPolicy.Name,
                pl.StartDate,
                pl.EndDate,
                pl.Reason,
                pl.Status
            })
            .OrderBy(pl => pl.Id)
            .ToListAsync();

        var result = requests.Select(pl => new LeaveRequestListDto
        {
            Id = pl.Id,
            FullName = string.Join(" ", new[]
            {
                pl.FirstName,
                pl.MiddleName,
                pl.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            LeaveType = pl.LeaveType,
            StartDate = pl.StartDate,
            EndDate = pl.EndDate,
            Reason = pl.Reason,
            Status = pl.Status
        });

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var requests = await _context.LeaveRequests
            .Where(p => p.UserId == userId)
            .Select(pl => new LeaveRequestListDto
            {
                Id = pl.Id,
                LeaveType = pl.TimeOffPolicy.Name,
                StartDate = pl.StartDate,
                EndDate = pl.EndDate,
                Reason = pl.Reason,
                Status = pl.Status
            })
            .ToListAsync();

        return Ok(requests);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveLeaveRequest(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = await _context.LeaveRequests.FindAsync(id);
        if (request == null) return NotFound();
        
        if (request.Status != LeaveRequest.RequestStatus.Pending)
            return BadRequest("Only pending requests can be updated.");

        request.Status = LeaveRequest.RequestStatus.Approved;
        request.ReviewedBy = userId;
        request.ReviewedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.CreateAsync(
            request.UserId,
            Notification.NotifType.LeaveApproved,
            "Your leave request has been approved.",
            true
        );

        return Ok(new { message = "Leave request approved successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectLeaveRequest(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = await _context.LeaveRequests.FindAsync(id);
        if (request == null) return NotFound();

        if (request.Status != LeaveRequest.RequestStatus.Pending)
            return BadRequest("Only pending requests can be updated.");

        request.Status = LeaveRequest.RequestStatus.Rejected;
        request.ReviewedBy = userId;
        request.ReviewedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.CreateAsync(
            request.UserId,
            Notification.NotifType.LeaveRejected,
            "Your leave request has been rejected.",
            true
        );

        return Ok(new { message = "Leave request rejected successfully."});
    }

    [Authorize]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelLeaveRequest(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = await _context.LeaveRequests.FindAsync(id);
        if (request == null) return NotFound();

        if (request.UserId != userId)
            return BadRequest("You cannot cancel others' requests.");

        if (request.Status != LeaveRequest.RequestStatus.Pending)
            return BadRequest("Only pending requests can be updated.");

        request.Status = LeaveRequest.RequestStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Leave request cancelled successfully."});
    }
}