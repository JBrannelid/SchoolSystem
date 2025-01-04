USE [master]
GO
/****** Object:  Database [SchoolDB]    Script Date: 2025-01-04 17:13:32 ******/
CREATE DATABASE [SchoolDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SchoolDB', FILENAME = N'C:\Users\brann\SchoolDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SchoolDB_log', FILENAME = N'C:\Users\brann\SchoolDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [SchoolDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SchoolDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SchoolDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SchoolDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SchoolDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SchoolDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SchoolDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SchoolDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [SchoolDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SchoolDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SchoolDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SchoolDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SchoolDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SchoolDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SchoolDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SchoolDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SchoolDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SchoolDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SchoolDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SchoolDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SchoolDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SchoolDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SchoolDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SchoolDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SchoolDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SchoolDB] SET  MULTI_USER 
GO
ALTER DATABASE [SchoolDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SchoolDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SchoolDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SchoolDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SchoolDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SchoolDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SchoolDB] SET QUERY_STORE = OFF
GO
USE [SchoolDB]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[EmployeePIN] [nvarchar](13) NOT NULL,
	[FirstName] [nvarchar](200) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Gender] [varchar](10) NOT NULL,
	[StartDate] [date] NOT NULL,
	[Position] [varchar](20) NOT NULL,
	[FKDepartmentID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeePIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_EmployeeOverview]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Show all employees (Name, Position, yearsWorked)
CREATE VIEW [dbo].[vw_EmployeeOverview] AS
SELECT 
    e.EmployeePIN,
    e.FirstName AS FirstName,
    e.LastName AS LastName,
    e.Position,
    DATEDIFF(YEAR, e.StartDate, GETDATE()) AS YearsWorked,
    d.Name AS Department
FROM Employees e
JOIN Departments d ON e.FKDepartmentID = d.DepartmentID
WHERE e.IsActive = 1
GO
/****** Object:  Table [dbo].[Students]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentPIN] [nvarchar](13) NOT NULL,
	[FirstName] [nvarchar](200) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Gender] [varchar](10) NOT NULL,
	[FKClassId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[EnrollmentDate] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentPIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[CourseId] [int] IDENTITY(1,1) NOT NULL,
	[CourseName] [nvarchar](250) NOT NULL,
	[CourseCode] [nvarchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseEnrollments]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseEnrollments](
	[EnrollmentId] [int] IDENTITY(1,1) NOT NULL,
	[FKStudentPIN] [nvarchar](13) NOT NULL,
	[FKCourseId] [int] NOT NULL,
	[FKTeacherPIN] [nvarchar](13) NOT NULL,
	[Grade] [char](2) NULL,
	[GradeAssignedDate] [datetime2](7) NULL,
	[EnrollmentDate] [date] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EnrollmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_StudentCourseGrades]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Show students, courses, teachers and grades 
CREATE VIEW [dbo].[vw_StudentCourseGrades] AS
SELECT 
    s.StudentPIN,
    s.FirstName AS StudentFirstName,
    s.LastName AS StudentLastName,
    c.CourseName,
    c.CourseCode,
    e.FirstName AS TeacherFirstName,
    e.LastName AS TeacherLastName,
    ce.Grade,
    ce.GradeAssignedDate
FROM CourseEnrollments ce
JOIN Students s ON ce.FKStudentPIN = s.StudentPIN
JOIN Courses c ON ce.FKCourseId = c.CourseId
JOIN Employees e ON ce.FKTeacherPIN = e.EmployeePIN
WHERE ce.IsActive = 1 AND s.IsActive = 1 AND c.IsActive = 1
GO
/****** Object:  View [dbo].[vw_TeachersByDepartment]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Show teachers on active department 
CREATE VIEW [dbo].[vw_TeachersByDepartment] AS
SELECT 
    d.Name AS DepartmentName,
    COUNT(e.EmployeePIN) AS TeacherCount
FROM Employees e
JOIN Departments d ON e.FKDepartmentID = d.DepartmentID
WHERE e.Position = 'Teacher' AND e.IsActive = 1
GROUP BY d.Name

GO
/****** Object:  Table [dbo].[Salaries]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salaries](
	[SalaryID] [int] IDENTITY(1,1) NOT NULL,
	[FKEmployeePIN] [nvarchar](13) NOT NULL,
	[SalaryAmount] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SalaryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_AvgSalaryByDepartment]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Show average salary for all employees on every department
CREATE VIEW [dbo].[vw_AvgSalaryByDepartment] AS
SELECT 
    d.Name AS DepartmentName,
    AVG(s.SalaryAmount) AS AvgSalary
FROM Employees e
JOIN Departments d ON e.FKDepartmentID = d.DepartmentID
JOIN Salaries s ON s.FKEmployeePIN = e.EmployeePIN
WHERE e.IsActive = 1
GROUP BY d.Name

GO
/****** Object:  Table [dbo].[Classes]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classes](
	[ClassId] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [nvarchar](10) NOT NULL,
	[Year] [int] NOT NULL,
	[Section] [char](1) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GradeValues]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GradeValues](
	[Grade] [char](2) NOT NULL,
	[GradeValue] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Grade] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Classes] ON 
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (1, N'1A', 1, N'A', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (2, N'1B', 1, N'B', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (3, N'1C', 1, N'C', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (4, N'2A', 2, N'A', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (5, N'2B', 2, N'B', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (6, N'2C', 2, N'C', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (7, N'3A', 3, N'A', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (8, N'3B', 3, N'B', 1)
GO
INSERT [dbo].[Classes] ([ClassId], [ClassName], [Year], [Section], [IsActive]) VALUES (9, N'3C', 3, N'C', 1)
GO
SET IDENTITY_INSERT [dbo].[Classes] OFF
GO
SET IDENTITY_INSERT [dbo].[CourseEnrollments] ON 
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (1, N'20001010-1234', 1, N'19820101-1234', N'A ', CAST(N'2023-06-10T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (2, N'20010202-2345', 2, N'19850505-2345', N'B ', CAST(N'2023-06-15T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (3, N'20030803-3456', 3, N'19890220-5678', N'C ', CAST(N'2023-06-20T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (4, N'20040604-4567', 4, N'19830310-6789', N'A ', CAST(N'2023-06-12T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (5, N'20051105-5678', 5, N'19900505-8901', N'B ', CAST(N'2023-06-14T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
INSERT [dbo].[CourseEnrollments] ([EnrollmentId], [FKStudentPIN], [FKCourseId], [FKTeacherPIN], [Grade], [GradeAssignedDate], [EnrollmentDate], [IsActive]) VALUES (6, N'20060906-6789', 1, N'19840712-7890', N'E ', CAST(N'2023-06-18T00:00:00.0000000' AS DateTime2), CAST(N'2025-01-02' AS Date), 1)
GO
SET IDENTITY_INSERT [dbo].[CourseEnrollments] OFF
GO
SET IDENTITY_INSERT [dbo].[Courses] ON 
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (1, N'Matematik', N'MA101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (2, N'Fysik', N'FY101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (3, N'Kemi', N'KE101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (4, N'Biologi', N'BI101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (5, N'Svenska', N'SV101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (6, N'Engelska', N'EN101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (7, N'Historia', N'HI101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (8, N'Geografi', N'GE101', 1)
GO
INSERT [dbo].[Courses] ([CourseId], [CourseName], [CourseCode], [IsActive]) VALUES (9, N'Idrott och hälsa', N'IH101', 1)
GO
SET IDENTITY_INSERT [dbo].[Courses] OFF
GO
SET IDENTITY_INSERT [dbo].[Departments] ON 
GO
INSERT [dbo].[Departments] ([DepartmentID], [Name]) VALUES (3, N'Administration')
GO
INSERT [dbo].[Departments] ([DepartmentID], [Name]) VALUES (2, N'Faculty of Humanities/sports')
GO
INSERT [dbo].[Departments] ([DepartmentID], [Name]) VALUES (1, N'Faculty of Science')
GO
SET IDENTITY_INSERT [dbo].[Departments] OFF
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19791210-3456', N'Carla', N'Nilsson', N'Female', CAST(N'2018-11-01' AS Date), N'Principal', 3, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19801215-4567', N'David', N'Svensson', N'Male', CAST(N'2010-01-01' AS Date), N'Administrator', 3, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19820101-1234', N'Anna', N'Johansson', N'Female', CAST(N'2015-08-15' AS Date), N'Teacher', 1, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19830310-6789', N'Fredrik', N'Larsson', N'Male', CAST(N'2013-04-10' AS Date), N'Teacher', 2, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19840712-7890', N'Greta', N'Olofsson', N'Female', CAST(N'2017-07-23' AS Date), N'Teacher', 1, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19850505-2345', N'Björn', N'Andersson', N'Male', CAST(N'2012-03-25' AS Date), N'Teacher', 1, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19890220-5678', N'Elin', N'Karlsson', N'Female', CAST(N'2019-09-12' AS Date), N'Teacher', 2, 1)
GO
INSERT [dbo].[Employees] ([EmployeePIN], [FirstName], [LastName], [Gender], [StartDate], [Position], [FKDepartmentID], [IsActive]) VALUES (N'19900505-8901', N'Håkan', N'Berg', N'Male', CAST(N'2020-06-15' AS Date), N'Teacher', 2, 1)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'F ', 0)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'E ', 1)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'D ', 2)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'C ', 3)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'B ', 4)
GO
INSERT [dbo].[GradeValues] ([Grade], [GradeValue]) VALUES (N'A ', 5)
GO
SET IDENTITY_INSERT [dbo].[Salaries] ON 
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (1, N'19820101-1234', CAST(33410.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (2, N'19850505-2345', CAST(35520.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (3, N'19791210-3456', CAST(55000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (4, N'19801215-4567', CAST(42200.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (5, N'19890220-5678', CAST(36100.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (6, N'19830310-6789', CAST(37340.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (7, N'19840712-7890', CAST(30000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Salaries] ([SalaryID], [FKEmployeePIN], [SalaryAmount]) VALUES (8, N'19900505-8901', CAST(31400.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[Salaries] OFF
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20001010-1234', N'Eva', N'Lindström', N'Female', 1, 1, CAST(N'2021-09-01' AS Date))
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20010202-2345', N'Johan', N'Eriksson', N'Male', 2, 1, CAST(N'2021-09-01' AS Date))
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20030803-3456', N'Lina', N'Johansson', N'Female', 3, 1, CAST(N'2022-09-01' AS Date))
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20040604-4567', N'Mats', N'Karlsson', N'Male', 1, 1, CAST(N'2022-09-01' AS Date))
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20051105-5678', N'Sara', N'Olsson', N'Female', 2, 1, CAST(N'2021-09-01' AS Date))
GO
INSERT [dbo].[Students] ([StudentPIN], [FirstName], [LastName], [Gender], [FKClassId], [IsActive], [EnrollmentDate]) VALUES (N'20060906-6789', N'Peter', N'Svensson', N'Male', 3, 1, CAST(N'2021-09-01' AS Date))
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Classes__F8BF561B85FC9A68]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Classes] ADD UNIQUE NONCLUSTERED 
(
	[ClassName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Courses__FC00E0006E04D591]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Courses] ADD UNIQUE NONCLUSTERED 
(
	[CourseCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_CourseCode]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Courses] ADD  CONSTRAINT [UQ_CourseCode] UNIQUE NONCLUSTERED 
(
	[CourseCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Departme__737584F680D42168]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Departments] ADD UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_PIN]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [UC_PIN] UNIQUE NONCLUSTERED 
(
	[EmployeePIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_GradeValue]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[GradeValues] ADD  CONSTRAINT [UQ_GradeValue] UNIQUE NONCLUSTERED 
(
	[GradeValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_Student_PIN]    Script Date: 2025-01-04 17:13:32 ******/
ALTER TABLE [dbo].[Students] ADD  CONSTRAINT [UC_Student_PIN] UNIQUE NONCLUSTERED 
(
	[StudentPIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[CourseEnrollments] ADD  DEFAULT (getdate()) FOR [EnrollmentDate]
GO
ALTER TABLE [dbo].[CourseEnrollments] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Courses] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getdate()) FOR [StartDate]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT (getdate()) FOR [EnrollmentDate]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_Course] FOREIGN KEY([FKCourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_Enrollment_Course]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_Grade] FOREIGN KEY([Grade])
REFERENCES [dbo].[GradeValues] ([Grade])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_Enrollment_Grade]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_Student] FOREIGN KEY([FKStudentPIN])
REFERENCES [dbo].[Students] ([StudentPIN])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_Enrollment_Student]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_Teacher] FOREIGN KEY([FKTeacherPIN])
REFERENCES [dbo].[Employees] ([EmployeePIN])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_Enrollment_Teacher]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Department] FOREIGN KEY([FKDepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employee_Department]
GO
ALTER TABLE [dbo].[Salaries]  WITH CHECK ADD  CONSTRAINT [FK_Salary_Employee] FOREIGN KEY([FKEmployeePIN])
REFERENCES [dbo].[Employees] ([EmployeePIN])
GO
ALTER TABLE [dbo].[Salaries] CHECK CONSTRAINT [FK_Salary_Employee]
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [FK_Student_Class] FOREIGN KEY([FKClassId])
REFERENCES [dbo].[Classes] ([ClassId])
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [FK_Student_Class]
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD  CONSTRAINT [CHK_ValidSection] CHECK  (([Section]='C' OR [Section]='B' OR [Section]='A'))
GO
ALTER TABLE [dbo].[Classes] CHECK CONSTRAINT [CHK_ValidSection]
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD  CONSTRAINT [CHK_ValidYear] CHECK  (([Year]>=(1) AND [Year]<=(3)))
GO
ALTER TABLE [dbo].[Classes] CHECK CONSTRAINT [CHK_ValidYear]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [CHK_GradeAndDate] CHECK  (([Grade] IS NULL AND [GradeAssignedDate] IS NULL OR [Grade] IS NOT NULL AND [GradeAssignedDate] IS NOT NULL))
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [CHK_GradeAndDate]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [CHK_Employee_Gender] CHECK  (([Gender]='Other' OR [Gender]='Female' OR [Gender]='Male'))
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [CHK_Employee_Gender]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [CHK_Employee_PIN] CHECK  (([EmployeePIN] like '[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]-[0-9][0-9][0-9][0-9]' AND (substring([EmployeePIN],(5),(2))>='01' AND substring([EmployeePIN],(5),(2))<='12') AND (substring([EmployeePIN],(7),(2))>='01' AND substring([EmployeePIN],(7),(2))<='31')))
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [CHK_Employee_PIN]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [CHK_Employee_Position] CHECK  (([Position]='Teacher' OR [Position]='Administrator' OR [Position]='Principal'))
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [CHK_Employee_Position]
GO
ALTER TABLE [dbo].[Salaries]  WITH CHECK ADD  CONSTRAINT [CHK_Salary_Amount] CHECK  (([SalaryAmount]>(0)))
GO
ALTER TABLE [dbo].[Salaries] CHECK CONSTRAINT [CHK_Salary_Amount]
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [CHK_ValidGender] CHECK  (([Gender]='Other' OR [Gender]='Female' OR [Gender]='Male'))
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [CHK_ValidGender]
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [CHK_ValidPIN] CHECK  (([StudentPIN] like '[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]-[0-9][0-9][0-9][0-9]' AND (substring([StudentPIN],(5),(2))>='01' AND substring([StudentPIN],(5),(2))<='12') AND (substring([StudentPIN],(7),(2))>='01' AND substring([StudentPIN],(7),(2))<='31')))
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [CHK_ValidPIN]
GO
/****** Object:  StoredProcedure [dbo].[GetStudentInfo]    Script Date: 2025-01-04 17:13:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetStudentInfo] (@StudentPIN NVARCHAR(13))
AS
BEGIN
    SELECT 
        s.StudentPIN, 
        s.FirstName, 
        s.LastName, 
        c.ClassName, 
        e.FirstName AS TeacherFirstName, 
        e.LastName AS TeacherLastName, 
        ce.Grade, 
        ce.GradeAssignedDate
    FROM Students s
    LEFT JOIN Classes c ON s.FKClassId = c.ClassId
    LEFT JOIN CourseEnrollments ce ON ce.FKStudentPIN = s.StudentPIN
    LEFT JOIN Employees e ON ce.FKTeacherPIN = e.EmployeePIN
    WHERE s.StudentPIN = @StudentPIN;
END
GO
USE [master]
GO
ALTER DATABASE [SchoolDB] SET  READ_WRITE 
GO
