using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }

        [JsonIgnore]
        public IList<PhotoTag>? PhotoTags { get; set; }
    }
}
