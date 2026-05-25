using System.ComponentModel.DataAnnotations;
using eTerminiAPI.Domain.Enums;

namespace eTerminiAPI.Application.DTOs.Appointments;

public class UpdateAppointmentStatusDto
{
    [Required]
    public AppointmentStatus Status { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}
