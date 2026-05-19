using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class Department : TenantBaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;
    public InstitutionBranch? Branch { get; set; }
    public ICollection<PublicService> Services { get; set; } = new List<PublicService>();
    public ICollection<StaffMember> StaffMembers { get; set; } = new List<StaffMember>();
}
