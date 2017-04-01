using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MagicInventorySystem
{
    class MainMenu : Menu
    {
        // Menus to run
        OwnerMenu ownerMenu;
        CustomerMenu customerMenu;
        FranchiseMenu franchiseMenu;

        public MainMenu()
        {
            // Initialize the JSON files if they don't exist
            JSONUtility.InitializeJSONFiles();
            /*
            // Create the menus
            ownerMenu = new OwnerMenu();
            customerMenu = new CustomerMenu();
            franchiseMenu = new FranchiseMenu();            
            */
            //Title name
            Title = "Main Menu";
            Options = new List<string> {
                "Owner",
                "Franchise Owner",
                "Customer",
                "Exit"
            };

            DisplayOptionsText = "Enter selection number";
        }

        public void Run()
        {
            // Set Window size
            Console.WindowHeight = 45;
            Console.BufferHeight = 45;
            Console.WindowWidth = 100;
            Console.BufferWidth = 100;
            // Run forever, as the user can exit in the menu
            while (true)
            {
                HandleMenu();
            }
        }

        public override void HandleMenu()
        {
            // Selected Option
            int op = -1;

            op = DisplayMenu();

            switch (op)
            {
                case 1:
                    ownerMenu = new OwnerMenu(); // Initialize when used
                    ownerMenu.HandleMenu();
                    break;
                case 2:
                    franchiseMenu = new FranchiseMenu(); // Initialize when used
                    franchiseMenu.SelectStore(ownerMenu.Stores);
                    franchiseMenu.HandleMenu();
                    break;
                case 3:
                    customerMenu = new CustomerMenu(); // Initialize when used
                    customerMenu.SelectStore(ownerMenu.Stores);
                    customerMenu.HandleMenu();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
