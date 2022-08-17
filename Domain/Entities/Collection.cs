using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Collection
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public bool IsPrivate { get; set; } = false;

        public int UserId { get; set; }
        public User? User { get; set; }

        public IList<PhotoCollection>? PhotoCollections { get; set; }
    }
}
