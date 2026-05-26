using eTerminiAPI.Application.DTOs.TimeSlots;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface ITimeSlotService
{
    Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date, int durationMinutes = 30);
    Task<bool> IsSlotFreeAsync(Guid doctorId, DateTime slotStart, int durationMinutes = 30);
}
