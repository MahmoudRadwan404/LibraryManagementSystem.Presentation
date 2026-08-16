using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
            public Guid? ParentCategoryId { get; set; }      
            public Category? ParentCategory { get; set; }

            public ICollection<Category> ?SubCategories { get; set; } = new List<Category>();  // reverse nav, no extra column
        
        public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
