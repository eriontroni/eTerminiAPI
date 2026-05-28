namespace eTerminiAPI.Application.DTOs.Catalog;

public class ServiceCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int InstitutionCount { get; set; }
    public int ServiceCount { get; set; }
}
