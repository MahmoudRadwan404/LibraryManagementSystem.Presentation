using LibraryManagementSystem.Application.DTOs.Category;
using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.ICategory;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.ICategory;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Infrastructure.Services;
public class CategoryService : GenericService<Category, CategoryDto, CreateCategoryDto, UpdateCategoryDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork)
    {
        _categoryRepo = repo;
    }

    protected override CategoryDto MapToDto(Category c) => new() { Id = c.Id, Name = c.Name };

    protected override Category MapToEntity(CreateCategoryDto dto) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name
    };

    protected override void ApplyUpdate(Category c, UpdateCategoryDto dto) => c.Name = dto.Name;

    protected override void MarkDeleted(Category c) =>
        throw new InvalidOperationException("Category uses hard delete — see DeleteAsync override.");

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await _categoryRepo.GetByIdAsync(id)
            ?? throw new NotFoundException(ErrorMessages.CategoryNotFound);

        _categoryRepo.Delete(entity); // hard delete — Category has no IsDeleted column
        await _unitOfWork.SaveChangesAsync();
    }
}