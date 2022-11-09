using ConsoleTableExt;
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace CodingTracker
{
    class Program
    {
        private static readonly DAL dal = new DAL();
        private static Stopwatch stopwatch = new Stopwatch();
        private static DateTime stopwatchStartTime = DateTime.MinValue;
        private static DateTime stopwatchEndTime = DateTime.MinValue;
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
            DAL dal = new();
            dal.InitializeDatabase();
        }

        private static void ProcessInput(string userInput)
        {
            switch (userInput)
            {
                case "v":
                    ViewTableFiltered();
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
                case "srt":
                    StartStopwatch();
                    break;
                case "stp":
                    StopStopwatch();
                    break;
                case "0":
                    ExitApp();
                    break;
                default:
                    break;
            }
        }

        private static void ViewTableFiltered()
        {
            string filter = InputValidator.GetUserFilterChoice();
            List<CodingSession> sessions = dal.GetCodingSessions();
            sessions = FilterSessions(sessions, filter);
            ViewTable(sessions);
        }

        private static void ViewTable(List<CodingSession> sessions)
        {
            var tableData = new List<List<object>>();
            foreach (CodingSession codingSession in sessions)
            {
                tableData.Add(new List<object>
                {   codingSession.id,
                    codingSession.duration,
                    codingSession.GetPretifiedTime(codingSession.startTime),
                    codingSession.GetPretifiedTime(codingSession.endTime)
                });
            }
            ConsoleTableBuilder.From(tableData).WithTitle("Your Coding Time").WithColumn("Id", "Duration", "Start Time", "End Time").ExportAndWriteLine();
        }

        private static List<CodingSession> FilterSessions(List<CodingSession> allSessions, string filterStr)
        {
            return filterStr switch
            {
                "a" => allSessions,
                "d" => allSessions.Where(s => ConvertToDate(s.startTime).Day == DateTime.Now.Day).ToList(),
                "w" => allSessions.Where(s => ConvertToDate(s.startTime).Day >= DateTime.Now.Day - 7).ToList(),
                "m" => allSessions.Where(s => ConvertToDate(s.startTime).Month == DateTime.Now.Month).ToList(),
                "y" => allSessions.Where(s => ConvertToDate(s.startTime).Year == DateTime.Now.Year).ToList(),
                _ => allSessions,
            };
        }

        private static void AddEntry()
        {
            string startTime = InputValidator.GetStartTime();
            string endTime = InputValidator.GetEndTime(ConvertToDate(startTime));
            TimeSpan duration = CalculateDuration(startTime, endTime);
            dal.AddEntry(startTime, endTime, duration.ToString());
        }

        private static void DeleteEntry()
        {
            ViewTableFiltered();
            int id = InputValidator.GetIdForUpdate("remove");
            if (id == -1) { return; }
            dal.DeleteEntry(id);
            Console.WriteLine("\nSuccessfully delteded entry " + id);
        }

        private static void UpdateEntry()
        {
            ViewTable(dal.GetCodingSessions());
            int id = InputValidator.GetIdForUpdate("update");
            string startTime = InputValidator.GetStartTime();
            string endTime = InputValidator.GetEndTime(ConvertToDate(startTime));
            TimeSpan duration = CalculateDuration(startTime, endTime);
            dal.UpdateEntry(id, startTime, endTime, duration.ToString());
        }

        private static DateTime ConvertToDate(string time)
        {
            return DateTime.ParseExact(time, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None);
        }

        private static string ConvertFromDate(DateTime time)
        {
            return time.ToString(@"dd-MM-yy HH-mm-ss");
        }

        private static TimeSpan CalculateDuration(string startTimeStr, string endTimeStr)
        {
            DateTime startTime = ConvertToDate(startTimeStr);
            DateTime endTime = ConvertToDate(endTimeStr);
            return endTime.Subtract(startTime);
        }

        private static void StartStopwatch()
        {
            Console.WriteLine("\nWe've started a stopwatch - get coding!");
            stopwatch.Start();
            stopwatchStartTime = DateTime.Now;
        }

        private static void StopStopwatch()
        {
            if (stopwatch != null)
            {
                stopwatch.Stop();
                stopwatchEndTime = DateTime.Now;
                TimeSpan duration = stopwatch.Elapsed;
                dal.AddEntry(ConvertFromDate(stopwatchStartTime), ConvertFromDate(stopwatchEndTime), duration.ToString());
                Console.WriteLine("Stopwatch time recorded");
            }
            else
            {
                Console.WriteLine("\nNo stopwatch is running.");
            }
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
            Console.WriteLine("\tsrt - Start stopwatch");
            Console.WriteLine("\tstp - Stop stopwatch");
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

