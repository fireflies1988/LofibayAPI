using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTOs.Responses.Admin
{
    public class StatsResponse
    {
        public int NumberOfPhotos { get; set; }
        public int NumberOfUnfeaturedPhotos { get; set; }
        public int NumberOfFeaturedPhotos { get; set; }
        public int NumberOfRejectedPhotos { get; set; }
        public int NumberOfDeletedPhotos { get; set; }

        public int NumberOfUsers { get; set; }
        public int NumberOfUnverifiedUsers { get; set; }
        public int NumberOfVerifiedUsers { get; set; }

        public int NumberOfCollections { get; set; }
        public int NumberOfPrivateCollections { get; set; }
        public int NumberOfPublicCollections { get; set; }
    }
}
