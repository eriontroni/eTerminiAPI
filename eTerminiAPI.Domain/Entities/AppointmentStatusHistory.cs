using eTerminiAPI.Domain.Common;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Domain.Entities;

public class AppointmentStatusHistory : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public AppointmentStatus OldStatus { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    public string? Reason { get; set; }
    public Guid ChangedByUserId { get; set; }

    public Appointment Appointment { get; set; } = null!;
}
