using System.Collections.Generic;
using Orbital.Model;
using Orbital.Tests.Static;
using Shared.Dtos;
using Orbital.Static;
using Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Orbital.Shared.Static
{
    public static class DatabaseSeeding
    {
        public static void InitializeDbForTests(OrbitalContext db)
        {
            //db.Database.SetConnectionString("Data Source=rodintest.db");
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            //db.BackendPayloads.RemoveRange(db.BackendPayloads);
            db.BackendPayloads.AddRange(Samples.PayloadSamples);
            db.SaveChanges();
        }



    }
}
