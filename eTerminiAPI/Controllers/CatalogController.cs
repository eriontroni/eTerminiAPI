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
        => Ok(await _catalog.GetCategoriesAsync());

    /// <summary>
    /// Institucionet, opsionalisht të filtruara sipas kategorisë.
    /// </summary>
    [HttpGet("institutions")]
    public async Task<IActionResult> GetInstitutions([FromQuery] Guid? categoryId)
        => Ok(await _catalog.GetInstitutionsAsync(categoryId));

    /// <summary>
    /// Shërbimet, opsionalisht filtruara sipas institucionit dhe/ose kategorisë.
    /// </summary>
    [HttpGet("services")]
    public async Task<IActionResult> GetServices(
        [FromQuery] Guid? institutionId,
        [FromQuery] Guid? categoryId)
        => Ok(await _catalog.GetServicesAsync(institutionId, categoryId));

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
}
