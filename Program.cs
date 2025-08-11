using DatabaseChallenge;
using Spectre.Console;

bool keepRunning = true;

Navigation.StartScreen();

while (keepRunning)
{
    string userInput = Navigation.Option();
    ProcessInput(userInput);
}

void ProcessInput(string userInput)
{
    switch (userInput)
    {
        case "load":
            DatabaseFunctions.Load();
            break;
        case "ids":
            DatabaseFunctions.ViewIDs();
            break;
        case "view":
            DatabaseFunctions.ViewFile();
            break;
        case "page":
            Communication.PageMedicalStaff();
            break;
        case "add":
            DatabaseFunctions.AddEmployee();
            break;
        case "remove":
            DatabaseFunctions.RemoveEmployee();
            break;
        case "clear":
            Navigation.ClearScreen();
            break;
        case "exit":
            keepRunning = Navigation.ExitYesNo();
            break;
        default:
            AnsiConsole.MarkupLine(
                """
                
                [red rapidblink]Unknown command[/]. Please try again.
                Type [lightskyblue1 italic]help[/] for a list of available commands.
                """);
            break;
    }
}
