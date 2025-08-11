# KampaDatabase
Console app - Hospital Database, final exercise on the Kampa Plays tutorial series on YouTube

> Originally this project was almost 2 years old. I learned about the Spectre Console Nuget package and thought it would be cool to pimp this project up.
> While going through the code I realized I had hardcoded complete paths and decided to purge the original commits so now it looks like a new project.
> This probably means I won't be able to help myself and refactor some things beyond just making it work with Spectre Console.

## Instructions

Build a C# console application to serve as a hospital administrative portal with the following features:

Ability to:

 * Provide the portal user with a help function to display available portal commands.
 * Provide feedback when an invalid command is used.

 * Add employees to the portal (storing them on the file system)
   * First name
   * Last name
   * Job title

   * Doctors
       * have a specialization (Surgeon, Cardiologist, etc.)
   * Nurses
     * have a level (RN, LPN, etc.)
   * Custodian

 * Remove a specific employee
 * Load employee data into the system on demand
 * View a specific employee
 * Page all medical staff (doctors, nurses)
 * Handle and report application errors due to file system operations
 * Use object oriented principles to ensure further system upgrades can be easily implemented
