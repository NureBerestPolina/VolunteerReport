using EnRoute.Domain.Models;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.Domain.Models;

namespace VolunteerReport.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<ReportCategory> ReportCategories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }
        public DbSet<Accusation> Accusations { get; set; }
        public DbSet<IssuedToken> IssuedTokens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique();
        }
    }
}
