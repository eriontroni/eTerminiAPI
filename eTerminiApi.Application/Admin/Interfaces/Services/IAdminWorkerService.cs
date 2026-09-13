using eTerminiAPI.Application.Admin.DTOs.Workers;

namespace eTerminiAPI.Application.Admin.Interfaces.Services;

public interface IAdminWorkerService
{
    Task<IEnumerable<WorkerAdminDto>> GetAllAsync();
    Task<WorkerAdminDto>              GetByIdAsync(Guid id);
    Task<WorkerAdminDto>              CreateAsync(CreateWorkerDto dto);
    Task<WorkerAdminDto>              UpdateAsync(Guid id, UpdateWorkerDto dto);
    Task<WorkerAdminDto>              ToggleActiveAsync(Guid id);
    Task<WorkerAdminDto>              AssignInstitutionAsync(Guid id, AssignInstitutionDto dto);
    Task                             DeleteAsync(Guid id);
}
