using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTOs.Requests.Users
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Username { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Biography { get; set; }

        public UpdateUserAddressRequest? Address { get; set; }

        public int? GenderId { get; set; }
    }
}
