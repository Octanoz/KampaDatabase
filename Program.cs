using Spectre.Console;
using DatabaseChallenge.Enums;
using DatabaseChallenge.Services;

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
        case "view names":
            DatabaseFunctions.ViewFile(ViewOrder.Names);
            break;
        case "view roles":
            DatabaseFunctions.ViewFile(ViewOrder.Roles);
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
            throw new ArgumentException($"Unknown command: {userInput}, please create a support ticket");
    }
}
