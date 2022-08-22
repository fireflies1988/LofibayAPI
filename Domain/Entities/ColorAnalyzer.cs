using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ColorAnalyzer
    {
        public int ColorAnalyzerId { get; set; }
        public string? ColorAnalyzerName { get; set; }

        [JsonIgnore]
        public ICollection<PhotoColor>? PhotoColors { get; set; }
    }
}
