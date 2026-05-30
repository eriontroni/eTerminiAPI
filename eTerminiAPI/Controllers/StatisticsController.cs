using eTerminiAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statistics;

    public StatisticsController(IStatisticsService statistics)
    {
        _statistics = statistics;
    }

    /// <summary>
    /// Statistikat e platformës: qytete, institucione dhe përdorues të regjistruar.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetStatistics()
        => Ok(await _statistics.GetStatisticsAsync());
}
