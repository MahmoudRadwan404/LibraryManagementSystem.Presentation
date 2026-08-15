using LibraryManagementSystem.Application.DTOs.Category;
using LibraryManagementSystem.Application.IServices.ICategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll() =>
        Ok(await _categoryService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Staff,Librarian,Administrator")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var created = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        await _categoryService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteAsync(id); // hard delete, per CategoryService override
        return NoContent();
    }
}