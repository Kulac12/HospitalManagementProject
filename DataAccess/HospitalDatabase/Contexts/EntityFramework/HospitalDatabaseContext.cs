
using Domain_one.HospitalDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            public DbSet<Patient> Patient { get; set; }
            public DbSet<Appointment> Appointment { get; set; }
            public DbSet<OperationClaim> OperationClaims { get; set; }
            public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        #endregion

        #region
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.User)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Polyclinic)
                .WithMany(p => p.Doctor)
                .HasForeignKey(d => d.PolyclinicId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);


        }

        #endregion

    }
}
