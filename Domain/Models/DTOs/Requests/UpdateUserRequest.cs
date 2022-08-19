using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models.DTOs.Requests
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }
        [JsonIgnore]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public UpdateUserAddressRequest? Address { get; set; }

        public int? GenderId { get; set; }
    }
}
