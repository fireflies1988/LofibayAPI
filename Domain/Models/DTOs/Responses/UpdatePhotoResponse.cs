using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses
{
    public class UpdatePhotoResponse
    {
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
        public IList<PhotoTag>? PhotoTags { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
