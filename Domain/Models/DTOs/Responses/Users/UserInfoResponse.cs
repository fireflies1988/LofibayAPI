using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Users
{
    public class UserInfoResponse
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public UserAddress? Address { get; set; }

        public UserGender? Gender { get; set; }

        public Role? Role { get; set; }
    }
}
