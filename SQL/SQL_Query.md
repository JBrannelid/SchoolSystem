/*
SchoolDB - School Administration Database
Created: 2024-12-16
Author: Johannes Brannelid

Highschool: Chas Academy
Lab: Individuellt databasprojekt 
Course: Databases
*/

CREATE DATABASE SchoolDB
GO

USE SchoolDB
GO

-- Departments table used for tracing employee, calculating department-salary, statistic 
CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
)
GO

-- Classes maintain infomration about school classes and organizing and track students
CREATE TABLE Classes (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(10) UNIQUE NOT NULL,
    Year INT NOT NULL,
    Section CHAR(1) NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1,						-- Default equals true/is active 

    CONSTRAINT CHK_ValidYear CHECK (Year BETWEEN 1 AND 3),
    CONSTRAINT CHK_ValidSection CHECK (Section IN ('A', 'B', 'C'))
)
GO

-- Employees table with constraints for a safer datastorage. 
CREATE TABLE Employees (
    EmployeePIN  NVARCHAR(13) PRIMARY KEY,
    FirstName NVARCHAR(200) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    StartDate DATE NOT NULL DEFAULT GETDATE(), -- Used to calculate years of service
    Position VARCHAR(20) NOT NULL,
    FKDepartmentID INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Employee_Department FOREIGN KEY (FKDepartmentID) 
        REFERENCES Departments(DepartmentID),
	CONSTRAINT UC_PIN UNIQUE (EmployeePIN),											-- Every PIN has to be Unique 
    CONSTRAINT CHK_Employee_Gender CHECK (Gender IN ('Male', 'Female', 'Other')),-- Constraint for Gender
    CONSTRAINT CHK_Employee_Position CHECK (Position IN ('Principal', 'Administrator', 'Teacher')), -- As for now a Employees can have three positions 
    CONSTRAINT CHK_Employee_PIN CHECK (											-- Strict constraint for a swedish social sequrity number
        EmployeePIN LIKE '[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]-[0-9][0-9][0-9][0-9]'
        AND SUBSTRING(EmployeePIN, 5, 2) BETWEEN '01' AND '12'
        AND SUBSTRING(EmployeePIN, 7, 2) BETWEEN '01' AND '31'
    )
)
GO

-- Calculate and manage Employees salaries
CREATE TABLE Salaries (
    SalaryID INT PRIMARY KEY IDENTITY(1,1),
    FKEmployeePIN NVARCHAR(13) NOT NULL,
    SalaryAmount DECIMAL(10, 2) NOT NULL,

    CONSTRAINT FK_Salary_Employee FOREIGN KEY (FKEmployeePIN) 
        REFERENCES Employees(EmployeePIN),
    CONSTRAINT CHK_Salary_Amount CHECK (SalaryAmount > 0)
)
GO

-- Create People table with constraints for a safer datastorage 
CREATE TABLE Students (
    StudentPIN NVARCHAR(13) PRIMARY KEY,			-- Personal Information Number (PIN)
    FirstName NVARCHAR(200) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    FKClassId INT NULL,								-- Only for students
	IsActive BIT NOT NULL DEFAULT 1,
	EnrollmentDate DATE NOT NULL DEFAULT GETDATE(),

	CONSTRAINT FK_Student_Class FOREIGN KEY (FKClassId) 
		REFERENCES Classes(ClassId),
    CONSTRAINT UC_Student_PIN UNIQUE (StudentPIN),									-- Every PIN has to be Unique 
    CONSTRAINT CHK_ValidGender CHECK (Gender IN ('Male', 'Female', 'Other')),		-- Constraint for Gender

    CONSTRAINT CHK_ValidPIN CHECK (													-- Validate PIN	
		-- Extract PIN-format (YYYYMMDD-XXXX)
		StudentPIN LIKE '[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]-[0-9][0-9][0-9][0-9]'
		-- Check if month is between 01-12
        AND SUBSTRING(StudentPIN, 5, 2) BETWEEN '01' AND '12'
		-- Check if days is between 01-21
		AND SUBSTRING(StudentPIN, 7, 2) BETWEEN '01' AND '31'
	)
)
GO

