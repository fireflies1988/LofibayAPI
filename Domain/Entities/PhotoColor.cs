using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoColor
    {
        [JsonIgnore]
        public int PhotoId { get; set; }
        [JsonIgnore]
        public Photo? Photo { get; set; }

        public string? ColorName { get; set; }
        [JsonIgnore]
        public Color? Color { get; set; }

        [JsonIgnore]
        public int ColorAnalyzerId { get; set; }
        public ColorAnalyzer? ColorAnalyzer { get; set; }

        public float PredominantPercent { get; set; }
    }
}
