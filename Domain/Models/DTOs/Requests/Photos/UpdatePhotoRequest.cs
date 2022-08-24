using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Requests.Photos
{
    public class UpdatePhotoRequest
    {
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
        public IList<string>? Tags { get; set; }
        [JsonIgnore]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
