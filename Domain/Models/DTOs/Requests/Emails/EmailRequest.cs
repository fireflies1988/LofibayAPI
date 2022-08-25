using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTOs.Requests.Emails
{
    public class EmailRequest
    {
        [Required]
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
    }
}
