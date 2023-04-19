using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal class Nurse : Employee, IPage
    {
        public Nurse(int employeeID, string firstName, string lastName, string jobTitle, string level) : base(employeeID, firstName, lastName, jobTitle)
        {
            Level = level;
        }

        public string Level { get; set; }

        public void Page()
        {
            Console.WriteLine($"Paging {JobTitle} {LastName}.");
        }
    }
}
