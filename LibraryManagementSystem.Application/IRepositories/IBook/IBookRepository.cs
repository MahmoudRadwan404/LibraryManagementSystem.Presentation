using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.IBook
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        public  Task<Book?> GetBookAsyncWithAuthorsAndCategories(Guid bookid);
        Task<Book?> GetForBorrowAsync(Guid bookId);
        Task<int> CountActiveLoansAsync(Guid bookId);
        Task<(IEnumerable<Book> Items, int TotalCount)> SearchAsync(
            string? search, Guid? categoryId, int page, int pageSize);
    }
}
