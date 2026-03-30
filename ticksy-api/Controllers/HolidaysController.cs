using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class HolidaysController : ControllerBase
{
    private readonly AppDbContext _context;

    public HolidaysController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateHoliday([FromBody] HolidayCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var holiday = new Holiday
        {
            Name =  dto.Name,
            Date = dto.Date,
            Type = dto.Type,
            CalendarId = dto.CalendarId,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Holidays.Add(holiday);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Holiday created successfully." });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetHolidays([FromQuery] int calendarId, [FromQuery] int year)
    {
        var holidays = await _context.Holidays
            .Where(h => h.CalendarId == calendarId && h.Date.Year == year && h.DeletedAt == null)
            .Select(h => new HolidayListDto
            {
                Id = h.Id,
                Name = h.Name,
                Date = h.Date
            })
            .ToListAsync();

        return Ok(holidays);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHoliday(int id, [FromBody] HolidayUpdateDto dto)
    {
        var holiday = await _context.Holidays.FindAsync(id);
        if (holiday == null) return NotFound();

        if (holiday.DeletedAt != null)
            return BadRequest("Cannot update a deleted holiday.");

        if (!string.IsNullOrWhiteSpace(dto.Name)) holiday.Name = dto.Name;
        if (dto.Date.HasValue) holiday.Date = dto.Date.Value;
        if (dto.Type.HasValue) holiday.Type = dto.Type.Value;

        holiday.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Holiday updated successfully."});
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHoliday(int id)
    {
        var holiday = await _context.Holidays.FindAsync(id);
        if (holiday == null) return NotFound();

        if (holiday.DeletedAt != null)
            return BadRequest("Holiday is already deleted.");

        holiday.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Holiday deleted successfully."});
    }
}