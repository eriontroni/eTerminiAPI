using eTerminiAPI.Domain.Common;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Domain.Entities;

public class User : TenantBaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Citizen;
    public bool IsActive { get; set; } = true;

    /// <summary>Roli i Admin Panel (RBAC). Null për përdorues jo-admin ose SuperAdmin.</summary>
    public Guid? AdminRoleId { get; set; }
    public AdminRole? AdminRole { get; set; }

    public Tenant? Tenant { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
