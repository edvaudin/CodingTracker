﻿using ConsoleTableExt;
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Globalization;

namespace CodingTracker
{
    class Program
    {
        private static readonly DAL dal = new DAL();
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
            List<CodingSession> sessions = dal.GetCodingSessions();
            string output = string.Empty;
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

        private static void AddEntry()
        {
            string startTime = InputValidator.GetStartTime();
            string endTime = InputValidator.GetEndTime(ConvertToDate(startTime));
            TimeSpan duration = CalculateDuration(startTime, endTime);
            dal.AddEntry(startTime, endTime, duration.ToString());
        }

        private static DateTime ConvertToDate(string time)
        {
            return DateTime.ParseExact(time, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None);
        }

        private static TimeSpan CalculateDuration(string startTimeStr, string endTimeStr)
        {
            DateTime startTime = ConvertToDate(startTimeStr);
            DateTime endTime = ConvertToDate(endTimeStr);
            return endTime.Subtract(startTime);
        }

        private static void DeleteEntry()
        {
            ViewTable();
            int id = InputValidator.GetIdForRemoval();
            if (id == -1) { return; }
            dal.DeleteEntry(id);
            Console.WriteLine("Successfully delteded entry " + id);
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

