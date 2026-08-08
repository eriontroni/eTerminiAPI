using eTerminiAPI.Application.DTOs.Catalog;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;

namespace eTerminiAPI.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly IUnitOfWork _uow;

    public CatalogService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<ServiceCategoryDto>> GetCategoriesAsync(Guid? tenantId = null)
    {
        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToList();
        var institutions = (await _uow.Institutions.FindAsync(i =>
            i.IsActive && (tenantId == null || i.TenantId == tenantId))).ToList();

        return categories
            // When scoped to a tenant, only surface categories that actually have institutions there.
            .Where(c => tenantId == null || institutions.Any(i => i.CategoryId == c.Id))
            .Select(c =>
            {
                var institutionCount = institutions.Count(i => i.CategoryId == c.Id);

                return new ServiceCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ServiceCount = 0,
                    InstitutionCount = institutionCount,
                };
            }).OrderBy(c => c.Name);
    }

    public async Task<IEnumerable<InstitutionSummaryDto>> GetInstitutionsAsync(Guid? categoryId = null, Guid? tenantId = null)
    {
        var institutions = (await _uow.Institutions.FindAsync(i =>
            i.IsActive && (tenantId == null || i.TenantId == tenantId))).ToList();
        var services = (await _uow.PublicServices.FindAsync(s => s.IsActive)).ToList();

        return institutions
            .Where(i => !categoryId.HasValue || i.CategoryId == categoryId.Value)
            .Select(i =>
            {
                var serviceCount = services.Count(s => s.InstitutionId == i.Id);

                return new InstitutionSummaryDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    City = i.City,
                    Address = i.Address,
                    PhoneNumber = i.PhoneNumber,
                    ServiceCount = serviceCount,
                };
            })
            .OrderBy(i => i.Name);
    }

    public async Task<IEnumerable<PublicServiceDto>> GetServicesAsync(Guid? institutionId = null, Guid? categoryId = null, Guid? tenantId = null)
    {
        var services = (await _uow.PublicServices.FindAsync(s => s.IsActive)).ToList();
        var institutions = (await _uow.Institutions.GetAllAsync()).ToList();

        var instMap = institutions.ToDictionary(i => i.Id);

        // Restrict to services whose institution belongs to the user's tenant.
        var tenantInstitutionIds = tenantId == null
            ? null
            : institutions.Where(i => i.TenantId == tenantId).Select(i => i.Id).ToHashSet();

        return services
            .Where(s => !institutionId.HasValue || s.InstitutionId == institutionId.Value)
            .Where(s => tenantInstitutionIds == null || tenantInstitutionIds.Contains(s.InstitutionId))
            .Select(s => MapService(s, instMap))
            .OrderBy(s => s.Name);
    }

    public async Task<PublicServiceDto> GetServiceByIdAsync(Guid serviceId)
    {
        var service = await _uow.PublicServices.GetByIdAsync(serviceId)
            ?? throw new KeyNotFoundException("Shërbimi nuk u gjet.");

        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);

        return MapService(service, institutions);
    }

    public async Task<IEnumerable<ProviderDto>> GetProvidersForServiceAsync(Guid serviceId)
    {
        var service = await _uow.PublicServices.GetByIdAsync(serviceId)
            ?? throw new KeyNotFoundException("Shërbimi nuk u gjet.");

        var depts = (await _uow.Departments.FindAsync(d => d.InstitutionId == service.InstitutionId)).ToList();
        var deptIds = depts.Select(d => d.Id).ToHashSet();

        var staffMembers = (await _uow.StaffMembers.FindAsync(s =>
            deptIds.Contains(s.DepartmentId) && s.IsActive)).ToList();

        if (staffMembers.Count == 0)
            return Enumerable.Empty<ProviderDto>();

        var userIds = staffMembers.Select(s => s.UserId).ToHashSet();
        var users = (await _uow.Users.FindAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id);

        var departments = depts.ToDictionary(d => d.Id);

        return staffMembers.Select(s =>
        {
            users.TryGetValue(s.UserId, out var user);
            departments.TryGetValue(s.DepartmentId, out var dept);
            return new ProviderDto
            {
                Id = s.Id,
                FullName = user is null ? string.Empty : $"{user.FirstName} {user.LastName}",
                Title = s.Title,
                DepartmentId = s.DepartmentId,
                DepartmentName = dept?.Name ?? string.Empty,
            };
        }).OrderBy(p => p.FullName);
    }

    private static PublicServiceDto MapService(
        Domain.Entities.PublicService s,
        IReadOnlyDictionary<Guid, Domain.Entities.Institution> instMap)
    {
        instMap.TryGetValue(s.InstitutionId, out var inst);

        return new PublicServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            DurationMinutes = s.DurationMinutes,
            InstitutionId = s.InstitutionId,
            InstitutionName = inst?.Name ?? string.Empty,
        };
    }
}
