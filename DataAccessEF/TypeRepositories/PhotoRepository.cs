using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccessEF.TypeRepositories
{
    class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
