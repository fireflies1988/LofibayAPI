using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEF.TypeRepositories
{
    class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(LofibayDbContext context) : base(context)
        {
        }

        public async Task<Photo?> GetPhotoDetailsByIdAsync(int id)
        {
            return await Context.Photos!
                .Include(p => p.User)!
                .Include(p => p.LikedPhotos)!
                .Include(p => p.PhotoColors)!
                    .ThenInclude(pc => pc.ColorAnalyzer)
                .Include(p => p.PhotoTags)
                .FirstOrDefaultAsync(p => p.PhotoId == id);
        }

        public async Task<Photo?> GetPhotoIncludingColorAnalyzerByIdAsync(int id)
        {
            return await Context.Photos!
                .Include(p => p.PhotoColors)!
                    .ThenInclude(pc => pc.ColorAnalyzer)
                .FirstOrDefaultAsync(p => p.PhotoId == id);
        }
    }
}
