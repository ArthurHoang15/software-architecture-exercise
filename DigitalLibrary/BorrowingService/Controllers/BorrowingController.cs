// BorrowingService/Controllers/BorrowingController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Net.Http.Json;
using BorrowingService.Models;

namespace BorrowingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowingController : ControllerBase
{
    private readonly SqlConnection      _db;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration     _config;

    public BorrowingController(
        SqlConnection db,
        IHttpClientFactory clientFactory,
        IConfiguration config)
    {
        _db            = db;
        _clientFactory = clientFactory;
        _config        = config;
    }

    private string IdentityUrl     => _config["ServiceUrls:Identity"]!;
    private string BookUrl         => _config["ServiceUrls:Book"]!;
    private string NotificationUrl => _config["ServiceUrls:Notification"]!;

    // ─────────────────────────────────────────────────────────────────
    // GET api/borrowing  — Lấy toàn bộ lịch sử mượn
    // ─────────────────────────────────────────────────────────────────
    /// <summary>Lấy danh sách tất cả bản ghi mượn sách</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _db.QueryAsync<BorrowRecord>(
            "SELECT * FROM BorrowRecords ORDER BY BorrowDate DESC");
        return Ok(records);
    }

    // ─────────────────────────────────────────────────────────────────
    // GET api/borrowing/user/{userId}  — Sách đang mượn của 1 user
    // ─────────────────────────────────────────────────────────────────
    /// <summary>Lấy danh sách sách đang mượn của một người dùng</summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var records = await _db.QueryAsync<BorrowRecord>(
            "SELECT * FROM BorrowRecords WHERE UserId = @UserId AND ReturnDate IS NULL",
            new { UserId = userId });
        return Ok(records);
    }

    // ─────────────────────────────────────────────────────────────────
    // POST api/borrowing  — Mượn sách (nghiệp vụ chính)
    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Xử lý yêu cầu mượn sách:
    /// 1. Check User tồn tại và còn quota mượn
    /// 2. Check Sách còn hàng trong kho
    /// 3. Lưu BorrowRecord vào DB
    /// 4. Giảm Stock của sách (-1)
    /// 5. Ghi log Notification
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowRequest request)
    {
        var client = _clientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        // ── Bước 1: Đếm sách đang mượn của user ──────────────────────
        int currentBorrow = await _db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM BorrowRecords WHERE UserId = @UserId AND ReturnDate IS NULL",
            new { UserId = request.UserId });

        // ── Bước 2: Gọi Identity Service kiểm tra user & quota ────────
        UserRankInfo? rankInfo;
        try
        {
            var rankRes = await client.GetAsync(
                $"{IdentityUrl}/api/users/{request.UserId}/rank-check?currentBorrowCount={currentBorrow}");

            if (!rankRes.IsSuccessStatusCode)
                return NotFound(new { Message = $"Không tìm thấy người dùng Id = {request.UserId}" });

            rankInfo = await rankRes.Content.ReadFromJsonAsync<UserRankInfo>();
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { Message = "Identity Service không phản hồi.", Detail = ex.Message });
        }

        if (rankInfo == null || !rankInfo.CanBorrow)
        {
            return BadRequest(new
            {
                Message = $"Người dùng đã đạt giới hạn mượn sách ({rankInfo?.MaxBorrow ?? 0} cuốn).",
                CurrentBorrow = currentBorrow,
                MaxBorrow = rankInfo?.MaxBorrow
            });
        }

        // ── Bước 3: Gọi Book Service kiểm tra sách còn hàng ──────────
        BookDto? book;
        try
        {
            var bookRes = await client.GetAsync($"{BookUrl}/api/books/{request.BookId}");

            if (!bookRes.IsSuccessStatusCode)
                return NotFound(new { Message = $"Không tìm thấy sách Id = {request.BookId}" });

            book = await bookRes.Content.ReadFromJsonAsync<BookDto>();
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { Message = "Book Service không phản hồi.", Detail = ex.Message });
        }

        if (book == null || book.Stock <= 0)
            return BadRequest(new { Message = $"Sách '{book?.Title}' hiện đã hết trong kho." });

        // ── Bước 4: Lưu BorrowRecord vào BorrowingDB ──────────────────
        var now    = DateTime.Now;
        var dueDate = now.AddDays(14);  // Thời hạn 14 ngày

        await _db.ExecuteAsync(
            @"INSERT INTO BorrowRecords (UserId, BookId, BorrowDate, DueDate)
              VALUES (@UserId, @BookId, @BorrowDate, @DueDate)",
            new { request.UserId, request.BookId, BorrowDate = now, DueDate = dueDate });

        // ── Bước 5: Gọi Book Service giảm Stock đi 1 ─────────────────
        try
        {
            await client.PutAsync($"{BookUrl}/api/books/{request.BookId}/stock?delta=-1", null);
        }
        catch
        {
            // Log lỗi nhưng không rollback - cần saga/compensation trong thực tế
            Console.WriteLine($"[WARN] Không thể cập nhật Stock sách Id={request.BookId}");
        }

        // ── Bước 6: Gọi Notification Service ghi log ─────────────────
        try
        {
            var msg = $"[MƯỢN SÁCH] User '{rankInfo.FullName}' (Id={request.UserId}) " +
                      $"đã mượn sách '{book.Title}' (Id={request.BookId}). " +
                      $"Hạn trả: {dueDate:dd/MM/yyyy}.";

            await client.PostAsJsonAsync($"{NotificationUrl}/api/notifications", new { Message = msg });
        }
        catch
        {
            Console.WriteLine("[WARN] Notification Service không phản hồi.");
        }

        return Ok(new
        {
            Message    = "Mượn sách thành công!",
            UserId     = request.UserId,
            UserName   = rankInfo.FullName,
            BookId     = request.BookId,
            BookTitle  = book.Title,
            BorrowDate = now,
            DueDate    = dueDate
        });
    }

    // ─────────────────────────────────────────────────────────────────
    // PUT api/borrowing/{id}/return  — Trả sách
    // ─────────────────────────────────────────────────────────────────
    /// <summary>Xử lý trả sách: cập nhật ReturnDate và tăng Stock</summary>
    [HttpPut("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        var record = await _db.QueryFirstOrDefaultAsync<BorrowRecord>(
            "SELECT * FROM BorrowRecords WHERE Id = @Id", new { Id = id });

        if (record == null)
            return NotFound(new { Message = $"Không tìm thấy bản ghi mượn Id = {id}" });

        if (record.ReturnDate != null)
            return BadRequest(new { Message = "Sách này đã được trả rồi." });

        // Cập nhật ReturnDate
        await _db.ExecuteAsync(
            "UPDATE BorrowRecords SET ReturnDate = GETDATE() WHERE Id = @Id",
            new { Id = id });

        // Tăng Stock +1
        var client = _clientFactory.CreateClient();
        try
        {
            await client.PutAsync($"{BookUrl}/api/books/{record.BookId}/stock?delta=1", null);
        }
        catch
        {
            Console.WriteLine($"[WARN] Không thể cập nhật Stock sách Id={record.BookId}");
        }

        // Ghi log
        try
        {
            var msg = $"[TRẢ SÁCH] Bản ghi Id={id}, UserId={record.UserId}, BookId={record.BookId} đã trả sách.";
            await client.PostAsJsonAsync($"{NotificationUrl}/api/notifications", new { Message = msg });
        }
        catch { /* ignore */ }

        return Ok(new { Message = "Trả sách thành công!", RecordId = id });
    }
}
