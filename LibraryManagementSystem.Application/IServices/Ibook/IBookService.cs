using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.IServices.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.Ibook
{
    public interface IBookService : IGenericService<BookDto, CreateBookDto, UpdateBookDto>
    {
        Task<PagedResultDto<BookDto>> SearchAsync(string? search, Guid? categoryId, int page, int pageSize);
    }
}
