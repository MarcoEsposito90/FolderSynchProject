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

        private List<FolderItem> _LatestUpdateFolderItems;
        [DataMember]
        public List<FolderItem> LatestUpdateFolderItems
        {
            get
            {
                return _LatestUpdateFolderItems;
            }
            private set
            {
                _LatestUpdateFolderItems = value;
                foreach(FolderItem f in _LatestUpdateFolderItems)
                        f.setLatestUpdateItems();
            }
        }

        [DataMember]
        public List<FileItem> LatestUpdateFileItems
        {
            get;
            private set;
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
        public List<Change> DetectChanges()
        {
            throw new NotImplementedException();
        }


        /* ---------------------------------------------------------------- */
        /* ------------ METHODS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        protected ObservableCollection<Item> GetItems()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            items.Concat(getFolderItems());
            items.Concat(getFileItems());

            return items;
        }

        /************************************************************************/
        protected List<FolderItem> getFolderItems()
        {
            List<FolderItem> items = new List<FolderItem>();
            DirectoryInfo dirInfo = new DirectoryInfo(Path);

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                FolderItem item = new FolderItem(directory.Name, directory.FullName);
                items.Add(item);
            }

            return items;
        }


        /************************************************************************/
        protected List<FileItem> getFileItems()
        {
            List<FileItem> items = new List<FileItem>();
            DirectoryInfo dirInfo = new DirectoryInfo(Path);

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                FileItem item = new FileItem(file.Name, file.FullName);
                items.Add(item);
            }

            return items;
        }


        /************************************************************************/
        public void setLatestUpdateItems()
        {
            LatestUpdateFolderItems = getFolderItems();
            LatestUpdateFileItems = getFileItems();
        }
    }
}
