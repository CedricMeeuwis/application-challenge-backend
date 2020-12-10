using ApplicationChallengeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationChallengeAPI.Data
{
    public class TafeltennisContext : DbContext
    {
        public TafeltennisContext(DbContextOptions<TafeltennisContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Ploeg> Ploegen { get; set; }

        public DbSet<Tafel> Tafels { get; set; }
        public DbSet<Wedstrijd> Wedstrijden { get; set; }
        public DbSet<Competitie> Competities { get; set; }
        public DbSet<Tournooi> Tournooien { get; set; }
        public DbSet<MatchContext> MatchContexten { get; set; }
        public DbSet<Challenge> Challenges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Tafel>().ToTable("Tafel");
            modelBuilder.Entity<Ploeg>().ToTable("Ploeg");
            modelBuilder.Entity<Wedstrijd>().ToTable("Wedstrijd");
            modelBuilder.Entity<Competitie>().ToTable("Competitie");
            modelBuilder.Entity<Tournooi>().ToTable("Tournooi");
            modelBuilder.Entity<MatchContext>().ToTable("MatchContext");

            modelBuilder.Entity<MatchContext>()
                .HasOne(w => w.Wedstrijd)
                .WithOne(m => m.MatchContext)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Wedstrijd>()
                .HasOne(w => w.MatchContext)
                .WithOne(m => m.Wedstrijd)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
