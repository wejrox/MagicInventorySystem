﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    class FranchiseMenu : Menu
    {
        Store CurStore { get; set; }

        public FranchiseMenu() : base()
        {
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
            // Load the most recent inventory 
            CurStore.LoadInventory();

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
            if (CurStore.StoreInventory?.Any() != true)
            {
                Console.WriteLine("No inventory available for this store. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            // Get Threshold
            Console.WriteLine("How low can an item's stock-level be before restocking? \n(type c or cancel to return to the previous menu)");
            int threshold = -1;
            while (threshold < 0)
            {
                threshold = GetIntOptionSelected();
                // If the user cancels
                if (threshold == -2)
                    return;
            }
            // Clear for next page
            Console.Clear();

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

            List<Item> posReq = new List<Item>();
            // Display items
            if (CurStore.StoreInventory != null && CurStore.StoreInventory.Count > 0)
            {
                for (int i = 0; i < CurStore.StoreInventory.Count; i++)
                {
                    // Needs restock?
                    bool restock = false;
                    if (CurStore.StoreInventory[i].StockLevel <= threshold)
                        restock = true;

                    // Display everything if no threshold
                    if (!hasThreshold)
                    {
                        posReq.Add(CurStore.StoreInventory[i]);
                    }
                    // Must need restocking to add to the list
                    else if (hasThreshold && restock)
                    {
                        posReq.Add(CurStore.StoreInventory[i]);
                    }
                }

                for (int i = 0; i < posReq.Count; i++)
                {
                    Console.Write("{0, 4}", i);
                    Console.Write("{0, 30}", posReq[i].Name);
                    Console.Write("{0, 15}", posReq[i].StockLevel);
                    if (posReq[i].StockLevel <= threshold)
                        Console.Write("{0, 10}", "TRUE");
                    else
                        Console.Write("{0, 10}", "FALSE");
                    Console.WriteLine();
                }
            }

            Console.WriteLine();

            #region Validate Request
            // Get requested item
            Console.WriteLine ("Enter Request to process\n(type c or cancel to return to the previous menu): ");
            int op = -1;
            while (op < 0 || op > posReq.Count - 1)
            {
                op = GetIntOptionSelected();
                // If the user cancels
                if (op == -2)
                    return;
                if(op < 0 || op > posReq.Count - 1)
                    Console.WriteLine("\'{0}\' is not a valid option on this list.", op);
                // Don't let the user re-stock if it doesn't need it
                else if (posReq[op].StockLevel > threshold)
                {
                    Console.WriteLine("This item doesn't meet the threshold requirements for restocking. Would you like to request anyway?(Y/y or N/n)");
                    string op1 = "";
                    while (op1.ToLower() != "y" && op1.ToLower() != "n")
                    {
                        op1 = Console.ReadLine();

                        if (op1.ToLower() != "y" && op1.ToLower() != "n")
                            Console.WriteLine("\'{0}\' is not a valid option", op1);
                    }

                    // Reset the loop if user wants to abort
                    if(op1.ToLower() == "n")
                        op = -1;

                    // If user wants to continue nothing is changed and the option will be processed
                }
            }
            #endregion

            // How many to request?
            Console.WriteLine("How many would you like? (Max 20)");
            int amt = -1;
            while (amt < 1 || amt > 20) // Nobody should order more than 99?
            {
                amt = GetIntOptionSelected();
                // If the user cancels
                if (amt == -2)
                    return;
                if (amt < 1)
                    Console.WriteLine("You must request at least 1 stock");
                else if (amt > 20)
                    Console.WriteLine("You can't request more than 20 stock as that is the maximum we have at any given time.");
            }

            // Create the request
            StockRequest sr = new StockRequest(CurStore.StoreInventory[op], CurStore.Id, amt);
            // Handle Request
            JSONUtility.AddAndSaveStockRequest(sr);

            Console.WriteLine("The owner has been notified of your stock request.\nPress any key to return to the Franchise Holder menu...");
            Console.ReadKey();
        }

        // Add an inventory item from the owner stock
        public void AddNewInventoryItem()
        {
            List<Item> ownerStock = JSONUtility.GetInventory("Owners");

            if (ownerStock?.Any() != true)
            {
                Console.WriteLine("Owner has no stock available to add. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

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

            List<Item> dispItems = new List<Item>();

            // Generate stock items to display
            for (int i = 0; i < ownerStock.Count; i++)
            {
                // Display if the current store doesn't contain the item
                bool shouldDisplay = true;

                // Check that this item doesn't exist in your inventory
                for (int x = 0; x < CurStore.StoreInventory.Count; x++)
                {
                    // Check names
                    if (CurStore.StoreInventory[x].Name == ownerStock[i].Name)
                    {
                        // Don't display it
                        shouldDisplay = false;
                        break; // Leave loop if item been found
                    }
                }

                if(shouldDisplay)
                    dispItems.Add(ownerStock[i]);
            }

            // Print items
            for(int i = 0; i < dispItems.Count; i++)
            {
                Console.Write("{0, 4}", i);
                Console.Write("{0, 30}", dispItems[i].Name);
                Console.Write("{0, 15}", dispItems[i].StockLevel);
                Console.WriteLine();
            }

            Console.WriteLine();
            // Get the index of the item to add to stock
            Console.Write("Please enter the ID of the item you would like to add: ");
            int op = -1;
            while (op < 0 || op > dispItems.Count - 1)
            {
                op = GetIntOptionSelected();
                if (op == -2)
                    return;
                if (op < 0 || op > dispItems.Count - 1)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 0 to {1}.", op, dispItems.Count - 1);
            }

            // Copy the item, give default stock level
            Item item = new Item(dispItems[op].Name, 0, dispItems[op].Price);
            // Add the item
            CurStore.AddInventoryItem(item);

            // Create the request of the last item added
            StockRequest sr = new StockRequest(CurStore.StoreInventory[CurStore.StoreInventory.Count - 1], CurStore.Id, 10);
            // Handle Request
            JSONUtility.AddAndSaveStockRequest(sr);

            Console.WriteLine("\'{0}\' has been added to your inventory, the Owner will process your stock request. \nPress any key to return to the Franchise Holder Menu", item.Name);
            Console.ReadKey();
        }

        // Set which store is currently in use
        public void SelectStore(List<Store> stores)
        {
            // option entered
            int option = -1;

            // Print store menu
            Console.WriteLine("======================================");
            Console.WriteLine(DefaultTitle);
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Select a store (type in the ID desired)");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();

            foreach (Store s in stores)
            {
                Console.Write("{0, 4}", s.Id + 1 + ". ");
                Console.Write("{0, 10}", s.StoreName);
                Console.WriteLine();
            }

            // Get the option selected
            while (option < 1 || option > stores.Count)
            {
                option = GetIntOptionSelected();
                if (option < 1 || option > stores.Count)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 1 to {1}.", option, stores.Count);
            }

            CurStore = stores[option - 1];
            Title = "Franchise Holder Menu (" + CurStore.StoreName + ")";
        }
    }
}
