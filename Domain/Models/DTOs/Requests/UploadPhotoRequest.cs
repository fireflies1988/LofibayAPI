using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTOs.Requests
{
    public class UploadPhotoRequest
    {
        [Required]
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
    }
}
