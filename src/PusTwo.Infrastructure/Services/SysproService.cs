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

    

            public async Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber)
            {
                try
                {
                    await using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string sql = @"
                        SELECT 
                            j.StockCode,
                            b.Route,
                            b.Operation,
                            b.WorkCentre
                        FROM JOB$ j
                        INNER JOIN BomOperations b ON j.StockCode = b.StockCode
                        WHERE j.Job = @JobNumber 
                        AND LTRIM(RTRIM(b.Route)) = '0'
                        ORDER BY b.Operation ASC";

                    var result = await connection.QueryAsync<BomOperationDto>(sql, new { JobNumber = jobNumber });
                    return result.ToList();
                }
                catch (Exception ex)
                {
                   throw new InvalidOperationException(
        $"Gagal koneksi ke Syspro: {ex.Message}\nInner: {ex.InnerException?.Message}", ex);
                }
            }
    }
}