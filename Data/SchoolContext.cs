using System;
using System.IO; 
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using School.Models;

namespace School.Data;

public partial class SchoolContext : DbContext
{
    private readonly string _connectionString;

    public SchoolContext()
    {
        var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<GradeValue> GradeValues { get; set; }

    public virtual DbSet<Salary> Salaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<VwAvgSalaryByDepartment> VwAvgSalaryByDepartments { get; set; }

    public virtual DbSet<VwEmployeeOverview> VwEmployeeOverviews { get; set; }

    public virtual DbSet<VwStudentCourseGrade> VwStudentCourseGrades { get; set; }

    public virtual DbSet<VwTeachersByDepartment> VwTeachersByDepartments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927C058E97CD2");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B85FC9A68").IsUnique();

            entity.Property(e => e.ClassName).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Section)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A739AE3D75");

            entity.HasIndex(e => e.CourseCode, "UQ_CourseCode").IsUnique();

            entity.HasIndex(e => e.CourseCode, "UQ__Courses__FC00E0006E04D591").IsUnique();

            entity.Property(e => e.CourseCode).HasMaxLength(10);
            entity.Property(e => e.CourseName).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__CourseEn__7F68771B6909B574");

            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FkcourseId).HasColumnName("FKCourseId");
            entity.Property(e => e.FkstudentPin)
                .HasMaxLength(13)
                .HasColumnName("FKStudentPIN");
            entity.Property(e => e.FkteacherPin)
                .HasMaxLength(13)
                .HasColumnName("FKTeacherPIN");
            entity.Property(e => e.Grade)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Fkcourse).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.FkcourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Course");

            entity.HasOne(d => d.FkstudentPinNavigation).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.FkstudentPin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Student");

            entity.HasOne(d => d.FkteacherPinNavigation).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.FkteacherPin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Teacher");

            entity.HasOne(d => d.GradeNavigation).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.Grade)
                .HasConstraintName("FK_Enrollment_Grade");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCD87EF5524");

            entity.HasIndex(e => e.Name, "UQ__Departme__737584F680D42168").IsUnique();

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeePin).HasName("PK__Employee__860A38C960DE1EAB");

            entity.HasIndex(e => e.EmployeePin, "UC_PIN").IsUnique();

            entity.Property(e => e.EmployeePin)
                .HasMaxLength(13)
                .HasColumnName("EmployeePIN");
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.FkdepartmentId).HasColumnName("FKDepartmentID");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Fkdepartment).WithMany(p => p.Employees)
                .HasForeignKey(d => d.FkdepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Department");
        });

        modelBuilder.Entity<GradeValue>(entity =>
        {
            entity.HasKey(e => e.Grade).HasName("PK__GradeVal__DF0ADB7B99C526CB");

            entity.HasIndex(e => e.GradeValue1, "UQ_GradeValue").IsUnique();

            entity.Property(e => e.Grade)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GradeValue1).HasColumnName("GradeValue");
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.HasKey(e => e.SalaryId).HasName("PK__Salaries__4BE204B76DAFF8F7");

            entity.Property(e => e.SalaryId).HasColumnName("SalaryID");
            entity.Property(e => e.FkemployeePin)
                .HasMaxLength(13)
                .HasColumnName("FKEmployeePIN");
            entity.Property(e => e.SalaryAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.FkemployeePinNavigation).WithMany(p => p.Salaries)
                .HasForeignKey(d => d.FkemployeePin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Salary_Employee");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentPin).HasName("PK__Students__B9153FD688E22A8C");

            entity.HasIndex(e => e.StudentPin, "UC_Student_PIN").IsUnique();

            entity.Property(e => e.StudentPin)
                .HasMaxLength(13)
                .HasColumnName("StudentPIN");
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.FkclassId).HasColumnName("FKClassId");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);

            entity.HasOne(d => d.Fkclass).WithMany(p => p.Students)
                .HasForeignKey(d => d.FkclassId)
                .HasConstraintName("FK_Student_Class");
        });

        modelBuilder.Entity<VwAvgSalaryByDepartment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AvgSalaryByDepartment");

            entity.Property(e => e.AvgSalary).HasColumnType("decimal(38, 6)");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
        });

        modelBuilder.Entity<VwEmployeeOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_EmployeeOverview");

            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.EmployeePin)
                .HasMaxLength(13)
                .HasColumnName("EmployeePIN");
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwStudentCourseGrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StudentCourseGrades");

            entity.Property(e => e.CourseCode).HasMaxLength(10);
            entity.Property(e => e.CourseName).HasMaxLength(250);
            entity.Property(e => e.Grade)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.StudentFirstName).HasMaxLength(200);
            entity.Property(e => e.StudentLastName).HasMaxLength(100);
            entity.Property(e => e.StudentPin)
                .HasMaxLength(13)
                .HasColumnName("StudentPIN");
            entity.Property(e => e.TeacherFirstName).HasMaxLength(200);
            entity.Property(e => e.TeacherLastName).HasMaxLength(100);
        });

        modelBuilder.Entity<VwTeachersByDepartment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TeachersByDepartment");

            entity.Property(e => e.DepartmentName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
