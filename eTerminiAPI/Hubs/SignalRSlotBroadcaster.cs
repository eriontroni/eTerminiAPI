using eTerminiAPI.Application.Interfaces.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace eTerminiAPI.API.Hubs;

public class SignalRSlotBroadcaster : ISlotAvailabilityBroadcaster
{
    private readonly IHubContext<AppointmentsHub> _hub;

    public SignalRSlotBroadcaster(IHubContext<AppointmentsHub> hub)
    {
        _hub = hub;
    }

    public Task SlotsChangedAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        var group = AppointmentsHub.SlotGroupName(doctorId, date.Date);
        return _hub.Clients
            .Group(group)
            .SendAsync("SlotsUpdated", new { doctorId, date = date.Date }, cancellationToken);
    }
}
