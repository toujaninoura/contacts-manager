using ContactsManager.Application.DTOs.Contacts;
using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.API.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactService contactService, ILogger<ContactController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET /api/contacts page={Page} pageSize={PageSize}", page, pageSize);

        var response = await _contactService.GetAllAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET /api/contacts/{Id}", id);

        try
        {
            var response = await _contactService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Contact id={Id} not found: {Message}", id, ex.Message);
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
    {
        _logger.LogInformation("POST /api/contacts email={Email}", dto.Email);

        var response = await _contactService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContactDto dto)
    {
        _logger.LogInformation("PUT /api/contacts/{Id}", id);

        try
        {
            var response = await _contactService.UpdateAsync(id, dto);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Contact id={Id} not found for update: {Message}", id, ex.Message);
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE /api/contacts/{Id}", id);

        try
        {
            await _contactService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Contact id={Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
