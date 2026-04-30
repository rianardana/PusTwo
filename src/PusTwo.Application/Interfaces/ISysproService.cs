using PusTwo.Application.DTOs.Syspro;

namespace PusTwo.Application.Interfaces
{
    public interface ISysproService
    {
        Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber);
        Task<List<MachineLookupDto>> GetMachinesAsync();
        Task<JobLookupDto?> GetJobInfoAsync(string jobNumber);
        Task<List<NonProdGroupDto>> GetNonProdGroupsAsync();
        Task<List<NonProdCodeDto>> GetNonProdCodesByGroupAsync(string groupCode);
        Task<List<DowntimeRecordDto>> GetDowntimeHistoryAsync(DateTime dateFrom, DateTime dateTo);
    }
}