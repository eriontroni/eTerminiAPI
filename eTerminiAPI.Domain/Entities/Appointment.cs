using eTerminiAPI.Domain.Common;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Domain.Entities;

public class Appointment : TenantBaseEntity
{
    public Guid UserId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public Guid? TimeSlotId { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public StaffMember? Doctor { get; set; }
    public TimeSlot? TimeSlot { get; set; }
    public ICollection<AppointmentStatusHistory> StatusHistory { get; set; } = new List<AppointmentStatusHistory>();
}
