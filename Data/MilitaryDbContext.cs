using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App
{
    public class MilitaryDbContext : DbContext
    {
        public MilitaryDbContext(DbContextOptions<MilitaryDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<ServiceOfUnit> ServiceOfUnits { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Soldier> Soldiers { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Authority>()
                .HasOne(a => a.User)
                .WithMany(u => u.Authorities)
                .HasForeignKey(a => a.Username);

            modelBuilder.Entity<ServiceOfUnit>()
                .HasOne(s => s.Unit)
                .WithMany(u => u.ServicesOfUnit)
                .HasForeignKey(s => s.UnitId);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Soldier)
                .WithOne(soldier => soldier.Service)
                .HasForeignKey<Service>(s => s.SoldierId);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Unit)
                .WithMany()
                .HasForeignKey(s => s.UnitId);

            modelBuilder.Entity<Soldier>()
                .HasOne(s => s.Unit)
                .WithMany()
                .HasForeignKey(s => s.UnitId);

            modelBuilder.Entity<ServiceOfUnit>().ToTable("ser_of_unit", "ms");
            modelBuilder.Entity<Unit>().ToTable("unit", "ms");
            modelBuilder.Entity<Service>().ToTable("service", "ms");
            modelBuilder.Entity<Soldier>().ToTable("soldier", "ms");
            modelBuilder.Entity<User>().ToTable("users", "ms");
            modelBuilder.Entity<Authority>().ToTable("authorities", "ms");
        }
    }
}
