namespace School.Models;

public partial class CourseEnrollment
{
    public int EnrollmentId { get; set; }

    public string FkstudentPin { get; set; } = null!;

    public int FkcourseId { get; set; }

    public string FkteacherPin { get; set; } = null!;

    public string? Grade { get; set; }

    public DateTime? GradeAssignedDate { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public bool IsActive { get; set; }

    public virtual Course Fkcourse { get; set; } = null!;

    public virtual Student FkstudentPinNavigation { get; set; } = null!;

    public virtual Employee FkteacherPinNavigation { get; set; } = null!;

    public virtual GradeValue? GradeNavigation { get; set; }
}