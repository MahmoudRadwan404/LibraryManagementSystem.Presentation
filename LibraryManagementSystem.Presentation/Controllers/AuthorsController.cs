using LibraryManagementSystem.Application.DTOs.Author;
using LibraryManagementSystem.Application.IServices.IAuthor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService) => _authorService = authorService;

    [HttpGet]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll() =>
        Ok(await _authorService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id)
    {
        var author = await _authorService.GetByIdAsync(id);
        return author is null ? NotFound() : Ok(author);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
    {
        var created = await _authorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuthorDto dto)
    {
        await _authorService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _authorService.DeleteAsync(id);
        return NoContent();
    }
}