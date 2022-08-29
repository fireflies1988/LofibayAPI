using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }
    }
}
