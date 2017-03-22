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

        // Display a menu and return the option chosen
        // Takes a title, and the options to choose from
        public int DisplayMenu()
        {
            // option entered
            int option = -1;

            // Repeatedly prints as long as the option entered doesn't exist
            while (option < 0 || option > Options.Count)
            {
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
                    if (option < 0 || option > Options.Count)
                        Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", option, Options.Count);
                }
            }
            Console.Clear();
            return option;
        }

        // Get the integer equivalent of the option selected
        public int GetIntOptionSelected()
        {
            int opt = -1;
            try
            {
                string op = Console.ReadLine();
                Console.WriteLine();
                opt = int.Parse(op);
            }
            catch (Exception) { return -1; } // Didn't work for whatever reason (probably not a number, re-enter)

            return opt;
        }
    }
}

