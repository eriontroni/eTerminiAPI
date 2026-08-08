using eTerminiAPI.Application.DTOs.Appointments;
using eTerminiAPI.Application.Interfaces.Caching;
using eTerminiAPI.Application.Interfaces.Realtime;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Entities;
using eTerminiAPI.Domain.Enums;
using eTerminiAPI.Infrastructure.Caching;

namespace eTerminiAPI.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private const int DefaultSlotDurationMinutes = 30;
    private static readonly int[] CachedSlotDurations = { 15, 30, 45, 60 };

    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;
    private readonly ISlotAvailabilityBroadcaster _broadcaster;

    public AppointmentService(
        IUnitOfWork uow,
        ICacheService cache,
        ISlotAvailabilityBroadcaster broadcaster)
    {
        _uow = uow;
        _cache = cache;
        _broadcaster = broadcaster;
    }

    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto, Guid userId, Guid tenantId)
    {
        var doctors = await _uow.StaffMembers.FindAsync(s => s.Id == dto.DoctorId && s.IsActive);
        var doctor = doctors.FirstOrDefault()
            ?? throw new KeyNotFoundException("Mjeku/stafi nuk u gjet ose nuk është aktiv.");

        if (dto.AppointmentDate <= DateTime.UtcNow)
            throw new ArgumentException("Data e terminit duhet të jetë në të ardhmen.");

        var slotStart = dto.AppointmentDate;
        var slotEnd = slotStart.AddMinutes(DefaultSlotDurationMinutes);
        var windowStart = slotStart.AddMinutes(-DefaultSlotDurationMinutes);

        var conflicts = await _uow.Appointments.FindAsync(a =>
            a.DoctorId == dto.DoctorId &&
            a.AppointmentDate.HasValue &&
            a.AppointmentDate > windowStart &&
            a.AppointmentDate < slotEnd &&
            a.Status != AppointmentStatus.Cancelled);

        if (conflicts.Any())
            throw new InvalidOperationException("Ky termin është tashmë i rezervuar për këtë mjek.");

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

        await InvalidateAvailableSlotsCache(dto.DoctorId, dto.AppointmentDate);

        var results = await EnrichAndMap(new List<Appointment> { appointment });
        return results.First();
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

        if (appointment.DoctorId.HasValue && appointment.AppointmentDate.HasValue)
            await InvalidateAvailableSlotsCache(appointment.DoctorId.Value, appointment.AppointmentDate.Value);

        var results = await EnrichAndMap(new List<Appointment> { appointment });
        return results.First();
    }

    public async Task<AppointmentResponseDto> RescheduleAsync(
        Guid id,
        RescheduleAppointmentDto dto,
        Guid requestingUserId,
        bool isStaffOrAbove)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Termini nuk u gjet.");

        if (!isStaffOrAbove && appointment.UserId != requestingUserId)
            throw new UnauthorizedAccessException("Nuk keni leje të riprogramoni këtë termin.");

        if (appointment.Status == AppointmentStatus.Cancelled || appointment.Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Terminet e anuluara ose të kryera nuk mund të riprogramohen.");

        if (!appointment.DoctorId.HasValue)
            throw new InvalidOperationException("Termini nuk ka mjek të caktuar.");

        if (dto.AppointmentDate <= DateTime.UtcNow)
            throw new ArgumentException("Data e re duhet të jetë në të ardhmen.");

        var oldDate = appointment.AppointmentDate;
        if (oldDate == dto.AppointmentDate)
            throw new ArgumentException("Data e re është e njëjtë me datën aktuale.");

        var slotStart = dto.AppointmentDate;
        var slotEnd = slotStart.AddMinutes(DefaultSlotDurationMinutes);
        var windowStart = slotStart.AddMinutes(-DefaultSlotDurationMinutes);

        var conflicts = await _uow.Appointments.FindAsync(a =>
            a.Id != appointment.Id &&
            a.DoctorId == appointment.DoctorId &&
            a.AppointmentDate.HasValue &&
            a.AppointmentDate > windowStart &&
            a.AppointmentDate < slotEnd &&
            a.Status != AppointmentStatus.Cancelled);

        if (conflicts.Any())
            throw new InvalidOperationException("Ky termin është tashmë i rezervuar për këtë mjek.");

        var history = new AppointmentStatusHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            OldStatus = appointment.Status,
            NewStatus = appointment.Status,
            Reason = string.IsNullOrWhiteSpace(dto.Reason)
                ? $"Riprogramuar nga {oldDate:yyyy-MM-dd HH:mm} në {dto.AppointmentDate:yyyy-MM-dd HH:mm}."
                : $"{dto.Reason} (Riprogramuar nga {oldDate:yyyy-MM-dd HH:mm} në {dto.AppointmentDate:yyyy-MM-dd HH:mm}.)",
            ChangedByUserId = requestingUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _uow.AppointmentStatusHistories.AddAsync(history);

        appointment.AppointmentDate = dto.AppointmentDate;
        appointment.UpdatedAt = DateTime.UtcNow;
        _uow.Appointments.Update(appointment);

        await _uow.SaveChangesAsync();

        if (oldDate.HasValue)
            await InvalidateAvailableSlotsCache(appointment.DoctorId.Value, oldDate.Value);
        await InvalidateAvailableSlotsCache(appointment.DoctorId.Value, dto.AppointmentDate);

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

        if (appointment.DoctorId.HasValue && appointment.AppointmentDate.HasValue)
            await InvalidateAvailableSlotsCache(appointment.DoctorId.Value, appointment.AppointmentDate.Value);
    }

    private async Task InvalidateAvailableSlotsCache(Guid doctorId, DateTime date)
    {
        foreach (var duration in CachedSlotDurations)
        {
            var key = CacheKeys.AvailableSlots(doctorId, date.Date, duration);
            await _cache.RemoveAsync(key);
        }

        await _broadcaster.SlotsChangedAsync(doctorId, date.Date);
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

        var serviceIds = appointments
            .Select(a => ExtractServiceId(a.Notes))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        var services = serviceIds.Any()
            ? (await _uow.PublicServices.FindAsync(s => serviceIds.Contains(s.Id)))
                .ToDictionary(s => s.Id)
            : new Dictionary<Guid, PublicService>();

        var instIds = services.Values.Select(s => s.InstitutionId).ToHashSet();
        var institutions = instIds.Any()
            ? (await _uow.Institutions.FindAsync(i => instIds.Contains(i.Id)))
                .ToDictionary(i => i.Id)
            : new Dictionary<Guid, Institution>();

        return appointments.Select(a =>
        {
            users.TryGetValue(a.UserId, out var user);
            StaffMember? doctor = a.DoctorId.HasValue && staffMembers.TryGetValue(a.DoctorId.Value, out var sm) ? sm : null;
            User? doctorUser = doctor != null && staffUsers.TryGetValue(doctor.UserId, out var du) ? du : null;

            PublicService? svc = null;
            Institution? inst = null;
            var svcId = ExtractServiceId(a.Notes);
            if (svcId.HasValue && services.TryGetValue(svcId.Value, out var s))
            {
                svc = s;
                institutions.TryGetValue(s.InstitutionId, out inst);
            }

            return MapToResponse(a, user, doctor, doctorUser, svc, null, inst);
        });
    }

    private static string? EncodeServiceTag(Guid? serviceId, string? userNotes)
    {
        if (!serviceId.HasValue)
            return userNotes;
        var prefix = $"[svc:{serviceId.Value:N}]";
        return string.IsNullOrWhiteSpace(userNotes) ? prefix : $"{prefix} {userNotes}";
    }

    private static Guid? ExtractServiceId(string? notes)
    {
        if (string.IsNullOrEmpty(notes) || !notes.StartsWith("[svc:"))
            return null;
        var end = notes.IndexOf(']');
        if (end <= 5) return null;
        var raw = notes.Substring(5, end - 5);
        return Guid.TryParseExact(raw, "N", out var id) ? id : null;
    }

    private static string? StripServiceTag(string? notes)
    {
        if (string.IsNullOrEmpty(notes) || !notes.StartsWith("[svc:"))
            return notes;
        var end = notes.IndexOf(']');
        if (end < 0) return notes;
        var rest = notes.Substring(end + 1).TrimStart();
        return string.IsNullOrEmpty(rest) ? null : rest;
    }

    private static AppointmentResponseDto MapToResponse(
        Appointment a,
        User? user,
        StaffMember? doctor,
        User? doctorUser,
        PublicService? service = null,
        ServiceCategory? category = null,
        Institution? institution = null)
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
            Notes = StripServiceTag(a.Notes),
            ServiceId = service?.Id,
            ServiceName = service?.Name,
            CategoryId = category?.Id,
            CategoryName = category?.Name,
            InstitutionName = institution?.Name,
            TenantId = a.TenantId,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}
