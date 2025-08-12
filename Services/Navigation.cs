namespace DatabaseChallenge.Services;

using Spectre.Console;

public static class Navigation
{
    public static void StartScreen()
    {
        AnsiConsole.Write(new FigletText("Hospital Portal").Centered().Color(Color.Green));
        Console.WriteLine();
    }

    public static string Option()
    {
        List<string> options =
        [
            "load \t\tLoad existing employees' details from file",
            "ids \t\tView all employee IDs in the system",
            "view names \tEnter the employee's ID number to view the details, names first",
            "view roles \tEnter the employee's ID number to view the details, roles first",
            "page \t\tPages all medical staff",
            "add \t\tInput the employees details and add them to the system",
            "remove \tEnter the employee's ID number to remove it from the system",
            "clear \tClear the screen, back to welcome screen",
            "exit \t\tExit the program",
        ];

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("\tChoose from the following options:")
                                         .AddChoices(options)
            );

        string[] splitChoice = choice.Split();
        if (splitChoice[0] is "view")
        {
            return $"{splitChoice[0]} {splitChoice[1]}";
        }

        return splitChoice[0];
    }

    public static void ClearScreen()
    {
        Console.Clear();
        StartScreen();
    }

    public static bool YesNo()
    {
        char choice = AnsiConsole.Prompt(
            new TextPrompt<char>("Please enter y or n: ")
                .AddChoice('y')
                .AddChoice('n')
                .DefaultValue('n')
        );

        if (choice is 'n')
        {
            ClearScreen();
            return false;
        }

        return true;
    }

    public static bool ExitYesNo()
    {
        char exitChoice = AnsiConsole.Prompt(
            new TextPrompt<char>("Are you sure you want to exit the portal?")
                .AddChoice('y')
                .AddChoice('n')
                .DefaultValue('n')
            );

        if (exitChoice is 'n')
        {
            ClearScreen();
            return true;
        }

        return false;
    }
}