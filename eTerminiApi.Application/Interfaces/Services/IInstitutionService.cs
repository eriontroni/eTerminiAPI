using eTerminiAPI.Application.DTOs.Institutions;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface IInstitutionService
{
    Task<InstitutionContextDto> GetContextAsync(Guid tenantId);
    Task<IEnumerable<InstitutionSearchResultDto>> SearchAsync(string query, Guid tenantId, int limit = 20);
}
