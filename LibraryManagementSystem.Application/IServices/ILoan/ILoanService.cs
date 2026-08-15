using LibraryManagementSystem.Application.DTOs.Loan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.ILoan
{
    public interface ILoanService
    {
        Task<LoanDto> BorrowBookAsync(Guid bookId, Guid memberId, Guid staffId);
        Task<LoanDto> ReturnBookAsync(Guid loanId, Guid staffId);
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
    }
}
