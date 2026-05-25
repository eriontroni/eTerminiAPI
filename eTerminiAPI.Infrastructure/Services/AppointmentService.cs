using eTerminiAPI.Application.DTOs.Appointments;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Entities;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _uow;

    public AppointmentService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto, Guid userId, Guid tenantId)
    {
        var doctors = await _uow.StaffMembers.FindAsync(s => s.Id == dto.DoctorId && s.IsActive);
        var doctor = doctors.FirstOrDefault()
            ?? throw new KeyNotFoundException("Mjeku/stafi nuk u gjet ose nuk është aktiv.");

        if (dto.AppointmentDate <= DateTime.UtcNow)
            throw new ArgumentException("Data e terminit duhet të jetë në të ardhmen.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            Status = AppointmentStatus.Pending,
            Notes = dto.Notes,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.Appointments.AddAsync(appointment);
        await _uow.SaveChangesAsync();

        var users = await _uow.Users.FindAsync(u => u.Id == userId);
        var user = users.FirstOrDefault();

        var doctorUsers = await _uow.Users.FindAsync(u => u.Id == doctor.UserId);
        var doctorUser = doctorUsers.FirstOrDefault();

        return MapToResponse(appointment, user, doctor, doctorUser);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(Guid tenantId)
    {
        var appointments = (await _uow.Appointments.GetAllAsync())
            .Where(a => a.TenantId == tenantId)
            .ToList();

        return await EnrichAndMap(appointments);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetByUserIdAsync(Guid userId)
    {
        var appointments = (await _uow.Appointments.FindAsync(a => a.UserId == userId)).ToList();
        return await EnrichAndMap(appointments);
    }

    public async Task<AppointmentResponseDto> GetByIdAsync(Guid id)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Termini nuk u gjet.");

        var results = await EnrichAndMap(new List<Appointment> { appointment });
        return results.First();
    }

    public async Task<AppointmentResponseDto> UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto, Guid changedByUserId)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Termini nuk u gjet.");

        var oldStatus = appointment.Status;

        var history = new AppointmentStatusHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            OldStatus = oldStatus,
            NewStatus = dto.Status,
            Reason = dto.Reason,
            ChangedByUserId = changedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.AppointmentStatusHistories.AddAsync(history);

        appointment.Status = dto.Status;
        appointment.UpdatedAt = DateTime.UtcNow;
        _uow.Appointments.Update(appointment);

        await _uow.SaveChangesAsync();

        var results = await EnrichAndMap(new List<Appointment> { appointment });
        return results.First();
    }

    public async Task DeleteAsync(Guid id, Guid requestingUserId, bool isStaffOrAbove)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Termini nuk u gjet.");

        if (!isStaffOrAbove && appointment.UserId != requestingUserId)
            throw new UnauthorizedAccessException("Nuk keni leje të fshini këtë termin.");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Terminet e kryera nuk mund të fshihen.");

        appointment.IsDeleted = true;
        appointment.UpdatedAt = DateTime.UtcNow;
        _uow.Appointments.Update(appointment);

        await _uow.SaveChangesAsync();
    }

    private async Task<IEnumerable<AppointmentResponseDto>> EnrichAndMap(List<Appointment> appointments)
    {
        if (!appointments.Any())
            return Enumerable.Empty<AppointmentResponseDto>();

        var userIds = appointments.Select(a => a.UserId).ToHashSet();
        var doctorIds = appointments
            .Where(a => a.DoctorId.HasValue)
            .Select(a => a.DoctorId!.Value)
            .ToHashSet();

        var users = (await _uow.Users.FindAsync(u => userIds.Contains(u.Id)))
            .ToDictionary(u => u.Id);

        var staffMembers = doctorIds.Any()
            ? (await _uow.StaffMembers.FindAsync(s => doctorIds.Contains(s.Id)))
                .ToDictionary(s => s.Id)
            : new Dictionary<Guid, StaffMember>();

        var staffUserIds = staffMembers.Values.Select(s => s.UserId).ToHashSet();
        var staffUsers = staffUserIds.Any()
            ? (await _uow.Users.FindAsync(u => staffUserIds.Contains(u.Id)))
                .ToDictionary(u => u.Id)
            : new Dictionary<Guid, User>();

        return appointments.Select(a =>
        {
            users.TryGetValue(a.UserId, out var user);
            StaffMember? doctor = a.DoctorId.HasValue && staffMembers.TryGetValue(a.DoctorId.Value, out var sm) ? sm : null;
            User? doctorUser = doctor != null && staffUsers.TryGetValue(doctor.UserId, out var du) ? du : null;
            return MapToResponse(a, user, doctor, doctorUser);
        });
    }

    private static AppointmentResponseDto MapToResponse(
        Appointment a,
        User? user,
        StaffMember? doctor,
        User? doctorUser)
    {
        return new AppointmentResponseDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserFullName = user is null ? string.Empty : $"{user.FirstName} {user.LastName}",
            DoctorId = a.DoctorId,
            DoctorFullName = doctorUser is null ? string.Empty : $"{doctorUser.FirstName} {doctorUser.LastName}",
            DoctorTitle = doctor?.Title ?? string.Empty,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status.ToString(),
            StatusCode = (int)a.Status,
            Notes = a.Notes,
            TenantId = a.TenantId,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}
