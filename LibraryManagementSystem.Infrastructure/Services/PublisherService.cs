using LibraryManagementSystem.Application.DTOs.Publisher;
using LibraryManagementSystem.Application.IRepositories.IPublisher;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.IPublisher;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Infrastructure.Services;

public class PublisherService : GenericService<Publisher, PublisherDto, CreatePublisherDto, UpdatePublisherDto>, IPublisherService
{
    public PublisherService(IPublisherRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork) { }

    protected override PublisherDto MapToDto(Publisher p) => new() { Id = p.Id, Name = p.Name };

    protected override Publisher MapToEntity(CreatePublisherDto dto) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    protected override void ApplyUpdate(Publisher p, UpdatePublisherDto dto)
    {
        p.Name = dto.Name;
        p.UpdatedAt = DateTime.UtcNow;
    }

    protected override void MarkDeleted(Publisher p) => p.IsDeleted = true;
}