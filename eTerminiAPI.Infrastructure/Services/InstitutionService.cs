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

        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToDictionary(c => c.Id);

        return institutions
            .OrderBy(i => i.Name)
            .Take(limit)
            .Select(i =>
            {
                string? categoryName = null;
                if (i.CategoryId.HasValue && categories.TryGetValue(i.CategoryId.Value, out var cat))
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
