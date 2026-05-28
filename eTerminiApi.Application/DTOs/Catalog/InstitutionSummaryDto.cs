namespace eTerminiAPI.Application.DTOs.Catalog;

public class InstitutionSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public int ServiceCount { get; set; }
}
