using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoTag
    {
        [JsonIgnore]
        public int PhotoId { get; set; }
        [JsonIgnore]
        public Photo? Photo { get; set; }

        public string? TagName { get; set; }
        [JsonIgnore]
        public Tag? Tag { get; set; }
    }
}
