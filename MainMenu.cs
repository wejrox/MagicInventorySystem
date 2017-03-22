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
            // Initialize the JSON files
            InitializeJSON();

            // Create the menus
            ownerMenu = new OwnerMenu();
            customerMenu = new CustomerMenu();
            franchiseMenu = new FranchiseMenu();

            

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

        public override void 

        public void Run()
        {
            // Selected Option
            int op = -1;
            // Run forever, as the user can exit in the menu
            while (true)
            {
                op = DisplayMenu();

                switch(op)
                {
                    case 1:
                        //ownerMenu.DisplayMenu();
                        break;
                    case 2:
                        //franchiseMenu.DisplayMenu();
                        break;
                    case 3:
                        customerMenu.SelectStore(ownerMenu._stores);
                        customerMenu.HandleMenu();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public void InitializeJSON()
        {
            string[] JsonFiles = {
                @"dat\Owners_inventory.txt",
                @"dat\Stock_requests.txt",
                @"dat\Altona_inventory.txt",
                @"dat\Epping_inventory.txt",
                @"dat\Melbourne_inventory.txt",
                @"dat\Springvale_inventory.txt",
                @"dat\Olinda_inventory.txt"
            };

            // Initialize the directory
            Directory.CreateDirectory(@"dat");
            // Create each of the JSON Inventories if they don't exist
            foreach (string s in JsonFiles)
            {
                if (!File.Exists(s))
                {
                    FileStream f = File.Create(s);
                    f.Close();
                }
                    
            }
            
        }
    }
}
