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
    public class Update
    {
        public string Username { get; private set; }
        public string BaseFolder { get; private set; }
        public string UpdateFolder { get; private set; }
        public int Number { get; private set; }
        public string TransactionID { get; private set; }
        public DateTime Timestamp { get; set; }
        public ObservableCollection<UpdateEntry> UpdateEntries { get; set; }

        public Update(string baseFolder, DateTime timestamp, int number, string transactionID)
        {
            this.BaseFolder = baseFolder;
            this.UpdateFolder = baseFolder + "_" + number;
            this.Timestamp = timestamp;
            this.Number = number;
            this.TransactionID = transactionID;
            this.UpdateEntries = new ObservableCollection<UpdateEntry>();
        }

        [DataContract]
        public class UpdateEntry
        {
            public string ItemName { get; private set; }
            public DateTime EntryTimestamp { get; private set; }

            public int UpdateType { get; private set; }

            public static int DELETE = 0;
            public static int NEW = 1;
            public static int MODIFIED = 2;

            public UpdateEntry(string itemName, int updateType)
            {
                this.ItemName = itemName;
                this.UpdateType = updateType;
                this.EntryTimestamp = DateTime.Now;

            }
        }
    }
}
