using Microsoft.EntityFrameworkCore;
using PusTwo.Domain.Entities;

namespace PusTwo.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DownTime> DownTimes => Set<DownTime>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DownTime>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Machine).HasMaxLength(50).IsRequired();
                entity.Property(e => e.WorkCentre).HasMaxLength(50);
                entity.Property(e => e.JobNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.StockCode).HasMaxLength(100);
                entity.Property(e => e.GroupCode).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Remark).HasMaxLength(500);
                entity.Property(e => e.Shift).HasMaxLength(10);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.HasIndex(e => e.JobNumber);
                entity.HasIndex(e => e.Machine);
                entity.HasIndex(e => e.EntryDate);
            });
        }
    }
}