using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public string? VerificationTokenHash { get; set; }
        public string? VerificationTokenSalt { get; set; }
        public DateTime VerificationTokenExpDate { get; set; }
        public string? ResetTokenHash { get; set; }
        public string? ResetTokenSalt { get; set; }
        public DateTime ResetTokenExpDate { get; set; }
        public bool Verified { get; set; } = false;
        public string? AvatarUrl { get; set; }
        public string? AvatarUrlPublicId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public DateTime? DeletedDate { get; set; }

        public UserAddress? Address { get; set; }

        public int GenderId { get; set; } = Genders.Unknown;
        public UserGender? Gender { get; set; }

        public int RoleId { get; set; } = Roles.User;
        public Role? Role { get; set; }

        public ICollection<Photo>? Photos { get; set; }

        public IList<LikedPhoto>? LikedPhotos { get; set; }

        public ICollection<Collection>? Collections { get; set; }

        public IList<UserFollower>? Followers { get; set; }
        public IList<UserFollower>? Following { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }

        public ICollection<Notification>? Notifications { get; set; }
    }
}
