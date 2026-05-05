using PusTwo.Application.DownTime.DTOs;

namespace PusTwo.Application.DownTime.Interfaces
{
    public interface IDownTimeService
    {
        Task<DownTimeResult> CreateAsync(CreateDownTimeCommand command);
        Task<int> CreateBatchAsync(CreateDownTimeBatchCommand batch);
        Task<DownTimeResult?> GetByIdAsync(int id);
        Task<List<DownTimeResult>> GetByJobAsync(string jobNumber);
        Task<List<DownTimeResult>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<List<DownTimeResult>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
    }
}