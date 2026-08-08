using System.Security.Claims;
using eTerminiAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/catalog")]
[AllowAnonymous]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalog;

    public CatalogController(ICatalogService catalog)
    {
        _catalog = catalog;
    }

    /// <summary>
    /// Lista e kategorive të shërbimeve publike.
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
        => Ok(await _catalog.GetCategoriesAsync(GetTenantId()));

    /// <summary>
    /// Institucionet, të filtruara sipas komunës (tenantit) të përdoruesit dhe opsionalisht kategorisë.
    /// </summary>
    [HttpGet("institutions")]
    public async Task<IActionResult> GetInstitutions([FromQuery] Guid? categoryId)
        => Ok(await _catalog.GetInstitutionsAsync(categoryId, GetTenantId()));

    /// <summary>
    /// Shërbimet, filtruara sipas komunës së përdoruesit dhe opsionalisht institucionit/kategorisë.
    /// </summary>
    [HttpGet("services")]
    public async Task<IActionResult> GetServices(
        [FromQuery] Guid? institutionId,
        [FromQuery] Guid? categoryId)
        => Ok(await _catalog.GetServicesAsync(institutionId, categoryId, GetTenantId()));

    /// <summary>
    /// Detajet e një shërbimi.
    /// </summary>
    [HttpGet("services/{serviceId:guid}")]
    public async Task<IActionResult> GetService(Guid serviceId)
    {
        try
        {
            return Ok(await _catalog.GetServiceByIdAsync(serviceId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Ofruesit/zyrtarët që mund të ofrojnë një shërbim të caktuar.
    /// </summary>
    [HttpGet("services/{serviceId:guid}/providers")]
    public async Task<IActionResult> GetProviders(Guid serviceId)
    {
        try
        {
            return Ok(await _catalog.GetProvidersForServiceAsync(serviceId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tenanti (komuna) i përdoruesit të kyçur, ose null për vizitorë anonimë.
    /// </summary>
    private Guid? GetTenantId()
    {
        var raw = User.FindFirstValue("tenantId");
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}
