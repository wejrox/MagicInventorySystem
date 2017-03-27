using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Web;

namespace MagicInventorySystem
{
    class OwnerMenu : Menu
    {
        // Stock Items (Perhaps make a stock class?)
        List<Item> _stock = new List<Item>();
        // Stores
        public List<Store> _stores { get; private set; }
        // Requests made by stores
        List<StockRequest> _stockRequests = new List<StockRequest>();

        // Owners Menu
        public OwnerMenu()
        {
            Title = "Owner Menu";
            Options = new List<string>
            {
                "Display All Stock Requests",
                "Display Stock Requests (True/False)",
                "Display All Product Lines"
            };
            _stock = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(@"dat\Owners_inventory.txt"));
            _stockRequests = JsonConvert.DeserializeObject<List<StockRequest>>(File.ReadAllText(@"dat\Stock_requests.txt"));
            // Initialise stores
            _stores = new List<Store> {
                new Store("Olinda"),
                new Store("Springvale"),
                new Store("Altona"),
                new Store("Melbourne"),
                new Store("Epping")
            };
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
                }
            }
            //Is this needed? Was here before so I left it.
            Console.ReadKey();
        }

        // Prints out all current stock requests
        void DisplayAllStockRequests()
        {
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
            foreach (StockRequest sr in _stockRequests)
            {
                string y = "";
                bool available = sr.Quantity < sr.ItemRequested.StockLevel;
                y += string.Format("{0,10}", sr.Id);
                y += string.Format("{0,15}", _stores[sr.StoreRequesting].StoreName);
                y += string.Format("{0,17}", sr.ItemRequested.Name);
                y += string.Format("{0,17}", sr.Quantity);
                y += string.Format("{0,17}", sr.ItemRequested.StockLevel);
                y += string.Format("{0,17}", available);

                formattedData.Add(y);
            }

            DisplayTable("Current Stock", heading, formattedData);

            Console.ReadKey();
        }

        // Prints out stock requests with availabiity matching parameter
        void DisplayStockRequests(bool available)
        {
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
            foreach (StockRequest sr in _stockRequests)
            {
                string y = "";
                bool _available = sr.Quantity < sr.ItemRequested.StockLevel;
                if (_available == available)
                {
                    y += string.Format("{0,10}", sr.Id);
                    y += string.Format("{0,15}", _stores[sr.StoreRequesting].StoreName);
                    y += string.Format("{0,17}", sr.ItemRequested.Name);
                    y += string.Format("{0,17}", sr.Quantity);
                    y += string.Format("{0,17}", sr.ItemRequested.StockLevel);
                    y += string.Format("{0,17}", available);

                    formattedData.Add(y);
                }
            }

            int op = DisplayTable("Current Stock", heading, formattedData);
            Console.WriteLine(op);
            ProcessRequest(op);
        }

        // Adds the stock to the shop's inventory stock level
        // Removes from _itemStock StockLevel
        void ProcessRequest(int op)
        {
            int itemId = _stockRequests[op].ItemRequested.Id;

            _stores[_stockRequests[op].StoreRequesting].StoreInventory[itemId].AddStock(_stockRequests[op].Quantity);
            _stock[itemId].RemoveStock(_stockRequests[op].Quantity);
            _stockRequests.RemoveAt(op);
        }

        void DisplayAllProductLines()
        {
            string[] headers = { "ID", "Product", "Current Stock" };

            //Generate heading
            string heading = "";

            //Generate heading
            heading += string.Format("{0,10}", headers[0]);
            heading += string.Format("{0,15}", headers[1]);
            heading += string.Format("{0,17}", headers[2]);

            List<string> formattedData = new List<string>();
            //Generate each line 
            if(_stock != null)
            {
                for (int i = 0; i < _stock.Count; i++)
                {
                    string y = "";
                    y += string.Format("{0,10}", i);
                    y += string.Format("{0,15}", _stock[i].Name);
                    y += string.Format("{0,17}", _stock[i].StockLevel);

                    formattedData.Add(y);
                }
            }

            int op = DisplayTable("Current Stock", heading, formattedData);
            Console.WriteLine(op);
            ProcessRequest(op);
        }

        // Table for OwnerMenu
        // Displays a table using the data given
        int DisplayTable(string title, string heading, List<string> formattedData)
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
    }
}
