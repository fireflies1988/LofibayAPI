using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PhotoTag
    {
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
