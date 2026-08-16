using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.ICategory
{
    public interface ICategoryRepository : IGenericRepository<Category> { 
        
        
        Task<IEnumerable<Category>> GetTreeAsync();
        
        }
    
    
}