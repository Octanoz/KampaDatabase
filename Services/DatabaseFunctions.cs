namespace DatabaseChallenge.Services;

using System.Text;
using System.Globalization;
using Spectre.Console;
using DatabaseChallenge.Enums;
using DatabaseChallenge.Models;

public static class DatabaseFunctions
{
    private static readonly Dictionary<string, List<string>> jobRoles = new()
    {
        ["Doctor"] = ["Endocrinology", "Gastroenterology", "Geriatrics", "Heart Surgery", "Neurology", "Oncology", "Pediatrics", "Pulmonology", "Surgery"],
        ["Nurse"] = ["APRN", "CNA", "CNO", "LPN", "NRP", "RN"],
        ["Custodian"] = ["Floor Care", "Laundry", "Maintenance", "Waste Management"],
        ["Other"] = ["Not eligible for salary payment - contact Payroll"]
    };

    //Development file path: <project files>\bin\Debug\net.9.0
    private static readonly string employeeFilesPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\EmployeeFiles"));
    private static readonly HashSet<int> employeeIDs = [];
    internal static List<Employee> Employees { get; private set; } = [];
    public static bool EmployeesLoaded { get; set; } = false;


    public static void Load()
    {
        string[] files = Directory.GetFiles(employeeFilesPath);

        foreach (string file in files)
        {
            string[] employeeDetails = File.ReadAllLines(file);

            int employeeID = int.Parse(employeeDetails[0]);
            if (!employeeIDs.Add(employeeID))
                continue;

            string firstName = employeeDetails[1];
            string lastName = employeeDetails[2];
            string jobTitle = employeeDetails[3];
            string jobSpecific = employeeDetails[4];

            Employees.Add(jobTitle switch
            {
                "Doctor" => new Doctor(employeeID, firstName, lastName, jobTitle, jobSpecific),
                "Nurse" => new Nurse(employeeID, firstName, lastName, jobTitle, jobSpecific),
                "Custodian" => new Custodian(employeeID, firstName, lastName, jobTitle, jobSpecific),
                _ => new Other(employeeID, firstName, lastName, jobTitle, jobSpecific)
            });
        }

        AnsiConsole.Write(new Markup(files.Length switch
        {
            1 => "\tLoaded [yellow1]1[/] employee from the system.",
            var x when x > 1 => $"\tLoaded [yellow1]{x}[/] employees from the system.",
            _ => "\t[red slowblink]There are no employees in the file system.[/]"
        }));

        Console.WriteLine("\n");

        EmployeesLoaded = files.Length is not 0;
    }

    public static void AddEmployee()
    {
        if (!EmployeesLoaded)
        {
            DatabaseNotLoaded();
            return;
        }

        AnsiConsole.MarkupLine(
            """            
            Please enter the details for the employee file:
            
            Type [lightskyblue1 italic]exit[/] to return to the main portal.
            
            
            """);

        int newId = Random.Shared.Next(100, 1000);
        while (true)
        {
            while (employeeIDs.Contains(newId))
            {
                newId = Random.Shared.Next(100, 1000);
            }

            string firstName = TitleCase(PromptFirstName());
            if (firstName.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                break;

            string lastNamePrefix = PromptLastNamePrefix();
            string lastName = PromptLastName();
            if (lastNamePrefix is not "N/A")
                lastName = $"{lastNamePrefix}{lastName}";
            lastName = TitleCase(lastName);

            string jobTitle = PromptJobTitle();
            var (fullPath, details) = ProcessEmployee(newId, firstName, lastName, jobTitle);

            File.WriteAllText(fullPath, details);

            AnsiConsole.MarkupLine("\n[lightgreen]Employee file successfully added to the file system.[/]");
            if (jobTitle is "Other")
            {
                AnsiConsole.MarkupLineInterpolated(
                    $"""
                    [orange1]Job title was set to 'Other', please contact payroll as soon as possible with ticket number[/] [lightskyblue1]{Random.Shared.Next(10000, 100000)}.[/]
                    [orange1]Without a specific job title this employee will not receive their wages or benefits.[/]
                    """);                    
            }

            Load();

            Console.WriteLine("Do you want to add another employee file?");
            bool addAnotherFile = Navigation.YesNo();
            if (addAnotherFile)
            {
                Console.WriteLine();
                continue;
            }

            break;
        }
    }

    private static string PromptFirstName() =>
        AnsiConsole.Prompt(
            new TextPrompt<string>("First name: ")
                .Validate(x => x switch
                {
                    var name when string.IsNullOrWhiteSpace(name) => ValidationResult.Error("First name cannot be left empty"),
                    var name when name.Length < 3 => ValidationResult.Error("First name should be at least 3 characters long"),
                    var name when name.Contains(' ') => ValidationResult.Error("First name cannot contain whitespace, connect names with hyphens if needed"),
                    var name when name.Any(c => char.IsDigit(c)) => ValidationResult.Error("Names cannot contain numeric characters"),
                    _ => ValidationResult.Success()
                }));

    private static string PromptLastNamePrefix() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Does the surname included prefixes?")
                .AddChoices("N/A", "van ", "van de ", "van der ", "vande ", "vander ", "of ", "di ", "de ", "dela ", "della ", "de la ", "le ", "la "));

