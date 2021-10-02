using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbital.Model;
using Orbital.Shared.Static;

namespace Orbital.Tests.Services
{
    public class DbTestService
    {
        public DbTestService(DbContextOptions<OrbitalContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<OrbitalContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new OrbitalContext(ContextOptions))
            {
                DatabaseSeeding.InitializeDbForTests(context);
            }
        }
    }
}
