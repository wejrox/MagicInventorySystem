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
        public List<Item> _stock = new List<Item>();
        // Stores
        public List<Store> _stores { get; private set; }
        // Requests made by stores
        List<StockRequest> _stockRequests = new List<StockRequest>();
 
        // Owners Menu
        string ownerTitle = "(Owner)";
        string[] ownerOptions = { "Display All Stock Requests", "Display Stock Requests (True/False)", "Display All Product Lines" };
        
        string[] franchiseHolderOptions = { "Display Inventory", "Display Inventory (Threshold)", "Add New Inventory Item" };
        // Customer
        string[] customerOptions = { "Display Products", "Display Workshops" };

        public OwnerMenu()
        {
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
            int op = Formatter.DisplayMenu(ownerTitle, ownerOptions);
            switch(op)
            {
                case 1:
                    DisplayAllStockRequests();
                    break;
                case 2:
                    DisplayStockRequests(Formatter.GetBooleanResponse());
                    break;
                case 3:
                    DisplayAllProductLines();
                    break;
            }
            Console.ReadKey();
        }

        // Prints out all current stock requests
        void DisplayAllStockRequests()
        {
            string[] headers = { "Request ID", "Store ID", "Product", "Quantity", "Current Stock", "Available" };

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
            foreach (StockRequest ir in _stockRequests)
            {
                string y = "";
                bool available = ir.Quantity < ir.ItemRequested.StockLevel;
                y += string.Format("{0,10}", ir.Id);
                y += string.Format("{0,15}", ir.StoreRequesting);
                y += string.Format("{0,17}", ir.ItemRequested.Name);
                y += string.Format("{0,17}", ir.Quantity);
                y += string.Format("{0,17}", ir.ItemRequested.StockLevel);
                y += string.Format("{0,17}", available);

                formattedData.Add(y);
            }

            Formatter.DisplayTable("Current Stock", heading, formattedData);

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
            foreach (StockRequest ir in _stockRequests)
            {
                string y = "";
                bool _available = ir.Quantity < ir.ItemRequested.StockLevel;
                if (_available == available)
                {
                    y += string.Format("{0,10}", ir.Id);
                    y += string.Format("{0,15}", ir.StoreRequesting);
                    y += string.Format("{0,17}", ir.ItemRequested.Name);
                    y += string.Format("{0,17}", ir.Quantity);
                    y += string.Format("{0,17}", ir.ItemRequested.StockLevel);
                    y += string.Format("{0,17}", available);

                    formattedData.Add(y);
                }
            }

            int op = Formatter.DisplayTable("Current Stock", heading, formattedData);
            Console.WriteLine(op);
            ProcessRequest(op);
        }

        // Adds the stock to the shop's inventory stock level
        // Removes from _itemStock StockLevel
        void ProcessRequest(int op)
        {
            int storeId = _stockRequests[op].StoreRequesting;
            int itemId = _stockRequests[op].ItemRequested.Id;
            Console.WriteLine(storeId + " " + itemId);

            _stores[storeId].StoreInventory[itemId].AddStock(_stockRequests[op].Quantity);
            _stock[itemId].RemoveStock(_stockRequests[op].Quantity);
            _stockRequests.RemoveAt(op);
        }

        void DisplayAllProductLines()
        {

        }

        public void AddStockRequest(StockRequest sr)
        {

        }
        
    }
}
