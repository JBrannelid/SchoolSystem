using Microsoft.Extensions.DependencyInjection;
using School.Core.Interfaces;
using School.Core.Services;
using School.Data;
using School.UI.Menus;
using Spectre.Console;

namespace School
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                // Create a connection to the database with EF.
                var dbContext = new SchoolContext();

                // Check database connection 
                if (!await dbContext.Database.CanConnectAsync())
                {
                    throw new Exception("Could not connect to the database. Please check your connection.");
                }

                // Create instense of the services. Each service handles specific logic. Pass dbContext as a argument 
                var studentService = new StudentService(dbContext);
                var employeeService = new EmployeeService(dbContext);
                var courseService = new CourseService(dbContext);
                var departmentService = new DepartmentService(dbContext);
                var gradeService = new GradeService(dbContext);

                // Create the console interface using Spectre.Console. Used for tabels and bars with colors 
                var console = AnsiConsole.Create(new AnsiConsoleSettings());

                // Create the main menu by passing all necessary services
                var mainMenu = new MainMenu(
                    console,
                    studentService,
                    employeeService,
                    courseService,
                    departmentService,
                    gradeService);

                // Display the main menu and wait for user interaction
                // The await keyword is used to Render Main Menu with an asynchronous method
                await mainMenu.RenderMainMenuAsync();
            }
            // Catch any exceptions in case of any error setting up the DI and instances 
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAn error occurred.");
                Console.WriteLine(ex.Message);

                Console.ResetColor(); 
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}