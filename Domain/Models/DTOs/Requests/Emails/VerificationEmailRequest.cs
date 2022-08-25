using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Emails
{
    public class VerificationEmailRequest
    {
        [Required]
        public string? To { get; set; }
        [Required]
        public string? VerificationCode { get; set; }
    }
}
