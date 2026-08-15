using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWorkRepository(ApplicationDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void ClearTracking() => _context.ChangeTracker.Clear();
    }
}
