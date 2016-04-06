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

        public DateTime Timestamp { get; set; }
        public ObservableCollection<UpdateEntry> UpdateEntries { get; set; }
        Folder folder;

        public Update(Folder folder, DateTime timestamp)
        {
            this.folder = folder;
            this.Timestamp = timestamp;
            this.UpdateEntries = new ObservableCollection<UpdateEntry>();

        }

        [DataContract]
        public class UpdateEntry
        {
            public Item Item { get; private set; }
            public string ItemName { get; set; }

            public int UpdateType { get; private set; }

            public static int DELETE = 0;
            public static int NEW = 1;
            public static int MODIFIED = 2;

            public UpdateEntry(Item item, int updateType)
            {
                this.Item = item;
                this.ItemName = item.Name;
                this.UpdateType = updateType;

            }
        }
    }
}
