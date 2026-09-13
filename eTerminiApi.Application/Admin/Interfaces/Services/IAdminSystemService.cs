using eTerminiAPI.Application.Admin.DTOs.System;

namespace eTerminiAPI.Application.Admin.Interfaces.Services;

public interface IAdminSystemService
{
    Task<IEnumerable<AuditLogDto>>  GetLogsAsync(int page = 1, int pageSize = 50);
    Task<IEnumerable<UserAdminDto>> GetUsersAsync();
}
