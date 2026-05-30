using eTerminiAPI.Application.DTOs.Departments;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponseDto>> GetAllAsync(Guid tenantId);
    Task<DepartmentResponseDto> GetByIdAsync(Guid id, Guid tenantId);
    Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto, Guid tenantId);
    Task<DepartmentResponseDto> UpdateAsync(Guid id, UpdateDepartmentDto dto, Guid tenantId);
    Task DeleteAsync(Guid id, Guid tenantId);
}
