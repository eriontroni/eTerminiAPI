using eTerminiAPI.Application.DTOs.Statistics;

namespace eTerminiAPI.Application.Interfaces.Services;

public interface IStatisticsService
{
    Task<StatisticsDto> GetStatisticsAsync();
}
