using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccessEF.TypeRepositories
{
    public class ColorRepository : GenericRepository<Color>, IColorRepository
    {
        public ColorRepository(LofibayDbContext context) : base(context)
        {
        }
    }
}
