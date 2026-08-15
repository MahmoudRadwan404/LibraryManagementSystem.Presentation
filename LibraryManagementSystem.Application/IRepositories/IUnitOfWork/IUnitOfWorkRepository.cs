using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.IUnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        void ClearTracking();

    }
}
