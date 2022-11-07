using Microsoft.Data.Sqlite;
using System.Configuration;

namespace CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayTitle();
            InitializeDatabase();

            while (true)
            {
                DisplayOptionsMenu();
                string userInput = InputValidator.GetUserOption();
                ProcessInput(userInput);
            }
        }

        private static void InitializeDatabase()
        {
            using DAL dal = new();
            dal.InitializeDatabase();
        }

        private static void ProcessInput(string userInput)
        {
            switch (userInput)
            {
                case "v":
                    ViewTable();
                    break;
                case "a":
                    AddEntry();
                    break;
                case "d":
                    DeleteEntry();
                    break;
                case "u":
                    UpdateEntry();
                    break;
                case "0":
                    ExitApp();
                    break;
                default:
                    break;
            }
        }

        private static void ViewTable()
        {
            using DAL dal = new();
            List<CodingSession> sessions = dal.GetCodingSessions();
            string output = string.Empty;
            foreach (CodingSession codingSession in sessions)
            {
                output += $"{codingSession}\n";
            }
            Console.WriteLine(output);
        }


        private static void AddEntry()
        {
            Console.WriteLine("Not implemented");
        }

        private static void DeleteEntry()
        {
            Console.WriteLine("Not implemented");
        }

        private static void UpdateEntry()
        {
            Console.WriteLine("Not implemented");
        }

        private static void ExitApp()
        {
            Environment.Exit(0);
        }

        private static void DisplayOptionsMenu()
        {
            Console.WriteLine("\nChoose an action from the following list:");
            Console.WriteLine("\tv - View your tracker");
            Console.WriteLine("\ta - Add a new entry");
            Console.WriteLine("\td - Delete an entry");
            Console.WriteLine("\tu - Update an entry");
            Console.WriteLine("\tc - Create new habit");
            Console.WriteLine("\t0 - Quit this application");
            Console.Write("Your option? ");
        }

        private static void DisplayTitle()
        {
            Console.WriteLine("Coding Tracker\r");
            Console.WriteLine("-------------\n");
        }
    }
}

