using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Users
{
    public class ForgotPasswordRequest
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
    }
}
