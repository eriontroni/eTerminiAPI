using eTerminiAPI.Application.Interfaces.Notifications;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace eTerminiAPI.Infrastructure.Notifications;

public class LogAppointmentNotifier : IAppointmentNotifier
{
    public const string ReminderType = "AppointmentReminder";

    private readonly IUnitOfWork _uow;
    private readonly ILogger<LogAppointmentNotifier> _logger;

    public LogAppointmentNotifier(IUnitOfWork uow, ILogger<LogAppointmentNotifier> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task SendReminderAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var when = appointment.AppointmentDate?.ToString("yyyy-MM-dd HH:mm") ?? "?";
        var title = "Përkujtues termini";
        var message = $"[{appointment.Id}] Ju keni një termin më {when}.";

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = appointment.UserId,
            TenantId = appointment.TenantId,
            Title = title,
            Message = message,
            Type = ReminderType,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification);
        await _uow.SaveChangesAsync();

        _logger.LogInformation(
            "Përkujtues u dërgua për terminin {AppointmentId} (user {UserId}) për datën {AppointmentDate}.",
            appointment.Id, appointment.UserId, appointment.AppointmentDate);
    }
}
