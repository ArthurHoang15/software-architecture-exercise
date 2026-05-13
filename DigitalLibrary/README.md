# 📚 Digital Library System — Hướng dẫn Cài đặt & Chạy

## Kiến trúc Tổng quan

```
[Blazor UI :7200]
       │
[Gateway :8000]  ←─── Ocelot API Gateway
       │
 ┌─────┼───────────┬──────────────────┐
 ▼     ▼           ▼                  ▼
[Identity :5001] [Book :5003] [Borrowing :5005] [Notification :5007]
      │                │              │                   │
 IdentityDB        BookDB       BorrowingDB        NotificationDB
```

## Ports

| Service             | HTTPS Port |
|---------------------|-----------|
| Identity Service    | 5001      |
| Book Service        | 5003      |
| Borrowing Service   | 5005      |
| Notification Service| 5007      |
| **Gateway (Ocelot)**| **8000**  |
| Blazor Frontend     | 7200      |

## Bước 1 — Tạo Database

Mở **SQL Server Management Studio**, chạy file:

```
SQL/01_CreateDatabases.sql
```

Kiểm tra: 4 database `IdentityDB`, `BookDB`, `BorrowingDB`, `NotificationDB` đã được tạo.

## Bước 2 — Cấu hình Connection String

Mở từng file `appsettings.json` trong 4 service, sửa `ConnectionStrings` nếu SQL Server của bạn
dùng SQL Authentication thay vì Windows Authentication:

```json
"Server=localhost;Database=IdentityDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

## Bước 3 — Chạy tất cả Projects

**Cách 1 — Visual Studio:**
1. Chuột phải Solution → **Configure Startup Projects**
2. Chọn **Multiple startup projects**
3. Đặt tất cả 5 project (4 API + Gateway) là **Start**
4. Nhấn F5

**Cách 2 — Command Line (mở 5 terminal riêng):**
```bash
# Terminal 1
cd IdentityService && dotnet run

# Terminal 2
cd BookService && dotnet run

# Terminal 3
cd NotificationService && dotnet run

# Terminal 4
cd BorrowingService && dotnet run

# Terminal 5
cd Gateway && dotnet run

# Terminal 6 (Frontend - tùy chọn)
cd BlazorFrontend && dotnet run
```

## Bước 4 — Kiểm thử với Swagger

Luồng kiểm thử chuẩn:

### 1. Identity Service (`https://localhost:5001`)
- `GET /api/users` → Xem danh sách user
- `GET /api/users/1/rank-check?currentBorrowCount=0` → Kiểm tra quyền mượn

### 2. Book Service (`https://localhost:5003`)
- `GET /api/books` → Xem danh sách sách và Stock
- `PUT /api/books/101/stock?delta=-1` → Test giảm Stock thủ công

### 3. Borrowing Service (`https://localhost:5005`) — Nghiệp vụ chính
```json
POST /api/borrowing
Body: { "userId": 1, "bookId": 101 }
```
Service sẽ tự động:
- ✅ Check user tồn tại và còn quota mượn
- ✅ Check sách còn Stock
- ✅ Lưu BorrowRecord vào BorrowingDB
- ✅ Gọi Book Service giảm Stock -1
- ✅ Gọi Notification Service ghi log

### 4. Notification Service (`https://localhost:5007`)
- `GET /api/notifications` → Xem tất cả log

## Bước 5 — Kiểm thử qua Gateway (`http://localhost:8000`)

```
GET  http://localhost:8000/identity/api/users
GET  http://localhost:8000/books/api/books
POST http://localhost:8000/borrowing/api/borrowing
GET  http://localhost:8000/notifications/api/notifications
```

## Quy tắc nghiệp vụ

| Hạng người dùng | Giới hạn mượn |
|----------------|--------------|
| Gold           | 5 cuốn       |
| Silver         | 3 cuốn       |

- Thời hạn mượn: **14 ngày** kể từ ngày mượn
- Sách hết Stock → không thể mượn
- Trả sách: `PUT /api/borrowing/{id}/return`

## Tiêu chí đánh giá

| Tiêu chí        | Mô tả                                               | Điểm   |
|----------------|-----------------------------------------------------|--------|
| Tính đóng gói   | Mỗi service chỉ truy cập DB của chính nó            | ✅ Đạt |
| Giao tiếp API   | IHttpClientFactory, xử lý lỗi khi service "chết"   | ✅ Đạt |
| Swagger         | Đầy đủ mô tả, 4 service có Swagger riêng            | ✅ Đạt |
| Frontend Blazor | Danh sách sách + nút Mượn + Lịch sử mượn            | ✅ Cộng điểm |
| Gateway Ocelot  | Port 8000, route đến cả 4 service                   | ✅ Cộng điểm |