-- Store and track active and inactive courses
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(250) NOT NULL,
    CourseCode NVARCHAR(10) NOT NULL UNIQUE,
	IsActive BIT NOT NULL DEFAULT 1,

	CONSTRAINT UQ_CourseCode UNIQUE NONCLUSTERED (CourseCode)
)
GO

-- Define valid grades and their numerical values
CREATE TABLE GradeValues (
    Grade CHAR(2) PRIMARY KEY,
    GradeValue INT NOT NULL,

    CONSTRAINT UQ_GradeValue UNIQUE (GradeValue) 
)
GO

-- Track studens enrollments and grade and connecting students, courses and teachers
CREATE TABLE CourseEnrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    FKStudentPIN NVARCHAR(13) NOT NULL,
    FKCourseId INT NOT NULL,
    FKTeacherPIN NVARCHAR(13) NOT NULL,
    Grade CHAR(2) NULL,
    GradeAssignedDate DATETIME2 NULL,
    EnrollmentDate DATE NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,

	-- Set different CONSTRAINT that will validate condition everytime add or update to the table arrise 
    CONSTRAINT FK_Enrollment_Student FOREIGN KEY (FKStudentPIN) 
        REFERENCES Students(StudentPIN),
    CONSTRAINT FK_Enrollment_Course FOREIGN KEY (FKCourseId) 
        REFERENCES Courses(CourseId),
    CONSTRAINT FK_Enrollment_Teacher FOREIGN KEY (FKTeacherPIN) 
        REFERENCES Employees(EmployeePIN),
    CONSTRAINT FK_Enrollment_Grade FOREIGN KEY (Grade) 
        REFERENCES GradeValues(Grade),

   CONSTRAINT CHK_GradeAndDate CHECK (
        (Grade IS NULL AND GradeAssignedDate IS NULL) OR
        (Grade IS NOT NULL AND GradeAssignedDate IS NOT NULL)
    )
)
GO


-- Insert Classes
INSERT INTO Classes (ClassName, Year, Section)
VALUES 
('1A', 1, 'A'),
('1B', 1, 'B'),
('1C', 1, 'C'),
('2A', 2, 'A'),
('2B', 2, 'B'),
('2C', 2, 'C'),
('3A', 3, 'A'),
('3B', 3, 'B'),
('3C', 3, 'C')
GO

-- Insert grade values
INSERT INTO GradeValues (Grade, GradeValue)
VALUES 
('A', 5),
('B', 4),
('C', 3),
('D', 2),
('E', 1),
('F', 0)
GO

-- Insert Departments
INSERT INTO Departments (Name)
VALUES 
('Faculty of Science'), -- Handles Biology, Chemistry, Physics, Mathematics, sociology
('Faculty of Humanities/sports'), -- Handles History, Languages, Literature, sports
('Administration') -- Handles the administrative functions of the institution
GO

-- Insert test data for Employees
INSERT INTO Employees (EmployeePIN, FirstName, LastName, Gender, StartDate, Position, FKDepartmentID, IsActive)
VALUES 
('19820101-1234', 'Anna', 'Johansson', 'Female', '2015-08-15', 'Teacher', 1, 1),
('19850505-2345', 'Björn', 'Andersson', 'Male', '2012-03-25', 'Teacher', 1, 1),
('19791210-3456', 'Carla', 'Nilsson', 'Female', '2018-11-01', 'Principal', 3, 1),
('19801215-4567', 'David', 'Svensson', 'Male', '2010-01-01', 'Administrator', 3, 1),
('19890220-5678', 'Elin', 'Karlsson', 'Female', '2019-09-12', 'Teacher', 2, 1),
('19830310-6789', 'Fredrik', 'Larsson', 'Male', '2013-04-10', 'Teacher', 2, 1),
('19840712-7890', 'Greta', 'Olofsson', 'Female', '2017-07-23', 'Teacher', 1, 1),
('19900505-8901', 'Håkan', 'Berg', 'Male', '2020-06-15', 'Teacher', 2, 1)
GO

