using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.IGeneric
{
    public interface IGenericService<TDto, TCreateDto, TUpdateDto>
    {
        Task<TDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> CreateAsync(TCreateDto dto);
        Task UpdateAsync(Guid id, TUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
