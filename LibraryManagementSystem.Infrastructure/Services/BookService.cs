using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.ErrorMessages;
using LibraryManagementSystem.Application.Errors.Exceptions;
using LibraryManagementSystem.Application.IRepositories.IAuthor;
using LibraryManagementSystem.Application.IRepositories.IBook;
using LibraryManagementSystem.Application.IRepositories.ICategory;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.Ibook;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IAuthorRepository _authorRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IBookRepository bookRepo, IAuthorRepository authorRepo,
                            ICategoryRepository categoryRepo, IUnitOfWork unitOfWork)
        {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<BookDto?> GetByIdAsync(Guid id)
        {
            var book = await _bookRepo.GetBookAsyncWithAuthorsAndCategories(id);
            if (book is null) return null;
           
            var activeLoans = await _bookRepo.CountActiveLoansAsync(id);
            return MapToDto(book, activeLoans);
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _bookRepo.GetAllAsync();
            var dtos = new List<BookDto>();
            foreach (var b in books)
                dtos.Add(MapToDto(b, await _bookRepo.CountActiveLoansAsync(b.Id)));
            return dtos;
        }

        public async Task<PagedResultDto<BookDto>> SearchAsync(
            string? search, Guid? categoryId, int page, int pageSize)
        {
            var (items, totalCount) = await _bookRepo.SearchAsync(search, categoryId, page, pageSize);

            var dtos = new List<BookDto>();
            foreach (var b in items)
                dtos.Add(MapToDto(b, await _bookRepo.CountActiveLoansAsync(b.Id)));

            return new PagedResultDto<BookDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
           var Metadata = dto.Metadata.HasValue ? dto.Metadata.Value.GetRawText() : null;
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Isbn = dto.Isbn,
                PublishYear = dto.PublishYear,
                Edition = dto.Edition,
                Language = dto.Language,
                PageCount = dto.PageCount,
                PublisherId = dto.PublisherId,
                Quantity = dto.Quantity,
                Metadata = Metadata,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await AttachAuthorsAndCategoriesAsync(book, dto.AuthorIds, dto.CategoryIds);

            await _bookRepo.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(book, 0);
        }

        public async Task UpdateAsync(Guid id, UpdateBookDto dto)
        {
            var Metadata = dto.Metadata.HasValue ? dto.Metadata.Value.GetRawText() : null;
            var book = await _bookRepo.GetByIdAsync(id)
                ?? throw new NotFoundException(ErrorMessages.BookNotFound);

            book.Title = dto.Title;
            book.Isbn = dto.Isbn;
            book.PublishYear = dto.PublishYear;
            book.Edition = dto.Edition;
            book.Language = dto.Language;
            book.PageCount = dto.PageCount;
            book.PublisherId = dto.PublisherId;
            // NOTE: Quantity intentionally NOT updated here — it's owned by the
            // borrow/return flow to keep the concurrency-safe path the single
            // source of truth for that field.
            book.Metadata = Metadata;
            book.UpdatedAt = DateTime.UtcNow;

            book.Authors.Clear();
            book.Categories.Clear();
            await AttachAuthorsAndCategoriesAsync(book, dto.AuthorIds, dto.CategoryIds);

            _bookRepo.Update(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _bookRepo.GetByIdAsync(id)
                ?? throw new NotFoundException(ErrorMessages.BookNotFound);

            book.IsDeleted = true;
            _bookRepo.Update(book);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task AttachAuthorsAndCategoriesAsync(Book book, List<Guid> authorIds, List<Guid> categoryIds)
        {
            foreach (var authorId in authorIds)
            {
                var author = await _authorRepo.GetByIdAsync(authorId)
                    ?? throw new NotFoundException(ErrorMessages.AuthorNotFound);
                book.Authors.Add(author);
            }

            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepo.GetByIdAsync(categoryId)
                    ?? throw new NotFoundException(ErrorMessages.CategoryNotFound);
                book.Categories.Add(category);
            }
        }

        private static BookDto MapToDto(Book b, int activeLoans) => new()
        {

            Id = b.Id,
            Title = b.Title,
            Isbn = b.Isbn,
            PublishYear = b.PublishYear,
            Edition = b.Edition,
            Language = b.Language,
            PageCount = b.PageCount,
            PublisherId = b.PublisherId,
            Quantity = b.Quantity,
            Metadata =string.IsNullOrEmpty(b.Metadata)
        ? null
        : JsonDocument.Parse(b.Metadata).RootElement,
            AuthorNames = b.Authors.Select(a => a.Name).ToList(),
            CategoryNames = b.Categories.Select(c => c.Name).ToList()
        };
    }
}
