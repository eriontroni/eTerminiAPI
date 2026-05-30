using eTerminiAPI.Application.DTOs.Statistics;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;

namespace eTerminiAPI.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _uow;

    public StatisticsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var institutions = (await _uow.Institutions.FindAsync(i => i.IsActive)).ToList();
        var users = (await _uow.Users.GetAllAsync()).ToList();

        var totalCities = institutions
            .Where(i => !string.IsNullOrWhiteSpace(i.City))
            .Select(i => i.City.Trim().ToLowerInvariant())
            .Distinct()
            .Count();

        return new StatisticsDto
        {
            TotalCities = totalCities,
            TotalInstitutions = institutions.Count,
            TotalUsers = users.Count,
        };
    }
}
