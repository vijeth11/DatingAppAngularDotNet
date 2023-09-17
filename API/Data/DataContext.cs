using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    // Install EntityFrameworkCore,EntityFrameworkCoreDesign,EntityFrameworkCoreSqlServer from nuget packages
    // Create a class that extends DbContext and add it in program file
    public class DataContext: DbContext
    {

        public DataContext(DbContextOptions options):base(options) { }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
                   .HaveConversion<DateOnlyConverter>()
                   .HaveColumnType("date");
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });
            modelBuilder.Entity<UserLike>()
                .HasOne(k => k.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
               .HasOne(k => k.TargetUser)
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(s => s.TargetUserId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(l => l.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(l => l.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }

    /// <summary>
    /// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
        { }
    }
}
