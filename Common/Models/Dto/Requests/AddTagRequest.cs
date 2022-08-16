using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dto.Requests
{
    public class AddTagRequest
    {
        [Required]
        public string? TagName { get; set; }
    }
}
