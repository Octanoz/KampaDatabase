namespace DatabaseChallenge.Services;

using Spectre.Console;
using DatabaseChallenge.Interfaces;

public static class Communication
{
    public static void PageMedicalStaff()
    {
        if (!DatabaseFunctions.EmployeesLoaded)
        {
            DatabaseFunctions.DatabaseNotLoaded();
            Navigation.ClearScreen();
            return;
        }

        List<string> pageables = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Staff to page:")
                .AddChoiceGroup("Doctors", DatabaseFunctions.GetDoctors().Select(d => $"{d.EmployeeID} - {d.JobTitle} {d.LastName}"))
                .AddChoiceGroup("Nurses", DatabaseFunctions.GetNurses().Select(n => $"{n.EmployeeID} - {n.JobTitle} {n.LastName}"))
            );
        
        foreach (var employeeString in pageables)
        {
            int id = int.Parse(employeeString[..3]);
            IPage pageableEmployee = (IPage)DatabaseFunctions.Employees.First(e => e.EmployeeID == id);
            pageableEmployee.Page();
        }
        Console.WriteLine();
    }
}