using LibraryManagementSystem.Application.Actions;
using LibraryManagementSystem.Application.DTOs.Loan;
using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.IAuditLog;
using LibraryManagementSystem.Application.IRepositories.IBook;
using LibraryManagementSystem.Application.IRepositories.ILoan;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.ILoan;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Domain.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Services
{
    public class LoanService : ILoanService
    {
        private readonly IBookRepository _bookRepo;
        private readonly ILoanRepository _loanRepo;
        private readonly IAuditLogRepository _auditLogRepo;
        private readonly IUnitOfWork _unitOfWork;
        private const int MaxRetries = 3;

        public LoanService(IBookRepository bookRepo, ILoanRepository loanRepo,
                            IAuditLogRepository auditLogRepo, IUnitOfWork unitOfWork)
        {
            _bookRepo = bookRepo;
            _loanRepo = loanRepo;
            _auditLogRepo = auditLogRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoanDto> BorrowBookAsync(Guid bookId, Guid memberId, Guid staffId)
        {
            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                var book = await _bookRepo.GetForBorrowAsync(bookId)
                    ?? throw new NotFoundException(ErrorMessages.BookNotFound);

                if (book.Quantity <= 0)
                    throw new ConflictException(ErrorMessages.NoCopiesAvailable);

                book.Quantity -= 1;                 // ← the write RowVersion actually protects
                book.UpdatedAt = DateTime.UtcNow;
                _bookRepo.Update(book);           

                var loan = new Loan
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    MemberId = memberId,
                    ProcessedByUserId = staffId,
                    BorrowedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(14),
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _loanRepo.AddAsync(loan);

                await _auditLogRepo.LogAsync(staffId, AuditActions.ProcessLoan, nameof(Loan),
                    loan.Id.ToString(), null, new { loan.BookId, loan.MemberId, loan.DueDate });

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                    return MapToDto(loan, book.Title);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Someone else's UPDATE hit Book.RowVersion first — 0 rows affected on ours.
                    // Detach the failed attempt's tracked changes and retry from a fresh read.
                    _unitOfWork.ClearTracking();
                    if (attempt == MaxRetries - 1)
                        throw new ConflictException(ErrorMessages.BorrowConflictRetryExceeded);
                }
            }

            throw new ConflictException(ErrorMessages.BorrowConflictRetryExceeded);
        }


        public async Task<LoanDto> ReturnBookAsync(Guid loanId, Guid staffId)
        {
            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                var loan = await _loanRepo.GetByIdAsync(loanId)
                    ?? throw new NotFoundException(ErrorMessages.LoanNotFound);

                if (loan.Status != LoanStatus.Active)
                    throw new ConflictException(ErrorMessages.LoanAlreadyReturned);

                var book = await _bookRepo.GetForBorrowAsync(loan.BookId)
                    ?? throw new NotFoundException(ErrorMessages.BookNotFound);

                book.Quantity += 1;                 // ← the write RowVersion protects here too
                book.UpdatedAt = DateTime.UtcNow;
                _bookRepo.Update(book);            

                var oldStatus = loan.Status;
                loan.ReturnedAt = DateTime.UtcNow;
                loan.Status = LoanStatus.Returned;
                loan.UpdatedAt = DateTime.UtcNow;
                if (loan.ReturnedAt > loan.DueDate)
                    loan.Fine = CalculateFine(loan.DueDate, loan.ReturnedAt.Value);

                _loanRepo.Update(loan);

                await _auditLogRepo.LogAsync(staffId, AuditActions.ProcessReturn, nameof(Loan),
                    loan.Id.ToString(), new { Status = oldStatus }, new { loan.Status, loan.Fine });

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                    return MapToDto(loan, book.Title);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _unitOfWork.ClearTracking();
                    if (attempt == MaxRetries - 1)
                        throw new ConflictException(ErrorMessages.ReturnConflictRetryExceeded);
                }
            }

            throw new ConflictException(ErrorMessages.ReturnConflictRetryExceeded);
        }
        public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        {
            var loans = await _loanRepo.GetActiveLoansAsync();
            return loans.Select(l => MapToDto(l, l.Book?.Title ?? string.Empty));
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _loanRepo.GetOverdueLoansAsync();
            return loans.Select(l => MapToDto(l, l.Book?.Title ?? string.Empty));
        }

        private static decimal CalculateFine(DateTime dueDate, DateTime returnedAt)
        {
            var daysLate = (returnedAt - dueDate).Days;
            return daysLate > 0 ? daysLate * 0.5m : 0m; // $0.50/day — adjust as needed
        }

        private static LoanDto MapToDto(Loan l, string bookTitle) => new()
        {
            Id = l.Id,
            BookId = l.BookId,
           // BookTitle = bookTitle,
            MemberId = l.MemberId,
            ProcessedByUserId = l.ProcessedByUserId.Value,
            BorrowedAt = l.BorrowedAt,
            DueDate = l.DueDate,
            ReturnedAt = l.ReturnedAt,
            Fine = l.Fine,
            Status = l.Status.ToString()
        };
    }
}
