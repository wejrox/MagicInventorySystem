using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MagicInventorySystem
{
    class JSONUtility
    {
        public static void InitializeJSONFiles()
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

        #region Reading from JSON
        // Get the Inventory of the name given
        public static List<Item> GetInventory(string invName)
        {
            List<Item> inventory = null;
            try
            {
                inventory = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(@"dat\" + invName + @"_inventory.txt"));
            }
            catch (Exception) { Console.WriteLine("Error reading data for \'{0}\'. Please re-load the application.", invName); }

            return inventory;
        }

        // Get stock requests
        public static List<StockRequest> GetStockRequests()
        {
            List<StockRequest> stock = null;
            try
            {
                stock = JsonConvert.DeserializeObject<List<StockRequest>>(File.ReadAllText(@"dat\Stock_requests_inventory.txt"));
            }
            catch (Exception) { Console.WriteLine("Error reading data for 'Stock Requests'. Please re-load the application."); }

            return stock;
        }
        #endregion

        #region Writing to files
        // Saves the Inventory of the store given
        public static void SaveStoreInventory(string storeName, List<Item> inventory)
        {
            File.WriteAllText(@"dat\" + storeName + @"_inventory.txt", JsonConvert.SerializeObject(inventory, Formatting.Indented));
        }

        // Add stock request
        public static void AddAndSaveStockRequest(StockRequest sr)
        {
            List<StockRequest> stockRequests = new List<StockRequest>();
            try
            {
                stockRequests = JsonConvert.DeserializeObject<List<StockRequest>>(File.ReadAllText(@"dat\Stock_requests.txt"));
            }
            catch (Exception) { Console.WriteLine("Error saving Stock Request."); return; }
            
            // In-case the file is empty
            if(stockRequests == null)
                stockRequests = new List<StockRequest>();

            stockRequests.Add(sr);

            File.WriteAllText(@"dat\Stock_requests.txt", JsonConvert.SerializeObject(stockRequests, Formatting.Indented));
        }

        // Saving stock requests
        public static void SaveStockRequests(List<StockRequest> stockRequests)
        {
            File.WriteAllText(@"dat\Stock_requests.txt", JsonConvert.SerializeObject(stockRequests, Formatting.Indented));
        }
        #endregion
    }
}
