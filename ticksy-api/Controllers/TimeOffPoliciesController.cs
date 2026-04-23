using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/policies")]
[ApiController]
public class TimeOffPoliciesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeOffPoliciesController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePolicy([FromBody] PolicyCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _context.TimeOffPolicies.AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower() && t.DeletedAt == null))
            return BadRequest("A time off policy with this name already exists.");
        
        var policy = new TimeOffPolicy
        {
            Name =  dto.Name,
            MaxDays = dto.MaxDays,
            Rules = dto.Rules,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.TimeOffPolicies.Add(policy);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Time off policy created successfully." });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPolicies()
    {
        var policies = await _context.TimeOffPolicies
            .Where(p => p.DeletedAt == null)
            .Select(pl => new PolicyListDto
            {
                Id = pl.Id,
                Name = pl.Name
            })
            .ToListAsync();

        return Ok(policies);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPolicy(int id)
    {

        var policy = await _context.TimeOffPolicies
            .FirstOrDefaultAsync(t => t.Id == id);

        if (policy == null) return NotFound();

        if (policy.DeletedAt != null)
            return NotFound("This policy has been deleted.");

        var result = new PolicyCreateDto
        {
            Name = policy.Name,
            MaxDays = policy.MaxDays,
            Rules = policy.Rules,
        };

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePolicy(int id, [FromBody] PolicyUpdateDto dto)
    {
        var policy = await _context.TimeOffPolicies.FindAsync(id);
        if (policy == null) return NotFound();

        if (policy.DeletedAt != null)
            return BadRequest("Cannot update a deleted policy.");

        if(!string.IsNullOrWhiteSpace(dto.Name)) policy.Name = dto.Name;
        policy.MaxDays = dto.MaxDays;
        if(!string.IsNullOrWhiteSpace(dto.Rules)) policy.Rules = dto.Rules;

        policy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Time off policy updated successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        var policy = await _context.TimeOffPolicies.FindAsync(id);
        if (policy == null) return NotFound();

        if (policy.DeletedAt != null)
            return BadRequest("Policy is already deleted.");

        policy.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Time off policy deleted successfully."});
    }
}