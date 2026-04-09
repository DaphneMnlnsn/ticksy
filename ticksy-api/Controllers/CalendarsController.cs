using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class CalendarsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public CalendarsController(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateCalendar([FromBody] CalendarListDto dto, [FromQuery] string? country = null)
    {
        if (await _context.HolidayCalendars.AnyAsync(c => 
            c.Name.ToLower() == dto.Name.ToLower()))
            return BadRequest("A calendar with this name already exists.");

        var calendar = new HolidayCalendar
        {
            Name = dto.Name,
            Source = dto.Source,
            ExternalCalendarId = country
        };

        if (!string.IsNullOrEmpty(country))
        {
            calendar.Source = Holiday.HolidaySource.Imported;
        }

        _context.HolidayCalendars.Add(calendar);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(country))
        {
            int currentYear = DateTime.UtcNow.Year;
            await ExecuteHolidayImport(calendar.Id, currentYear, country);
        }

        return Ok(new { 
            message = "Calendar created successfully.", 
            id = calendar.Id,
            calendar = calendar.Name
        });
    }

    private async Task<int> ExecuteHolidayImport(int calendarId, int year, string country)
    {
        var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{country}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return 0;

        var content = await response.Content.ReadAsStringAsync();
        var externalHolidays = System.Text.Json.JsonSerializer.Deserialize<List<ExternalHolidayDto>>(content, 
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (externalHolidays == null) return 0;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var newHolidays = externalHolidays.Select(eh => new Holiday
        {
            Name = eh.LocalName ?? eh.Name,
            Date = DateOnly.FromDateTime(eh.Date),
            Type = Holiday.HolidayType.Imported,
            CalendarId = calendarId,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        _context.Holidays.AddRange(newHolidays);
        await _context.SaveChangesAsync();
        
        return newHolidays.Count;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCalendars()
    {
        var calendars = await _context.HolidayCalendars
            .Select(c => new CalendarListDto
            {
                Id = c.Id,
                Name = c.Name,
                Source = c.Source
            })
            .ToListAsync();

        return Ok(calendars);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("import/{id}")]
    public async Task<IActionResult> ImportHolidays(int id, [FromQuery] int year, [FromQuery] string country)
    {
        var calendar = await _context.HolidayCalendars.FindAsync(id);
        if (calendar == null) return NotFound("Calendar not found.");

        if (year < 2000 || year > 2100)
            return BadRequest("Invalid year.");

        if (string.IsNullOrWhiteSpace(country))
            return BadRequest("Country is required.");
        
        var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{country}";
        
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return BadRequest("Failed to fetch holidays from external API.");

        var content = await response.Content.ReadAsStringAsync();

        var externalHolidays = System.Text.Json.JsonSerializer.Deserialize<List<ExternalHolidayDto>>(
            content,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        if (externalHolidays == null)
            return BadRequest("Invalid data from API");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existingSet = await _context.Holidays
            .Where(h => h.CalendarId == id && h.Date.Year == year && h.DeletedAt == null)
            .Select(h => new 
            { 
                Date = h.Date, 
                Name = h.Name.ToLower().Trim()
            })
            .ToListAsync();

        var existingLookup = existingSet
            .Select(e => $"{e.Date:yyyy-MM-dd}|{e.Name}")
            .ToHashSet();

        var newHolidays = externalHolidays
            .Where(eh =>
            {
                var name = (eh.LocalName ?? eh.Name).ToLower().Trim();
                var date = DateOnly.FromDateTime(eh.Date);
                var key = $"{date:yyyy-MM-dd}|{name}";

                return !existingLookup.Contains(key);
            })
            .Select(eh => new Holiday
            {
                Name = eh.LocalName ?? eh.Name,
                Date = DateOnly.FromDateTime(eh.Date),
                Type = Holiday.HolidayType.Imported,
                CalendarId = id,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();

        _context.Holidays.AddRange(newHolidays);

        if (!string.IsNullOrEmpty(calendar.ExternalCalendarId) &&
            calendar.ExternalCalendarId != country)
        {
            return BadRequest($"This calendar is already linked to {calendar.ExternalCalendarId}.");
        }

        calendar.Source = Holiday.HolidaySource.Imported;
        calendar.ExternalCalendarId = country;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Holidays imported successfully.",
            importedCount = newHolidays.Count
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCalendar(int id)
    {
        var calendar = await _context.HolidayCalendars.FindAsync(id);
        if (calendar == null) return NotFound();

        _context.HolidayCalendars.Remove(calendar);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Calendar deleted successfully."});
    }
}