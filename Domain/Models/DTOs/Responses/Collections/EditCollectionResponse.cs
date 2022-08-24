using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Collections
{
    public class EditCollectionResponse
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public int Views { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int UserId { get; set; }
    }
}
