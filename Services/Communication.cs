namespace DatabaseChallenge.Services;

using DatabaseChallenge.Interfaces;

using Spectre.Console;

public static class Communication
{
    public static void PageMedicalStaff()
    {
        if (DatabaseFunctions.EmployeesLoaded)
        {
            foreach (var employee in DatabaseFunctions.Employees)
            {
                if (employee is IPage pageable)
                    pageable.Page();
            }
            Console.WriteLine();
        }
        else
        {
            DatabaseFunctions.DatabaseNotLoaded();
            Navigation.ClearScreen();
        }
    }
}