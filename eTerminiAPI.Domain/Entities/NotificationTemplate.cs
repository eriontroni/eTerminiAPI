using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class NotificationTemplate : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Type { get; set; }
    public bool IsActive { get; set; } = true;
}
