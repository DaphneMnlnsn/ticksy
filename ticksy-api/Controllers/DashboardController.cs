using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [Authorize]
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var summary = await _dashboardService.GetDashboardSummary(userId, start, end);

        return Ok(summary);
    }

    [Authorize]
    [HttpGet("daily-hours")]
    public async Task<IActionResult> GetDailyTrackedHours([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dailyTracked = await _dashboardService.GetDailyTrackedHours(userId, start, end);

        return Ok(dailyTracked);
    }

    [Authorize]
    [HttpGet("weekly-hours")]
    public async Task<IActionResult> GetWeeklyTrackedHours([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var weeklyTracked = await _dashboardService.GetWeeklyTrackedHours(userId, start, end);

        return Ok(weeklyTracked);
    }

    [Authorize]
    [HttpGet("monthly-hours")]
    public async Task<IActionResult> GetMonthlyTrackedHours([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var monthlyTracked = await _dashboardService.GetMonthlyTrackedHours(userId, start, end);

        return Ok(monthlyTracked);
    }
}