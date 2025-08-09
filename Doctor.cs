using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal class Doctor : Employee, IPage
    {
        public Doctor(int employeeID, string firstName, string lastName, string jobTitle, string specialization) : base(employeeID, firstName, lastName, jobTitle)
        {
            Specialization = specialization;
        }

        public string Specialization { get; set; }

        public void Page()
        {
            Console.WriteLine($"Paging {JobTitle} {LastName}.");
        }
    }
}
