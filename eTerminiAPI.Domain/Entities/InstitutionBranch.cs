using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class InstitutionBranch : TenantBaseEntity
{
    public Guid InstitutionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
