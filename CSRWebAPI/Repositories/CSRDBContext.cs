using CSRWebAPI.Repositories.Identity;
using CSRWebAPI.Repositories.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories
{
    public partial class CSRDBContext : IdentityDbContext<User, Role, long>
    {

        public CSRDBContext(DbContextOptions<CSRDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            base.OnConfiguring(optionBuilder);
        }

        #region ========================================== Database Models ==========================================
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Preference> Preferences { get; set; }
        public virtual DbSet<Shareholder> Shareholders { get; set; }
        public virtual DbSet<ShareholderAddress> ShareholderAddresses { get; set; }
        public virtual DbSet<ShareholderBank> ShareholderBanks { get; set; }
        public virtual DbSet<State> States { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
