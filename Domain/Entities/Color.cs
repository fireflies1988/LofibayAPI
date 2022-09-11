using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Color
    {
        public string? Name { get; set; }

        [JsonIgnore]
        public IList<PhotoColor>? PhotoColors { get; set; }
    }
}
