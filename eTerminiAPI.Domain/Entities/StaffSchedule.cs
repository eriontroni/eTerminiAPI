using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class StaffSchedule : TenantBaseEntity
{
    public Guid StaffMemberId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public StaffMember StaffMember { get; set; } = null!;
}
