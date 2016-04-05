using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{

    //[DataContract]
    public class Folder : Item
    {
        //[DataMember]
        public ObservableCollection<Item> Items {
            get;
            set;
        }

        public Folder(string name, string path) {
            this.Items = new ObservableCollection<Item>();
            this.Name = name;
            this.Path = path;
        }
    }
}
