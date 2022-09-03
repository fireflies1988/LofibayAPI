using Domain.Entities;
using Domain.Models.DTOs.Responses.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Collections
{
    public class ViewCollectionResponse
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public int Views { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public BasicUserInfoResponse? User { get; set; }
    }
}
