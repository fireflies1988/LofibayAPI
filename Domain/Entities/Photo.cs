﻿namespace Domain.Entities
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string? PublicId { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TakenAt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public string? Format { get; set; }
        public string? Camera { get; set; }
        public string? Software { get; set; }
        public int FacesDetected { get; set; }
        public string? Phash { get; set; }
        public bool SemiTransparent { get; set; } = false;
        public bool Grayscale { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public bool HasSensitiveContent { get; set; } = false;
        public long Views { get; set; }
        public long Downloads { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public DateTime? DeletedDate { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public IList<LikedPhoto>? LikedPhotos { get; set; }

        public IList<PhotoTag>? PhotoTags { get; set; }

        public IList<PhotoCollection>? PhotoCollections { get; set; }

        public IList<PhotoColor>? PhotoColors { get; set; }
    }
}
