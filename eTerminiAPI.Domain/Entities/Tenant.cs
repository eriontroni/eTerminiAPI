using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TenantSetting> Settings { get; set; } = new List<TenantSetting>();
    public ICollection<Institution> Institutions { get; set; } = new List<Institution>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
