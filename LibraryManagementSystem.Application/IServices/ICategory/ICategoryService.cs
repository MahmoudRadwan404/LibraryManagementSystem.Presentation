using LibraryManagementSystem.Application.DTOs.Category;
using LibraryManagementSystem.Application.IServices.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.ICategory
{
    public interface ICategoryService : IGenericService<CategoryDto, CreateCategoryDto, UpdateCategoryDto> { }
}
