﻿using ApplicationChallengeAPI.Models;
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
        public DbSet<Wedstrijd> Wedstrijden { get; set; }
        public DbSet<Competitie> Competities { get; set; }
        public DbSet<Tournooi> Tournooien { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Tafel>().ToTable("Tafel");
            modelBuilder.Entity<Ploeg>().ToTable("Ploeg");
            modelBuilder.Entity<Wedstrijd>().ToTable("Wedstrijd");
            modelBuilder.Entity<Competitie>().ToTable("Competitie");
            modelBuilder.Entity<Tournooi>().ToTable("Tournooi");
        }
    }
}
