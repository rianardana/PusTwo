using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PusTwo.Application.DownTime.DTOs;
using PusTwo.Application.DownTime.Interfaces;
using PusTwo.Domain.Entities;
using PusTwo.Infrastructure.Data;

namespace PusTwo.Infrastructure.Services
{
    public class DownTimeService : IDownTimeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DownTimeService> _logger;

        public DownTimeService(AppDbContext context, IMapper mapper, ILogger<DownTimeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DownTimeResult> CreateAsync(CreateDownTimeCommand command)
        {
            try
            {
                var entity = _mapper.Map<DownTime>(command);
                _context.DownTimes.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Downtime created: Machine={Machine}, Job={Job}, Minutes={Minutes}", 
                    entity.Machine, entity.JobNumber, entity.DowntimeMinutes);

                return _mapper.Map<DownTimeResult>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating downtime entry");
                throw;
            }
        }

        public async Task<int> CreateBatchAsync(CreateDownTimeBatchCommand batch)
        {
            try
            {
                var entities = new List<DownTime>();

                foreach (var entry in batch.Entries)
                {
                    var entity = _mapper.Map<DownTime>(entry);
                    entity.EntryDate = batch.EntryDate;
                    entity.Shift = batch.Shift;
                    entity.Machine = batch.Machine;
                    entity.WorkCentre = batch.WorkCentre;
                    entity.JobNumber = batch.JobNumber;
                    entity.StockCode = batch.StockCode;
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.IsDeleted = false;

                    entities.Add(entity);
                }

                await _context.DownTimes.AddRangeAsync(entities);
                var savedCount = await _context.SaveChangesAsync();

                _logger.LogInformation("Batch downtime created: {Count} entries, Job={Job}", 
                    savedCount, batch.JobNumber);

                return savedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating batch downtime entries");
                throw;
            }
        }

        public async Task<DownTimeResult?> GetByIdAsync(int id)
        {
            var entity = await _context.DownTimes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            return entity != null ? _mapper.Map<DownTimeResult>(entity) : null;
        }

        public async Task<List<DownTimeResult>> GetAllAsync()
        {
            return await _context.DownTimes
                .AsNoTracking()
                .Where(d => !d.IsDeleted)
                .ProjectTo<DownTimeResult>(_mapper.ConfigurationProvider)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task<List<DownTimeResult>> GetByJobAsync(string jobNumber)
        {
            return await _context.DownTimes
                .AsNoTracking()
                .Where(d => d.JobNumber == jobNumber && !d.IsDeleted)
                .ProjectTo<DownTimeResult>(_mapper.ConfigurationProvider)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task<List<DownTimeResult>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.DownTimes
                .AsNoTracking()
                .Where(d => d.EntryDate >= from && d.EntryDate <= to && !d.IsDeleted)
                .ProjectTo<DownTimeResult>(_mapper.ConfigurationProvider)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.DownTimes.FindAsync(id);
                if (entity == null) return false;

                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Downtime deleted: Id={Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting downtime entry: Id={Id}", id);
                throw;
            }
        }
    }
}