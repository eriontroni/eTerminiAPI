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

    public async Task<IEnumerable<ServiceCategoryDto>> GetCategoriesAsync()
    {
        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToList();
        var services = (await _uow.PublicServices.FindAsync(s => s.IsActive)).ToList();
        var departments = (await _uow.Departments.GetAllAsync()).ToList();

        var deptToInst = departments.ToDictionary(d => d.Id, d => d.InstitutionId);

        return categories.Select(c =>
        {
            var serviceCount = services.Count(s => s.CategoryId == c.Id);
            var institutionIds = services
                .Where(s => s.CategoryId == c.Id)
                .Select(s => deptToInst.TryGetValue(s.DepartmentId, out var iid) ? iid : Guid.Empty)
                .Where(iid => iid != Guid.Empty)
                .Distinct()
                .Count();

            return new ServiceCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ServiceCount = serviceCount,
                InstitutionCount = institutionIds,
            };
        }).OrderBy(c => c.Name);
    }

    public async Task<IEnumerable<InstitutionSummaryDto>> GetInstitutionsAsync(Guid? categoryId = null)
    {
        var institutions = (await _uow.Institutions.FindAsync(i => i.IsActive)).ToList();
        var services = (await _uow.PublicServices.FindAsync(s => s.IsActive)).ToList();
        var departments = (await _uow.Departments.GetAllAsync()).ToList();

        var deptToInst = departments.ToDictionary(d => d.Id, d => d.InstitutionId);

        var filteredInstitutionIds = categoryId.HasValue
            ? services
                .Where(s => s.CategoryId == categoryId.Value)
                .Select(s => deptToInst.TryGetValue(s.DepartmentId, out var iid) ? iid : Guid.Empty)
                .Where(iid => iid != Guid.Empty)
                .ToHashSet()
            : null;

        return institutions
            .Where(i => filteredInstitutionIds == null || filteredInstitutionIds.Contains(i.Id))
            .Select(i =>
            {
                var serviceCount = services.Count(s =>
                    (!categoryId.HasValue || s.CategoryId == categoryId.Value) &&
                    deptToInst.TryGetValue(s.DepartmentId, out var iid) && iid == i.Id);

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

    public async Task<IEnumerable<PublicServiceDto>> GetServicesAsync(Guid? institutionId = null, Guid? categoryId = null)
    {
        var services = (await _uow.PublicServices.FindAsync(s => s.IsActive)).ToList();
        var departments = (await _uow.Departments.GetAllAsync()).ToList();
        var institutions = (await _uow.Institutions.GetAllAsync()).ToList();
        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToList();

        var deptMap = departments.ToDictionary(d => d.Id);
        var instMap = institutions.ToDictionary(i => i.Id);
        var catMap = categories.ToDictionary(c => c.Id);

        return services
            .Where(s => !categoryId.HasValue || s.CategoryId == categoryId.Value)
            .Where(s =>
            {
                if (!institutionId.HasValue) return true;
                return deptMap.TryGetValue(s.DepartmentId, out var d) && d.InstitutionId == institutionId.Value;
            })
            .Select(s => MapService(s, deptMap, instMap, catMap))
            .OrderBy(s => s.Name);
    }

    public async Task<PublicServiceDto> GetServiceByIdAsync(Guid serviceId)
    {
        var service = await _uow.PublicServices.GetByIdAsync(serviceId)
            ?? throw new KeyNotFoundException("Shërbimi nuk u gjet.");

        var departments = (await _uow.Departments.GetAllAsync()).ToDictionary(d => d.Id);
        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);
        var categories = (await _uow.ServiceCategories.GetAllAsync()).ToDictionary(c => c.Id);

        return MapService(service, departments, institutions, categories);
    }

    public async Task<IEnumerable<ProviderDto>> GetProvidersForServiceAsync(Guid serviceId)
    {
        var service = await _uow.PublicServices.GetByIdAsync(serviceId)
            ?? throw new KeyNotFoundException("Shërbimi nuk u gjet.");

        var staffMembers = (await _uow.StaffMembers.FindAsync(s =>
            s.DepartmentId == service.DepartmentId && s.IsActive)).ToList();

        if (staffMembers.Count == 0)
            return Enumerable.Empty<ProviderDto>();

        var userIds = staffMembers.Select(s => s.UserId).ToHashSet();
        var users = (await _uow.Users.FindAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id);

        var departments = (await _uow.Departments.GetAllAsync()).ToDictionary(d => d.Id);

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
        IReadOnlyDictionary<Guid, Domain.Entities.Department> deptMap,
        IReadOnlyDictionary<Guid, Domain.Entities.Institution> instMap,
        IReadOnlyDictionary<Guid, Domain.Entities.ServiceCategory> catMap)
    {
        deptMap.TryGetValue(s.DepartmentId, out var dept);
        Domain.Entities.Institution? inst = null;
        if (dept != null) instMap.TryGetValue(dept.InstitutionId, out inst);
        catMap.TryGetValue(s.CategoryId, out var cat);

        return new PublicServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            DurationMinutes = s.DurationMinutes,
            CategoryId = s.CategoryId,
            CategoryName = cat?.Name ?? string.Empty,
            InstitutionId = inst?.Id ?? Guid.Empty,
            InstitutionName = inst?.Name ?? string.Empty,
            DepartmentId = s.DepartmentId,
            DepartmentName = dept?.Name ?? string.Empty,
        };
    }
}
