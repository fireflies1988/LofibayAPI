using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Collections
{
    public class CreateCollectionResponse
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public int Views { get; set; }

        public int UserId { get; set; }

        public IList<PhotoCollection>? PhotoCollections { get; set; }
    }
}
