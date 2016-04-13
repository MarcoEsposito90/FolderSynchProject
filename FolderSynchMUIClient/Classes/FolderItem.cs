using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public class FolderItem : Item
    {
        [DataMember]
        public LocalFolder parentFolder { get; set; }

        [DataMember]
        public string RelativePath { get; private set; }

        [DataMember]
        public ObservableCollection<Item> Items
        {
            get;
            set;
        }

        public FolderItem(string name, string relativePath)
        {
            this.Name = name;
            this.RelativePath = relativePath;
            this.Items = new ObservableCollection<Item>();
        }
    }
}
