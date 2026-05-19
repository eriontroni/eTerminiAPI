namespace eTerminiAPI.Domain.Common;

public abstract class TenantBaseEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
