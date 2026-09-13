using eTerminiAPI.Application.Admin.DTOs.Tenants;

namespace eTerminiAPI.Application.Admin.Interfaces.Services;

public interface IAdminTenantService
{
    Task<IEnumerable<TenantDto>> GetAllAsync();
    Task<TenantDto> CreateAsync(CreateTenantDto dto);
    Task DeleteAsync(Guid id);
}
