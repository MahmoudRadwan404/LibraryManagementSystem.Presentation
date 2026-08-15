using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Book
{
    public class BorrowRequestDto
    {
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
    }
}
