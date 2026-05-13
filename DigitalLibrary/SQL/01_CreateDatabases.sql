-- =============================================
-- DIGITAL LIBRARY SYSTEM - DATABASE SCRIPTS
-- Chạy file này trên SQL Server Management Studio
-- =============================================

-- ================================
-- 1. IdentityDB
-- ================================
CREATE DATABASE IdentityDB;
GO
USE IdentityDB;
GO

CREATE TABLE Users (
    Id          INT PRIMARY KEY,
    Username    NVARCHAR(50)  NOT NULL UNIQUE,
    FullName    NVARCHAR(100) NOT NULL,
    Rank        NVARCHAR(20)  NOT NULL  -- 'Gold' or 'Silver'
);
GO

-- Dữ liệu mẫu
INSERT INTO Users (Id, Username, FullName, Rank) VALUES
(1, 'sv01', 'Nguyen Van A', 'Gold'),
(2, 'sv02', 'Tran Thi B',   'Silver'),
(3, 'sv03', 'Le Van C',     'Silver');
GO

-- ================================
-- 2. BookDB
-- ================================
CREATE DATABASE BookDB;
GO
USE BookDB;
GO

CREATE TABLE Books (
    Id      INT PRIMARY KEY,
    Title   NVARCHAR(200) NOT NULL,
    Author  NVARCHAR(100) NOT NULL,
    Stock   INT NOT NULL DEFAULT 0
);
GO

-- Dữ liệu mẫu
INSERT INTO Books (Id, Title, Author, Stock) VALUES
(101, 'Clean Code',              'Robert C. Martin', 5),
(102, 'Design Patterns',         'Gang of Four',     2),
(103, 'The Pragmatic Programmer','David Thomas',     3),
(104, 'Domain-Driven Design',    'Eric Evans',       1);
GO

-- ================================
-- 3. BorrowingDB
-- ================================
CREATE DATABASE BorrowingDB;
GO
USE BorrowingDB;
GO

CREATE TABLE BorrowRecords (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    UserId      INT           NOT NULL,
    BookId      INT           NOT NULL,
    BorrowDate  DATETIME      NOT NULL DEFAULT GETDATE(),
    DueDate     DATETIME      NOT NULL,   -- Ngày phải trả (BorrowDate + 14 ngày)
    ReturnDate  DATETIME      NULL        -- NULL = chưa trả
);
GO

-- ================================
-- 4. NotificationDB
-- ================================
CREATE DATABASE NotificationDB;
GO
USE NotificationDB;
GO

CREATE TABLE Logs (
    Id        INT PRIMARY KEY IDENTITY(1,1),
    Message   NVARCHAR(MAX) NOT NULL,
    SentDate  DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

PRINT 'Tất cả 4 database đã được tạo thành công!';
