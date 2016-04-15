using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class Update : IComparable
    {
        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string BaseFolder { get; private set; }

        [DataMember]
        public string UpdateFolder { get; private set; }

        [DataMember]
        public int Number { get; private set; }

        [DataMember]
        public string TransactionID { get; private set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public List<UpdateEntry> UpdateEntries { get; set; }

        public Update(string baseFolder, DateTime timestamp, int number, string transactionID)
        {
            this.BaseFolder = baseFolder;
            this.UpdateFolder = baseFolder + "_" + number;
            this.Timestamp = timestamp;
            this.Number = number;
            this.TransactionID = transactionID;
            this.UpdateEntries = new List<UpdateEntry>();
        }

        public int CompareTo(object obj)
        {
            if(!obj.GetType().Equals(typeof(Update)))
                throw new Exception("ivoking compareTo for another type object in Update.CompareTo");

            Update u = (Update)obj;
            return Number - u.Number;
        }


        public void printUpdate()
        {
            Console.WriteLine("Username: " + Username);
            Console.WriteLine("BaseFolder: " + BaseFolder);
            Console.WriteLine("UpdateFolder: " + UpdateFolder);
            Console.WriteLine("Number: " + Number);
            Console.WriteLine("TransactionID: " + TransactionID);
            Console.WriteLine("Timestamp: " + Timestamp);
            
            foreach(UpdateEntry entry in UpdateEntries)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("ItemLocalPath: " + entry.ItemLocalPath);
                Console.WriteLine("EntryTimestamp: " + entry.EntryTimestamp);
                Console.WriteLine("UpdateType: " + entry.UpdateType);
            }
        }

        [DataContract]
        public class UpdateEntry
        {
            [DataMember]
            public string ItemLocalPath { get; private set; }

            [DataMember]
            public DateTime EntryTimestamp { get; private set; }

            [DataMember]
            public int UpdateType { get; private set; }

            
            public static readonly int NEW_FILE = 0;
            public static readonly int MODIFIED_FILE = 1;
            public static readonly int NEW_DIRECTORY = 2;
            public static readonly int DELETED_DIRECTORY = 3;
            public static readonly int DELETED_FILE = 4;

            public UpdateEntry(string itemLocalPath, int updateType, DateTime timestamp)
            {
                this.ItemLocalPath = itemLocalPath;
                this.UpdateType = updateType;
                this.EntryTimestamp = timestamp;

            }
        }

        
    }
}
