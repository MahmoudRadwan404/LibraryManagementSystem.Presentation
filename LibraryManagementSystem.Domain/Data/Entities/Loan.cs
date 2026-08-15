using LibraryManagementSystem.Domain.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Data.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;

        public Guid MemberId { get; set; }
        public Member? Member { get; set; } = null!;

        public Guid ?ProcessedByUserId { get; set; }
        public SystemUser ?ProcessedByUser { get; set; } = null!;

        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal? Fine { get; set; }
        public LoanStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
