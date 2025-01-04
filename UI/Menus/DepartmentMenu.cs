using School.Core.Interfaces;
using School.Models;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class DepartmentMenu : MenuBase
{
    private readonly IDepartmentService _departmentService;

    // Constructor that initializes the menu with console and department service
    public DepartmentMenu(IAnsiConsole console, IDepartmentService departmentService)
        : base(console)
    {
        _departmentService = departmentService;
    }

    // Main method to display menu and handle user interactions
    public async Task ShowAsync()
    {
        while (true)
        {
            _console.Clear();
            ShowHeader("Department Management");

            // Get user menu choice
            var choice = _console.Prompt(
                new SelectionPrompt<DepartmentMenuOption>()
                    .Title("Select an option:")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<DepartmentMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                switch (choice)
                {
                    case DepartmentMenuOption.ViewTeacherCount:
                        await ShowTeacherCountAsync();
                        break;
                    case DepartmentMenuOption.ViewSalaryStats:
                        await ShowSalaryStatsAsync();
                        break;
                    case DepartmentMenuOption.ViewTotalSalaries:
                        await ShowTotalSalariesAsync();
                        break;
                    case DepartmentMenuOption.Back:
                        return;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message);
            }
        }
    }

    // Display the number of teachers in each department
    private async Task ShowTeacherCountAsync()
    {
        await ShowLoadingSpinnerAsync("Loading teacher statistics...", async () =>
        {
            var teacherStats = await _departmentService.GetTeacherCountByDepartmentAsync();

            if (!teacherStats.Any())
            {
                await ShowErrorAsync("No department statistics available");
                return;
            }

            // Display teacher count in a table
            var table = CreateBaseTable("Department", "Number of Teachers");

            foreach (var stat in teacherStats)
            {
                table.AddRow(
                    stat.DepartmentName,
                    (stat.TeacherCount?.ToString() ?? "0").PadLeft(9)
                );
            }

            _console.Write(table);
            _console.WriteLine();

            // Create and display bar chart
            var chart = new BarChart()
                .Width(60)
                .Label("[blue]Teachers by Department[/]")
                .CenterLabel();

            // Declare an array of colors use in the Teacher by Department bar. Intial index 0 = Blue
            var colors = new[] { Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Red };
            int colorIndex = 0;

            foreach (var teacherData in teacherStats.OrderByDescending(s => s.TeacherCount))
            {
                chart.AddItem(teacherData.DepartmentName,
                    teacherData.TeacherCount ?? 0,   // Accept null value if their is no teachers 
                    colors[colorIndex % colors.Length]);
                colorIndex++; // Index of array of colors  +1  
            }

            _console.Write(chart);
            await PauseAsync();
        });
    }

    // Display salary statistics for each department
    private async Task ShowSalaryStatsAsync()
    {
        await ShowLoadingSpinnerAsync("Loading salary statistics...", async () =>
        {
            var salaryStats = await _departmentService.GetDepartmentSalaryStatsAsync();

            if (!salaryStats.Any())
            {
                await ShowErrorAsync("No salary statistics available");
                return;
            }

            // Display salary statistics in a table
            var table = CreateBaseTable("Department", "Average Salary");

            foreach (var stat in salaryStats)
            {
                table.AddRow(
                    stat.DepartmentName,
                    stat.AvgSalary?.ToString("C2") ?? "N/A" // Format avrage salary to 2 decimal 
                );
            }

            _console.Write(table);
            _console.WriteLine();

            // Create and display bar chart
            var chart = new BarChart()
                .Width(60)
                .Label("[blue]Average Salary by Department[/]")
                .CenterLabel();

            // Declare an array of colors use in the Average Salary by Department. Intial index 0 = Blue
            var colors = new[] { Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Red };
            int colorIndex = 0;

            foreach (var salaryStat in salaryStats.OrderByDescending(s => s.AvgSalary))
            {
                chart.AddItem(
                    salaryStat.DepartmentName,
                    // Round avg. salary by 2 decimal. Accept null value. BarChart just accept double as value though I would like decinal för money 
                    Math.Round((double)(salaryStat.AvgSalary ?? 0), 2), 
                    colors[colorIndex % colors.Length]);
            colorIndex++;   // Index of array of colors  +1  
            }

            _console.Write(chart);
            await PauseAsync();
        });
    }

    // Display total salaries for each department
    private async Task ShowTotalSalariesAsync()
    {
        await ShowLoadingSpinnerAsync("Loading total salaries...", async () =>
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();

            if (!departments.Any())
            {
                await ShowErrorAsync("No departments found");
                return;
            }

            decimal grandTotal = 0;

            // Display total salaries in a table
            var table = CreateBaseTable("Department", "Total Monthly Salary");

            foreach (var dept in departments)
            {
                var totalSalary = await _departmentService.GetDepartmentTotalSalaryAsync(dept.DepartmentId);
                grandTotal += totalSalary;

                table.AddRow(
                    dept.Name,
                    totalSalary.ToString("C2")
                );
            }

            // Add total row at the bottom
            var totalRule = new Rule("[blue]Total[/]").RightJustified();
            var totalSalaryRule = new Rule($"[green]{grandTotal:C}[/]").RightJustified();
            table.AddRow(totalRule, totalSalaryRule);

            _console.Write(table);
            _console.WriteLine();

            // Create and display bar chart
            var chart = new BarChart()
                .Width(60)
                .Label("[blue]Total Salary Distribution[/]")
                .CenterLabel();

            // Declare an array of colors use in the Total Salary Distribution. Intial index 0 = Orange
            var colors = new[] { Color.Orange1, Color.Red, Color.Maroon, Color.Purple, Color.Fuchsia };
            int colorIndex = 0;

            foreach (var dept in departments)
            {
                var totalSalary = await _departmentService.GetDepartmentTotalSalaryAsync(dept.DepartmentId);
                chart.AddItem(
                    dept.Name,
                    (double)totalSalary,
                    colors[colorIndex % colors.Length]);
                colorIndex++; // Index of array of colors  +1  
            }

            _console.Write(chart);
            await PauseAsync();
        });
    }
}