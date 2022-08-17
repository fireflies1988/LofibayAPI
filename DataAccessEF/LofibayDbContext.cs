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

                b.HasOne<UserGender>(u => u.Gender)
                    .WithMany(ug => ug.Users)
                    .HasForeignKey(u => u.GenderId);
            });

            modelBuilder.Entity<Photo>(b =>
            {
                b.Property(p => p.PhotoUrl)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<LikedPhoto>(b =>
            {
                b.HasKey(lp => new { lp.UserId, lp.PhotoId });

                b.HasOne<User>(lp => lp.User)
                    .WithMany(u => u.LikedPhotos)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<Photo>(lp => lp.Photo)
                    .WithMany(p => p.LikedPhotos)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PhotoTag>().HasKey(pt => new { pt.PhotoId, pt.TagId });

            modelBuilder.Entity<PhotoCollection>(b =>
            {
                b.HasKey(pc => new { pc.PhotoId, pc.CollectionId });

                b.HasOne<Photo>(pc => pc.Photo)
                    .WithMany(p => p.PhotoCollections)
                    .OnDelete(DeleteBehavior.NoAction);
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

            modelBuilder.Entity<Role>()
               .HasData(
                    new Role { RoleId = Domain.Enums.Roles.Admin, RoleName = "Admin" },
                    new Role { RoleId = Domain.Enums.Roles.User, RoleName = "User" }
                );

            modelBuilder.Entity<UserGender>(b =>
            {
                b.HasKey(ug => ug.GenderId);

                b.HasData(
                    new UserGender { GenderId = Genders.Unknown, Gender = "Unknown" },
                    new UserGender { GenderId = Genders.Male, Gender = "Male" },
                    new UserGender { GenderId = Genders.Female, Gender = "Female" }
                    );
            });

            modelBuilder.Entity<Tag>(b =>
            {
                b.Property(t => t.TagName).IsRequired();
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
        public DbSet<LikedPhoto>? LikedPhotos { get; set; }
        public DbSet<PhotoTag>? PhotoTags { get; set; }
        public DbSet<PhotoCollection>? PhotoCollections { get; set; }
        public DbSet<UserFollower>? UserFollowers { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }
    }
}
