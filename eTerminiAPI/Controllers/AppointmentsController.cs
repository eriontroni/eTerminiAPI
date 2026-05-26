using System.Security.Claims;
using eTerminiAPI.Application.DTOs.Appointments;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Krijo termin të ri. Vetëm qytetarët dhe lart mund ta krijojnë.
    /// </summary>
    [HttpPost]
    //[Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();

        if (userId == Guid.Empty || tenantId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        try
        {
            var result = await _appointmentService.CreateAsync(dto, userId, tenantId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Merr të gjitha terminet e tenantit (vetëm stafi dhe lart).
    /// </summary>
    [HttpGet]
    //[Authorize(Policy = "StaffOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var tenantId = GetTenantId();
        if (tenantId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        var result = await _appointmentService.GetAllAsync(tenantId);
        return Ok(result);
    }

    /// <summary>
    /// Merr terminet e përdoruesit të kyçur.
    /// </summary>
    [HttpGet("my")]
    //[Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        var result = await _appointmentService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Merr terminin sipas ID-së.
    /// </summary>
    [HttpGet("{id:guid}")]
    //[Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _appointmentService.GetByIdAsync(id);

            var userId = GetUserId();
            var role = GetRole();
            var isStaff = role is nameof(UserRole.Staff)
                or nameof(UserRole.InstitutionAdmin)
                or nameof(UserRole.SuperAdmin);

            if (!isStaff && result.UserId != userId)
                return Forbid();

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Ndrysho statusin e terminit (vetëm stafi dhe lart).
    /// </summary>
    [HttpPut("{id:guid}/status")]
    //[Authorize(Policy = "StaffOrAbove")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        var changedByUserId = GetUserId();
        if (changedByUserId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        try
        {
            var result = await _appointmentService.UpdateStatusAsync(id, dto, changedByUserId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Fshij (soft delete) termin. Qytetari mund të fshijë vetëm terminet e veta.
    /// </summary>
    [HttpDelete("{id:guid}")]
    //[Authorize(Policy = "CitizenOrAbove")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        var role = GetRole();
        var isStaff = role is nameof(UserRole.Staff)
            or nameof(UserRole.InstitutionAdmin)
            or nameof(UserRole.SuperAdmin);

        try
        {
            await _appointmentService.DeleteAsync(id, userId, isStaff);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── helpers ────────────────────────────────────────────────

    private Guid GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }

    private Guid GetTenantId()
    {
        var raw = User.FindFirstValue("tenantId");
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }

    private string? GetRole() =>
        User.FindFirstValue(ClaimTypes.Role);
}
