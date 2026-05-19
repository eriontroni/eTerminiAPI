using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class Notification : TenantBaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public string? Type { get; set; }

    public User User { get; set; } = null!;
}
