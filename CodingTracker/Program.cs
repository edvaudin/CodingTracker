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
            Viewer.DisplayTitle();
            InitializeDatabase();

            while (true)
            {
                Viewer.DisplayOptionsMenu();
                string userInput = UserInput.GetUserOption();
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
            Viewer.DisplayFilterOptionsMenu();
            string filter = UserInput.GetUserFilterChoice();
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
                "d" => allSessions.Where(s => Validator.ConvertToDate(s.startTime).Day == DateTime.Now.Day).ToList(),
                "w" => allSessions.Where(s => Validator.ConvertToDate(s.startTime).Day >= DateTime.Now.Day - 7).ToList(),
                "m" => allSessions.Where(s => Validator.ConvertToDate(s.startTime).Month == DateTime.Now.Month).ToList(),
                "y" => allSessions.Where(s => Validator.ConvertToDate(s.startTime).Year == DateTime.Now.Year).ToList(),
                _ => allSessions,
            };
        }

        private static void AddEntry()
        {
            Viewer.DisplayPromptForTime("start");
            string startTime = UserInput.GetStartTime();
            Viewer.DisplayPromptForTime("finish");
            string endTime = UserInput.GetEndTime(Validator.ConvertToDate(startTime));
            TimeSpan duration = Validator.CalculateDuration(startTime, endTime);
            dal.AddEntry(startTime, endTime, duration.ToString());
        }

        private static void DeleteEntry()
        {
            ViewTableFiltered();
            Console.Write($"Which entry do you want to remove? ");
            int id = UserInput.GetIdForUpdate();
            if (id == -1) { return; }
            dal.DeleteEntry(id);
            Console.WriteLine("\nSuccessfully delteded entry " + id);
        }

        private static void UpdateEntry()
        {
            ViewTable(dal.GetCodingSessions());
            Console.Write($"Which entry do you want to update? ");
            int id = UserInput.GetIdForUpdate();
            Viewer.DisplayPromptForTime("start");
            string startTime = UserInput.GetStartTime();
            Viewer.DisplayPromptForTime("finish");
            string endTime = UserInput.GetEndTime(Validator.ConvertToDate(startTime));
            TimeSpan duration = Validator.CalculateDuration(startTime, endTime);
            dal.UpdateEntry(id, startTime, endTime, duration.ToString());
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
                dal.AddEntry(Validator.ConvertFromDate(stopwatchStartTime), Validator.ConvertFromDate(stopwatchEndTime), duration.ToString());
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
    }
}

