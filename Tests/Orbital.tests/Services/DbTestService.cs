using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Orbital.Model;
using Orbital.Shared.Static;

namespace Orbital.Tests.Services
{
    public class DbTestService
    {
        public DbTestService()
        {
            ContextOptions = new DbContextOptionsBuilder<OrbitalContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;

            Seed();
        }

        protected DbContextOptions<OrbitalContext> ContextOptions { get; }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        private void Seed()
        {
            using var context = new OrbitalContext(ContextOptions);
            DatabaseSeeding.InitializeDbForTests(context);
        }
    }
}
