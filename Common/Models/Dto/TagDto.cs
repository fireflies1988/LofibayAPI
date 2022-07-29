using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dto
{
    public class TagDto
    {
        [Required]
        public string? TagName { get; set; }
    }
}
