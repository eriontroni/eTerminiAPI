using System.Security.Claims;
using eTerminiAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstitutionsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionsController(IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    /// <summary>
    /// Kthon qytetin dhe emrin e tenantit të përdoruesit të kyçur.
    /// Përdoret për etiketën "Po kërkoni institucione në: [Qyteti]".
    /// </summary>
    [HttpGet("context")]
    public async Task<IActionResult> GetContext()
    {
        var tenantId = GetTenantId();
        if (tenantId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        try
        {
            var result = await _institutionService.GetContextAsync(tenantId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kërkon institucione sipas emrit, të filtruara vetëm për tenatnin e përdoruesit.
    /// Kërkon minimum 2 karaktere. Kthen maksimum 20 rezultate.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        var tenantId = GetTenantId();
        if (tenantId == Guid.Empty)
            return Unauthorized(new { message = "Token i pavlefshëm." });

        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return BadRequest(new { message = "Kërkimi duhet të ketë të paktën 2 karaktere." });

        var results = await _institutionService.SearchAsync(query, tenantId, limit: 20);
        return Ok(results);
    }

    private Guid GetTenantId()
    {
        var raw = User.FindFirstValue("tenantId");
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }
}