-- Sett fix salaries 
INSERT INTO Salaries (FKEmployeePIN, SalaryAmount)
VALUES
('19820101-1234', 33410.00),  -- Anna Johansson (Teacher)
('19850505-2345', 35520.00),  -- Björn Andersson (Teacher)
('19791210-3456', 55000.00),  -- Carla Nilsson (Principal)
('19801215-4567', 42200.00),  -- David Svensson (Administrator)
('19890220-5678', 36100.00),  -- Elin Karlsson (Teacher)
('19830310-6789', 37340.00),  -- Fredrik Larsson (Teacher)
('19840712-7890', 30000.00),  -- Greta Olofsson (Teacher)
('19900505-8901', 31400.00)   -- Håkan Berg (Teacher)
GO

-- Insert test data for students
INSERT INTO Students (StudentPIN, FirstName, LastName, Gender, FKClassId, IsActive, EnrollmentDate)
VALUES 
('20001010-1234', 'Eva', 'Lindström', 'Female', 1, 1, '2021-09-01'),
('20010202-2345', 'Johan', 'Eriksson', 'Male', 2, 1, '2021-09-01'),
('20030803-3456', 'Lina', 'Johansson', 'Female', 3, 1, '2022-09-01'),
('20040604-4567', 'Mats', 'Karlsson', 'Male', 1, 1, '2022-09-01'),
('20051105-5678', 'Sara', 'Olsson', 'Female', 2, 1, '2021-09-01'),
('20060906-6789', 'Peter', 'Svensson', 'Male', 3, 1, '2021-09-01')
GO

-- Inset test data for Courses
INSERT INTO Courses (CourseName, CourseCode, IsActive)
VALUES 
('Matematik', 'MA101', 1),			-- Mathematics
('Fysik', 'FY101',1),				-- Physics
('Kemi', 'KE101',1),				-- Chemistry
('Biologi', 'BI101',1),				-- Biology
('Svenska', 'SV101',1),				-- Swedish
('Engelska', 'EN101',1),			-- English
('Historia', 'HI101',1),			-- History
('Geografi', 'GE101',1),			-- Geography
('Idrott och hälsa', 'IH101',1);	-- Physical Education
GO

-- Inset test data for CourseEnrollments
INSERT INTO CourseEnrollments (FKStudentPIN, FKCourseId, FKTeacherPIN, Grade, GradeAssignedDate, IsActive)
VALUES 
('20001010-1234', 1, '19820101-1234', 'A', '2023-06-10', 1),
('20010202-2345', 2, '19850505-2345', 'B', '2023-06-15', 1),
('20030803-3456', 3, '19890220-5678', 'C', '2023-06-20', 1),
('20040604-4567', 4, '19830310-6789', 'A', '2023-06-12', 1),
('20051105-5678', 5, '19900505-8901', 'B', '2023-06-14', 1),
('20060906-6789', 1, '19840712-7890', 'E', '2023-06-18', 1)
GO

-- Procedure --

-- This procedure collect all relevent information about a student
CREATE PROCEDURE GetStudentInfo (@StudentPIN NVARCHAR(13))
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

-- Views --

-- Show all employees (Name, Position, yearsWorked)
CREATE VIEW vw_EmployeeOverview AS
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

-- Show students, courses, teachers and grades 
CREATE VIEW vw_StudentCourseGrades AS
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

-- Show teachers on active department 
CREATE VIEW vw_TeachersByDepartment AS
SELECT 
    d.Name AS DepartmentName,
    COUNT(e.EmployeePIN) AS TeacherCount
FROM Employees e
JOIN Departments d ON e.FKDepartmentID = d.DepartmentID
WHERE e.Position = 'Teacher' AND e.IsActive = 1
GROUP BY d.Name
GO

-- Show average salary for all employees on every department
CREATE VIEW vw_AvgSalaryByDepartment AS
SELECT 
    d.Name AS DepartmentName,
    AVG(s.SalaryAmount) AS AvgSalary
FROM Employees e
JOIN Departments d ON e.FKDepartmentID = d.DepartmentID
JOIN Salaries s ON s.FKEmployeePIN = e.EmployeePIN
WHERE e.IsActive = 1
GROUP BY d.Name
GO