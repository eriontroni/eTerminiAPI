using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class TenantSetting : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public Tenant Tenant { get; set; } = null!;
}
