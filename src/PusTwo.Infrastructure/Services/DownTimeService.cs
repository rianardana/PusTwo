using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PusTwo.Application.DTOs;
using PusTwo.Application.Interfaces;
using PusTwo.Domain.Entities;
using PusTwo.Infrastructure.Data;

namespace PusTwo.Infrastructure.Services
{
    public class DownTimeService : IDownTimeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DownTimeService> _logger;

        public DownTimeService(AppDbContext context, ILogger<DownTimeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DownTimeDto> CreateDownTimeAsync(DownTimeDto dto)
        {
            var entity = new DownTime
            {
                Machine = dto.Machine,
                WorkCentre = dto.WorkCentre,
                JobNumber = dto.JobNumber,
                StockCode = dto.StockCode,
                GroupCode = dto.GroupCode,
                Code = dto.Code,
                Description = dto.Description,
                DowntimeMinutes = dto.DowntimeMinutes,
                Remark = dto.Remark,
                EntryDate = dto.EntryDate,
                Shift = dto.Shift,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.DownTimes.Add(entity);
            await _context.SaveChangesAsync();

            return new DownTimeDto
            {
                Id = entity.Id,
                Machine = entity.Machine,
                WorkCentre = entity.WorkCentre,
                JobNumber = entity.JobNumber,
                StockCode = entity.StockCode,
                GroupCode = entity.GroupCode,
                Code = entity.Code,
                Description = entity.Description,
                DowntimeMinutes = entity.DowntimeMinutes,
                Remark = entity.Remark,
                EntryDate = entity.EntryDate,
                Shift = entity.Shift,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<List<DownTimeDto>> GetDownTimesByJobAsync(string jobNumber)
        {
            return await _context.DownTimes
                .AsNoTracking()
                .Where(d => d.JobNumber == jobNumber && !d.IsDeleted)
                .Select(d => new DownTimeDto
                {
                    Id = d.Id,
                    Machine = d.Machine,
                    WorkCentre = d.WorkCentre,
                    JobNumber = d.JobNumber,
                    StockCode = d.StockCode,
                    GroupCode = d.GroupCode,
                    Code = d.Code,
                    Description = d.Description,
                    DowntimeMinutes = d.DowntimeMinutes,
                    Remark = d.Remark,
                    EntryDate = d.EntryDate,
                    Shift = d.Shift,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                })
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task<List<DownTimeDto>> GetDownTimesByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.DownTimes
                .AsNoTracking()
                .Where(d => d.EntryDate >= from && d.EntryDate <= to && !d.IsDeleted)
                .Select(d => new DownTimeDto
                {
                    Id = d.Id,
                    Machine = d.Machine,
                    WorkCentre = d.WorkCentre,
                    JobNumber = d.JobNumber,
                    StockCode = d.StockCode,
                    GroupCode = d.GroupCode,
                    Code = d.Code,
                    Description = d.Description,
                    DowntimeMinutes = d.DowntimeMinutes,
                    Remark = d.Remark,
                    EntryDate = d.EntryDate,
                    Shift = d.Shift,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                })
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task<bool> DeleteDownTimeAsync(int id)
        {
            var entity = await _context.DownTimes.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}