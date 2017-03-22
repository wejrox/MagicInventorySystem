using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    // Static class for displaying data
    public static class Formatter
    {

        // Displays a table using the data given
        public static int DisplayTable(string title, string heading, List<string> formattedData)
        {
            int option = -1;
            // Generate the title
            Console.WriteLine("======================================");
            Console.WriteLine(title);
            Console.WriteLine("======================================");
            Console.WriteLine();

            // Print heading
            Console.WriteLine(heading);
            // Print each line
            foreach (string s in formattedData)
                Console.WriteLine(s);

            Console.WriteLine();
            Console.WriteLine("Enter a request to process: ");

            string op = "";
            try
            {
                op = "" + Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine();
                option = int.Parse(op);
                if (option < 0 || option > formattedData.Count + 1)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, formattedData.Count + 1);
            }
            catch (Exception)
            {
                Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, formattedData.Count + 1);
            }

            return option;
        }

	    // Displays a table using the data given, includes the ability to use pages.
        public static int DisplayTable(string title, string heading, List<string> formattedData, string displayOptionsText, char[] displayOptions)
        {
            int option = -1;
            // Generate the title
            Console.WriteLine("======================================");
            Console.WriteLine(title);
            Console.WriteLine("======================================");
            Console.WriteLine();

            // Print heading
            Console.WriteLine(heading);
            // Print each line
            foreach (string s in formattedData)
                Console.WriteLine(s);

            Console.WriteLine();
            Console.WriteLine(displayOptionsText);
            bool isValidOption = false;
            char op;

            while (isValidOption != true)
            {
                op = Console.ReadKey().KeyChar;

                foreach (char c in displayOptions)
                {
                    if (c == op)
                    {
                        isValidOption = true;
                        break;
                    }
                }
            }
            /*
            try
            {
                op = "" + Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine();
                option = int.Parse(op);
                if (option < 0 || option > formattedData.Count + 1)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, formattedData.Count + 1);
            }
            catch (Exception)
            {
                Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, formattedData.Count + 1);
            }
            */
            return option;
        }

        // Display a menu and return the option chosen
        // Takes a title, and the options to choose from
        public static int DisplayMenu(string title, string[] options)
        {
            // option entered
            int option = -1;

            // Repeatedly prints as long as the option entered doesn't exist
            while (option < 0 || option > options.Length + 1)
            {
                Console.WriteLine("======================================");
                Console.WriteLine(title);
                Console.WriteLine("======================================");
                Console.WriteLine();
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine("{0, 2}. {1}", i + 1, options[i]);
                }

                Console.WriteLine("{0, 2}. Quit", options.Length + 1);

                Console.WriteLine();
                Console.WriteLine("Enter an option: ");

                // Placeholder for the option chosen, for catch
                string op = "";

                // Attempts to parse the option entered by the user.
                // Will tell user if option is invalid.
                try
                {
                    op = "" + Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.WriteLine();
                    option = int.Parse(op);
                    if (option < 0 || option > options.Length + 1)
                        Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, options.Length + 1);
                }
                catch (Exception)
                {
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", op, options.Length + 1);
                }
            }

            if (option == options.Length + 1)
                Environment.Exit(0);

            return option;
        }

        // Gets a boolean response from the user
        public static bool GetBooleanResponse()
        {
            Console.WriteLine("True/False?");
            string response = "";
            bool bResponse = false;

            while (response != "T" && response != "True" && response != "true" && response != "TRUE" &&
                    response != "F" && response != "False" && response != "false" && response != "FALSE")
            {
                try
                {
                    response = Console.ReadLine();

                    if (response != "T" && response != "True" && response != "true" && response != "TRUE" &&
                    response != "F" && response != "False" && response != "false" && response != "FALSE")
                    {
                        Console.WriteLine("\'{0}\' is not a valid option. Please enter \'T/True/true/TRUE\' or \'F/False/false/FALSE\'.", response);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter \'T/True/true/TRUE\' or \'F/False/false/FALSE\'.", response);
                }
            }

            if (response == "T" || response == "True" || response == "true" || response == "TRUE")
                bResponse = true;
            else if (response == "F" || response == "False" || response == "false" || response == "FALSE")
                bResponse = false;

            return bResponse;
        }
    }
}
