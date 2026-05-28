using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace eTerminiAPI.API.Hubs;

[AllowAnonymous]
public class AppointmentsHub : Hub
{
    public Task JoinSlotGroup(Guid doctorId, DateTime date)
    {
        var group = SlotGroupName(doctorId, date);
        return Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    public Task LeaveSlotGroup(Guid doctorId, DateTime date)
    {
        var group = SlotGroupName(doctorId, date);
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

    public static string SlotGroupName(Guid doctorId, DateTime date) =>
        $"slots:{doctorId:N}:{date:yyyy-MM-dd}";
}
