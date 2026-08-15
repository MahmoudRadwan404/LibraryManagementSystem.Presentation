using LibraryManagementSystem.Application.DTOs.Member;
using LibraryManagementSystem.Application.IServices.IMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService) => _memberService = memberService;

    [HttpGet]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll() =>
        Ok(await _memberService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<MemberDto>> GetById(Guid id)
    {
        var member = await _memberService.GetByIdAsync(id);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
    {
        var created = await _memberService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberDto dto)
    {
        await _memberService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _memberService.DeleteAsync(id);
        return NoContent();
    }
}