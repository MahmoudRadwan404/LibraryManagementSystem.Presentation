using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.IGeneric;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.IGeneric;

public abstract class GenericService<TEntity, TDto, TCreateDto, TUpdateDto>
    : IGenericService<TDto, TCreateDto, TUpdateDto>
    where TEntity : class
{
    protected readonly IGenericRepository<TEntity> _repo;
    protected readonly IUnitOfWork _unitOfWork;

    protected GenericService(IGenericRepository<TEntity> repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    protected abstract TDto MapToDto(TEntity entity);
    protected abstract TEntity MapToEntity(TCreateDto dto);
    protected abstract void ApplyUpdate(TEntity entity, TUpdateDto dto);
    protected abstract void MarkDeleted(TEntity entity);

    public async Task<TDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? default : MapToDto(entity);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repo.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<TDto> CreateAsync(TCreateDto dto)
    {
        var entity = MapToEntity(dto);
        await _repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task UpdateAsync(Guid id, TUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(ErrorMessages.EntityNotFound);
        ApplyUpdate(entity, dto);
        _repo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(ErrorMessages.EntityNotFound);
        MarkDeleted(entity);
        _repo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}