namespace eTerminiAPI.Application.DTOs.Appointments;

public class AppointmentResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public Guid? DoctorId { get; set; }
    public string DoctorFullName { get; set; } = string.Empty;
    public string DoctorTitle { get; set; } = string.Empty;
    public DateTime? AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? Notes { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
