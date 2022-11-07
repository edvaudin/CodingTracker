using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker
{
    internal static class InputValidator
    {
        public static bool IsValidOption(string? input)
        {
            string[] validOptions = { "v", "a", "d", "u", "0" };
            foreach (string validOption in validOptions)
            {
                if (input == validOption)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetUserOption()
        {
            string input = Console.ReadLine();
            while (!IsValidOption(input))
            {
                Console.Write("This is not a valid input. Please enter one of the above options: ");
                input = Console.ReadLine();
            }
            return input;
        }
    }
}
