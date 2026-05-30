namespace eTerminiAPI.Application.DTOs.Institutions;

public class InstitutionSearchResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
}
