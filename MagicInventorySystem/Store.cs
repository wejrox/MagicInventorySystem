﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MagicInventorySystem
{
    class Store
    {
        static int NEXTID;
        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public string StoreName { get; private set; }
        [JsonProperty]
        public List<Workshop> Workshops { get; set; }
        [JsonProperty]
        public List<Item> StoreInventory { get; set; }

        public Store()
        {
            Id = NEXTID++;

            Workshops = new List<Workshop>();
            StoreInventory = new List<Item>();
            // Initialize workshops
            Workshops.Add(new Workshop(20, "06:15"));
            Workshops.Add(new Workshop(10, "08:45"));
            Workshops.Add(new Workshop(15, "12:15"));
            Workshops.Add(new Workshop(20, "14:45"));
        }

        public Store(string name) : this()
        {
            StoreName = name;
            LoadInventory();
        }

        // Add another item to the inventory
        public void AddInventoryItem(Item itemToAdd)
        {
            StoreInventory.Add(itemToAdd);
            // Save the new inventory
            JSONUtility.SaveStoreInventory(StoreName, StoreInventory);
        }

        public void LoadInventory()
        {
            // Initialise store inventory
            StoreInventory = JSONUtility.GetInventory(StoreName);

            // If the JSON files are empty, it will return null so set it
            if (StoreInventory == null)
                StoreInventory = new List<Item>();
        }
    }
}
