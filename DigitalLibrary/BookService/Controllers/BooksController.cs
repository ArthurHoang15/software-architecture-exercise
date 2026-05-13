// BookService/Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BookService.Models;

namespace BookService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly SqlConnection _db;

    public BooksController(SqlConnection db) => _db = db;

    /// <summary>Lấy danh sách tất cả sách</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _db.QueryAsync<Book>("SELECT * FROM Books");
        return Ok(books);
    }

    /// <summary>Lấy thông tin sách theo Id</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _db.QueryFirstOrDefaultAsync<Book>(
            "SELECT * FROM Books WHERE Id = @Id", new { Id = id });

        if (book == null)
            return NotFound(new { Message = $"Không tìm thấy sách có Id = {id}" });

        return Ok(book);
    }

    /// <summary>
    /// Cập nhật số lượng tồn kho (Stock).
    /// delta = -1 khi mượn sách, delta = +1 khi trả sách.
    /// </summary>
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromQuery] int delta)
    {
        var book = await _db.QueryFirstOrDefaultAsync<Book>(
            "SELECT * FROM Books WHERE Id = @Id", new { Id = id });

        if (book == null)
            return NotFound(new { Message = $"Không tìm thấy sách có Id = {id}" });

        int newStock = book.Stock + delta;
        if (newStock < 0)
            return BadRequest(new { Message = "Số lượng tồn kho không đủ!" });

        await _db.ExecuteAsync(
            "UPDATE Books SET Stock = @Stock WHERE Id = @Id",
            new { Stock = newStock, Id = id });

        return Ok(new { Message = "Cập nhật Stock thành công.", NewStock = newStock });
    }

    /// <summary>Thêm sách mới</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book)
    {
        await _db.ExecuteAsync(
            "INSERT INTO Books (Id, Title, Author, Stock) VALUES (@Id, @Title, @Author, @Stock)",
            book);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }
}
