namespace School.Models;

public partial class VwStudentCourseGrade
{
    public string StudentPin { get; set; } = null!;

    public string StudentFirstName { get; set; } = null!;

    public string StudentLastName { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public string TeacherFirstName { get; set; } = null!;

    public string TeacherLastName { get; set; } = null!;

    public string? Grade { get; set; }

    public DateTime? GradeAssignedDate { get; set; }
}