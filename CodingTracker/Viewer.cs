namespace CodingTracker
{
    internal static class Viewer
    {
        public static void DisplayPromptForTime(string verb)
        {
            Console.WriteLine($"When did you {verb} coding? Please answer below in the following format: dd-MM-yy HH-mm-ss");
        }

        public static void DisplayFilterOptionsMenu()
        {
            Console.WriteLine("\nChoose which records you want to view from the following list:");
            Console.WriteLine("\ta - all");
            Console.WriteLine("\td - today");
            Console.WriteLine("\tw - this week");
            Console.WriteLine("\tm - this month");
            Console.WriteLine("\ty - this year");
            Console.Write("Your option? ");
        }

        public static void DisplayOptionsMenu()
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

        public static void DisplayTitle()
        {
            Console.WriteLine("Coding Tracker\r");
            Console.WriteLine("-------------\n");
        }
    }
}