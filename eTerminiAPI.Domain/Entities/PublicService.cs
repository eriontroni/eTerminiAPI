using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class PublicService : TenantBaseEntity
{
    public Guid DepartmentId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Department Department { get; set; } = null!;
    public ServiceCategory Category { get; set; } = null!;
    public ICollection<ServiceRequirement> Requirements { get; set; } = new List<ServiceRequirement>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
