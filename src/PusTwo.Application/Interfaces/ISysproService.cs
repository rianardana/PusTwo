using PusTwo.Application.Syspro.DTOs;

namespace PusTwo.Application.Syspro.Interfaces
{
    public interface ISysproService
    {
        Task<List<MachineLookupDto>> GetMachinesAsync();
        Task<JobLookupDto?> GetJobInfoAsync(string jobNumber);
        Task<List<NonProdGroupDto>> GetNonProdGroupsAsync();
        Task<List<NonProdCodeDto>> GetNonProdCodesByGroupAsync(string groupCode);
        Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber);
        Task<List<DowntimeRecordDto>> GetDowntimeHistoryAsync(DateTime dateFrom, DateTime dateTo);
    }
}