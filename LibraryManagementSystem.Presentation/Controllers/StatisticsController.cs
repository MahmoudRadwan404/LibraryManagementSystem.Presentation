using LibraryManagementSystem.Application.DTOs.Statistics;
using LibraryManagementSystem.Application.IServices.IStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = "Staff,Librarian,Administrator")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService) => _statisticsService = statisticsService;

    [HttpGet]
    public async Task<ActionResult<StatisticsDto>> GetDashboard() =>
        Ok(await _statisticsService.GetDashboardStatsAsync());
}