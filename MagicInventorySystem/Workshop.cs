using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagicInventorySystem
{
    public class Workshop
    {
        static int NextBookingID = 0;
        [JsonProperty]
        public int MaxWorkshopParticipants { get; private set; }
        [JsonProperty]
        public int CurWorkshopParticipants { get; private set; }
        [JsonProperty]
        public string WorkshopTime { get; private set; }

        public int BookingID { get; private set; }

        public Workshop(int maxWorkshopParticipants, string workshopTime)
        {
            MaxWorkshopParticipants = maxWorkshopParticipants;
            CurWorkshopParticipants = 0;
            WorkshopTime = workshopTime;

            NextBookingID = 0;
        }

        // Tries to add a workshop participant, returns whether or not it was successful
        public bool AddWorkshopParticipant()
        {
            if (CurWorkshopParticipants < MaxWorkshopParticipants)
            {
                CurWorkshopParticipants++;
                return true;
            }
            else
                return false;
        }

        public int GetBookingId()
        {
            BookingID = NextBookingID++;
            return BookingID;
        }
    }
}
