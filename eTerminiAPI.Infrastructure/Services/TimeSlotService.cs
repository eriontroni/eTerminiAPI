using eTerminiAPI.Application.DTOs.TimeSlots;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Infrastructure.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly IUnitOfWork _uow;

    // Slot times are stored/compared as naive local (Kosovo) time.
    private static readonly TimeZoneInfo KosovoTz = ResolveKosovoTimeZone();

    public TimeSlotService(IUnitOfWork uow)
    {
        _uow = uow;
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

        // Availability is read live from the database (no cache) so slots are always current.
        var doctors = await _uow.StaffMembers.FindAsync(s => s.Id == doctorId && s.IsActive);
        var doctor = doctors.FirstOrDefault()
            ?? throw new KeyNotFoundException("Mjeku/stafi nuk u gjet ose nuk është aktiv.");

        var day = date.Date;
        var dayOfWeek = day.DayOfWeek;

        // Working window comes from the staff schedule when defined; otherwise a default
        // 09:00–17:00 day is used so availability is driven by booked Appointments, not schedules.
        var schedules = await _uow.StaffSchedules.FindAsync(s =>
            s.StaffMemberId == doctorId &&
            s.DayOfWeek == dayOfWeek &&
            s.IsActive);

        var scheduleList = schedules
            .Select(s => (s.StartTime, s.EndTime))
            .ToList();

        if (scheduleList.Count == 0)
            scheduleList.Add((new TimeOnly(9, 0), new TimeOnly(17, 0)));

        var dayStart = day;
        var dayEnd = day.AddDays(1);
        var bookedAppointments = (await _uow.Appointments.FindAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.HasValue &&
                a.AppointmentDate >= dayStart &&
                a.AppointmentDate < dayEnd &&
                a.Status != AppointmentStatus.Cancelled))
            .Select(a => a.AppointmentDate!.Value)
            .ToList();

        var slots = new List<AvailableSlotDto>();
        // "Now" in Kosovo local time, matching the naive scale of the slot times.
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, KosovoTz);

        foreach (var (startTime, endTime) in scheduleList)
        {
            var slotStart = day.Add(startTime.ToTimeSpan());
            var scheduleEnd = day.Add(endTime.ToTimeSpan());

            while (slotStart.AddMinutes(durationMinutes) <= scheduleEnd)
            {
                var slotEnd = slotStart.AddMinutes(durationMinutes);

                var isBooked = bookedAppointments.Any(b => b >= slotStart && b < slotEnd);
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

        var conflicts = await _uow.Appointments.FindAsync(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDate.HasValue &&
            a.AppointmentDate >= slotStart &&
            a.AppointmentDate < slotEnd &&
            a.Status != AppointmentStatus.Cancelled);

        return !conflicts.Any();
    }
}
