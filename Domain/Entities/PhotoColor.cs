using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoColor
    {
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        public string? ColorName { get; set; }
        public Color? Color { get; set; }

        public int ColorAnalyzerId { get; set; }
        public ColorAnalyzer? ColorAnalyzer { get; set; }

        public float PredominantPercent { get; set; }
    }
}
