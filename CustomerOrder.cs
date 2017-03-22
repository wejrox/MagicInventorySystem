using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    // Created when a customer begins an order by selecting an item in the stock list. 
    // Used only for printing a receipt.
    class CustomerOrder
    {
        static int NextReferenceID = 0;
        private int CurrentBookingReferenceID { get; set; } // ID to be displayed to customer

        public List<Item> OrderItems { get; set; } // Items being bought, used for removing from the store inventory upon order completion
        public List<int> OrderQuantity { get; set; } // Amount of each item bought, index should match OrderItems item index
        public List<Workshop> Workshops { get; set; } // Workshops, used for removing availability upon order completion

        private string StoreName { get; set; }

        public bool BookedIntoWorkshop { get; set; } // Apply discount if booked in

        private int DiscountModifierPercentage { get; set; } = 10;
        private double TotalCost { get; set; } // Total cost after calculations

        public CustomerOrder()
        {
            OrderItems = new List<Item>();
            OrderQuantity = new List<int>();
            Workshops = new List<Workshop>();
            CurrentBookingReferenceID = NextReferenceID++;
            BookedIntoWorkshop = false;
        }

        public CustomerOrder(string storeName) : this()
        {
            StoreName = storeName;
        }

        // Returns the receipt
        public override string ToString()
        {
            // Initialise sections
            string title = ""; // Title of the receipt
            string itemsTitle = ""; // Text to display above Items purchased
            string itemsHeader = ""; // Column headers
            string items = ""; // List of items
            string workshopsTitle = ""; // Text to display above workshops(s) booked
            string workshopsHeader = ""; // Column headers
            string workshops = ""; // List of workshops
            string totalPriceText = ""; // Display text before price
            double price = 0; // Total price before discount
            string combined = ""; // Whole receipt

            // Set title, showing booking reference ID
            title += "======================================\n";
            title += "Receipt for Purchase ID: " + CurrentBookingReferenceID + "\n";
            title += "======================================";

            // Set items title
            itemsTitle = "Items Purchased\n";
            itemsTitle += "--------------------------------------";
            // Set receipt header for items purchased
            itemsHeader = String.Format("{0, 4} {1, 30} {2, 10} {3, 15}", "ID", "Name", "Quantity", "Price (Each)");

            // Add each item to be displayed
            for (int i = 0; i < OrderItems.Count; i++)
            {
                items += String.Format("{0, 4} {1, 30} {2, 10} {3, 15}", OrderItems[i].Id, OrderItems[i].Name, OrderQuantity[i], OrderItems[i].Price);
                items += "\n";
                // Add to the total (undiscounted) price
            }

            // Print Workshops if booked in
            if (BookedIntoWorkshop)
            {
                // Set items title
                workshopsTitle = "Workshops Booked\n";
                workshopsTitle += "--------------------------------------";
                // Set workshop header
                workshopsHeader = String.Format("{0, 20} {1, 20}", "Workshop Time", "Store Name");
                // Add workshops
                for (int i = 0; i < Workshops.Count; i++)
                {
                    workshops += String.Format("{0, 20} {1, 20}", Workshops[i].WorkshopTime, StoreName);
                }
            }

            // Set price text
            if (BookedIntoWorkshop)
                totalPriceText = "Total price of all items (after 10% workshop discount) : ";
            else
                totalPriceText = "Total price of all items : ";

            // Calculate total price
            if (BookedIntoWorkshop)
                TotalCost = price * ((100 - DiscountModifierPercentage) / 100); // Items accumulated price modified by discount rate
            else TotalCost = price;

            // Add it all together
            combined += title;
            combined += "\n\n"; // Line space
            combined += itemsTitle;
            combined += "\n";
            combined += itemsHeader;
            combined += "\n";
            combined += items;
            combined += "\n\n"; // Line space
            combined += workshopsTitle;
            combined += "\n";
            combined += workshopsHeader;
            combined += "\n";
            combined += workshops;
            combined += "\n\n"; // Line space
            combined += totalPriceText;
            combined += TotalCost;
            combined += "\n\n"; // Line space

            // Send back the receipt
            return combined;
        }
    }
}
