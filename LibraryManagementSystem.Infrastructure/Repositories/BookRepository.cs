using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IBook;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Domain.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Book?> GetBookAsyncWithAuthorsAndCategories(Guid bookid)
        {
            var book = await _context.Books.Where(e=>e.Id==bookid).Include(a=>a.Authors).Include(c=>c.Categories).FirstOrDefaultAsync();
            return book;
        }
        public async Task<Book?> GetForBorrowAsync(Guid bookId) =>
            await _context.Set<Book>().SingleOrDefaultAsync(b => b.Id == bookId);

        public async Task<int> CountActiveLoansAsync(Guid bookId) =>
            await _context.Loans.CountAsync(l => l.BookId == bookId && l.Status == LoanStatus.Active);

        public async Task<(IEnumerable<Book> Items, int TotalCount)> SearchAsync(
            string? search, Guid? categoryId, int page, int pageSize)
        {
            var query = _context.Set<Book>()
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    (b.Isbn != null && b.Isbn.Contains(search)) ||
                    b.Authors.Any(a => a.Name.Contains(search)));   

            //if (!string.IsNullOrWhiteSpace(search))
            //    query = query.Where(b => b.Title.Contains(search) || (b.Isbn != null && b.Isbn.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(b => b.Categories.Any(c => c.Id == categoryId.Value));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
