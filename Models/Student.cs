namespace School.Models;

public partial class Student
{
    public string StudentPin { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int? FkclassId { get; set; }

    public bool IsActive { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual Class? Fkclass { get; set; }
}