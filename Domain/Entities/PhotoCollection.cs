using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PhotoCollection
    {
        [JsonIgnore]
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }

        [JsonIgnore]
        public int CollectionId { get; set; }
        [JsonIgnore]
        public Collection? Collection { get; set; }
    }
}
