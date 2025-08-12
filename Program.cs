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
    if (userInput is "exit")
    {
        keepRunning = Navigation.ExitYesNo();
        return;
    }

    Action action = userInput switch
    {
        "load" => DatabaseFunctions.Load,
        "ids" => DatabaseFunctions.ViewIDs,
        "view names" => () => DatabaseFunctions.ViewFile(ViewOrder.Names),
        "view roles" => () => DatabaseFunctions.ViewFile(ViewOrder.Roles),
        "page" => Communication.PageMedicalStaff,
        "add" => DatabaseFunctions.AddEmployee,
        "remove" => DatabaseFunctions.RemoveEmployee,
        "clear" => Navigation.ClearScreen,
        _ => throw new ArgumentException($"Unknown command: {userInput}, please create a support ticket")
    };

    action();
}
