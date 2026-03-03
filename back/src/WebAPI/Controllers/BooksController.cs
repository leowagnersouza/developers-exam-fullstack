using Domain.Entities;
using Domain.Interfaces;
using Domain.Requests;
using Domain.Queries;
using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IService<Book> _service;

    public BooksController(IService<Book> service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista livros com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WebAPI.Models.BookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetBooksQuery query)
    {
        var books = await _service.GetAllAsync();
        // Paginação simples aplicada na memória (ideal implementar paginação no repositório)
        var page = books.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
        var result = page.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Description = b.Description,
            CreatedDate = b.CreatedDate,
            LastUpdatedDate = b.LastUpdatedDate
        });

        return Ok(result);
    }

    /// <summary>
    /// Obtém livro por id.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WebAPI.Models.BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var b = await _service.GetByIdAsync(id);
        var dto = new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Description = b.Description,
            CreatedDate = b.CreatedDate,
            LastUpdatedDate = b.LastUpdatedDate
        };

        return Ok(dto);
    }

    /// <summary>
    /// Cria um novo livro.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebAPI.Models.BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var book = new Book(command.Title, command.Author, command.Description);
        var validation = await _service.InsertAsync(book);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        var dto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            CreatedDate = book.CreatedDate,
            LastUpdatedDate = book.LastUpdatedDate
        };

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, dto);
    }

    /// <summary>
    /// Atualiza um livro.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateBookCommand command)
    {
        if (id != command.Id) return BadRequest("Id mismatch");

        var book = await _service.GetByIdAsync(id);
        book.UpdateDetails(command.Title, command.Author, command.Description);
        var validation = await _service.UpdateAsync(book);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        return NoContent();
    }

    /// <summary>
    /// Remove um livro.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
