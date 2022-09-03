using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.TypeRepositories
{
    public class LikedPhotoRepository : GenericRepository<LikedPhoto>, ILikedPhotoRepository
    {
        public LikedPhotoRepository(LofibayDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Photo?>> GetPhotosThatUserLikedAsync(int userId)
        {
            return await Context.LikedPhotos!
                .Include(lp => lp.Photo)
                    .ThenInclude(p => p.User)
                .Include(lp => lp.Photo)
                    .ThenInclude(p => p.LikedPhotos)
                .Where(lp => lp.UserId == userId)
                .Select(lp => lp.Photo)
                .Where(p => !p.DeletedDate.HasValue).ToListAsync();
        }
    }
}
