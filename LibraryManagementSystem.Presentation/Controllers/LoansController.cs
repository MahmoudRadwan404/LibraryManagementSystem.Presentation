using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.DTOs.Loan;
using LibraryManagementSystem.Application.IServices.ILoan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/loans")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService) => _loanService = loanService;

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<LoanDto>> Borrow([FromBody] BorrowRequestDto dto)
    {
        var staffId = GetCurrentUserId();
        var loan = await _loanService.BorrowBookAsync(dto.BookId, dto.MemberId, staffId);
        return Ok(loan);
    }

    [HttpPost("{id}/return")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<LoanDto>> Return(Guid id)
    {
        var staffId = GetCurrentUserId();
        var loan = await _loanService.ReturnBookAsync(id, staffId);
        return Ok(loan);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetActive() =>
        Ok(await _loanService.GetActiveLoansAsync());

    [HttpGet("overdue")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdue() =>
        Ok(await _loanService.GetOverdueLoansAsync());

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}