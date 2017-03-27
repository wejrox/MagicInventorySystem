using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    public abstract class Menu : IMenu
    {
        // Default menu title, which the store name is appended to
        protected string DefaultTitle = "Welcome to Marvellous Magic";

        protected List<string> Options { get; set; } // Menu options
        protected string Title { get; set; } // Title to display
        protected string DisplayOptionsText { get; set; } // The informational text to instruct users what to input for options
        protected bool ShouldExitMenu { get; set; } // Whether or not the menu should exit next loop

        public Menu()
        {
            ShouldExitMenu = false;
        }

        // Handling the option provided, implemented inside each menu
        public abstract void HandleMenu();

        // Display a menu and return the option chosen
        // Displays the options created in the constructor
        public int DisplayMenu()
        {
            // Set up for the menu
            Console.Clear();
            // option entered
            int option = -1;

            Console.WriteLine("======================================");
            Console.WriteLine(DefaultTitle);
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine(Title);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();
            // Print options
            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0, 2}. {1}", i + 1, Options[i]);
            }

            Console.WriteLine();
            Console.WriteLine("Enter an option: ");

            // Get option entered until valid
            while (option < 0 || option > Options.Count)
            {
                option = GetIntOptionSelected();
                if (option > Options.Count)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", option, Options.Count);
            }
            Console.Clear();
            return option;
        }

        // Get the integer equivalent of the string option entered
        public int GetIntOptionSelected()
        {
            int opt = -1;
            string op = "";
            try
            {
                op = Console.ReadLine();
                // Handle cancel
                if (op.ToLower() == "cancel" || op.ToLower() == "c")
                { 
                    return -2;
                }
                // Handle quit
                else if (op.ToLower() == "quit" || op.ToLower() == "q")
                    Environment.Exit(0);
                Console.WriteLine();
                opt = int.Parse(op);

                if (opt < 0)
                    Console.WriteLine("\'{0}\' is below 0. Please enter a number within range.", opt);
            }
            catch (Exception)
            {
                Console.WriteLine("\'{0}\' is not a number, please re-enter.", op);
                return -1;
            } // Didn't work for whatever reason (probably not a number, re-enter)

            return opt;
        }

        // Gets a boolean response from the user
        public bool GetBooleanResponse()
        {
            Console.WriteLine("True/False?");
            string response = "";
            bool bResponse = false;

            while (response.ToLower() != "t" && response.ToLower() != "true" &&
                    response.ToLower() != "f" && response.ToLower() != "false")
            {
                try
                {
                    response = Console.ReadLine();

                    if (response.ToLower() != "t" && response.ToLower() != "true" &&
                    response.ToLower() != "f" && response.ToLower() != "false")
                    {
                        Console.WriteLine("\'{0}\' is not a valid option. Please enter \'T/True/true/TRUE\' or \'F/False/false/FALSE\'.", response);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter \'T/True/true/TRUE\' or \'F/False/false/FALSE\'.", response);
                }
            }

            if (response.ToLower() == "t" || response.ToLower() == "true")
                bResponse = true;
            else if (response.ToLower() == "f" || response.ToLower() == "false")
                bResponse = false;

            return bResponse;
        }
    }
}

