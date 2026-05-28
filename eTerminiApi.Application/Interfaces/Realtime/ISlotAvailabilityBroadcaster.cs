namespace eTerminiAPI.Application.Interfaces.Realtime;

public interface ISlotAvailabilityBroadcaster
{
    Task SlotsChangedAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
}
