using School.Core.Interfaces;
using School.Models;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class CourseMenu : MenuBase
{
    private readonly ICourseService _courseService;

    // Constructor that initializes the menu with console and course service
    public CourseMenu(IAnsiConsole console, ICourseService courseService)
        : base(console)
    {
        _courseService = courseService;
    }

    // Main method to display menu and handle user interactions
    public async Task ShowAsync()
    {
        while (true)
        {
            ShowHeader("Course Management");

            // Get user menu choice
            var choice = _console.Prompt(
                new SelectionPrompt<CourseMenuOption>()
                    .Title("Select an option:")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<CourseMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                switch (choice)
                {
                    case CourseMenuOption.ViewAll:
                        await ShowAllCoursesAsync();
                        break;
                    case CourseMenuOption.ViewActive:
                        await ShowActiveCoursesAsync();
                        break;
                    case CourseMenuOption.AddNew:
                        await AddNewCourseAsync();
                        break;
                    case CourseMenuOption.Update:
                        await UpdateCourseAsync();
                        break;
                    case CourseMenuOption.Remove:
                        await RemoveCourseAsync();
                        break;
                    case CourseMenuOption.ViewGrades:
                        await ViewGradesAsync();
                        break;
                    case CourseMenuOption.Back:
                        return;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message);
            }
        }
    }

    // Display all courses, including inactive ones
    private async Task ShowAllCoursesAsync()
    {
        await ShowLoadingSpinnerAsync("Loading all courses...", async () =>
        {
            var courses = await _courseService.GetAllCoursesAsync();
            await DisplayCoursesAsync(courses);
        });
    }

    // Display only active courses
    private async Task ShowActiveCoursesAsync()
    {
        await ShowLoadingSpinnerAsync("Loading active courses...", async () =>
        {
            var courses = await _courseService.GetActiveCourses();
            await DisplayCoursesAsync(courses);
        });
    }

    // Display courses in a table format with enrollment statistics
    private async Task DisplayCoursesAsync(IEnumerable<Course> courses)
    {
        // Visa tabellen med kursinformation
        var table = CreateBaseTable("ID", "Course Name", "Code", "Students", "Status");

        foreach (var course in courses)
        {
            string status = course.IsActive ? "Active" : "Inactive";
            string statusColor = course.IsActive ? "green" : "red";

            table.AddRow(
                course.CourseId.ToString(),
                course.CourseName,
                course.CourseCode,
                course.CourseEnrollments.Count.ToString(),
                $"[{statusColor}]{status}[/]"
            );
        }

        _console.Write(table);
        await ShowEnrollmentStatisticsAsync(courses);
        await PauseAsync();
    }

    // Display enrollment statistics as a bar chart
    private async Task ShowEnrollmentStatisticsAsync(IEnumerable<Course> courses)
    {
        var stats = new BarChart()
            .Width(60)
            .Label("[blue]Course Enrollment Statistics[/]")
            .CenterLabel();

        // Create an array of colors 
        var colors = new[] { Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Red };
        int colorIndex = 0;

        foreach (var course in courses)
        {
            stats.AddItem(
                course.CourseCode,
                course.CourseEnrollments.Count,
                colors[colorIndex % colors.Length]
            );
            colorIndex++;
        }

        _console.WriteLine();
        _console.Write(stats);
    }

    // Add a new course to the system
    private async Task AddNewCourseAsync()
    {
        ShowHeader("Add New Course");

        var name = _console.Prompt(
            new TextPrompt<string>("[blue]Enter course name:[/]")
                .Validate(name => !string.IsNullOrWhiteSpace(name)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Course name cannot be empty[/]")));

        var code = _console.Prompt(
            new TextPrompt<string>("[blue]Enter course code:[/]")
                .Validate(code =>
                {
                    if (string.IsNullOrWhiteSpace(code))
                        return ValidationResult.Error("[red]Course code cannot be empty[/]");
                    if (code.Length > 10)
                        return ValidationResult.Error("[red]Course code cannot be longer than 10 characters[/]");
                    return ValidationResult.Success();
                }));

        var course = new Course
        {
            CourseName = name,
            CourseCode = code,
            IsActive = true
        };

        await ShowLoadingSpinnerAsync("Adding new course...", async () =>
        {
            var success = await _courseService.AddCourseAsync(course);
            if (success)
                await ShowConfirmationAsync("Course added successfully!");
            else
                await ShowErrorAsync("Failed to add course. Please check if the course code is unique.");
        });
    }

    // Update an existing course's information
    private async Task UpdateCourseAsync()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        if (!courses.Any())
        {
            await ShowErrorAsync("No courses found");
            return;
        }

        var selectedCourse = _console.Prompt(
            new SelectionPrompt<Course>()
                .Title("[blue]Select course to update:[/]")
                .PageSize(10)
                .AddChoices(courses)
                .UseConverter(c => $"{c.CourseCode} - {c.CourseName}"));

        var newName = _console.Prompt(
            new TextPrompt<string>($"[blue]Enter new name (current: {selectedCourse.CourseName}):[/]")
                .AllowEmpty());

        if (!string.IsNullOrEmpty(newName))
            selectedCourse.CourseName = newName;

        var newCode = _console.Prompt(
            new TextPrompt<string>($"[blue]Enter new code (current: {selectedCourse.CourseCode}):[/]")
                .AllowEmpty());

        if (!string.IsNullOrEmpty(newCode))
            selectedCourse.CourseCode = newCode;

        var statusChoice = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Update course status:[/]")
                .AddChoices(new[] { "Keep Current", "Set Active", "Set Inactive" }));

        switch (statusChoice)
        {
            case "Set Active":
                selectedCourse.IsActive = true;
                break;
            case "Set Inactive":
                selectedCourse.IsActive = false;
                break;
        }

        await ShowLoadingSpinnerAsync("Updating course...", async () =>
        {
            var success = await _courseService.UpdateCourseAsync(selectedCourse);
            if (success)
                await ShowConfirmationAsync("Course updated successfully!");
            else
                await ShowErrorAsync("Failed to update course");
        });
    }

    // Remove a course (soft delete)
    private async Task RemoveCourseAsync()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        if (!courses.Any())
        {
            await ShowErrorAsync("No courses found");
            return;
        }

        var selectedCourse = _console.Prompt(
            new SelectionPrompt<Course>()
                .Title("[blue]Select course to remove:[/]")
                .PageSize(10)
                .AddChoices(courses)
                .UseConverter(c => $"{c.CourseCode} - {c.CourseName}"));

        if (await ConfirmActionAsync($"remove course {selectedCourse.CourseName}"))
        {
            await ShowLoadingSpinnerAsync("Removing course...", async () =>
            {
                var success = await _courseService.RemoveCourseAsync(selectedCourse.CourseId);
                if (success)
                    await ShowConfirmationAsync("Course removed successfully!");
                else
                    await ShowErrorAsync("Failed to remove course");
            });
        }
    }

    // View grades for a specific student
    private async Task ViewGradesAsync()
    {
        var pin = _console.Prompt(
            new TextPrompt<string>("[blue]Enter student PIN (YYYYMMDD-XXXX):[/]")
                .Validate(pin =>
                {
                    if (!(pin.Length == 13 && pin[8] == '-'))
                        return ValidationResult.Error("[red]Invalid PIN format[/]");
                    return ValidationResult.Success();
                }));

        await ShowLoadingSpinnerAsync("Loading grades...", async () =>
        {
            var grades = await _courseService.GetStudentGradesAsync(pin);

            if (!grades.Any())
            {
                await ShowErrorAsync("No grades found for this student");
                return;
            }

            var table = CreateBaseTable("Course", "Code", "Grade", "Graded Date");

            foreach (var grade in grades)
            {
                var gradeColor = GetGradeColor(grade.Grade);
                table.AddRow(
                    grade.CourseName,
                    grade.CourseCode,
                    grade.Grade ?? "N/A",
                    grade.GradeAssignedDate?.ToString("yyyy-MM-dd") ?? "N/A"
                );
            }

            _console.Clear();
            _console.Write(table);
            await PauseAsync();
        });
    }

    // Helper method to determine the color for displaying grades
    private string GetGradeColor(string? grade)
    {
        if (grade == null) return "grey";

        return grade.Trim() switch
        {
            "A" => "green",
            "B" => "blue",
            "C" => "yellow",
            "D" => "orange",
            "F" => "red",
            _ => "white"
        };
    }
}