using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserGender
    {
        public int GenderId { get; set; }
        public string? Gender { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
