using Domain_one.HospitalDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_two.HospitalDatabase.Contexts.EntityFramework
{
    public class HospitalDatabaseContext:DbContext 
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = .; Database = HospitalManagement; User Id = sa; Password = 1; TrustServerCertificate=True;");

            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);

        }


        #region Tables
     
        public DbSet<Category> Category { get; set; }

        #endregion

    }
}
