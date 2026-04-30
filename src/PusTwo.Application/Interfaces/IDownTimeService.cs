using PusTwo.Application.DTOs;

namespace PusTwo.Application.Interfaces
{
    public interface IDownTimeService
    {
        Task<DownTimeDto> CreateDownTimeAsync(DownTimeDto dto);
        Task<List<DownTimeDto>> GetDownTimesByJobAsync(string jobNumber);
        Task<List<DownTimeDto>> GetDownTimesByDateRangeAsync(DateTime from, DateTime to);
        Task<bool> DeleteDownTimeAsync(int id);
    }
}