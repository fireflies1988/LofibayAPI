using Domain.Entities;
using Domain.Interfaces.Repositories;
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
    }
}
