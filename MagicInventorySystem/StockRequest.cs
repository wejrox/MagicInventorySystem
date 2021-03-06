﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagicInventorySystem
{
    class StockRequest
    {
        static int NEXTID = 0;
        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public Item ItemRequested { get; private set; }
        [JsonProperty]
        public int StoreRequesting { get; private set; } // The store that's requesting, always the same ID
        [JsonProperty]
        public int Quantity { get; private set; }

        public StockRequest()
        {
            Id = NEXTID++;
        }

        public StockRequest(Item itemRequested, int storeRequesting, int quantity) : this()
        {
            ItemRequested = itemRequested;
            StoreRequesting = storeRequesting;
            Quantity = quantity;
        }
    }
}
