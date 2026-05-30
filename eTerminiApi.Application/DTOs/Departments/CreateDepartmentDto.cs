using System.ComponentModel.DataAnnotations;

namespace eTerminiAPI.Application.DTOs.Departments;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Emri është i detyrueshëm.")]
    [MaxLength(200, ErrorMessage = "Emri nuk mund të jetë më i gjatë se 200 karaktere.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Përshkrimi nuk mund të jetë më i gjatë se 1000 karaktere.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Institucioni është i detyrueshëm.")]
    public Guid InstitutionId { get; set; }

    public Guid? BranchId { get; set; }
}
