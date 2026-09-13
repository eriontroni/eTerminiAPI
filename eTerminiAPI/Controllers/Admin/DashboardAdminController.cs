using eTerminiAPI.API.Authorization;
using eTerminiAPI.Application.Admin.Interfaces.Services;
using eTerminiAPI.Domain.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[HasPermission(Permissions.Dashboard.View)]
public class DashboardAdminController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public DashboardAdminController(IAdminDashboardService service) => _service = service;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats() => Ok(await _service.GetStatsAsync());

    [HttpGet("active-appointments")]
    public async Task<IActionResult> GetActiveAppointments()
        => Ok(await _service.GetActiveAppointmentsAsync());
}
