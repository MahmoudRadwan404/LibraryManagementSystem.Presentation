using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Loan
{
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public Guid ProcessedByUserId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal? Fine { get; set; }
        public string Status { get; set; } = null!;
    }

}
