using Microsoft.EntityFrameworkCore;
using Orbital.Pocos;
using Shared.Dtos;
using Shared.Pocos;

namespace Orbital.Model
{

    public class OrbitalContext : DbContext
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
            // modelBuilder.Entity<BackendPayload>()
            //     .HasMany(p => p.Functions)
            //     .WithOne()
            //     .OnDelete(DeleteBehavior.ClientCascade);             
            // modelBuilder.Entity<Function>()
            //     .HasOne(f => f.BackendPayload)
            //     .WithMany(p => p.Functions)
            //     .OnDelete(DeleteBehavior.ClientCascade);  
        }

        public DbSet<BackendPayload> BackendPayloads { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Scan> ScanResults { get; set; }
        public DbSet<FunctionsDissection> DissectResults { get; set; }
    }
}
