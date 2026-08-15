using LibraryManagementSystem.Application.DTOs.SystemUser;
using LibraryManagementSystem.Application.IServices.ISystemUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/systemusers")]
[Authorize(Roles = "Administrator")] // applies to every action in this controller
public class SystemUsersController : ControllerBase
{
    private readonly ISystemUserService _systemUserService;

    public SystemUsersController(ISystemUserService systemUserService) => _systemUserService = systemUserService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemUserDto>>> GetAll() =>
        Ok(await _systemUserService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<SystemUserDto>> GetById(Guid id)
    {
        var user = await _systemUserService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<SystemUserDto>> Create([FromBody] CreateSystemUserDto dto)
    {
        var created = await _systemUserService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSystemUserDto dto)
    {
        await _systemUserService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _systemUserService.DeleteAsync(id);
        return NoContent();
    }
}