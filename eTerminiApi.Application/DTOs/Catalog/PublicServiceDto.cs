namespace eTerminiAPI.Application.DTOs.Catalog;

public class PublicServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public Guid InstitutionId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}
