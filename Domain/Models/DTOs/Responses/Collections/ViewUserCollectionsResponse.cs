using Domain.Models.DTOs.Responses.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Collections
{
    public class ViewUserCollectionsResponse
    {
        public int CollectionId { get; set; }
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsFeatured { get; set; } = false;
        public int Views { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public BasicUserInfoResponse? User { get; set; }
    }
}
