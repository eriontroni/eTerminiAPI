using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class StaffMember : TenantBaseEntity
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<StaffSchedule> Schedules { get; set; } = new List<StaffSchedule>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
