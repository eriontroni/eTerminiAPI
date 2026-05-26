using System.ComponentModel.DataAnnotations;

namespace eTerminiAPI.Application.DTOs.TimeSlots;

public class AvailabilityRequestDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Range(5, 240)]
    public int DurationMinutes { get; set; } = 30;
}
