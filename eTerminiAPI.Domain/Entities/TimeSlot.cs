using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class TimeSlot : TenantBaseEntity
{
    public Guid ServiceId { get; set; }
    public Guid StaffMemberId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    public PublicService Service { get; set; } = null!;
    public StaffMember StaffMember { get; set; } = null!;
    public Appointment? Appointment { get; set; }
}
