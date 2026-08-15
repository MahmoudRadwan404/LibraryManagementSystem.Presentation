using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.DTOs.Author
{
    public class CreateAuthorDto
    {
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
    }

}
