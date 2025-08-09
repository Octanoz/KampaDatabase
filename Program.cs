

//! Help / Add / Remove / Load / View / Page / Clear

using DatabaseChallenge;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

string employeeFilesPath = @"..\..\..\EmployeeFiles";
bool keepRunning = true;
bool employeesLoaded = false;
List<Employee> employees = new();

StartScreen();

while (keepRunning)
{
    Console.Write("\n> ");
    string userInput = Console.ReadLine()!.ToLower();
    ProcessInput(userInput);
}

#region Main Portal
static void StartScreen()
{
    Console.WriteLine("\t╔════════════════════════════╗");
    Console.WriteLine("\t║ Hospital Management Portal ║");
    Console.WriteLine("\t╚════════════════════════════╝");
    Console.WriteLine("\nType \"help\" for available commands.");
}

void ProcessInput(string userInput)
{
    switch (userInput)
    {
        case "help":
            Help();
            break;
        case "load":
            Load(employeeFilesPath);
            break;
        case "ids":
            ViewIDs();
            break;
        case "view":
            ViewFile();
            break;
        case "page":
            PageMedicalStaff();
            break;
        case "add":
            AddEmployee();
            break;
        case "remove":
            RemoveEmployee();
            break;
        case "clear":
            ClearScreen();
            break;
        case "exit":
            keepRunning = ExitYesNo();
            break;
        default:
            Console.WriteLine("\nUnknown command. Please try again.");
            Console.WriteLine("Type \"help\" for a list of available commands.");
            break;
    }
}

static void Help()
{
    Console.WriteLine("\t\thelp \tView this list of available commands");
    Console.WriteLine("\t\tload \tLoad existing employees' details from file");
    Console.WriteLine("\t\tids \tView all employee IDs in the system");
    Console.WriteLine("\t\tview \tEnter the employee's ID number to view the details");
    Console.WriteLine("\t\tpage \tPages all medical staff");
    Console.WriteLine("\t\tadd \tInput the employees details and add them to the system");
    Console.WriteLine("\t\tremove \tEnter the employee's ID number to remove it from the system");
    Console.WriteLine("\t\tclear \tClear the screen, back to welcome screen");
    Console.WriteLine("\t\texit \tExit the program");
}

#endregion
void Load(string employeeFilesPath)
{
    string[] files = Directory.GetFiles(employeeFilesPath);

    foreach (string file in files)
    {
        string[] employeeDetails = File.ReadAllLines(file);

        int employeeID = Convert.ToInt32(employeeDetails[0]);
        string firstName = employeeDetails[1];
        string lastName = employeeDetails[2];
        string jobTitle = employeeDetails[3];
        Employee employee;

        if (employees.Exists(e => e.EmployeeID == employeeID))
            continue;

        switch (jobTitle)
        {
            case "Doctor":
                string specialization = employeeDetails[4];
                employee = new Doctor(employeeID, firstName, lastName, jobTitle, specialization);
                employees.Add(employee);
                break;
            case "Nurse":
                string level = employeeDetails[4];
                employee = new Nurse(employeeID, firstName, lastName, jobTitle, level);
                employees.Add(employee);
                break;
            case "Custodian":
                employee = new Custodian(employeeID, firstName, lastName, jobTitle);
                employees.Add(employee);
                break;
            default:
                Console.WriteLine($"Unknown job title in {Path.Combine(employeeFilesPath, file)}. Please contact your system administrator.");
                break;
        }
    }

    switch (files.Length)
    {
        case 0:
            Console.WriteLine("There are no employees in the file system.");
            break;
        case 1:
            Console.WriteLine("Loaded 1 employee from the system.");
            employeesLoaded = true;
            break;
        default:
            Console.WriteLine($"Loaded {files.Length} employees from the system.");
            employeesLoaded = true;
            break;
    }
}

void ViewIDs()
{
    Console.WriteLine();


    if (employeesLoaded)
    {
        foreach (Employee employee in employees)
        {
            Console.WriteLine(employee.EmployeeID.ToString("D3"));
        }
    }
    else
    {
        Console.WriteLine("No employee files were loaded into the portal or the file system was updated since the last time the files were loaded. " +
            "\nPlease type \"load\" in the main portal to load the files." +
                "\nPress enter to return to main portal.");
        Console.ReadKey();
        ClearScreen();
    }
}

