using eTerminiAPI.Application.DTOs.Departments;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Entities;

namespace eTerminiAPI.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _uow;

    public DepartmentService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<DepartmentResponseDto>> GetAllAsync(Guid tenantId)
    {
        var departments = await _uow.Departments.FindAsync(d => d.TenantId == tenantId);
        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);
        var branches = (await _uow.InstitutionBranches.GetAllAsync()).ToDictionary(b => b.Id);

        return departments
            .OrderBy(d => d.Name)
            .Select(d => MapToResponse(d, institutions, branches));
    }

    public async Task<DepartmentResponseDto> GetByIdAsync(Guid id, Guid tenantId)
    {
        var department = await _uow.Departments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Departamenti nuk u gjet.");

        if (department.TenantId != tenantId)
            throw new UnauthorizedAccessException("Qasje e paautorizuar.");

        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);
        var branches = (await _uow.InstitutionBranches.GetAllAsync()).ToDictionary(b => b.Id);

        return MapToResponse(department, institutions, branches);
    }

    public async Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto, Guid tenantId)
    {
        var institution = await _uow.Institutions.GetByIdAsync(dto.InstitutionId)
            ?? throw new KeyNotFoundException("Institucioni nuk u gjet.");

        if (institution.TenantId != tenantId)
            throw new ArgumentException("Institucioni nuk i përket këtij tenanti.");

        var existing = await _uow.Departments.FindAsync(d =>
            d.InstitutionId == dto.InstitutionId &&
            d.Name == dto.Name &&
            d.TenantId == tenantId);

        if (existing.Any())
            throw new InvalidOperationException("Një departament me këtë emër ekziston tashmë në këtë institucion.");

        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description,
            InstitutionId = dto.InstitutionId,
            BranchId = dto.BranchId,
            TenantId = tenantId,
        };

        await _uow.Departments.AddAsync(department);
        await _uow.SaveChangesAsync();

        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);
        var branches = (await _uow.InstitutionBranches.GetAllAsync()).ToDictionary(b => b.Id);

        return MapToResponse(department, institutions, branches);
    }

    public async Task<DepartmentResponseDto> UpdateAsync(Guid id, UpdateDepartmentDto dto, Guid tenantId)
    {
        var department = await _uow.Departments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Departamenti nuk u gjet.");

        if (department.TenantId != tenantId)
            throw new UnauthorizedAccessException("Qasje e paautorizuar.");

        var institution = await _uow.Institutions.GetByIdAsync(dto.InstitutionId)
            ?? throw new KeyNotFoundException("Institucioni nuk u gjet.");

        if (institution.TenantId != tenantId)
            throw new ArgumentException("Institucioni nuk i përket këtij tenanti.");

        var duplicate = await _uow.Departments.FindAsync(d =>
            d.InstitutionId == dto.InstitutionId &&
            d.Name == dto.Name &&
            d.TenantId == tenantId &&
            d.Id != id);

        if (duplicate.Any())
            throw new InvalidOperationException("Një departament me këtë emër ekziston tashmë në këtë institucion.");

        department.Name = dto.Name;
        department.Description = dto.Description;
        department.InstitutionId = dto.InstitutionId;
        department.BranchId = dto.BranchId;
        department.IsActive = dto.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        _uow.Departments.Update(department);
        await _uow.SaveChangesAsync();

        var institutions = (await _uow.Institutions.GetAllAsync()).ToDictionary(i => i.Id);
        var branches = (await _uow.InstitutionBranches.GetAllAsync()).ToDictionary(b => b.Id);

        return MapToResponse(department, institutions, branches);
    }

    public async Task DeleteAsync(Guid id, Guid tenantId)
    {
        var department = await _uow.Departments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Departamenti nuk u gjet.");

        if (department.TenantId != tenantId)
            throw new UnauthorizedAccessException("Qasje e paautorizuar.");

        department.IsDeleted = true;
        department.UpdatedAt = DateTime.UtcNow;

        _uow.Departments.Update(department);
        await _uow.SaveChangesAsync();
    }

    private static DepartmentResponseDto MapToResponse(
        Department d,
        IReadOnlyDictionary<Guid, Institution> institutions,
        IReadOnlyDictionary<Guid, InstitutionBranch> branches)
    {
        institutions.TryGetValue(d.InstitutionId, out var institution);
        InstitutionBranch? branch = null;
        if (d.BranchId.HasValue) branches.TryGetValue(d.BranchId.Value, out branch);

        return new DepartmentResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            InstitutionId = d.InstitutionId,
            InstitutionName = institution?.Name ?? string.Empty,
            BranchId = d.BranchId,
            BranchName = branch?.Name,
            TenantId = d.TenantId,
            IsActive = d.IsActive,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
        };
    }
}
