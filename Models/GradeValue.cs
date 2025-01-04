namespace School.Models;

public partial class GradeValue
{
    public string Grade { get; set; } = null!;

    public int GradeValue1 { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();
}