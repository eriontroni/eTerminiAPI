namespace eTerminiAPI.Infrastructure.BackgroundServices;

public class ReminderOptions
{
    public const string SectionName = "Reminders";

    public int PollIntervalMinutes { get; set; } = 5;
    public int LeadTimeMinutes { get; set; } = 60 * 24;
    public int ToleranceMinutes { get; set; } = 30;
}
