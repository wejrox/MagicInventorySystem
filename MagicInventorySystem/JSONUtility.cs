using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
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

            #region Generate Dummy Data if desired
            bool shouldGenerateData = false;
            // Make sure the owner's inventory contains stock
            List<Item> tempInv = GetInventory("Owners");
            if (tempInv?.Any() != true)
            {
                Console.WriteLine("No owner stock exists, generate dummy data for testing? (Y/y or N/n)");

                string op = "";
                while (op.ToLower() != "y" && op.ToLower() != "n")
                {
                    op = Console.ReadLine();
                    if (op.ToLower() != "y" && op.ToLower() != "n")
                        Console.WriteLine("You did not enter one of the options provided");
                }

                if (op.ToLower() == "y")
                    shouldGenerateData = true;
            }

            if (shouldGenerateData)
            {
                if (tempInv?.Any() != true)
                {
                    tempInv = new List<Item>
                    {
                        new Item("Rabbit", 20, 20.0f),
                        new Item("Hat", 20, 25.0f),
                        new Item("Wand", 20, 15.0f),
                        new Item("Rat", 20, 5.0),
                        new Item("Cloak", 20, 20.0f),
                        new Item("Cards", 20, 7.0f),
                        new Item("Trick Book (Revision 1)", 20, 9.95f),
                        new Item("Vanishing Sheet", 20, 10.0f),
                        new Item("Extending Wand", 20, 15.0f),
                        new Item("Regular Coins", 20, 2.0f),
                        new Item("Trick Coins", 20, 5.0f),
                        new Item("Regular Ropes", 20, 10.0f),
                        new Item("Trick Ropes", 20, 20.0f),
                        new Item("Regular Silks", 20, 10.0f),
                        new Item("Trick Silks", 20, 20.0f),
                        new Item("Cup and Balls", 20, 5.0f),
                        new Item("Beginners Kit", 20, 50.0f)
                    };
                    SaveStoreInventory("Owners", tempInv);
                }

                tempInv = GetInventory("Olinda");
                if (tempInv?.Any() != true)
                {
                    tempInv = new List<Item>
                    {
                        new Item("Rabbit", 0, 20.0f),
                        new Item("Hat", 5, 25.0f),
                        new Item("Wand", 20, 15.0f),
                        new Item("Rat", 20, 5.0),
                        new Item("Cloak", 20, 20.0f),
                        new Item("Cards", 20, 7.0f),
                        new Item("Trick Book (Revision 1)", 20, 9.95f),
                        new Item("Vanishing Sheet", 20, 10.0f),
                        new Item("Extending Wand", 20, 15.0f),
                        new Item("Regular Coins", 20, 2.0f),
                        new Item("Trick Coins", 20, 5.0f),
                        new Item("Regular Ropes", 20, 10.0f),
                        new Item("Trick Ropes", 20, 20.0f),
                        new Item("Regular Silks", 20, 10.0f),
                        new Item("Trick Silks", 20, 20.0f),
                        new Item("Cup and Balls", 20, 5.0f),
                        new Item("Beginners Kit", 20, 50.0f)
                    };
                    SaveStoreInventory("Olinda", tempInv);
                }

                List<StockRequest> tempSR = new List<StockRequest>();
                tempSR = GetStockRequests();
                if (tempSR?.Any() != true)
                {
                    tempSR = new List<StockRequest>
                    {
                        new StockRequest(tempInv[0], 0, 10),
                        new StockRequest(tempInv[1], 0, 10),
                        new StockRequest(tempInv[2], 0, 20)
                    };
                    SaveStockRequests(tempSR);
                }
            }
            #endregion
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
                stock = JsonConvert.DeserializeObject<List<StockRequest>>(File.ReadAllText(@"dat\Stock_requests.txt"));
            }
            catch (Exception) { Console.WriteLine("Error reading data for 'Stock Requests'. Please re-load the application."); }

            return stock;
        }
        #endregion

        #region Writing to JSON
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

        // Remove stock request
        public static void RemoveAndSaveStockRequest(StockRequest sr)
        {
            List<StockRequest> stockRequests = new List<StockRequest>();
            try
            {
                stockRequests = JsonConvert.DeserializeObject<List<StockRequest>>(File.ReadAllText(@"dat\Stock_requests.txt"));
            }
            catch (Exception) { Console.WriteLine("Error saving Stock Request."); return; }

            // In-case the file is empty
            if (stockRequests == null)
                return;

            // Remove the stock request
            stockRequests.Remove(sr);

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
