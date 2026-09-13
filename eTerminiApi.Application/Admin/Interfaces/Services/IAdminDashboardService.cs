using eTerminiAPI.Application.Admin.DTOs.Dashboard;

namespace eTerminiAPI.Application.Admin.Interfaces.Services;

public interface IAdminDashboardService
{
    Task<DashboardStatsDto>                  GetStatsAsync();
    Task<IEnumerable<ActiveAppointmentDto>>  GetActiveAppointmentsAsync();
}
