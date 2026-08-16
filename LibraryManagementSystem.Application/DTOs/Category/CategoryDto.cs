using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Category
{
    public class CategoryDto:CreateCategoryDto
    {
        public Guid Id { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new();
    }
}
