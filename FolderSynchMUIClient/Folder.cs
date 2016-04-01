using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    class Folder : Item
    {
        public ObservableCollection<Item> Items { get; set; }

        public Folder() {
            this.Items = new ObservableCollection<Item>();
        }
    }
}
