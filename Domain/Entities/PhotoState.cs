using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoState
    {
        public int PhotoStateId { get; set; }
        public string? State { get; set; }

        [JsonIgnore]
        public ICollection<Photo>? Photos { get; set; }
    }
}