void ViewFile()
{
    bool viewFiles = true;
    while (viewFiles)
    {
        ViewIDs();
        if (!employeesLoaded)
            break;

        Console.WriteLine("\nPlease enter the employee ID you wish to view the details of: \nTyping \"exit\" or just enter will return you to the portal screen:");
        Console.Write("> ");

        string requestedID = Console.ReadLine()!;

        if (requestedID == "" || requestedID == "exit")
        {
            ClearScreen();
            break;
        }

        if (InvalidID(requestedID) == true)
            continue;

        Employee result = employees.Find(e => e.EmployeeID.ToString() == requestedID);

        if (result != null)
        {
            Console.WriteLine($"\nShowing details for {requestedID}");
            Console.WriteLine($"=======================\n");

            Console.WriteLine($"{result.LastName}, {result.FirstName}");
            Console.WriteLine($"{result.JobTitle}");

            if (result is Doctor doctor)
                Console.WriteLine($"{doctor.Specialization}");
            else if (result is Nurse nurse)
                Console.WriteLine($"{nurse.Level}");

            Console.WriteLine();
            Console.WriteLine("Do you want to look at another employee file?");
            viewFiles = YesNo();
        }
    }
}

void PageMedicalStaff()
{
    Console.WriteLine();
    if (employeesLoaded)
    {
        foreach (Employee employee in employees)
        {
            if (employee is IPage)
            {
                IPage pageable = (IPage)employee;
                pageable.Page();
            }
        }
    }
    else
    {
        Console.WriteLine("No employee files were loaded into the portal or the file system was updated since the last time the files were loaded. " +
            "\nPlease type \"load\" in the main portal to load the files." +
                "\nPress enter to return to main portal.");
        Console.ReadKey();
        ClearScreen();
    }
}

void AddEmployee()
{
    ViewIDs();
    if (!employeesLoaded)
        return;

    Console.WriteLine("Please enter the details for the employee file:\n" +
        "Type \"exit\" to return to the main portal.\n");

    while (true)
    {
        Console.Write("Employee ID: ");
        string employeeID = Console.ReadLine()!;
        if (employeeID == "exit")
            break;
        if (InvalidID(employeeID))
            continue;
        if (employees.Exists(e => e.EmployeeID.ToString() == employeeID))
        {
            Console.WriteLine($"An employee with employee ID {employeeID} already exists.");
            continue;
        }
        Console.Write("First name: ");
        string firstName = Console.ReadLine()!;
        Console.Write("Last name: ");
        string lastName = Console.ReadLine()!;
        Console.Write("Job Title: ");
        string jobTitle = Console.ReadLine()!;

        if (String.IsNullOrEmpty(employeeID) || String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(jobTitle))
        {
            FieldIsEmpty();
            continue;
        }

        firstName = TitleCase(firstName);
        lastName = TitleCase(lastName);
        jobTitle = TitleCase(jobTitle);

        if (jobTitle == "Doctor")
        {
            Console.Write("Specialization: ");
            string specialization = Console.ReadLine()!;

            if (String.IsNullOrEmpty(specialization))
            {
                FieldIsEmpty();
                continue;
            }
            else
            {
                specialization = TitleCase(specialization);
                string jobLetter = "D";
                string fullPath = FullPath(employeeID, lastName, jobLetter);

                StringBuilder sb = new();

                sb = WriteMedicalStaffFile(employeeID, firstName, lastName, jobTitle, specialization);

                File.WriteAllText(fullPath, sb.ToString());
            }
        }
        else if (jobTitle == "Nurse")
        {
            Console.Write("Level: ");
            string level = Console.ReadLine()!;

            if (String.IsNullOrEmpty(level))
            {
                FieldIsEmpty();
                continue;
            }
            else
            {
                level = level.ToUpper();
                string jobLetter = "N";
                string fullPath = FullPath(employeeID, lastName, jobLetter);

                StringBuilder sb = new();

                sb = WriteMedicalStaffFile(employeeID, firstName, lastName, jobTitle, level);

                File.WriteAllText(fullPath, sb.ToString());
            }
        }
        else
        {
            string jobLetter = "C";
            string fullPath = FullPath(employeeID, lastName, jobLetter);

            StringBuilder sb = new();

            sb = WriteGeneralStaffFile(employeeID, firstName, lastName, jobTitle);

            File.WriteAllText(fullPath, sb.ToString());
        }
        Console.WriteLine("\nEmployee file successfully added to the file system.");
        Load(employeeFilesPath);

        Console.WriteLine("Do you want to add another employee file?");
        bool addAnotherFile = YesNo();
        if (addAnotherFile)
        {
            Console.WriteLine();
            ViewIDs();
            continue;
        }
        else break;
    }
}

