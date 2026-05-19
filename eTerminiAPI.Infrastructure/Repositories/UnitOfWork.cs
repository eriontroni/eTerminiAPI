using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Domain.Entities;
using eTerminiAPI.Infrastructure.Persistence;

namespace eTerminiAPI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IGenericRepository<Tenant> Tenants { get; }
    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Institution> Institutions { get; }
    public IGenericRepository<InstitutionBranch> InstitutionBranches { get; }
    public IGenericRepository<Department> Departments { get; }
    public IGenericRepository<ServiceCategory> ServiceCategories { get; }
    public IGenericRepository<PublicService> PublicServices { get; }
    public IGenericRepository<ServiceRequirement> ServiceRequirements { get; }
    public IGenericRepository<StaffMember> StaffMembers { get; }
    public IGenericRepository<StaffSchedule> StaffSchedules { get; }
    public IGenericRepository<TimeSlot> TimeSlots { get; }
    public IGenericRepository<Appointment> Appointments { get; }
    public IGenericRepository<AppointmentStatusHistory> AppointmentStatusHistories { get; }
    public IGenericRepository<Notification> Notifications { get; }
    public IGenericRepository<NotificationTemplate> NotificationTemplates { get; }
    public IGenericRepository<AuditLog> AuditLogs { get; }
    public IGenericRepository<RefreshToken> RefreshTokens { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Tenants = new GenericRepository<Tenant>(context);
        Users = new GenericRepository<User>(context);
        Institutions = new GenericRepository<Institution>(context);
        InstitutionBranches = new GenericRepository<InstitutionBranch>(context);
        Departments = new GenericRepository<Department>(context);
        ServiceCategories = new GenericRepository<ServiceCategory>(context);
        PublicServices = new GenericRepository<PublicService>(context);
        ServiceRequirements = new GenericRepository<ServiceRequirement>(context);
        StaffMembers = new GenericRepository<StaffMember>(context);
        StaffSchedules = new GenericRepository<StaffSchedule>(context);
        TimeSlots = new GenericRepository<TimeSlot>(context);
        Appointments = new GenericRepository<Appointment>(context);
        AppointmentStatusHistories = new GenericRepository<AppointmentStatusHistory>(context);
        Notifications = new GenericRepository<Notification>(context);
        NotificationTemplates = new GenericRepository<NotificationTemplate>(context);
        AuditLogs = new GenericRepository<AuditLog>(context);
        RefreshTokens = new GenericRepository<RefreshToken>(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