    private static string PromptLastName() =>
        AnsiConsole.Prompt(
            new TextPrompt<string>("Last name: ")
                .Validate(x => x switch
                {
                    var name when string.IsNullOrWhiteSpace(name) => ValidationResult.Error("Last name cannot be left empty"),
                    var name when name.TrimEnd().Length < 3 => ValidationResult.Error("Last name should be at least 3 characters long"),
                    var name when name.Any(c => char.IsDigit(c)) => ValidationResult.Error("Names cannot contain numeric characters"),
                    _ => ValidationResult.Success()
                }));

    private static string PromptJobTitle() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Job Title: (Choose 'other' if an appropriate role is not listed and contact Payroll with the ticket number generated after processing.)")
                .AddChoices("Doctor", "Nurse", "Custodian", "Other")
            );

    private static (string, string) ProcessEmployee(int id, string firstName, string lastName, string jobTitle)
    {
        string category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Category/Level/Specialization:")
                .AddChoices(jobRoles[jobTitle])
            );

        char jobLetter = jobTitle switch
        {
            "Custodian" => 'C',
            "Doctor" => 'D',
            "Nurse" => 'N',
            _ => 'O'
        };

        string fullPath = FullPath(id, lastName, jobLetter);
        string empDetails = WriteStaffFile(id, firstName, lastName, jobTitle, category);

        return (fullPath, empDetails);
    }

    public static void RemoveEmployee()
    {
        if (!EmployeesLoaded)
        {
            DatabaseNotLoaded();
            return;
        }

        string[] fileHeaders = FilesOverview(ViewOrder.Regular);

        bool deleteFiles = true;
        while (deleteFiles)
        {
            string file = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select the file you wish to delete:")
                    .AddChoices(fileHeaders)
                );

            int requestedID = int.Parse(file[..3]);
            Panel deletePanel = new(new Markup(WriteFile(requestedID)));
            deletePanel.Header("Please confirm deletion").Padding(5, 0);
            AnsiConsole.Write(deletePanel);

            bool deleteConfirmed = Navigation.YesNo();
            if(deleteConfirmed)
            {
                string fileName = Directory.GetFiles(employeeFilesPath, $"{requestedID}*")[0];
                string fullPath = Path.Combine(employeeFilesPath, fileName);
                File.Delete(fullPath);
                Console.WriteLine("File successfully deleted.\n");

                Employee deletedEmployee = Employees.First(e => e.EmployeeID == requestedID);
                Employees.Remove(deletedEmployee);
                employeeIDs.Remove(requestedID);
                EmployeesLoaded = false;
            }

            Console.WriteLine("Do you want to delete another employee file?");
            deleteFiles = Navigation.YesNo();
        }   
    }

    public static void ViewIDs()
    {
        Console.WriteLine();

        if (EmployeesLoaded)
        {
            string ids = "";
            if (employeeIDs.Count <= 10)
            {
                ids = string.Join("  ", employeeIDs);
            }
            else
                ids = string.Join("\n", employeeIDs);

            Panel idPanel = new(new Markup(ids).Centered());
            idPanel.Header("ID List").Padding(4, 1);
            idPanel.Border = BoxBorder.Rounded;
            var paddedIdPanel = new Padder(idPanel).Padding(8, 0);
            
            AnsiConsole.Write(paddedIdPanel);
            Console.WriteLine();
        }
        else
        {
            DatabaseNotLoaded();
            Navigation.ClearScreen();
        }
    }

    public static void ViewFile(ViewOrder viewOrder = ViewOrder.Regular)
    {
        if (!EmployeesLoaded)
        {
            DatabaseNotLoaded();
            return;
        }

        string[] fileHeaders = FilesOverview(viewOrder);

        bool viewFiles = true;
        while (viewFiles)
        {
            string file = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select the file you wish to view the details of:")
                    .AddChoices(fileHeaders)
                );

            int requestedID = int.Parse(file[..3]);
            DisplayFile(requestedID);

            Console.WriteLine("Do you want to look at another employee file?");
            viewFiles = Navigation.YesNo();
        }
    }

    private static string[] FilesOverview(ViewOrder viewOrder) => viewOrder switch
    {
        ViewOrder.Regular => [.. Employees.Select(e => $"{e.EmployeeID} - {e.LastName, -20}\t{e.JobTitle}")],
        ViewOrder.Names => [.. Employees.Select(e => $"{e.EmployeeID} - {e.LastName, -20}{e.JobTitle}").OrderBy(s => s[6])],
        _ => [.. Employees.Select(e => $"{e.EmployeeID} - {e.JobTitle, -15}{e.LastName}").OrderBy(s => s[6])]
    };

    private static void DisplayFile(int id)
    {
        Employee employee = Employees.Find(e => e.EmployeeID == id)
                          ?? throw new InvalidDataException($"No employee with this id number: {id}");

        AnsiConsole.MarkupLineInterpolated(
                    $"""

                    Showing details for [lightslateblue]{id}[/]
                    =======================

                    {employee.LastName}, {employee.FirstName}
                    {employee.JobTitle}
                    """);

        AnsiConsole.MarkupLineInterpolated(employee switch
        {
            Doctor d => $"[turquoise2]{d.Specialization}[/]\n",
            Nurse n => $"[violet]{n.Level}[/]\n",
            Custodian c => $"[orchid]{c.Category}[/]\n",
            Other o => $"[red3_1 slowblink]{o.Category}[/]\n",
            _ => throw new ArgumentException($"Unknown role chosen: {employee.JobTitle}, please contact the system admin with the id {id} and the unknown role as reference.")
        });
    }
    
    private static string WriteFile(int id)
    {
        Employee employee = Employees.Find(e => e.EmployeeID == id)
                          ?? throw new InvalidDataException($"No employee with this id number: {id}");

        string header =
            $"""

            Employee ID [lightslateblue]{id}[/]
            ===============

            {employee.LastName}, {employee.FirstName}
            {employee.JobTitle}
            """;

        string jobSpecific = employee switch
        {
            Doctor d => $"[turquoise2]{d.Specialization}[/]\n",
            Nurse n => $"[violet]{n.Level}[/]\n",
            Custodian c => $"[orchid]{c.Category}[/]\n",
            Other o => $"[red3_1 slowblink]{o.Category}[/]\n",
            _ => ""
        };

        return $"{header}\n{jobSpecific}";
    }

    private static string WriteStaffFile(int employeeID, string firstName, string lastName, string jobTitle, string specialization) =>
        $"""
        {employeeID}
        {firstName}
        {lastName}
        {jobTitle}
        {specialization}
        """;

    private static string FullPath(int employeeID, string lastName, char jobLetter)
    {
        string fileName = $"{employeeID}_{lastName}_{jobLetter}.txt";
        return Path.Combine(employeeFilesPath, fileName);
    }

    private static string TitleCase(string str) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);

    public static void DatabaseNotLoaded()
    {
        AnsiConsole.MarkupLine(
            """
            [gold1]No employee files were loaded into the portal
            or the file system was updated since the last time the files were loaded.[/]
                
            Please choose [lightskyblue1 italic]load[/] in the main portal to load the files.
            Press [lightskyblue1 italic]any key[/] to return to main portal.
            """);

        Console.ReadKey();
    }

    internal static Doctor[] GetDoctors() => [.. Employees.OfType<Doctor>()];
    internal static Nurse[] GetNurses() => [.. Employees.OfType<Nurse>()];
}