void RemoveEmployee()
{
    ViewIDs();
    if (!employeesLoaded)
        return;


    Console.WriteLine("\nWhich of these employee files do you want to remove?");
    Console.Write("> ");
    string removeID = Console.ReadLine();


    try
    {
        string[] files = Directory.GetFiles(employeeFilesPath, removeID + "*");

        if (files.Length == 0)
        {
            Console.WriteLine($"Could not find a file for employee ID {removeID}. Please check if the ID exists in the system by typing \"ids\" in the main portal.");
            Console.WriteLine("If you have verified the ID exists, please contact your system administrator.\nReturning to main portal.");
            return;
        }
        else
        {
            string fullPath = Path.Combine(employeeFilesPath, files[0]);
            File.Delete(fullPath);
            Console.WriteLine("File successfully deleted.");
            employees.Clear();
            //Load(employeeFilesPath);
            employeesLoaded = false;
        }
    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine($"Directory where employee files are stored could not be found.\n" +
            $"Please contact your system administrator and provide the following error message \"{e.Message}\".\n" +
            $"System closing down after you press any key.");
        Console.ReadKey();
        keepRunning = false;
    }
    catch (IOException e)
    {
        Console.WriteLine($"IO Exception. Please contact your system administrator and provide the following error message \"{e.Message}\".\n" +
            $"System closing down after you press any key.");
        Console.ReadKey();
        keepRunning = false;
    }
}

StringBuilder WriteGeneralStaffFile(string employeeID, string firstName, string lastName, string jobTitle)
{
    StringBuilder sb = new();

    sb.AppendLine(employeeID);
    sb.AppendLine(firstName);
    sb.AppendLine(lastName);
    sb.AppendLine(jobTitle);

    return sb;
}

StringBuilder WriteMedicalStaffFile(string employeeID, string firstName, string lastName, string jobTitle, string specialization)
{
    StringBuilder sb = new();

    sb.AppendLine(employeeID);
    sb.AppendLine(firstName);
    sb.AppendLine(lastName);
    sb.AppendLine(jobTitle);
    sb.AppendLine(specialization);

    return sb;
}

string FullPath(string employeeID, string lastName, string jobLetter)
{
    string fileName = employeeID + "_" + lastName + "_" + jobLetter + @".txt";
    return Path.Combine(employeeFilesPath, fileName);
}

void FieldIsEmpty()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nNot all details were filled in.");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Please try again or type exit to return to the main portal.\n");
}

static void ClearScreen()
{
    Console.Clear();
    StartScreen();
}

static bool ExitYesNo()
{
    Console.WriteLine("\nAre you sure you want to exit the program?");

    while (true)
    {
        Console.Write("\nPlease enter y or n: ");
        ConsoleKeyInfo keyInfo = Console.ReadKey();
        char c = Char.ToLower(keyInfo.KeyChar);

        switch (c)
        {
            case 'y':
                return false;
            case 'n':
                ClearScreen();
                return true;
            default:
                continue;
        }
    }
}

static bool YesNo()
{
    Console.Write("\nPlease enter y or n: ");
    ConsoleKeyInfo keyInfo = Console.ReadKey();
    char c = Char.ToLower(keyInfo.KeyChar);

    switch (c)
    {
        case 'y':
            Console.WriteLine();
            return true;
        default:
            ClearScreen();
            return false;
    }
}

string TitleCase(string str)
{
    str = str.ToLower();
    return Char.ToUpper(str[0]) + str.Substring(1);
}

bool InvalidID(string requestedID)
{
    if (Convert.ToInt32(requestedID) < 100 || Convert.ToInt32(requestedID) > 999)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nEmployee IDs start at 100 and have a maximum value of 999");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Please choose from the available employee IDs:\n");

        return true;
    }
    else return false;
}
