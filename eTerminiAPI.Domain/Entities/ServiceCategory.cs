using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class ServiceCategory : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<PublicService> Services { get; set; } = new List<PublicService>();
}
