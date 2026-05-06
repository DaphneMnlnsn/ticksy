using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/reports")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("summary")]
    public async Task<IActionResult> GetAttendanceSummary([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var summary = await _reportService.GetAttendanceSummary(start, end);
        return Ok(summary);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("attendance-report")]
    public async Task<IActionResult> GetAttendanceReport([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var report = await _reportService.GetAttendanceReport(start, end);
        return Ok(report); 
    }
}