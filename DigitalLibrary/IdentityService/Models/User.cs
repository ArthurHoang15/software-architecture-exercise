// IdentityService/Models/User.cs
namespace IdentityService.Models;

public class User
{
    public int    Id       { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Rank     { get; set; } = string.Empty; // "Gold" | "Silver"
}

public class UserRankInfo
{
    public int    UserId      { get; set; }
    public string FullName    { get; set; } = string.Empty;
    public string Rank        { get; set; } = string.Empty;
    public int    MaxBorrow   { get; set; }  // Giới hạn mượn sách
    public int    CurrentBorrow { get; set; } // Số sách đang mượn (do BorrowingService truyền vào)
    public bool   CanBorrow   { get; set; }
}
