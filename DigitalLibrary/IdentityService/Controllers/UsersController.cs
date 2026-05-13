// IdentityService/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using IdentityService.Models;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly SqlConnection _db;

    public UsersController(SqlConnection db)
    {
        _db = db;
    }

    /// <summary>Lấy danh sách tất cả người dùng</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.QueryAsync<User>("SELECT * FROM Users");
        return Ok(users);
    }

    /// <summary>Lấy thông tin người dùng theo Id</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id", new { Id = id });

        if (user == null)
            return NotFound(new { Message = $"Không tìm thấy người dùng có Id = {id}" });

        return Ok(user);
    }

    /// <summary>
    /// Kiểm tra quyền mượn sách của người dùng.
    /// Silver: tối đa 3 cuốn. Gold: tối đa 5 cuốn.
    /// currentBorrowCount là số sách đang mượn (do BorrowingService truyền lên).
    /// </summary>
    [HttpGet("{id}/rank-check")]
    public async Task<IActionResult> CheckRank(int id, [FromQuery] int currentBorrowCount = 0)
    {
        var user = await _db.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id", new { Id = id });

        if (user == null)
            return NotFound(new { Message = $"Không tìm thấy người dùng có Id = {id}" });

        int maxBorrow = user.Rank == "Gold" ? 5 : 3;
        bool canBorrow = currentBorrowCount < maxBorrow;

        return Ok(new UserRankInfo
        {
            UserId        = user.Id,
            FullName      = user.FullName,
            Rank          = user.Rank,
            MaxBorrow     = maxBorrow,
            CurrentBorrow = currentBorrowCount,
            CanBorrow     = canBorrow
        });
    }

    /// <summary>Tạo người dùng mới</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        var existing = await _db.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id", new { user.Id });
        if (existing != null)
            return Conflict(new { Message = "Id đã tồn tại." });

        await _db.ExecuteAsync(
            "INSERT INTO Users (Id, Username, FullName, Rank) VALUES (@Id, @Username, @FullName, @Rank)",
            user);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}
