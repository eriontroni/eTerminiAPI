using eTerminiAPI.Application.DTOs.Catalog;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface ICatalogService
{
    Task<IEnumerable<ServiceCategoryDto>> GetCategoriesAsync(Guid? tenantId = null);
    Task<IEnumerable<InstitutionSummaryDto>> GetInstitutionsAsync(Guid? categoryId = null, Guid? tenantId = null);
    Task<IEnumerable<PublicServiceDto>> GetServicesAsync(Guid? institutionId = null, Guid? categoryId = null, Guid? tenantId = null);
    Task<PublicServiceDto> GetServiceByIdAsync(Guid serviceId);
    Task<IEnumerable<ProviderDto>> GetProvidersForServiceAsync(Guid serviceId);
}
