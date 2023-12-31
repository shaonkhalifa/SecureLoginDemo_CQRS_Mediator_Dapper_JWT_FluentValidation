USE [master]
GO
/****** Object:  Database [TESTDB]    Script Date: 12/18/2023 5:57:54 PM ******/
CREATE DATABASE [TESTDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TESTDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TESTDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TESTDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TESTDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TESTDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TESTDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TESTDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TESTDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TESTDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TESTDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TESTDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TESTDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TESTDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TESTDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TESTDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TESTDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TESTDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TESTDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TESTDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TESTDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TESTDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TESTDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TESTDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TESTDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TESTDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TESTDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TESTDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TESTDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TESTDB] SET RECOVERY FULL 
GO
ALTER DATABASE [TESTDB] SET  MULTI_USER 
GO
ALTER DATABASE [TESTDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TESTDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TESTDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TESTDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TESTDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TESTDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TESTDB', N'ON'
GO
ALTER DATABASE [TESTDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [TESTDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TESTDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[PermissionId] [int] NOT NULL,
	[PermissionName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermissionAssign]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionAssign](
	[PermissionAssignId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[PermissionId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleAssain]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleAssain](
	[RoleAssainId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_RoleAssain] PRIMARY KEY CLUSTERED 
(
	[RoleAssainId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentId] [int] IDENTITY(1,1) NOT NULL,
	[StudentName] [nvarchar](100) NOT NULL,
	[StudentEmail] [nvarchar](100) NULL,
	[FatherName] [nvarchar](50) NULL,
	[MotherName] [nvarchar](50) NULL,
	[RollNo] [int] NULL,
 CONSTRAINT [PK_StudentTbl] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Token] [nvarchar](max) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (1, N'UserRegister')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (2, N'StudentGet')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (3, N'StudentInsert')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (4, N'StudentUpdate')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (5, N'StudentDelete')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (6, N'StudentCreate')
INSERT [dbo].[Permission] ([PermissionId], [PermissionName]) VALUES (7, N'StudentOpen')
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([RoleId], [RoleName]) VALUES (1, N'Admin')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[RoleAssain] ON 

INSERT [dbo].[RoleAssain] ([RoleAssainId], [RoleId], [UserId]) VALUES (1, 1, 1)
SET IDENTITY_INSERT [dbo].[RoleAssain] OFF
GO
SET IDENTITY_INSERT [dbo].[Students] ON 

INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (1, N'Shaon Khalifa', N's@gmail.com', N'Md.Siddiqur Rahaman Khalifa', N'Syeda Nadira Akter', 1)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (2, N'Abdul Ahad', N'aa@gmail.com', N'Abdul Salam', N'Humaira Begum', 2)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (3, N'Rakim Unnoto Programmer', N'ddfkfk', N'dssd', N'', 20)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (4, N'string', N'string', N'string', N'string', 0)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (5, N'string', N'string', N'string', N'string', 0)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (6, N'string', N'string', N'string', N'string', 0)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (7, N'string', N'string', N'string', N'string', 0)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (8, N'string', N'string', N'string', N'string', 0)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (9, N'Shaon', N's@gmail.com', N'siddiqur', N'nadira', 50)
INSERT [dbo].[Students] ([StudentId], [StudentName], [StudentEmail], [FatherName], [MotherName], [RollNo]) VALUES (10, N'Rakibul', N'r@gmail.com', N'Kkkkk', N'pppp', 99)
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([UserId], [FirstName], [LastName], [UserName], [Password], [Token]) VALUES (1, N'Shaon', N'Khalifa', N'shaon', N'$2a$11$MaOjNAvfxDa/TM1P0PpFJuRxTd3YkN.jlRwBhBjZjTlBnMceaAMse', N'string')
SET IDENTITY_INSERT [dbo].[User] OFF
GO
/****** Object:  StoredProcedure [dbo].[UpdateStudentPrc]    Script Date: 12/18/2023 5:57:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateStudentPrc]
    @StudentId INT,
    @StudentName NVARCHAR(MAX),
    @StudentEmail NVARCHAR(MAX),
    @FatherName NVARCHAR(MAX),
    @MotherName NVARCHAR(MAX),
    @RollNo INT
AS
BEGIN
    UPDATE Students
    SET 
        StudentName = @StudentName,
        StudentEmail = @StudentEmail,
        FatherName = @FatherName,
        MotherName = @MotherName,
        RollNo = @RollNo
		WHERE StudentId=@StudentId;
END;
GO
USE [master]
GO
ALTER DATABASE [TESTDB] SET  READ_WRITE 
GO
