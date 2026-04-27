using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessAppMVC.Models;

namespace FitnessAppMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ── 4 Main Activity Tables ─────────────────────────────
        public DbSet<Workout>        Workouts        { get; set; }
        public DbSet<Meal>           Meals           { get; set; }
        public DbSet<SleepRecord>    SleepLogs       { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity Tables (Clean Names)
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            // Profile columns in Users table
            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property<string>("FullName").HasMaxLength(200);
                entity.Property<string>("Phone").HasMaxLength(20);
                entity.Property<int?>("Age");
                entity.Property<decimal?>("HeightCm").HasColumnType("decimal(5,2)");
                entity.Property<decimal?>("WeightKg").HasColumnType("decimal(5,2)");
                entity.Property<string>("FitnessGoal").HasMaxLength(200);
                entity.Property<string>("ActivityLevel").HasMaxLength(50);
                entity.Property<DateTime>("JoinedDate").HasDefaultValueSql("GETDATE()");
                entity.Property<bool>("IsActive").HasDefaultValue(true);
            });

            builder.Entity<Workout>(entity =>
            {
                entity.Property(e => e.WeightKg).HasColumnType("decimal(8,2)");
            });

            builder.Entity<Meal>(entity =>
            {
                entity.Property(e => e.Protein).HasColumnType("decimal(8,2)");
                entity.Property(e => e.Carbs).HasColumnType("decimal(8,2)");
                entity.Property(e => e.Fat).HasColumnType("decimal(8,2)");
            });

            builder.Entity<SleepRecord>(entity =>
            {
                entity.ToTable("SleepLogs");
                entity.Property(e => e.HoursSlept).HasColumnType("decimal(4,2)");
            });
        }
    }
}
