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
        public DbSet<User> User { get; set; }
        public DbSet<Polyclinic> Polyclinic { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        //public DbSet<Patient> Patient { get; set; }
        //public DbSet<Appointment> Appointment { get; set; }
        #endregion

        #region
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //veri tabanında enum değerleri Rol'ler için string olarak sakladık
            modelBuilder.Entity<User>()
                 .Property(u => u.UserRole)
                 .HasConversion<string>(); // Enum'ı string olarak kaydet


            //Doctor sınıfındaki UserId,PolyclinicId propertysine FK olarak belirttik
            //modelBuilder.Entity<Doctor>()
            //     .HasOne(d => d.User)
            //     .WithOne(u => u.Doctor)
            //     //.HasForeignKey(d => d.UserId)
            //     .HasForeignKey(d => d.PolyclinicId); 

            //Doctor sınıfındaki PolyclinicId propertysine FK olarak belirttik
            //modelBuilder.Entity<Doctor>()
            //    .HasOne(d => d.User)
            //    .WithMany(u => u.Doctor)
            //    .HasForeignKey(d => d.PolyclinicId);

           
            //modelBuilder.Entity<Patient>()
            //    .HasOne
           
        }

        #endregion

    }
}
