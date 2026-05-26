using eTerminiAPI.Application.Interfaces.Notifications;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Domain.Enums;
using eTerminiAPI.Infrastructure.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eTerminiAPI.Infrastructure.BackgroundServices;

public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppointmentReminderService> _logger;
    private readonly ReminderOptions _options;

    public AppointmentReminderService(
        IServiceScopeFactory scopeFactory,
        IOptions<ReminderOptions> options,
        ILogger<AppointmentReminderService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AppointmentReminderService u nis. Poll {PollMin} min, lead {LeadMin} min, tolerance {TolMin} min.",
            _options.PollIntervalMinutes, _options.LeadTimeMinutes, _options.ToleranceMinutes);

        var pollInterval = TimeSpan.FromMinutes(Math.Max(1, _options.PollIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë përpunimit të përkujtuesve.");
            }

            try
            {
                await Task.Delay(pollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("AppointmentReminderService po ndalet.");
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notifier = scope.ServiceProvider.GetRequiredService<IAppointmentNotifier>();

        var now = DateTime.UtcNow;
        var windowStart = now.AddMinutes(_options.LeadTimeMinutes - _options.ToleranceMinutes);
        var windowEnd = now.AddMinutes(_options.LeadTimeMinutes + _options.ToleranceMinutes);

        var upcoming = (await uow.Appointments.FindAsync(a =>
                a.AppointmentDate.HasValue &&
                a.AppointmentDate >= windowStart &&
                a.AppointmentDate <= windowEnd &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Completed &&
                a.Status != AppointmentStatus.NoShow))
            .ToList();

        if (upcoming.Count == 0)
            return;

        var existingReminders = (await uow.Notifications.FindAsync(n =>
                n.Type == LogAppointmentNotifier.ReminderType))
            .Select(n => n.Message)
            .ToList();

        var sent = 0;
        foreach (var appointment in upcoming)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var marker = $"[{appointment.Id}]";
            if (existingReminders.Any(m => m.Contains(marker)))
                continue;

            try
            {
                await notifier.SendReminderAsync(appointment, cancellationToken);
                sent++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Përkujtuesi dështoi për terminin {AppointmentId}.", appointment.Id);
            }
        }

        if (sent > 0)
            _logger.LogInformation("U dërguan {Count} përkujtues termini.", sent);
    }
}
