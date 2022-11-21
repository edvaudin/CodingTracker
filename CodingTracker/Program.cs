using ConsoleTableExt;
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Viewer.DisplayTitle();
            UserController.InitializeDatabase();

            while (true)
            {
                Viewer.DisplayOptionsMenu();
                string userInput = UserInput.GetUserOption();
                UserController.ProcessInput(userInput);
            }
        }
    }
}

