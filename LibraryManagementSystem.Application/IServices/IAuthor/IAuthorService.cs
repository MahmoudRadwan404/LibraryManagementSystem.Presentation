using LibraryManagementSystem.Application.DTOs.Author;
using LibraryManagementSystem.Application.IServices.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.IAuthor
{
    public interface IAuthorService : IGenericService<AuthorDto, CreateAuthorDto, UpdateAuthorDto> { }
}
