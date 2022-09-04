using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Collections
{
    public class CreateCollectionRequest
    {
        [Required]
        public string? CollectionName { get; set; }
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
        public int? PhotoId { get; set; }
    }
}
