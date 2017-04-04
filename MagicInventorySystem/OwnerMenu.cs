using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web;

namespace MagicInventorySystem
{
    class OwnerMenu : Menu
    {
        // Stock Items
        List<Item> Stock = new List<Item>();
        // Stores
        public List<Store> Stores { get; private set; }
        // Requests made by stores
        List<StockRequest> StockRequests = new List<StockRequest>();

        // Owners Menu
        public OwnerMenu() : base()
        {
            Title = "Owner Menu";
            Options = new List<string>
            {
                "Display All Stock Requests",
                "Display Stock Requests (True/False)",
                "Display All Product Lines",
                "Return to Main Menu",
                "Exit"
            };

            Stock = JSONUtility.GetInventory("Owners");
            
            Stores = new List<Store> {
                new Store("Olinda"),
                new Store("Springvale"),
                new Store("Altona"),
                new Store("Melbourne"),
                new Store("Epping")};
                
            StockRequests = JSONUtility.GetStockRequests();
        }

        // The owners menu
        public override void HandleMenu()
        {
            while (ShouldExitMenu == false)
            {
                int op = DisplayMenu();
                switch (op)
                {
                    case 1:
                        DisplayAllStockRequests();
                        break;
                    case 2:
                        DisplayStockRequests(GetBooleanResponse());
                        break;
                    case 3:
                        DisplayAllProductLines();
                        break;
                    case 4:
                        return;
                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // Prints out all current stock requests
        void DisplayAllStockRequests()
        {
            StockRequests = JSONUtility.GetStockRequests();
            if (StockRequests?.Any() != true)
            {
                Console.WriteLine("There are no stock requests to process. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            string[] headers = { "Request ID", "Store", "Product", "Quantity", "Current Stock", "Available" };

            // Generate heading
            string heading = "";
            // Generate heading
            // The first 2 columns are always the same (ID, Product Name) so they are set first
            heading += string.Format("{0,10}", headers[0]);
            heading += string.Format("{0,15}", headers[1]);
            heading += string.Format("{0,17}", headers[2]);
            heading += string.Format("{0,17}", headers[3]);
            heading += string.Format("{0,17}", headers[4]);
            heading += string.Format("{0,17}", headers[5]);

            List<string> formattedData = new List<string>();
            // Generate each line
            for (int i = 0; i < StockRequests.Count; i++)
            {
                // Get the current Item's index
                int reqItemOwnerIndex = -1;
                for (int x = 0; x < Stock.Count; x++)
                {
                    if (Stock[x].Name == StockRequests[i].ItemRequested.Name)
                        reqItemOwnerIndex = x;
                }

                string y = "";
                bool _available = StockRequests[i].Quantity <= Stock[reqItemOwnerIndex].StockLevel;
                y += string.Format("{0,10}", i);
                y += string.Format("{0,15}", Stores[StockRequests[i].StoreRequesting].StoreName);
                y += string.Format("{0,17}", StockRequests[i].ItemRequested.Name);
                y += string.Format("{0,17}", StockRequests[i].Quantity);
                y += string.Format("{0,17}", Stock[reqItemOwnerIndex].StockLevel);
                y += string.Format("{0,17}", _available);

                formattedData.Add(y);
            }

            int op = DisplayTable("Current Stock", heading, formattedData, true);
            // Cancel
            if (op == -2)
                return;

            // Data Validation
            int id = -1;
            for (int i = 0; i < Stock.Count; i++)
            {
                if (Stock[i].Name == StockRequests[op].ItemRequested.Name)
                    id = i;
            }


            if (id == -1)
            {
                Console.WriteLine("Item selected doesn't exist in Owner inventory");
                Console.ReadKey();
                return;
            }

            if (StockRequests[op].Quantity <= Stock[id].StockLevel)
            {
                ProcessRequest(op);
                Console.WriteLine("Stock has successfully been added! Press any key to continue..");
            }
            else
                Console.WriteLine("Cannot process request as there is not enough stock.");

            Console.ReadKey();
        }

        // Prints out stock requests with availabiity matching parameter
        void DisplayStockRequests(bool available)
        {
            StockRequests = JSONUtility.GetStockRequests();

            if (StockRequests?.Any() != true)
            {
                Console.WriteLine("There are no stock requests to process. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            string[] headers = { "Request ID", "Store", "Product", "Quantity", "Current Stock", "Available" };

            // Generate heading
            string heading = "";
            // Generate heading
            heading += string.Format("{0,10}", headers[0]);
            heading += string.Format("{0,15}", headers[1]);
            heading += string.Format("{0,17}", headers[2]);
            heading += string.Format("{0,17}", headers[3]);
            heading += string.Format("{0,17}", headers[4]);
            heading += string.Format("{0,17}", headers[5]);

            List<string> formattedData = new List<string>();
            // Generate each line
            for (int i = 0; i < StockRequests.Count; i++)
            {
                // Get the current Item's index
                int reqItemOwnerIndex = -1;
                for (int x = 0; x < Stock.Count; x++)
                {
                    if (Stock[x].Name == StockRequests[i].ItemRequested.Name)
                        reqItemOwnerIndex = x;
                }

                // Create each line
                string reqDetails = "";
                bool _available = StockRequests[i].Quantity <= Stock[reqItemOwnerIndex].StockLevel;
                if (_available == available)
                {
                    reqDetails += string.Format("{0,10}", i);
                    reqDetails += string.Format("{0,15}", Stores[StockRequests[i].StoreRequesting].StoreName);
                    reqDetails += string.Format("{0,17}", StockRequests[i].ItemRequested.Name);
                    reqDetails += string.Format("{0,17}", StockRequests[i].Quantity);
                    reqDetails += string.Format("{0,17}", Stock[reqItemOwnerIndex].StockLevel);
                    reqDetails += string.Format("{0,17}", available);

                    formattedData.Add(reqDetails);
                }
            }

            int op = DisplayTable("Current Stock", heading, formattedData, true);
            // Cancel
            if (op == -2)
                return;

            //Data Validation
            int id = -1;
            for (int i = 0; i < Stock.Count; i++)
            {
                if (Stock[i].Name == StockRequests[op].ItemRequested.Name)
                    id = i;
            }

            if (id == -1)
            {
                Console.WriteLine("Item selected doesn't exist in Owner inventory");
                Console.ReadKey();
                return;
            }

            if (StockRequests[op].Quantity <= Stock[id].StockLevel)
            {
                ProcessRequest(op);
                Console.WriteLine("Stock has successfully been added! Press any key to continue..");
            }
            else
                Console.WriteLine("Cannot process request as there is not enough stock.");

            Console.ReadKey();
        }

        // Modifies required files and arrays to indicate a stock request being handled
        void ProcessRequest(int opSelected)
        {
            Item req = StockRequests[opSelected].ItemRequested;
            int storeId = StockRequests[opSelected].StoreRequesting;
            int amountToGive = StockRequests[opSelected].Quantity;
            int itemIndex = -1;

            // Get the item index 
            for (int i = 0; i < Stores[storeId].StoreInventory.Count; i++)
            {
                // Name must match
                if (Stores[storeId].StoreInventory[i].Name == req.Name)
                    itemIndex = i;
            }

            if(itemIndex == -1)
            {
                Console.WriteLine("Item selected doesn't exist in the stores inventory to be replenished, press any key to cancel.");
                Console.ReadKey();
                return;
            }

            // Find the item in the stock inventory
            int stockId = -1;
            for (int i = 0; i < Stock.Count; i++)
            {
                // Name must match
                if (Stock[i].Name == req.Name)
                    stockId = i;
            }

            if (stockId == -1)
            {
                Console.WriteLine("Item selected doesn't exist in your stock list, press any key to cancel.");
                Console.ReadKey();
                return;
            }

            try
            {
                // Remove from owner stock
                Stock[stockId].RemoveStock(amountToGive);

                // Add stock to store requesting
                Stores[storeId].StoreInventory[itemIndex].AddStock(amountToGive);

                // Delete stock requests
                StockRequests.RemoveAt(opSelected);
            }
            catch
            {
                Console.WriteLine("Something unexpected went wrong applying the stock update, aborting. Press any key to continue.");
                Console.ReadKey();
                return;
            }

            // Update the owners stock
            JSONUtility.SaveStoreInventory("Owners", Stock);
            // Update the store inventory
            JSONUtility.SaveStoreInventory(Stores[storeId].StoreName, Stores[storeId].StoreInventory);
            // Update Stock Requests
            JSONUtility.SaveStockRequests(StockRequests);

            // Notify user of successs
            Console.WriteLine("Stock request has successfully been handled. Press any key to return to the previous menu.");
        }

        void DisplayAllProductLines()
        {
            if (Stock?.Any() != true)
            {
                Console.WriteLine("There is no stock. Press any key to return to the previous menu.");
                Console.ReadKey();
                return;
            }

            string[] headers = { "ID", "Product", "Current Stock" };

            //Generate heading
            string heading = "";

            //Generate heading
            heading += string.Format("{0,10}", headers[0]);
            heading += string.Format("{0,25}", headers[1]);
            heading += string.Format("{0,17}", headers[2]);

            List<string> formattedData = new List<string>();
            //Generate each line 
            if(Stock != null)
            {
                for (int i = 0; i < Stock.Count; i++)
                {
                    string y = "";
                    y += string.Format("{0,10}", i);
                    y += string.Format("{0,25}", Stock[i].Name);
                    y += string.Format("{0,17}", Stock[i].StockLevel);

                    formattedData.Add(y);
                }
            }

            // Just display, no handling as nothing is happening with the result
            DisplayTable("Current Stock", heading, formattedData, false);
        }

        // Table for OwnerMenu
        // Displays a table using the data given, prompts for response if needed
        int DisplayTable(string title, string heading, List<string> formattedData, bool needsProcessing)
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

            if (needsProcessing)
            {
                Console.WriteLine("Enter a request to process: ");

                while (option < 0 || option > formattedData.Count - 1)
                {
                    option = GetIntOptionSelected();
                    // Cancel
                    if (option == -2)
                        return -2;
                    if (option < 0 || option > formattedData.Count - 1)
                        Console.WriteLine("\'{0}\' is not a valid option. Please enter a valid option from 0 to {1}.", option, formattedData.Count - 1);
                }
            }
            else
            {
                Console.WriteLine("Press any key to return to the previous menu.");
                Console.ReadKey();
            }

            return option;
        }
    }
}
