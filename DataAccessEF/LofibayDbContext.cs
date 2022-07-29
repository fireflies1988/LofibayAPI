using Domain.Models;
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
                b.HasIndex(u => u.Email).IsUnique();
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
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "User" }
                );
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<Photo>? Photos { get; set; }
        public DbSet<UserAddress>? UserAddresses { get; set; }
        public DbSet<Tag>? Tags { get; set; }
        public DbSet<Collection>? Collections { get; set; }
        public DbSet<LikedPhoto>? LikedPhotos { get; set; }
        public DbSet<PhotoTag>? PhotoTags { get; set; }
        public DbSet<PhotoCollection>? PhotoCollections { get; set; }
        public DbSet<UserFollower>? UserFollowers { get; set; }
    }
}
