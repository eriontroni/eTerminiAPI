using eTerminiAPI.API.Authorization;
using eTerminiAPI.Application.Admin.DTOs.Tenants;
using eTerminiAPI.Application.Admin.Interfaces.Services;
using eTerminiAPI.Domain.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers.Admin;

[ApiController]
[Route("api/admin/tenants")]
public class TenantsAdminController : ControllerBase
{
    private readonly IAdminTenantService _service;

    public TenantsAdminController(IAdminTenantService service) => _service = service;

    [HttpGet]
    [HasPermission(Permissions.Tenants.View)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    [HasPermission(Permissions.Tenants.CreateUpdate)]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Tenants.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
