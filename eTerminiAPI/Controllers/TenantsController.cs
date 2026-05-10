using eTerminiAPI.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace eTerminiAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public TenantsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenants = await _uow.Tenants.GetAllAsync();
        var result = tenants
            .Where(t => t.IsActive && !t.IsDeleted)
            .Select(t => new { t.Id, t.Name, t.Slug })
            .OrderBy(t => t.Name);
        return Ok(result);
    }
}
