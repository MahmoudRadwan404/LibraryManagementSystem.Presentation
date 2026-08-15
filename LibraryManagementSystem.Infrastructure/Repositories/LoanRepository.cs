using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.ILoan;
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
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        public LoanRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync() =>
            await _context.Set<Loan>().Where(l => l.Status == LoanStatus.Active).ToListAsync();

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync() =>
            await _context.Set<Loan>()
                .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
                .ToListAsync();

        public async Task<Loan?> GetActiveLoanForBookAsync(Guid bookId, Guid memberId) =>
            await _context.Set<Loan>().SingleOrDefaultAsync(l =>
                l.BookId == bookId && l.MemberId == memberId && l.Status == LoanStatus.Active);
    }
}
