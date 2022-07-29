using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPhotoRepository Photos { get; }
        IUserRepository Users { get; }
        ITagRepository Tags { get; }

        Task<int> SaveChangesAsync();
    }
}
