namespace eTerminiAPI.Infrastructure.Caching;

internal static class CacheKeys
{
    public static string AvailableSlots(Guid doctorId, DateTime date, int durationMinutes) =>
        $"slots:available:{doctorId:N}:{date:yyyy-MM-dd}:{durationMinutes}";
}
