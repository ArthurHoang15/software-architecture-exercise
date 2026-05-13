// NotificationService/Controllers/NotificationsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

namespace NotificationService.Controllers;

public class NotifyRequest
{
    public string Message { get; set; } = string.Empty;
}

public class LogEntry
{
    public int      Id       { get; set; }
    public string   Message  { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly SqlConnection _db;
    public NotificationsController(SqlConnection db) => _db = db;

    /// <summary>Lấy lịch sử thông báo</summary>
    [HttpGet]
    public async Task<IActionResult> GetLogs()
    {
        var logs = await _db.QueryAsync<LogEntry>(
            "SELECT * FROM Logs ORDER BY SentDate DESC");
        return Ok(logs);
    }

    /// <summary>
    /// Ghi log thông báo mới.
    /// Ví dụ: gọi khi mượn sách thành công, hoặc khi sách quá hạn.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Notify([FromBody] NotifyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { Message = "Message không được để trống." });

        await _db.ExecuteAsync(
            "INSERT INTO Logs (Message, SentDate) VALUES (@Message, GETDATE())",
            new { request.Message });

        // Trong thực tế có thể gửi email/SMS ở đây
        Console.WriteLine($"[NOTIFICATION] {request.Message}");

        return Ok(new { Message = "Thông báo đã được ghi nhận.", Content = request.Message });
    }
}
