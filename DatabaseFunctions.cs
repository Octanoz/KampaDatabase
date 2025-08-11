namespace DatabaseChallenge;

using System.Text;
using Spectre.Console;


public static class DatabaseFunctions
{
    //Development file path: <project files>\bin\Debug\net.9.0
    private static readonly string employeeFilesPath = @"..\..\..\EmployeeFiles";
    private static readonly HashSet<int> employeeIDs = [];
    internal static List<Employee> Employees { get; private set; } = [];
    public static bool EmployeesLoaded { get; set; } = false;

    public static void Load()
    {
        Span<string> files = Directory.GetFiles(employeeFilesPath);

        foreach (string file in files)
        {
            string[] employeeDetails = File.ReadAllLines(file);

            int employeeID = int.Parse(employeeDetails[0]);
            if (!employeeIDs.Add(employeeID))
                continue;

            string firstName = employeeDetails[1];
            string lastName = employeeDetails[2];
            string jobTitle = employeeDetails[3];
            Employee employee;


            switch (jobTitle)
            {
                case "Doctor":
                    string specialization = employeeDetails[4];
                    employee = new Doctor(employeeID, firstName, lastName, jobTitle, specialization);
                    Employees.Add(employee);
                    break;
                case "Nurse":
                    string level = employeeDetails[4];
                    employee = new Nurse(employeeID, firstName, lastName, jobTitle, level);
                    Employees.Add(employee);
                    break;
                case "Custodian":
                    employee = new Custodian(employeeID, firstName, lastName, jobTitle);
                    Employees.Add(employee);
                    break;
                default:
                    AnsiConsole.MarkupLineInterpolated($"[red slowblink]Unknown job title in {file}.[/]\nPlease contact your system administrator.");
                    break;
            }
        }

        Markup loadedEmployees = new(files.Length switch
        {
            1 => "\tLoaded [yellow1]1[/] employee from the system.",
            var x when x > 0 => $"\tLoaded [yellow1]{x}[/] employees from the system.",
            _ => "\t[red slowblink]There are no employees in the file system.[/]"
        });

        AnsiConsole.Write(loadedEmployees);
        Console.WriteLine("\n\n");

        EmployeesLoaded = files.Length is not 0;
    }

    public static void AddEmployee()
    {
        ViewIDs();
        if (!EmployeesLoaded)
            return;

        AnsiConsole.MarkupLine(
            """            
            Please enter the details for the employee file:
            
            Type [lightskyblue1 italic]exit[/] to return to the main portal.
            
            
            """);

        int newId = Random.Shared.Next(100, 1000);
        while (employeeIDs.Contains(newId))
        {
            newId = Random.Shared.Next(100, 1000);
        }

        while (true)
        {            
            string firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("First name: ")
                    .Validate(x => x switch
                    {
                        var name when String.IsNullOrWhiteSpace(name) => ValidationResult.Error("First name cannot be left empty"),
                        var name when name.Length < 3 => ValidationResult.Error("First name should be at least 3 characters long"),
                        var name when name.Contains(' ') => ValidationResult.Error("First name cannot contain whitespace, connect names with hyphens if needed"),
                        var name when name.Any(c => Char.IsDigit(c)) => ValidationResult.Error("Names cannot contain numeric characters"),
                        _ => ValidationResult.Success()
                    }));

            if (firstName.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                break;

            string lastNamePrefix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Does the surname included prefixes?")
                    .AddChoices("N/A", "van ", "van de ", "van der ", "vande ", "vander ", "of ", "di ", "de ", "dela ", "della ", "de la ", "le "));
            
            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("Last name: ")
                    .Validate(x => x switch
                    {
                        var name when String.IsNullOrWhiteSpace(name) => ValidationResult.Error("Last name cannot be left empty"),
                        var name when name.TrimEnd().Length < 3 => ValidationResult.Error("Last name should be at least 3 characters long"),
                        var name when name.Any(c => Char.IsDigit(c)) => ValidationResult.Error("Names cannot contain numeric characters"),
                        _ => ValidationResult.Success()
                    }));

            if (lastName.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                break;

            if (lastNamePrefix is not "N/A")
                lastName = $"{lastNamePrefix}{lastName}";

            string jobTitle = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Job Title: (Contact Payroll if an appropriate role is not available)")
                    .AddChoices("Doctor", "Nurse", "Custodian")
                );

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
                    char jobLetter = 'D';
                    string fullPath = FullPath(newId, lastName, jobLetter);
                    string medicalStaffDetails = WriteMedicalStaffFile(newId, firstName, lastName, jobTitle, specialization);

