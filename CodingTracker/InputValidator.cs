using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker
{
    internal static class InputValidator
    {
        public static bool IsValidOption(string? input)
        {
            string[] validOptions = { "v", "a", "d", "u", "srt", "stp", "0" };
            foreach (string validOption in validOptions)
            {
                if (input == validOption)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidFilterOption(string? input)
        {
            string[] validOptions = { "a", "d", "w", "m", "y" };
            foreach (string validOption in validOptions)
            {
                if (input == validOption)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetStartTime()
        {
            Console.WriteLine($"When did you start coding? Please answer below in the following format: dd-MM-yy HH-mm-ss");
            string input = Console.ReadLine();
           
            while (!DateTime.TryParseExact(input, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\nInvalid date and time. Use the format: dd-MM-yy HH-mm-ss.");
                input = Console.ReadLine();
            }
            return input;
        }

        public static string GetEndTime(DateTime startTime)
        {
            Console.WriteLine($"When did you finish coding? Please answer below in the following format: dd-MM-yy HH-mm-ss");
            string input = Console.ReadLine();
            
            while (!DateTime.TryParseExact(input, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\nInvalid date and time. Use the format: dd-MM-yy HH-mm-ss.");
                input = Console.ReadLine();
            }
            if (DateTime.ParseExact(input, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None) < startTime)
            {
                Console.WriteLine("\nYou cannot have finished coding before you started! Try again.");
                input = GetEndTime(startTime);
            }
            return input;
        }

        public static int GetIdForUpdate(string verb)
        {
            Console.Write($"Which entry do you want to {verb}? ");
            DAL dal = new DAL();
            {
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
        }

        public static string GetUserFilterChoice()
        {
            Console.WriteLine("\nChoose which records you want to view from the following list:");
            Console.WriteLine("\ta - all");
            Console.WriteLine("\td - today");
            Console.WriteLine("\tw - this week");
            Console.WriteLine("\tm - this month");
            Console.WriteLine("\ty - this year");
            Console.Write("Your option? ");
            string input = Console.ReadLine();
            while (!IsValidFilterOption(input))
            {
                Console.Write("\nThis is not a valid input. Please enter one of the above options: ");
                input = Console.ReadLine();
            }
            return input;
        }

        public static string GetUserOption()
        {
            string input = Console.ReadLine();
            while (!IsValidOption(input))
            {
                Console.Write("\nThis is not a valid input. Please enter one of the above options: ");
                input = Console.ReadLine();
            }
            return input;
        }
    }
}
