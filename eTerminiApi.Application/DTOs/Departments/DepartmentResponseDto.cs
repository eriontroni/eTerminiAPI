namespace eTerminiAPI.Application.DTOs.Departments;

public class DepartmentResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstitutionId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid TenantId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
