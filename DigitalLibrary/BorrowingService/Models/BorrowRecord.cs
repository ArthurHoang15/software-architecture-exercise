// BorrowingService/Models/BorrowRecord.cs
namespace BorrowingService.Models;

public class BorrowRecord
{
    public int       Id         { get; set; }
    public int       UserId     { get; set; }
    public int       BookId     { get; set; }
    public DateTime  BorrowDate { get; set; }
    public DateTime  DueDate    { get; set; }
    public DateTime? ReturnDate { get; set; }
}

public class BorrowRequest
{
    public int UserId { get; set; }
    public int BookId { get; set; }
}

// DTOs nhận từ các service khác
public class UserDto
{
    public int    Id       { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Rank     { get; set; } = string.Empty;
}

public class UserRankInfo
{
    public int    UserId       { get; set; }
    public string FullName     { get; set; } = string.Empty;
    public string Rank         { get; set; } = string.Empty;
    public int    MaxBorrow    { get; set; }
    public int    CurrentBorrow { get; set; }
    public bool   CanBorrow    { get; set; }
}

public class BookDto
{
    public int    Id     { get; set; }
    public string Title  { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int    Stock  { get; set; }
}
