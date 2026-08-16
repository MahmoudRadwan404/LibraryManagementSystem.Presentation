using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.ICategory;
using LibraryManagementSystem.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<Category>> GetTreeAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var categoriesById = categories.ToDictionary(c => c.Id);

            foreach (var category in categories)
            {
                if (category.ParentCategoryId is Guid parentId &&
                    categoriesById.TryGetValue(parentId, out var parent))
                {
                    parent.SubCategories.Add(category);
                }
            }

            return categories.Where(c => c.ParentCategoryId == null);
        }
    }
}
