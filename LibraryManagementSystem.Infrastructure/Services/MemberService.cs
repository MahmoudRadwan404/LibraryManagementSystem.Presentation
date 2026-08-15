using LibraryManagementSystem.Application.DTOs.Member;
using LibraryManagementSystem.Application.IRepositories.IMember;
using LibraryManagementSystem.Application.IRepositories.IUnitOfWork;
using LibraryManagementSystem.Application.IServices.IMember;
using LibraryManagementSystem.Domain.Data.Entities;
using LibraryManagementSystem.Domain.Data.Enums;
using LibraryManagementSystem.Infrastructure.Services;

public class MemberService : GenericService<Member, MemberDto, CreateMemberDto, UpdateMemberDto>, IMemberService
{
    public MemberService(IMemberRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork) { }

    protected override MemberDto MapToDto(Member m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Email = m.Email,
        Phone = m.Phone,
        MembershipStatus = m.MembershipStatus.ToString()
    };

    protected override Member MapToEntity(CreateMemberDto dto) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Email = dto.Email,
        Phone = dto.Phone,
        MembershipStatus = Enum.Parse<MembershipStatus>(dto.MembershipStatus),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    protected override void ApplyUpdate(Member m, UpdateMemberDto dto)
    {
        m.Name = dto.Name;
        m.Email = dto.Email;
        m.Phone = dto.Phone;
        m.MembershipStatus = Enum.Parse<MembershipStatus>(dto.MembershipStatus);
        m.UpdatedAt = DateTime.UtcNow;
    }

    protected override void MarkDeleted(Member m) => m.IsDeleted = true;
}