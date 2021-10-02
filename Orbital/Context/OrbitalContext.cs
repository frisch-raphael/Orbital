using Microsoft.EntityFrameworkCore;
using Shared.Dtos;

namespace Orbital.Model
{
    public interface IOrbitalContext
    {
        DbSet<Payload> Payloads { get; set; }
    }

    public class OrbitalContext : DbContext, IOrbitalContext
    {
        private string DbPath { get; set; }

        public OrbitalContext(DbContextOptions<OrbitalContext> context) : base(context)

        {
            //var path = hostingEnvironment.ContentRootPath;
            //var projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            //DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}{projectName}.db";
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    options.UseSqlite($"Data Source={DbPath}");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Payload>()
            //    .HasMany(p => p.Functions)
            //    .WithOne()
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.ClientCascade);
        }
        public DbSet<Payload> Payloads { get; set; }
        public DbSet<ScanResult> ScanResults { get; set; }
    }
}
