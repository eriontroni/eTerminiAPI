using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class Institution : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<InstitutionBranch> Branches { get; set; } = new List<InstitutionBranch>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
