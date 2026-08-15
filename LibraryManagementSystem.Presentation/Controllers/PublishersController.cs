using LibraryManagementSystem.Application.DTOs.Publisher;
using LibraryManagementSystem.Application.IServices.IPublisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/publishers")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublishersController(IPublisherService publisherService) => _publisherService = publisherService;

    [HttpGet]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAll() =>
        Ok(await _publisherService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<PublisherDto>> GetById(Guid id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        return publisher is null ? NotFound() : Ok(publisher);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<PublisherDto>> Create([FromBody] CreatePublisherDto dto)
    {
        var created = await _publisherService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePublisherDto dto)
    {
        await _publisherService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _publisherService.DeleteAsync(id);
        return NoContent();
    }
}