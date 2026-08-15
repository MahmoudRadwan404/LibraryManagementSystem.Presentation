using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.IServices.Ibook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<PagedResultDto<BookDto>>> Search(
        [FromQuery] string? search, [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _bookService.SearchAsync(search, categoryId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
    {
        var created = await _bookService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        await _bookService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }
}