using eTerminiAPI.Application.DTOs.Institutions;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;

namespace eTerminiAPI.Infrastructure.Services;

public class InstitutionService : IInstitutionService
{
    private readonly IUnitOfWork _uow;

    public InstitutionService(IUnitOfWork uow) => _uow = uow;

    public async Task<InstitutionContextDto> GetContextAsync(Guid tenantId)
    {
        var tenant = await _uow.Tenants.GetByIdAsync(tenantId)
            ?? throw new KeyNotFoundException("Tenanti nuk u gjet.");

        var institutions = await _uow.Institutions.FindAsync(i => i.TenantId == tenantId && i.IsActive);

        var city = institutions
            .GroupBy(i => i.City)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? string.Empty;

        return new InstitutionContextDto
        {
            City = city,
            TenantName = tenant.Name,
        };
    }

    public async Task<IEnumerable<InstitutionSearchResultDto>> SearchAsync(string query, Guid tenantId, int limit = 20)
    {
        var trimmed = query.Trim();

        var institutions = await _uow.Institutions.FindAsync(i =>
            i.TenantId == tenantId &&
            i.IsActive &&
            i.Name.Contains(trimmed));

        if (!institutions.Any())
            return Enumerable.Empty<InstitutionSearchResultDto>();

        var institutionIds = institutions.Select(i => i.Id).ToHashSet();

        var departments = await _uow.Departments.FindAsync(d =>
            d.TenantId == tenantId && institutionIds.Contains(d.InstitutionId));

        var departmentIds = departments.Select(d => d.Id).ToHashSet();

        var services = await _uow.PublicServices.FindAsync(s =>
            s.TenantId == tenantId && s.IsActive && departmentIds.Contains(s.DepartmentId));

        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToDictionary(c => c.Id);

        var deptToInstitution = departments.ToDictionary(d => d.Id, d => d.InstitutionId);

        // Map each institution to its most-used category
        var institutionToCategory = services
            .Where(s => deptToInstitution.ContainsKey(s.DepartmentId))
            .GroupBy(s => deptToInstitution[s.DepartmentId])
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(s => s.CategoryId)
                       .OrderByDescending(cg => cg.Count())
                       .Select(cg => cg.Key)
                       .FirstOrDefault());

        return institutions
            .OrderBy(i => i.Name)
            .Take(limit)
            .Select(i =>
            {
                string? categoryName = null;
                if (institutionToCategory.TryGetValue(i.Id, out var catId) && catId != Guid.Empty)
                    if (categories.TryGetValue(catId, out var cat))
                        categoryName = cat.Name;

                return new InstitutionSearchResultDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    City = i.City,
                    CategoryName = categoryName,
                };
            });
    }
}
