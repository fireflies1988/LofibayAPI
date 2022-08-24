using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTOs.Requests.Users
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
