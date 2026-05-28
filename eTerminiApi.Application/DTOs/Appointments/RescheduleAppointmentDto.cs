using System.ComponentModel.DataAnnotations;

namespace eTerminiAPI.Application.DTOs.Appointments;

public class RescheduleAppointmentDto
{
    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}
