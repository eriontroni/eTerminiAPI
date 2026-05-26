using eTerminiAPI.Domain.Entities;

namespace eTerminiAPI.Application.Interfaces.Notifications;

public interface IAppointmentNotifier
{
    Task SendReminderAsync(Appointment appointment, CancellationToken cancellationToken = default);
}
