using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IRepositories.ILoan
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetOverdueLoansAsync(); // for the cron job
        Task<Loan?> GetActiveLoanForBookAsync(Guid bookId, Guid memberId);
    }
}