                    File.WriteAllText(fullPath, medicalStaffDetails);
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
                    char jobLetter = 'N';
                    string fullPath = FullPath(newId, lastName, jobLetter);
                    string medicalStaffDetails = WriteMedicalStaffFile(newId, firstName, lastName, jobTitle, level);

                    File.WriteAllText(fullPath, medicalStaffDetails);
                }
            }
            else if (jobTitle == "Custodian")
            {
                char jobLetter = 'C';
                string fullPath = FullPath(newId, lastName, jobLetter);
                string custodianDetails = WriteGeneralStaffFile(newId, firstName, lastName, jobTitle);

                File.WriteAllText(fullPath, custodianDetails);
            }

            AnsiConsole.MarkupLine("\n[lightgreen]Employee file successfully added to the file system.[/]");
            if (jobTitle is "Other")
            {
                AnsiConsole.MarkupLineInterpolated(
                    $"""
                    [orange1]Job title was set to 'Other', please contact payroll as soon as possible with ticket number[/] [lightskyblue1]{Random.Shared.Next(10000, 100000)}.[/]
                    [orange1]Without a specific job title this employee will not receive salary or benefits.[/]
                    """);                    
            }

            Load();
            EmployeesLoaded = true;

            Console.WriteLine("Do you want to add another employee file?");
            bool addAnotherFile = Navigation.YesNo();
            if (addAnotherFile)
            {
                Console.WriteLine();
                ViewIDs();
                continue;
            }
            break;
        }
    }

    public static void RemoveEmployee()
    {
        ViewIDs();
        if (!EmployeesLoaded)
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
                Employees.Clear();
                EmployeesLoaded = false;
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine($"Directory where employee files are stored could not be found.\n" +
                $"Please contact your system administrator and provide the following error message \"{e.Message}\".\n" +
                $"System closing down after you press any key.");
            Console.ReadKey();
            throw;
        }
        catch (IOException e)
        {
            Console.WriteLine($"IO Exception. Please contact your system administrator and provide the following error message \"{e.Message}\".\n" +
                $"System closing down after you press any key.");
            Console.ReadKey();
            throw;
        }
    }

    public static void ViewIDs()
    {
        Console.WriteLine();

        if (EmployeesLoaded)
        {
            foreach (var employee in Employees)
                Console.WriteLine(employee.EmployeeID.ToString("D3"));
        }
        else
        {
            DatabaseNotLoaded();

            Console.ReadKey();
            Navigation.ClearScreen();
        }
    }

    public static void ViewFile()
    {
        bool viewFiles = true;
        while (viewFiles)
        {
            ViewIDs();
            if (!EmployeesLoaded)
                break;

            Console.WriteLine("\nPlease enter the employee ID you wish to view the details of: \nTyping \"exit\" or just enter will return you to the portal screen:");
            Console.Write("> ");

            string requestedID = Console.ReadLine()!;

            if (requestedID == "" || requestedID == "exit")
            {
                Navigation.ClearScreen();
                break;
            }

            if (int.Parse(requestedID) < 100 || int.Parse(requestedID) > 999)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nEmployee IDs start at 100 and have a maximum value of 999");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please choose from the available employee IDs:\n");
                continue;
            }

            Employee? result = Employees.Find(e => e.EmployeeID.ToString() == requestedID);

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
                viewFiles = Navigation.YesNo();
            }
        }
    }

    private static string WriteGeneralStaffFile(int employeeID, string firstName, string lastName, string jobTitle)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{employeeID}");
        sb.AppendLine(firstName);
        sb.AppendLine(lastName);
        sb.AppendLine(jobTitle);

        return sb.ToString();
    }

    private static string WriteMedicalStaffFile(int employeeID, string firstName, string lastName, string jobTitle, string specialization)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{employeeID}");
        sb.AppendLine(firstName);
        sb.AppendLine(lastName);
        sb.AppendLine(jobTitle);
        sb.AppendLine(specialization);

        return sb.ToString();
    }

    private static string FullPath(int employeeID, string lastName, char jobLetter)
    {
        string fileName = $"{employeeID}_{lastName}_{jobLetter}.txt";
        return Path.Combine(employeeFilesPath, fileName);
    }

    private static void FieldIsEmpty()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nNot all details were filled in.");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Please try again or type exit to return to the main portal.\n");
    }

    private static string TitleCase(string str)
    {
        str = str.ToLower();
        return Char.ToUpper(str[0]) + str.Substring(1);
    }

    public static void DatabaseNotLoaded()
    {
        AnsiConsole.MarkupLine(
            """
            [gold1]No employee files were loaded into the portal
            or the file system was updated since the last time the files were loaded.[/]
                
            Please choose [lightskyblue1 italic]load[/] in the main portal to load the files.
            Press [lightskyblue1 italic]any key[/] to return to main portal.
            """);
    }
}