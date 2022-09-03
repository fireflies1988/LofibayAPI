using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEF
{
    public class LofibayDbContext : DbContext
    {
        public LofibayDbContext(DbContextOptions<LofibayDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.Email).IsRequired();

                b.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(u => u.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(u => u.VerificationTokenHash)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(u => u.VerificationTokenSalt)
                    .IsRequired()
                    .HasMaxLength(255);

                b.HasOne<UserGender>(u => u.Gender)
                    .WithMany(ug => ug.Users)
                    .HasForeignKey(u => u.GenderId);
            });

            modelBuilder.Entity<Photo>(b =>
            {
                b.Ignore(p => p.DownloadUrl);

                b.Property(p => p.PhotoUrl)
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property(p => p.PublicId).IsRequired();

                b.Property(p => p.Format).IsRequired();

                b.HasOne(p => p.User)
                    .WithMany(u => u.Photos)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Tag>(b =>
            {
                b.HasKey(t => t.Name);

                b.Property(t => t.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Color>(b =>
            {
                b.HasKey(c => c.Name);

                b.Property(c => c.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<ColorAnalyzer>(b =>
            {
                b.Property(ca => ca.ColorAnalyzerName).IsRequired().HasMaxLength(50);

                b.HasData(
                    new ColorAnalyzer { ColorAnalyzerId = Domain.Enums.ColorAnalyzers.Google, ColorAnalyzerName = "Google" },
                    new ColorAnalyzer { ColorAnalyzerId = Domain.Enums.ColorAnalyzers.Cloudinary, ColorAnalyzerName = "Cloudinary" });
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property(r => r.RoleName).IsRequired().HasMaxLength(50);

                b.HasData(
                    new Role { RoleId = Domain.Enums.Roles.Admin, RoleName = "Admin" },
                    new Role { RoleId = Domain.Enums.Roles.User, RoleName = "User" }
                );
            });

            modelBuilder.Entity<LikedPhoto>(b =>
            {
                b.HasKey(lp => new { lp.UserId, lp.PhotoId });

                b.HasOne<User>(lp => lp.User)
                    .WithMany(u => u.LikedPhotos)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<Photo>(lp => lp.Photo)
                    .WithMany(p => p.LikedPhotos);
            });

            modelBuilder.Entity<PhotoTag>(b =>
            {
                b.HasKey(pt => new { pt.PhotoId, pt.TagName });
            });

            modelBuilder.Entity<PhotoCollection>(b =>
            {
                b.HasKey(pc => new { pc.PhotoId, pc.CollectionId });

                b.HasOne<Photo>(pc => pc.Photo)
                    .WithMany(p => p.PhotoCollections);
            });

            modelBuilder.Entity<UserFollower>(b =>
            {
                b.HasKey(uf => new { uf.UserId, uf.FollowerId });

                b.HasOne<User>(uf => uf.User)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(uf => uf.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<User>(uf => uf.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(uf => uf.FollowerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserGender>(b =>
            {
                b.HasKey(ug => ug.GenderId);

                b.HasData(
                    new UserGender { GenderId = Genders.Unknown, Gender = "Unknown" },
                    new UserGender { GenderId = Genders.Male, Gender = "Male" },
                    new UserGender { GenderId = Genders.Female, Gender = "Female" }
                    );
            });

            modelBuilder.Entity<PhotoColor>(b =>
            {
                b.HasKey(pc => new { pc.PhotoId, pc.ColorName, pc.ColorAnalyzerId });
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.Property(rt => rt.ExpirationDate).HasColumnType("smalldatetime");

                b.Property(rt => rt.TokenHash)
                    .IsRequired()
                    .HasMaxLength(1000);

                b.Property(rt => rt.TokenSalt)
                    .IsRequired()
                    .HasMaxLength(1000);
            });
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<Photo>? Photos { get; set; }
        public DbSet<UserAddress>? UserAddresses { get; set; }
        public DbSet<UserGender>? UserGenders { get; set; }
        public DbSet<Tag>? Tags { get; set; }
        public DbSet<Collection>? Collections { get; set; }
        public DbSet<Color>? Colors { get; set; }
        public DbSet<ColorAnalyzer>? ColorAnalyzers { get; set; }
        public DbSet<LikedPhoto>? LikedPhotos { get; set; }
        public DbSet<PhotoTag>? PhotoTags { get; set; }
        public DbSet<PhotoCollection>? PhotoCollections { get; set; }
        public DbSet<UserFollower>? UserFollowers { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }
        public DbSet<PhotoColor>? PhotoColors { get; set; }
    }
}
