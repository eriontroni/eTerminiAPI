using System.ComponentModel.DataAnnotations;

namespace eTerminiAPI.Application.DTOs.Appointments;

public class CreateAppointmentDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
