using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Author
{
    public class AuthorDto : CreateAuthorDto
    {
        public Guid Id { get; set; }
       
    }
}
