using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public class FolderItem : Item
    {
        /* ---------------------------------------------------------------- */
        /* ------------ SERIALIZABLE PROPERTIES --------------------------- */
        /* ---------------------------------------------------------------- */

        private List<Item> _LatestUpdateItems;
        [DataMember]
        public List<Item> LatestUpdateItems
        {
            get
            {
                return _LatestUpdateItems;
            }
            private set
            {
                _LatestUpdateItems = value;
                foreach(Item i in _LatestUpdateItems)
                {
                    if (i.GetType().Equals(typeof(FolderItem)))
                    {
                        FolderItem f = (FolderItem)i;
                        f.setLatestUpdateItems();
                    }
                }
            }
        }


        public ObservableCollection<Item> Items
        {
            get
            {
                return GetItems();
            }
        }

        /* ---------------------------------------------------------------- */
        /* ------------ CONSTRUCTORS -------------------------------------- */
        /* ---------------------------------------------------------------- */

        public FolderItem(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        /* ---------------------------------------------------------------- */
        /* ------------ OVERRIDE METHODS ---------------------------------- */
        /* ---------------------------------------------------------------- */

        public override long CalculateSize()
        {
            long size = 0;

            foreach (Item i in Items)
                size += i.CalculateSize();

            return size;
        }


        /********************************************************************/
        public override List<Change> DetectChanges()
        {
            throw new NotImplementedException();
        }


        /* ---------------------------------------------------------------- */
        /* ------------ METHODS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        protected ObservableCollection<Item> GetItems()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();
            DirectoryInfo dirInfo = new DirectoryInfo(Path);

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                FolderItem item = new FolderItem(directory.Name, directory.FullName);
                items.Add(item);
            }

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                FileItem item = new FileItem(file.Name, file.FullName);
                items.Add(item);
            }

            return items;
        }

        /************************************************************************/
        protected void setLatestUpdateItems()
        {
            LatestUpdateItems = new List<Item>(GetItems());
        }
    }
}
