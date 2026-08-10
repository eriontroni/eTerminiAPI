using eTerminiAPI.Application.DTOs.TimeSlots;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Enums;
using eTerminiAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eTerminiAPI.Infrastructure.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _db;

    // Slot times are stored/compared as naive local (Kosovo) time.
    private static readonly TimeZoneInfo KosovoTz = ResolveKosovoTimeZone();

    public TimeSlotService(IUnitOfWork uow, AppDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    private static TimeZoneInfo ResolveKosovoTimeZone()
    {
        foreach (var id in new[] { "Central European Standard Time", "Europe/Belgrade", "Europe/Tirane" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.Utc;
    }

    public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date, int durationMinutes = 30)
    {
        if (durationMinutes <= 0)
            throw new ArgumentException("Kohëzgjatja e terminit duhet të jetë pozitive.");

        var day = date.Date;
        var dayOfWeek = day.DayOfWeek;
        var dayEnd = day.AddDays(1);

        // Lean, no-tracking, projected reads — avoids materializing full entities and
        // keeps the request fast even under load.
        var doctorExists = await _db.StaffMembers
            .AsNoTracking()
            .AnyAsync(s => s.Id == doctorId && s.IsActive);

        if (!doctorExists)
            throw new KeyNotFoundException("Mjeku/stafi nuk u gjet ose nuk është aktiv.");

        var scheduleList = await _db.StaffSchedules
            .AsNoTracking()
            .Where(s => s.StaffMemberId == doctorId && s.DayOfWeek == dayOfWeek && s.IsActive)
            .Select(s => new { s.StartTime, s.EndTime })
            .ToListAsync();

        if (scheduleList.Count == 0)
            scheduleList.Add(new { StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) });

        var bookedAppointments = await _db.Appointments
            .AsNoTracking()
            .Where(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.HasValue &&
                a.AppointmentDate >= day &&
                a.AppointmentDate < dayEnd &&
                a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.AppointmentDate!.Value)
            .ToListAsync();

        // Sort once so overlap checks can short-circuit.
        bookedAppointments.Sort();

        var slots = new List<AvailableSlotDto>();
        // "Now" in Kosovo local time, matching the naive scale of the slot times.
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, KosovoTz);

        foreach (var window in scheduleList)
        {
            var slotStart = day.Add(window.StartTime.ToTimeSpan());
            var scheduleEnd = day.Add(window.EndTime.ToTimeSpan());

            while (slotStart.AddMinutes(durationMinutes) <= scheduleEnd)
            {
                var slotEnd = slotStart.AddMinutes(durationMinutes);

                var isBooked = false;
                for (var i = 0; i < bookedAppointments.Count; i++)
                {
                    var b = bookedAppointments[i];
                    if (b >= slotEnd) break;
                    if (b >= slotStart) { isBooked = true; break; }
                }

                // Only slots earlier than the current local moment are "past".
                var isPast = slotStart <= nowLocal;

                if (!isBooked && !isPast)
                {
                    slots.Add(new AvailableSlotDto
                    {
                        StartTime = slotStart,
                        EndTime = slotEnd,
                        DurationMinutes = durationMinutes
                    });
                }

                slotStart = slotEnd;
            }
        }

        return slots.OrderBy(s => s.StartTime).ToList();
    }

    public async Task<bool> IsSlotFreeAsync(Guid doctorId, DateTime slotStart, int durationMinutes = 30)
    {
        var slotEnd = slotStart.AddMinutes(durationMinutes);

        return !await _db.Appointments
            .AsNoTracking()
            .AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.HasValue &&
                a.AppointmentDate >= slotStart &&
                a.AppointmentDate < slotEnd &&
                a.Status != AppointmentStatus.Cancelled);
    }
}
