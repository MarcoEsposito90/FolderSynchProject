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
            [DataMember]
            public string ItemName { get; private set; }

            [DataMember]
            public DateTime EntryTimestamp { get; private set; }

            [DataMember]
            public int UpdateType { get; private set; }

            public static readonly int DELETE = 0;
            public static readonly int NEW = 1;
            public static readonly int MODIFIED = 2;

            public UpdateEntry(string itemName, int updateType)
            {
                this.ItemName = itemName;
                this.UpdateType = updateType;
                this.EntryTimestamp = DateTime.Now;

            }
        }
    }
}
