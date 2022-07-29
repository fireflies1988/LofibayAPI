using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PhotoCollection
    {
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }
    }
}
