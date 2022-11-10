﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker
{
    internal static class UserInput
    {
        public static string GetStartTime()
        {
            string input = Console.ReadLine();

            while (!Validator.IsValidDateInput(input))
            {
                Console.WriteLine("\nInvalid date and time. Use the format: dd-MM-yy HH-mm-ss.");
                input = Console.ReadLine();
            }
            return input;
        }

        public static string GetEndTime(DateTime startTime)
        {
            string input = Console.ReadLine();

            while (!Validator.IsValidDateInput(input))
            {
                Console.WriteLine("\nInvalid date and time. Use the format: dd-MM-yy HH-mm-ss.");
                input = Console.ReadLine();
            }
            if (DateTime.ParseExact(input, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None) < startTime)
            {
                Console.WriteLine("\nYou cannot have finished coding before you started! Enter a different end time.");
                input = GetEndTime(startTime);
            }
            return input;
        }

        public static int GetIdForUpdate()
        {
            DAL dal = new DAL();
            List<int> validIds = dal.GetCodingSessions().Select(o => o.id).ToList();
            while (true)
            {
                if (Int32.TryParse(Console.ReadLine(), out int result))
                {
                    if (validIds.Contains(result) || result == -1)
                    {
                        return result;
                    }
                }
                Console.Write("\nThis is not a valid id, please enter a number or to return to main menu type '-1': ");
            }

        }

        public static string GetUserFilterChoice()
        {
            string input = Console.ReadLine();
            while (!Validator.IsValidFilterOption(input))
            {
                Console.Write("\nThis is not a valid input. Please enter one of the above options: ");
                input = Console.ReadLine();
            }
            return input;
        }

        public static string GetUserOption()
        {
            string input = Console.ReadLine();
            while (!Validator.IsValidOption(input))
            {
                Console.Write("\nThis is not a valid input. Please enter one of the above options: ");
                input = Console.ReadLine();
            }
            return input;
        }
    }
}