using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PusTwo.Application.DTOs.Syspro;
using PusTwo.Application.Interfaces;

namespace PusTwo.Infrastructure.Services
{
    public class SysproService : ISysproService
    {
        private readonly string _connectionString;
        private readonly ILogger<SysproService> _logger;

        public SysproService(IConfiguration configuration, ILogger<SysproService> logger)
        {
            _connectionString = configuration.GetConnectionString("SysproConnection") 
                ?? throw new InvalidOperationException("Connection string 'SysproConnection' not found.");
            _logger = logger;
        }

        // ✅ EXISTING
        public async Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT j.StockCode, b.Route, b.Operation, b.WorkCentre
                    FROM JOB$ j
                    INNER JOIN BomOperations b ON j.StockCode = b.StockCode
                    WHERE j.Job = @JobNumber AND LTRIM(RTRIM(b.Route)) = '0'
                    ORDER BY b.Operation ASC";

                var result = await connection.QueryAsync<BomOperationDto>(sql, new { JobNumber = jobNumber });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching WorkCentres for Job {Job}", jobNumber);
                throw new InvalidOperationException($"Gagal mengambil data BOM: {ex.Message}", ex);
            }
        }

        // 🔹 NEW: Machine Lookup
        public async Task<List<MachineLookupDto>> GetMachinesAsync()
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT Machine, Description, WorkCentre
                    FROM BomMachine
                    ORDER BY Machine ASC";

                var result = await connection.QueryAsync<MachineLookupDto>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Machines from BomMachine");
                throw new InvalidOperationException($"Gagal mengambil data Machine: {ex.Message}", ex);
            }
        }

        // 🔹 NEW: Job → StockCode Lookup
        public async Task<JobLookupDto?> GetJobInfoAsync(string jobNumber)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT TOP 1 Job, StockCode, StockDescription
                    FROM WipMaster
                    WHERE Job = @JobNumber";

                return await connection.QueryFirstOrDefaultAsync<JobLookupDto>(sql, new { JobNumber = jobNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching JobInfo for Job {Job}", jobNumber);
                throw new InvalidOperationException($"Gagal mengambil data Job: {ex.Message}", ex);
            }
        }

        // 🔹 NEW: Non-Prod Groups
        public async Task<List<NonProdGroupDto>> GetNonProdGroupsAsync()
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT GrpCode, Description
                    FROM NonProdGrpCode$
                    ORDER BY GrpCode ASC";

                var result = await connection.QueryAsync<NonProdGroupDto>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching NonProdGroups");
                throw new InvalidOperationException($"Gagal mengambil data NonProdGroup: {ex.Message}", ex);
            }
        }

        // 🔹 NEW: Downtime History (Read-Only View)
        public async Task<List<DowntimeRecordDto>> GetDowntimeHistoryAsync(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT EntryDate, Shift, Job, StockCode, NonProdCode, GrpCode, GrpDescription, TeardownTime, Machine
                    FROM vwNonProdDwnTime
                    WHERE EntryDate >= @DateFrom AND EntryDate <= @DateTo
                    ORDER BY EntryDate DESC, Shift ASC";

                var result = await connection.QueryAsync<DowntimeRecordDto>(sql, new { DateFrom = dateFrom, DateTo = dateTo });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching DowntimeHistory for range {From} to {To}", dateFrom, dateTo);
                throw new InvalidOperationException($"Gagal mengambil history downtime: {ex.Message}", ex);
            }
        }
    }
}