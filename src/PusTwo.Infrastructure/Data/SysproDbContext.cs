using Microsoft.EntityFrameworkCore;
using PusTwo.Infrastructure.Entities.Syspro;

namespace PusTwo.Infrastructure.Data
{
    public class SysproDbContext : DbContext
    {
        public SysproDbContext(DbContextOptions<SysproDbContext> options) : base(options) { }

        public DbSet<BomMachine> BomMachines => Set<BomMachine>();
        public DbSet<WipMaster> WipMasters => Set<WipMaster>();
        public DbSet<NonProdGrpCode> NonProdGrpCodes => Set<NonProdGrpCode>();
        public DbSet<NonProdGrpDesc> NonProdGrpDescs => Set<NonProdGrpDesc>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BomMachine>(entity =>
            {
                entity.ToTable("BomMachine");
                entity.HasKey(e => e.Machine);
                entity.Property(e => e.Machine).HasColumnName("Machine").HasMaxLength(20);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(50);
                entity.Property(e => e.WorkCentre).HasColumnName("WorkCentre").HasMaxLength(20);
            });

            modelBuilder.Entity<WipMaster>(entity =>
            {
                entity.ToTable("WipMaster");
                entity.HasKey(e => e.Job);
                entity.Property(e => e.Job).HasColumnName("Job").HasMaxLength(20);
                entity.Property(e => e.StockCode).HasColumnName("StockCode").HasMaxLength(50);
                entity.Property(e => e.StockDescription).HasColumnName("StockDescription").HasMaxLength(100);
            });

            modelBuilder.Entity<NonProdGrpCode>(entity =>
            {
                entity.ToTable("NonProdGrpCode$");
                entity.HasKey(e => e.GrpCode);
                entity.Property(e => e.GrpCode).HasColumnName("GrpCode").HasMaxLength(10);
                entity.Property(e => e.NonProdScrap).HasColumnName("NonProdScrap").HasMaxLength(10);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(50);
            });

            modelBuilder.Entity<NonProdGrpDesc>(entity =>
            {
                entity.ToTable("NonProdGrpDesc$");
                entity.HasKey(e => e.GrpCode);
                entity.Property(e => e.GrpCode).HasColumnName("GrpCode").HasMaxLength(10);
                entity.Property(e => e.GrpDescription).HasColumnName("GrpDescription").HasMaxLength(50);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            throw new InvalidOperationException("SysproDbContext is read-only. Cannot save changes.");
        }
    }
}