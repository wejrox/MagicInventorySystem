using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    class FranchiseMenu : Menu
    {
        OwnerMenu _OwnerMenu;
        Store CurStore { get; set; }

        public FranchiseMenu(OwnerMenu o) : base()
        {
            // Set the owner menu for when we need to make a stock request (use JSON instead?)
            _OwnerMenu = o;

            Title = "Franchise Holder Menu";
            Options = new List<string>
            {
                "Display Inventory",
                "Display Inventory (Threshold)",
                "Add New Inventory Item",
                "Return to Main Menu",
                "Exit"
            };
        }

        public override void HandleMenu()
        {
            while (ShouldExitMenu == false)
            {
                int op = DisplayMenu();

                switch (op)
                {
                    case 1:
                        // Display everything in the inventory
                        DisplayInventory(false);
                        break;
                    case 2:
                        // Display everything in the inventory that meets the threshold
                        DisplayInventory(true);
                        break;
                    case 3:
                        AddNewInventoryItem();
                        break;
                    case 4:
                        return;
                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // Displays the inventory, prompts for threshold for restock, prompts for threshold filter if required
        public void DisplayInventory(bool hasThreshold)
        {
            // Get Threshold
            Console.Write("How low can an item's stock-level be before restocking? ");
            int threshold = -1;
            while (threshold < 0)
                threshold = GetIntOptionSelected();

            // Headings
            Console.WriteLine("======================================");
            Console.WriteLine(DefaultTitle);
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Franchise Inventory");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();

            // Display the headers
            Console.Write("{0, 4}", "ID");
            Console.Write("{0, 30}", "Name");
            Console.Write("{0, 15}", "StockLevel");
            Console.Write("{0, 10}", "Re-Stock");
            Console.WriteLine();

            // Display items
            if (CurStore.StoreInventory != null && CurStore.StoreInventory.Count > 0)
            {
                for(int i = 0; i < CurStore.StoreInventory.Count; i++)
                {
                    // Needs restock?
                    bool restock = false;
                    if (CurStore.StoreInventory[i].StockLevel <= threshold)
                        restock = true;

                    // Display everything if no threshold
                    if (!hasThreshold)
                    {
                        Console.Write("{0, 4}", CurStore.StoreInventory[i].Id - 1);
                        Console.Write("{0, 30}", CurStore.StoreInventory[i].Name);
                        Console.Write("{0, 15}", CurStore.StoreInventory[i].StockLevel);
                        Console.Write("{0, 10}", restock);
                        Console.WriteLine();
                    }
                    // Display only if meets restock req.
                    else if (hasThreshold && restock)
                    {
                        Console.Write("{0, 4}", CurStore.StoreInventory[i].Id - 1);
                        Console.Write("{0, 30}", CurStore.StoreInventory[i].Name);
                        Console.Write("{0, 15}", CurStore.StoreInventory[i].StockLevel);
                        Console.Write("{0, 10}", restock);
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine();

            // Get requested item
            Console.Write("Enter Request to process: ");
            int op = -1;
            while (op < 0 || op > CurStore.StoreInventory.Count - 1)
            {
                op = GetIntOptionSelected();
                if(op < 0 || op > CurStore.StoreInventory.Count - 1)
                    Console.WriteLine("\'{0}\' is not a valid option on this list, or you did not enter a number.", op);
                // Don't let the user re-stock if it doesn't need it
                else if (CurStore.StoreInventory[op].StockLevel > threshold)
                {
                    Console.WriteLine("This item doesn't meet the threshold requirements for restocking.");
                    op = -1; // Reset option so the loop continues
                }
            }

            // How many to request?
            int amt = -1;
            while (amt < 0 || amt > 99) // Nobody should order more than 99?
                amt = GetIntOptionSelected();

            // Create the request
            StockRequest sr = new StockRequest(CurStore.StoreInventory[op], CurStore, amt);

            // Handle Request
            _OwnerMenu.AddStockRequest(sr);
        }

        // Add an inventory item from the owner stock
        public void AddNewInventoryItem()
        {
            // Headings
            Console.WriteLine("======================================");
            Console.WriteLine(Title);
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Add a new inventory item");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();

            // Display the headers
            Console.Write("{0, 4}", "ID");
            Console.Write("{0, 30}", "Name");
            Console.Write("{0, 15}", "StockLevel");
            Console.WriteLine();

            // Display Owner's stock
            if (_OwnerMenu._stock != null && _OwnerMenu._stock.Count > 0)
            {
                for(int i = 0; i < _OwnerMenu._stock.Count; i++)
                {
                    // Display if the current store doesn't contain the item
                    bool shouldDisplay = true;

                    // Check that this item doesn't exist in your inventory
                    for (int x = 0; x < CurStore.StoreInventory.Count; x++)
                    {
                        // Check names
                        if (CurStore.StoreInventory[x].Name == _OwnerMenu._stock[i].Name)
                        {
                            // Don't display it
                            shouldDisplay = false;
                            break; // Leave loop if item been found
                        }
                    }

                    // Print item if it doesn't exist
                    if (shouldDisplay)
                    {
                        Console.Write("{0, 4}", _OwnerMenu._stock[i].Id - 1);
                        Console.Write("{0, 30}", _OwnerMenu._stock[i].Name);
                        Console.Write("{0, 15}", _OwnerMenu._stock[i].StockLevel);
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine();
            // Get the index of the item to add to stock
            Console.Write("Please enter the ID of the item you would like to add: ");
            int op = -1;
            while (op < 0 || op > _OwnerMenu._stock.Count - 1)
            {
                op = GetIntOptionSelected();
                if (op < 0 || op > _OwnerMenu._stock.Count - 1)
                    Console.WriteLine("\'{0}\' is not a valid option on this list, or you did not enter a number.", op);
            }

            // Don't let the user add to inventory if it already exists 
            // (deals with numbers entered that aren't displayed)
            bool shouldAdd = true;
            for (int i = 0; i < _OwnerMenu._stock.Count; i++)
            {
                // Check that this item doesn't exist in your inventory
                for (int x = 0; x < CurStore.StoreInventory.Count; x++)
                {
                    // Check names
                    if (CurStore.StoreInventory[x].Name == _OwnerMenu._stock[i].Name)
                    {
                        // Don't display it
                        shouldAdd = false;
                        break; // Leave loop if item been found
                    }
                }
            }

            // Shouldn't be able to add the item?
            if(!shouldAdd)
            {
                Console.WriteLine("The item you have selected already exists in your inventory! (Owner Store ID: {0}).\nPress any key to return to Franchise Menu", op);
                Console.ReadKey();
                Console.Clear();
                return;
            }
            
            // Copy the item, give default stock level
            Item item = new Item(_OwnerMenu._stock[op].Name, 10, _OwnerMenu._stock[op].Price);
            // Add the item
            CurStore.AddInventoryItem(item);

            Console.WriteLine("\'{0}\' has been added to your inventory!\nPress any key to return to the Franchise Holder Menu", item.Name);
            Console.ReadKey();
        }

        // Set which store is currently in use
        public void SelectStore(List<Store> stores)
        {
            // option entered
            int option = -1;

            // Repeatedly prints as long as the option entered doesn't exist
            while (option < 0 || option > stores.Count - 1)
            {
                Console.WriteLine("======================================");
                Console.WriteLine(DefaultTitle);
                Console.WriteLine("======================================");
                Console.WriteLine();
                Console.WriteLine("Select a store");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine();

                foreach (Store s in stores)
                {
                    Console.Write("{0, 4}", (s.Id + 1) + ". ");
                    Console.Write("{0, 10}", s.StoreName);
                    Console.WriteLine();
                }

                option = GetIntOptionSelected() - 1;
                Console.Clear();
            }

            CurStore = stores[option];
            Title = "Franchise Holder Menu (" + CurStore.StoreName + ")";
            Console.Clear();
        }
    }
}
