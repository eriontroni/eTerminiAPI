using eTerminiAPI.Domain.Common;

namespace eTerminiAPI.Domain.Entities;

public class ServiceRequirement : BaseEntity
{
    public Guid ServiceId { get; set; }
    public string Description { get; set; } = string.Empty;

    public PublicService Service { get; set; } = null!;
}
