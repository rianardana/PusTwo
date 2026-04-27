using PusTwo.Application.DTOs.Syspro;

namespace PusTwo.Application.Interfaces
{
    public interface ISysproService
    {
        
        Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber);

        // 🔹 NEW: Machine Lookup (BomMachine)
        Task<List<MachineLookupDto>> GetMachinesAsync();

        // 🔹 NEW: Job → StockCode Lookup (WipMaster)
        Task<JobLookupDto?> GetJobInfoAsync(string jobNumber);

        // 🔹 NEW: Non-Production Group Codes (NonProdGrpCode$)
        Task<List<NonProdGroupDto>> GetNonProdGroupsAsync();

        // 🔹 NEW: Downtime History Query (vwNonProdDwnTime)
        Task<List<DowntimeRecordDto>> GetDowntimeHistoryAsync(DateTime dateFrom, DateTime dateTo);
    }
}