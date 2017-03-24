using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagicInventorySystem
{
    public class Item
    {
        static int NEXTID = 0;
        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public int StockLevel { get; private set; }
        [JsonProperty]
        public double Price { get; private set; }

        public Item()
        {
            Id = NEXTID++;
        }

        public Item(string name, int stockLevel, double price) : this()
        {
            Name = name;
            StockLevel = stockLevel;
            Price = price;
        }

        public void AddStock(int stock)
        {
            StockLevel += stock;
        }

        public void RemoveStock(int stock)
        {
            StockLevel -= stock;
        }
    }
}
