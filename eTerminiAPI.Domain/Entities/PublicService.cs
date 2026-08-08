using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class PublicService : TenantBaseEntity
{
    public Guid InstitutionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;
    public ICollection<ServiceRequirement> Requirements { get; set; } = new List<ServiceRequirement>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
