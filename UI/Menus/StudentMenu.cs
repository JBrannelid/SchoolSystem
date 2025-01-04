using School.Core.Interfaces;
using School.Models;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class StudentMenu : MenuBase
{
    private readonly IStudentService _studentService;

    // Constructor that initializes the menu with console and student service
    public StudentMenu(IAnsiConsole console, IStudentService studentService)
        : base(console)
    {
        _studentService = studentService;
    }

    // Main method to display menu and handle user interactions
    public async Task ShowAsync()
    {
        while (true)
        {
            ShowHeader("Student Management");

            // Get user menu choice
            var choice = _console.Prompt(
                new SelectionPrompt<StudentMenuOption>()
                    .Title("Select an option:")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<StudentMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                switch (choice)
                {
                    case StudentMenuOption.ViewAll:
                        await ShowAllStudentsAsync();
                        break;
                    case StudentMenuOption.ViewByClass:
                        await ShowStudentsByClassAsync();
                        break;
                    case StudentMenuOption.Search:
                        await SearchStudentsAsync();
                        break;
                    case StudentMenuOption.AddNew:
                        await AddNewStudentAsync();
                        break;
                    case StudentMenuOption.Update:
                        await UpdateStudentAsync();
                        break;
                    case StudentMenuOption.Remove:
                        await RemoveStudentAsync();
                        break;
                    case StudentMenuOption.Filter:
                        await ShowFilterOptionsAsync();
                        break;
                    case StudentMenuOption.Back:
                        return;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message);
            }
        }
    }

    // Display all students in the system
    private async Task ShowAllStudentsAsync()
    {
        await ShowLoadingSpinnerAsync("Loading students...", async () =>
        {
            var students = await _studentService.GetAllStudentsAsync();
            await DisplayStudentsAsync(students);
        });
    }

    // Display students from a selected class
    private async Task ShowStudentsByClassAsync()
    {
        var classes = await _studentService.GetClassesAsync();
        if (!classes.Any())
        {
            await ShowErrorAsync("No classes found in the system");
            return;
        }

        // Let user select a class
        var selectedClass = _console.Prompt(
            new SelectionPrompt<Class>()
                .Title("[blue]Select Class:[/]")
                .PageSize(10)
                .AddChoices(classes)
                .UseConverter(c => $"{c.ClassName} ({c.Year})"));

        await ShowLoadingSpinnerAsync(
            $"Loading students from {selectedClass.ClassName}...",
            async () =>
            {
                var students = await _studentService.GetStudentsByClassAsync(selectedClass.ClassId);
                if (!students.Any())
                {
                    await ShowErrorAsync($"No students found in {selectedClass.ClassName}");
                    return;
                }
                await DisplayStudentsAsync(students);
            });
    }

    // Search for students by name or PIN
    private async Task SearchStudentsAsync()
    {
        var searchTerm = _console.Prompt(
            new TextPrompt<string>("[blue]Enter search term (name or PIN):[/]")
                .Validate(term => !string.IsNullOrWhiteSpace(term)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Search term cannot be empty")));

        await ShowLoadingSpinnerAsync($"Searching for '{searchTerm}'...", async () =>
        {
            var students = await _studentService.SearchStudentsAsync(searchTerm);
            if (!students.Any())
            {
                await ShowErrorAsync("No students found matching the search term");
                return;
            }
            await DisplayStudentsAsync(students);
        });
    }

    // Show filter options for students
    private async Task ShowFilterOptionsAsync()
    {
        var filterChoice = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select filter type:[/]")
                .AddChoices(new[] { "By Gender", "By Enrollment Year", "Back" }));

        switch (filterChoice)
        {
            case "By Gender":
                await FilterByGenderAsync();
                break;
            case "By Enrollment Year":
                await FilterByEnrollmentYearAsync();
                break;
        }
    }

    // Filter students by gender
    private async Task FilterByGenderAsync()
    {
        var gender = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select gender:[/]")
                .AddChoices(new[] { "Male", "Female", "Other" }));

        await ShowLoadingSpinnerAsync($"Loading {gender} students...", async () =>
        {
            var students = await _studentService.GetStudentsByGenderAsync(gender);
            await DisplayStudentsAsync(students);
        });
    }

    // Filter students by enrollment year
    private async Task FilterByEnrollmentYearAsync()
    {
        var year = _console.Prompt(
            new TextPrompt<int>("[blue]Enter enrollment year (YYYY):[/]")
                .Validate(year => year >= 2000 && year <= DateTime.Now.Year
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"Please enter a valid year between 2000 and {DateTime.Now.Year}")));

        await ShowLoadingSpinnerAsync($"Loading students enrolled in {year}...", async () =>
        {
            var students = await _studentService.GetStudentsByEnrollmentYearAsync(year);
            await DisplayStudentsAsync(students);
        });
    }

    // Add a new student to the system
    private async Task AddNewStudentAsync()
    {
        ShowHeader("Add New Student");

        var pin = await GetValidPinAsync();
        var firstName = GetValidName("First Name");
        var lastName = GetValidName("Last Name");

        var gender = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select gender:[/]")
                .AddChoices(new[] { "Male", "Female", "Other" }));

        // Handle class assignment
        Class? selectedClass = await GetClassSelectionAsync();

        var student = new Student
        {
            StudentPin = pin,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            FkclassId = selectedClass?.ClassId,
            IsActive = true,
            EnrollmentDate = DateOnly.FromDateTime(DateTime.Now)
        };

        await ShowLoadingSpinnerAsync("Adding new student...", async () =>
        {
            var success = await _studentService.AddStudentAsync(student);
            if (success)
                await ShowConfirmationAsync("Student added successfully!");
            else
                await ShowErrorAsync("Failed to add student. Please check the input and try again.");
        });
    }

    // Update an existing student's information
    private async Task UpdateStudentAsync()
    {
        var pin = GetValidPin();
        var student = await _studentService.GetStudentByPinAsync(pin);

        if (student == null)
        {
            await ShowErrorAsync("Student not found");
            return;
        }

        DisplayCurrentStudentInfo(student);

        // Get updated information
        UpdateStudentInfo(student);

        // Handle class update
        await UpdateStudentClass(student);

        await ShowLoadingSpinnerAsync("Updating student information...", async () =>
        {
            var success = await _studentService.UpdateStudentAsync(student);
            if (success)
                await ShowConfirmationAsync("Student updated successfully!");
            else
                await ShowErrorAsync("Failed to update student");
        });
    }

    // Remove a student from the system
    private async Task RemoveStudentAsync()
    {
        var pin = GetValidPin();
        var student = await _studentService.GetStudentByPinAsync(pin);

        if (student == null)
        {
            await ShowErrorAsync("Student not found");
            return;
        }

        if (await ConfirmActionAsync($"remove student {student.FirstName} {student.LastName}"))
        {
            await ShowLoadingSpinnerAsync("Removing student...", async () =>
            {
                var success = await _studentService.RemoveStudentAsync(pin);
                if (success)
                    await ShowConfirmationAsync("Student removed successfully!");
                else
                    await ShowErrorAsync("Failed to remove student");
            });
        }
    }

    // Display students in a formatted table
    private async Task DisplayStudentsAsync(IEnumerable<Student> students)
    {
        var table = CreateBaseTable(
            "PIN",
            "Name",
            "Gender",
            "Class",
            "Enrollment Date",
            "Status"
        );

        foreach (var student in students)
        {
            string statusColor = student.IsActive ? "green" : "red";
            string status = student.IsActive ? "Active" : "Inactive";

            table.AddRow(
                student.StudentPin,
                $"{student.FirstName} {student.LastName}",
                student.Gender,
                student.Fkclass?.ClassName ?? "Not Assigned",
                student.EnrollmentDate.ToString("yyyy-MM-dd"),
                $"[{statusColor}]{status}[/]"
            );
        }

        _console.Write(table);
        await DisplayGenderDistributionAsync(students);
        await PauseAsync();
    }

    // Display gender distribution chart
    private async Task DisplayGenderDistributionAsync(IEnumerable<Student> students)
    {
        var stats = new BarChart()
            .Width(60)
            .Label("[blue]Students by Gender[/]")
            .CenterLabel()
            .AddItem("Male", students.Count(s => s.Gender == "Male"), Color.Blue)
            .AddItem("Female", students.Count(s => s.Gender == "Female"), Color.Green)
            .AddItem("Other", students.Count(s => s.Gender == "Other"), Color.Yellow);

        _console.WriteLine();
        _console.Write(stats);
    }

    // Helper method to get valid PIN
    private string GetValidPin()
    {
        return _console.Prompt(
            new TextPrompt<string>("[blue]Enter student PIN (YYYYMMDD-XXXX):[/]")
                .Validate(pin => pin.Length == 13 && pin[8] == '-'
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Invalid PIN format[/]")));
    }

    // Helper method to get valid PIN with additional validation
    private async Task<string> GetValidPinAsync()
    {
        while (true)
        {
            var pin = GetValidPin();
            if (await _studentService.ValidateStudentPinAsync(pin))
                return pin;
            _console.MarkupLine("[red]PIN is invalid or already exists[/]");
        }
    }

    // Helper method to get valid name input
    private string GetValidName(string fieldName)
    {
        return _console.Prompt(
            new TextPrompt<string>($"[blue]Enter {fieldName}:[/]")
                .Validate(name => !string.IsNullOrWhiteSpace(name)
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"[red]{fieldName} cannot be empty[/]")));
    }

    // Helper method to get class selection
    private async Task<Class?> GetClassSelectionAsync()
    {
        var classes = await _studentService.GetClassesAsync();
        if (!classes.Any())
            return null;

        var assignClass = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Would you like to assign a class?[/]")
                .AddChoices(new[] { "Yes", "No" }));

        if (assignClass == "Yes")
        {
            return _console.Prompt(
                new SelectionPrompt<Class>()
                    .Title("[blue]Select class:[/]")
                    .AddChoices(classes)
                    .UseConverter(c => $"{c.ClassName} ({c.Year})"));
        }

        return null;
    }

    // Helper method to display current student information
    private void DisplayCurrentStudentInfo(Student student)
    {
        _console.WriteLine();
        _console.MarkupLine("[blue]Current Information:[/]");
        _console.WriteLine($"Name: {student.FirstName} {student.LastName}");
        _console.WriteLine($"Gender: {student.Gender}");
        _console.WriteLine($"Class: {student.Fkclass?.ClassName ?? "Not Assigned"}");
    }

    // Helper method to update student information
    private void UpdateStudentInfo(Student student)
    {
        var firstName = _console.Prompt(
            new TextPrompt<string>("[blue]Enter new first name (or press Enter to keep current):[/]")
                .AllowEmpty());

        if (!string.IsNullOrEmpty(firstName))
            student.FirstName = firstName;

        var lastName = _console.Prompt(
            new TextPrompt<string>("[blue]Enter new last name (or press Enter to keep current):[/]")
                .AllowEmpty());

        if (!string.IsNullOrEmpty(lastName))
            student.LastName = lastName;

        var newGender = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select new gender (current: " + student.Gender + "):[/]")
                .AddChoices(new[] { "Keep Current", "Male", "Female", "Other" }));

        if (newGender != "Keep Current")
            student.Gender = newGender;
    }

    // Helper method to update student class
    private async Task UpdateStudentClass(Student student)
    {
        var classes = await _studentService.GetClassesAsync();
        if (!classes.Any())
            return;

        var updateClass = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Would you like to update the class assignment?[/]")
                .AddChoices(new[] { "Yes", "No" }));

        if (updateClass == "Yes")
        {
            var selectedClass = _console.Prompt(
                new SelectionPrompt<Class>()
                    .Title("[blue]Select new class:[/]")
                    .AddChoices(classes)
                    .UseConverter(c => $"{c.ClassName} ({c.Year})"));

            student.FkclassId = selectedClass.ClassId;
        }
    }
}