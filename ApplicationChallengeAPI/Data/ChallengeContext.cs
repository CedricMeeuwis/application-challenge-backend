using ApplicationChallengeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Data
{
    public class ChallengeContext : DbContext
    {
        public ChallengeContext(DbContextOptions<ChallengeContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Ploeg> Ploegen { get; set; }

        public DbSet<Tafel> Tafels { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Tafel>().ToTable("Tafel");
            modelBuilder.Entity<Ploeg>().ToTable("Ploeg");

            modelBuilder.Entity<Ploeg>()
                .HasOne(p => p.Kapitein)
                .WithMany(u => u.Crews)
                .HasForeignKey(p => p.KapiteinID);
        }
    }
}
