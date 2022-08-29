using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LikedPhoto
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public int PhotoId { get; set; }
        [JsonIgnore]
        public Photo? Photo { get; set; }
    }
}
