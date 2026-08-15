using LibraryManagementSystem.Application.DTOs.Author;
using LibraryManagementSystem.Application.IRepositories.IAuthor;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.IAuthor;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Infrastructure.Services;

public class AuthorService : GenericService<Author, AuthorDto, CreateAuthorDto, UpdateAuthorDto>, IAuthorService
{
    public AuthorService(IAuthorRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork) { }

    protected override AuthorDto MapToDto(Author a) => new() { Id = a.Id, Name = a.Name, Bio = a.Bio };

    protected override Author MapToEntity(CreateAuthorDto dto) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Bio = dto.Bio,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    protected override void ApplyUpdate(Author a, UpdateAuthorDto dto)
    {
        a.Name = dto.Name;
        a.Bio = dto.Bio;
        a.UpdatedAt = DateTime.UtcNow;
    }

    protected override void MarkDeleted(Author a) => a.IsDeleted = true;
}