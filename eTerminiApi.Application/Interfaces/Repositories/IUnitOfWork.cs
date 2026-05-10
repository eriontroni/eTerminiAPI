using eTerminiAPI.Domain.Entities;

namespace eTerminiAPI.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Tenant> Tenants { get; }
    IGenericRepository<User> Users { get; }
    IGenericRepository<Institution> Institutions { get; }
    IGenericRepository<InstitutionBranch> InstitutionBranches { get; }
    IGenericRepository<Department> Departments { get; }
    IGenericRepository<ServiceCategory> ServiceCategories { get; }
    IGenericRepository<PublicService> PublicServices { get; }
    IGenericRepository<ServiceRequirement> ServiceRequirements { get; }
    IGenericRepository<StaffMember> StaffMembers { get; }
    IGenericRepository<StaffSchedule> StaffSchedules { get; }
    IGenericRepository<TimeSlot> TimeSlots { get; }
    IGenericRepository<Appointment> Appointments { get; }
    IGenericRepository<AppointmentStatusHistory> AppointmentStatusHistories { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<NotificationTemplate> NotificationTemplates { get; }
    IGenericRepository<AuditLog> AuditLogs { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync();
}
