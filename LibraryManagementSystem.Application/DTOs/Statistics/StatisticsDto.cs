using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Statistics
{
    public class StatisticsDto
    {
        public int TotalBooks { get; set; }
        public int TotalCopies { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int OverdueCount { get; set; }
        public int ActiveMembersCount { get; set; }
        public List<PopularBookDto> MostBorrowedBooks { get; set; } = new();
    }

    public class PopularBookDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public int BorrowCount { get; set; }
    }
}
