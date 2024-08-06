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

            modelBuilder.Entity<User>(entity =>
            {

                entity.HasIndex(c => c.Email).IsUnique();

                entity.HasOne<Volunteer>()
                  .WithOne(v => v.User)
                  .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany<Accusation>()
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Volunteer>(entity =>
            {

            });

            modelBuilder.Entity<Accusation>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.NoAction); 

                entity.HasOne(a => a.Volunteer)
                      .WithMany()
                      .HasForeignKey(a => a.VolunteerId)
                      .OnDelete(DeleteBehavior.NoAction); 
            });
        }
    }
}
