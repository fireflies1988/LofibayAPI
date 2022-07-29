using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User
    {
        public int UserId { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public UserAddress? Address { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public ICollection<Photo>? Photos { get; set; }

        public IList<LikedPhoto>? LikedPhotos { get; set; }

        public IList<UserFollower>? Followers { get; set; }
        public IList<UserFollower>? Following { get; set; }
    }
}
