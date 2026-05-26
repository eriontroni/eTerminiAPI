using eTerminiAPI.Application.DTOs.TimeSlots;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Infrastructure.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly IUnitOfWork _uow;

    public TimeSlotService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date, int durationMinutes = 30)
    {
        if (durationMinutes <= 0)
            throw new ArgumentException("Kohëzgjatja e terminit duhet të jetë pozitive.");

        var doctors = await _uow.StaffMembers.FindAsync(s => s.Id == doctorId && s.IsActive);
        var doctor = doctors.FirstOrDefault()
            ?? throw new KeyNotFoundException("Mjeku/stafi nuk u gjet ose nuk është aktiv.");

        var day = date.Date;
        var dayOfWeek = day.DayOfWeek;

        var schedules = await _uow.StaffSchedules.FindAsync(s =>
            s.StaffMemberId == doctorId &&
            s.DayOfWeek == dayOfWeek &&
            s.IsActive);

        var scheduleList = schedules.ToList();
        if (scheduleList.Count == 0)
            return Enumerable.Empty<AvailableSlotDto>();

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
        var now = DateTime.UtcNow;

        foreach (var schedule in scheduleList)
        {
            var slotStart = day.Add(schedule.StartTime.ToTimeSpan());
            var scheduleEnd = day.Add(schedule.EndTime.ToTimeSpan());

            while (slotStart.AddMinutes(durationMinutes) <= scheduleEnd)
            {
                var slotEnd = slotStart.AddMinutes(durationMinutes);

                var isBooked = bookedAppointments.Any(b => b >= slotStart && b < slotEnd);
                var isPast = slotStart <= now;

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

        return slots.OrderBy(s => s.StartTime);
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
