using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoTag
    {
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        public string? TagName { get; set; }
        public Tag? Tag { get; set; }
    }
}
