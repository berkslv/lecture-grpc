using Microsoft.AspNetCore.Mvc;
using BookService.Models;

namespace BookService.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private static readonly List<Book> Books = Book.GenerateBooks();

    [HttpGet]
    public IActionResult GetBooks()
    {
        return Ok(Books);
    }

    [HttpGet("{id}")]
    public IActionResult GetBook(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        
        return Ok(book);
    }

    [HttpPost]
    public IActionResult CreateBook([FromBody] Book book)
    {
        book.Id = Books.Max(b => b.Id) + 1;
        Books.Add(book);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }
}
