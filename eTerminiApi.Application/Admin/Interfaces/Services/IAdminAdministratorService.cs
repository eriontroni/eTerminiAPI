using eTerminiAPI.Application.Admin.DTOs.Administrators;

namespace eTerminiAPI.Application.Admin.Interfaces.Services;

public interface IAdminAdministratorService
{
    Task<IEnumerable<AdministratorDto>>          GetAllAsync();
    Task<(bool Success, string Message, AdministratorDto? Admin)> CreateAsync(CreateAdministratorDto dto);
    Task<(bool Success, string Message)>         ToggleActiveAsync(Guid id);
}
