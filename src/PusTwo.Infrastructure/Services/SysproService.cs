using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PusTwo.Application.DTOs.Syspro;
using PusTwo.Application.Interfaces;
using PusTwo.Infrastructure.Data;
using PusTwo.Infrastructure.Entities.Syspro;

namespace PusTwo.Infrastructure.Services
{
    public class SysproService : ISysproService
    {
        private readonly SysproDbContext _sysproContext;
        private readonly string _connectionString;
        private readonly ILogger<SysproService> _logger;

        public SysproService(
            SysproDbContext sysproContext,
            IConfiguration configuration,
            ILogger<SysproService> logger)
        {
            _sysproContext = sysproContext;
            _connectionString = configuration.GetConnectionString("SysproConnection") 
                ?? throw new InvalidOperationException("Connection string 'SysproConnection' not found.");
            _logger = logger;
        }

        public async Task<List<MachineLookupDto>> GetMachinesAsync()
        {
            try
            {
                return await _sysproContext.BomMachines
                    .AsNoTracking()
                    .Where(m => m.Machine.StartsWith("DB"))
                    .Select(m => new MachineLookupDto
                    {
                        Machine = m.Machine,
                        Description = m.Description,
                        WorkCentre = m.WorkCentre
                    })
                    .OrderBy(m => m.Machine)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Machines from BomMachine");
                throw new InvalidOperationException($"Gagal mengambil data Machine: {ex.Message}", ex);
            }
        }

        public async Task<JobLookupDto?> GetJobInfoAsync(string jobNumber)
        {
            try
            {
                return await _sysproContext.WipMasters
                    .AsNoTracking()
                    .Where(w => w.Job == jobNumber)
                    .Select(w => new JobLookupDto
                    {
                        Job = w.Job,
                        StockCode = w.StockCode,
                        StockDescription = w.StockDescription
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching JobInfo for Job {Job}", jobNumber);
                throw new InvalidOperationException($"Gagal mengambil data Job: {ex.Message}", ex);
            }
        }

        public async Task<List<NonProdGroupDto>> GetNonProdGroupsAsync()
        {
            try
            {
                return await _sysproContext.NonProdGrpDescs
                    .AsNoTracking()
                    .Where(g => g.GrpCode != null && g.GrpCode != "")
                    .Select(g => new NonProdGroupDto
                    {
                        GrpCode = g.GrpCode,
                        GrpDescription = g.GrpDescription
                    })
                    .OrderBy(g => g.GrpCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching NonProdGroups");
                throw new InvalidOperationException($"Gagal mengambil data NonProdGroup: {ex.Message}", ex);
            }
        }

        public async Task<List<NonProdCodeDto>> GetNonProdCodesByGroupAsync(string groupCode)
        {
            try
            {
                return await _sysproContext.NonProdGrpCodes
                    .AsNoTracking()
                    .Where(c => c.GrpCode == groupCode && c.NonProdScrap != null && c.NonProdScrap != "")
                    .Select(c => new NonProdCodeDto
                    {
                        GrpCode = c.GrpCode,
                        NonProdScrap = c.NonProdScrap,
                        Description = c.Description
                    })
                    .OrderBy(c => c.NonProdScrap)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching NonProdCodes for Group {Group}", groupCode);
                throw new InvalidOperationException($"Gagal mengambil data NonProdCode: {ex.Message}", ex);
            }
        }

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

        public async Task<List<DowntimeRecordDto>> GetDowntimeHistoryAsync(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                
                const string sql = @"
                    SELECT EntryDate, Shift, Job, StockCode, NonProdCode, GrpCode, 
                           GrpDescription, TeardownTime, Machine
                    FROM vwNonProdDwnTime
                    WHERE EntryDate >= @DateFrom AND EntryDate <= @DateTo
                    ORDER BY EntryDate DESC, Shift ASC";

                var result = await connection.QueryAsync<DowntimeRecordDto>(
                    sql, new { DateFrom = dateFrom, DateTo = dateTo });
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