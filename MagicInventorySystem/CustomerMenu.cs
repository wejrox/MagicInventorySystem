using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MagicInventorySystem
{
    class CustomerMenu : Menu
    {
        // The store currently selected, for products etc.
        Store CurStore { get; set; }

        // Current Customer Order
        private CustomerOrder _CustomerOrder { get; set; }

        // Main Customer Menu
        public char[] PagingKeys { get; private set; }

        // Amount of products to display per page
        public int ItemsPerPage { get; private set; }

        // Products Menu
        public CustomerMenu() : base()
        {
            //Title name
            Title = "Customer Menu";
            Options = new List<string> {
                "Display Products",
                "Display Workshops",
                "Return to Main Menu",
                "Exit"
            };
            DisplayOptionsText = "[Legend: 'P' Next Page | 'R' Return to Menu | 'C' Complete Transaction] \nEnter Item Number to Purchase or Function";
            PagingKeys = new char[] { 'P', 'p', 'R', 'r', 'C', 'c' };
            ItemsPerPage = 5;
            _CustomerOrder = new CustomerOrder();
        }

        // Handles the menu display
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
                        DisplayProducts();
                        break;
                    case 2:
                        DisplayWorkshops();
                        break;
                    case 3:
                        return;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // Display the product menu
        void DisplayProducts()
        {
            if (CurStore?.StoreInventory?.Any() != true)
            {
                Console.WriteLine("There is no stock to display. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            // option entered
            int option = -1;
            // The index to start the next page on
            int nextDisplayIndex = 0;
            // Loop the menu?
            bool keepLooping = true;

            // Run the menu while use hasn't selected a numeric option (exiting to menu is handled inside here already)
            while (keepLooping)
            {
                option = -1;
                Console.WriteLine("======================================");
                Console.WriteLine(DefaultTitle);
                Console.WriteLine("======================================");
                Console.WriteLine();
                Console.WriteLine("Products");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine();

                // Display the headers
                Console.Write("{0, 4}", "ID");
                Console.Write("{0, 30}", "Name");
                Console.Write("{0, 15}", "StockLevel");
                Console.WriteLine();

                for (int i = nextDisplayIndex; i < nextDisplayIndex + ItemsPerPage && i < CurStore.StoreInventory.Count; i++)
                {
                    Console.Write("{0, 4}", i);
                    Console.Write("{0, 30}", CurStore.StoreInventory[i].Name);
                    Console.Write("{0, 15}", CurStore.StoreInventory[i].StockLevel);
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine(DisplayOptionsText);

                // Repeatedly prints as long as the option entered doesn't exist
                while (option < 0 || option > CurStore.StoreInventory.Count)
                {
                    // Will handle paging, menu, finishing transaction if true
                    bool shouldHandleKey = false;
                    // Will purchase item if true
                    bool shouldHandleNumber = false;

                    string input;
                    char c;

                    #region Validate Input
                    // Keeps running until a valid option is entered
                    // Checks paging keys, then if its an index if its not a paging key
                    while (true)
                    {
                        input = Console.ReadLine();
                        c = input.ToCharArray()[0]; // First Key
                        // Check if its a paging key
                        foreach (char pk in PagingKeys)
                        {
                            if (c == pk)
                                shouldHandleKey = true;
                        }

                        // Only if its not a paging key
                        if (!shouldHandleKey)
                        {
                            try
                            {
                                option = int.Parse(input);

                                // Check if its a valid item being displayed
                                if (option >= nextDisplayIndex && option < (nextDisplayIndex + ItemsPerPage) && option < CurStore.StoreInventory.Count)
                                {
                                    shouldHandleNumber = true;
                                }
                                else
                                {
                                    Console.WriteLine("\'{0}\' is not a valid option on this list.", option);
                                }
                            }
                            // Alert user if character is not a number and is not in the list of accepted characters
                            catch (FormatException) { Console.WriteLine("\'{0}\' is not a valid option", c); }
                        }

                        // If its either a paging key or a valid option, break the loop
                        if (shouldHandleKey || shouldHandleNumber)
                            break;
                    }
                    #endregion

                    if (shouldHandleKey)
                    {
                        // Make sure that the loop knows to exit since its not a number
                        option = 1;
                        switch (c)
                        {
                            // Next Page
                            case 'p':
                            case 'P':
                                if ((nextDisplayIndex + ItemsPerPage) < (CurStore.StoreInventory.Count))
                                {
                                    nextDisplayIndex += ItemsPerPage;
                                }
                                Console.Clear();
                                break;
                            // Return to the menu
                            case 'r':
                            case 'R':
                                return;
                            // Complete and finalise transaction
                            case 'c':
                            case 'C':
                                FinalizeTransaction();
                                Console.Clear();
                                keepLooping = false;
                                break;
                        }
                    }
                    else if (shouldHandleNumber)
                    {
                        int amount = -1;
                        // Amount will be -1 until a number is entered, and this stops negative numbers to increase stock etc.
                        while (amount < 1 || amount > CurStore.StoreInventory[option].StockLevel)
                        {
                            Console.WriteLine();
                            Console.Write("Please enter amount to purchase: ");
                            amount = GetIntOptionSelected();

                            // If not enough stock, inform and give current stock level
                            if (CurStore.StoreInventory[option].StockLevel < amount)
                            {
                                Console.WriteLine("Amount entered is too high, not enough stock! Stock Level: {0}", CurStore.StoreInventory[option].StockLevel);
                            }
                        }

                        // Will only reach this point when the user has chosen an item and has entered a valid amount.
                        HandleProductPurchase(option, amount);

                        Console.Write("Would you like to purchase another item? (Y/y OR N/n)");
                        string response = "";

                        // Ask as long as the option is invalid
                        while (response.ToLower() != "y" && response.ToLower() != "n")
                        {
                            response = Console.ReadLine();
                        }

                        // Ask to finalize if not continuing
                        if (response.ToLower() == "n")
                        {
                            Console.WriteLine("Would you like to finalize your transaction? (Y/y OR N/n)");
                            string response2 = "";

                            // Ask as long as the option is invalid
                            while (response2.ToLower() != "y" && response2.ToLower() != "n")
                            {
                                response2 = Console.ReadLine();
                            }

                            // Finalize?
                            if (response2.ToLower() == "y")
                                FinalizeTransaction();

                            // Exits Menu loop
                            keepLooping = false;
                        }
                        else
                            Console.Clear();
                    }
                } // End of option getting
            } // End of menu loop
        }

        // Display all workshops, let user choose one
        void DisplayWorkshops()
        {
            if (CurStore?.Workshops?.Any() != true)
            {
                Console.WriteLine("There are no workshops to enrol in. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("======================================");
            Console.WriteLine(DefaultTitle);
            Console.WriteLine(Title);
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Workshops");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine();

            Console.Write("{0, 4}", "ID");
            Console.Write("{0, 12}", "Time");
            Console.Write("{0, 20}", "Cur. Participants");
            Console.Write("{0, 20}", "Max. Participants");
            Console.Write("{0, 20}", "Places Remaining");
            Console.WriteLine();

            for (int i = 0; i < CurStore.Workshops.Count; i++)
            {
                Console.Write("{0, 4}", i);
                Console.Write("{0, 12}", CurStore.Workshops[i].WorkshopTime);
                Console.Write("{0, 20}", CurStore.Workshops[i].CurWorkshopParticipants);
                Console.Write("{0, 20}", CurStore.Workshops[i].MaxWorkshopParticipants);
                Console.Write("{0, 20}", (CurStore.Workshops[i].MaxWorkshopParticipants - CurStore.Workshops[i].CurWorkshopParticipants));
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Please enter a Workshop ID to book into from 0 to {0}", CurStore.Workshops.Count - 1);

            int op = -1;
            while (op < 0 || op > CurStore.Workshops.Count - 1)
            {
                op = GetIntOptionSelected();
                if (op < 0 || op > CurStore.Workshops.Count - 1)
                    Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 0 to {1}.", op, CurStore.Workshops.Count - 1);
            }

            // Takes option, makes sure there's space, handles if not
            HandleWorkshopBooking(op);

            // Wait for use input to exit the workshop booking screen
            Console.ReadKey();
        }

        // Adds the item selected to the current order, and the quantity
        void HandleProductPurchase(int index, int quantity)
        {
            try
            {
                _CustomerOrder.AddItem(CurStore.StoreInventory[index], quantity);
                CurStore.StoreInventory[index].RemoveStock(quantity);
            }
            // Should never happen, but just in-case
            catch (Exception e) { Console.WriteLine("Could not add item \'{0}\' to your order.", index); Debug.Write(e.StackTrace); return; }
        }

        // Add the workshop to the current orders list of workshops
        void HandleWorkshopBooking(int index)
        {
            if (CurStore.Workshops[index].CurWorkshopParticipants < CurStore.Workshops[index].MaxWorkshopParticipants)
            {
                try
                {
                    _CustomerOrder.Workshops.Add(CurStore.Workshops[index]);
                    _CustomerOrder.BookedIntoWorkshop = true;
                    CurStore.Workshops[index].AddWorkshopParticipant();

                    Console.WriteLine("Booking successful, please see receipt for booking ID.\nPress any key to continue..");
                }
                // Should never happen as checking is done, but just in-case
                catch (Exception) { Console.WriteLine("Could not book you into workshop \'{0}\'.", index); }
            }
            else
                Console.WriteLine("Workshop is full. Please select a different workshop.");
        }

        // Removes items from store stock, reduces workshop availability.
        // Prints receipt
        void FinalizeTransaction()
        {
            // Notify user if nothing will be displayed
            if (_CustomerOrder.OrderItems.Count == 0 && _CustomerOrder.OrderQuantity.Count == 0 && _CustomerOrder.Workshops.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("There is no transaction to display, nothing has been purchased or booked into yet!\nPress any key to return to the store menu.");
                Console.ReadKey();
                return;
            }
            // Remove item from store stock
            try
            {
                for (int i = 0; i < _CustomerOrder.OrderItems.Count; i++)
                {
                    // Find the current item in the store stock
                    int index = CurStore.StoreInventory.IndexOf(_CustomerOrder.OrderItems[i]);

                    // Reduce stock by amount purchased (checks will already be done to ensure it doesn't drop below 0)
                    CurStore.StoreInventory[index].RemoveStock(_CustomerOrder.OrderQuantity[i]);
                }

                // Save the JSON file
                JSONUtility.SaveStoreInventory(CurStore.StoreName, CurStore.StoreInventory);

                for (int i = 0; i < _CustomerOrder.Workshops.Count; i++)
                {
                    // Find the current Workshop in the stores list
                    int index = CurStore.Workshops.IndexOf(_CustomerOrder.Workshops[i]);

                    // Reduce the workshop availability by 1
                    CurStore.Workshops[index].AddWorkshopParticipant();
                }
            }
            catch (Exception) { Console.WriteLine("Couldn't complete transaction, aborting."); return; }

            // Print everything if the order is processed correctly
            Console.Clear();
            Console.WriteLine(_CustomerOrder);

            // Wait for key-press to finish viewing the receipt
            Console.WriteLine("Please press any key when you have finished viewing the receipt.");
            Console.ReadKey();

            // Re-Initialize the order to another can begin
            _CustomerOrder = new CustomerOrder(CurStore.StoreName);
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
            Title = "Customer Menu (" + CurStore.StoreName + ")";
        }
    }
}
