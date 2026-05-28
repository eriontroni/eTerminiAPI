using eTerminiAPI.Application.DTOs.Appointments;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto, Guid userId, Guid tenantId);
    Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(Guid tenantId);
    Task<IEnumerable<AppointmentResponseDto>> GetByUserIdAsync(Guid userId);
    Task<AppointmentResponseDto> GetByIdAsync(Guid id);
    Task<AppointmentResponseDto> UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto, Guid changedByUserId);
    Task<AppointmentResponseDto> RescheduleAsync(Guid id, RescheduleAppointmentDto dto, Guid requestingUserId, bool isStaffOrAbove);
    Task DeleteAsync(Guid id, Guid requestingUserId, bool isStaffOrAbove);
}
