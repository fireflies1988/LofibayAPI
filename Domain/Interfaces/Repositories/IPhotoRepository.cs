using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IPhotoRepository : IGenericRepository<Photo>
    {
        Task<Photo?> GetPhotoIncludingColorAnalyzerByIdAsync(int id);
        Task<Photo?> GetPhotoDetailsByIdAsync(int id);
    }
}
