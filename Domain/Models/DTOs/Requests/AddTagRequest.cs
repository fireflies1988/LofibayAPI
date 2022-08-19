using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests
{
    public class AddTagRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;
    }
}
