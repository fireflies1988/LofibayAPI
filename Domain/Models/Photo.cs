﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
        public bool IsFeatured { get; set; } = false;
        public long Views { get; set; }
        public long Downloads { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? DeletedDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public IList<LikedPhoto>? LikedPhotos { get; set; }

        public IList<PhotoTag>? PhotoTags { get; set; }

        public IList<PhotoCollection>? PhotoCollections { get; set; }
    }
}