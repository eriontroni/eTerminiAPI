using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class AdminRole : TenantBaseEntity
{
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Rolet sistem (p.sh. SuperAdmin) nuk mund të editohen ose fshihen.</summary>
    public bool IsSystem { get; set; }

    /// <summary>Lista e kodeve të lejeve (p.sh. "tenants.view"). Ruhet si kolonë JSON.</summary>
    public List<string> Permissions { get; set; } = new();

    public ICollection<User> Users { get; set; } = new List<User>();
}
