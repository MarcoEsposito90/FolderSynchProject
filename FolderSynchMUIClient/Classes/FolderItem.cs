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
        public List<Change> DetectChanges(DateTime lastUpdateTime)
        {
            Console.WriteLine("scanning directory: " + this.Path);
            List<Change> changes = new List<Change>();
            List<String> files = new List<string>(Directory.GetFiles(this.Path));


            foreach(FileItem f in LatestUpdateFileItems)
            {
                Console.WriteLine("checking file :" + f.Path);
                int found = files.FindIndex(item => item.Equals(f.Path));

                if(found == -1)
                {
                    Console.WriteLine("this file was canceled");
                    changes.Add(new Change(Change.DELETED_FILE, f.Path));
                }
                else
                {
                    FileInfo fi = new FileInfo(files[found]);
                    if (fi.LastWriteTime > lastUpdateTime)
                    {
                        Console.WriteLine("The file changed");
                        changes.Add(new Change(Change.CHANGED_FILE, f.Path));
                    }

                    files.RemoveAt(found);
                }
            }

            foreach(string file in files)
            {
                Console.WriteLine(file + " is a new file");
                changes.Add(new Change(Change.NEW_FILE, file));
            }

            List<string> subFolders = new List<string>(Directory.GetDirectories(this.Path));
            
            foreach(FolderItem fi in LatestUpdateFolderItems)
            {
                Console.WriteLine("Checking subFolder: " + fi.Path);
                int found = subFolders.FindIndex(item => item.Equals(fi.Path));

                if (found == -1)
                {
                    Console.WriteLine("this directory was canceled");
                    
                    foreach(Item i in fi.getAllLatestSubItems())
                    {
                        Console.WriteLine(i.Path + "canceled with upper directory: " + fi.Path);
                        int type = i.GetType().Equals(typeof(FileItem)) ? Change.DELETED_FILE : Change.DELETED_DIRECTORY;
                        changes.Add(new Change(type, i.Path));
                    }
                }
                else
                    changes.Concat(fi.DetectChanges(lastUpdateTime));
            } 

            return changes;
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
        protected List<Item> getAllLatestSubItems()
        {
            List<Item> items = new List<Item>();

            items.Concat(LatestUpdateFileItems);
            items.Concat(LatestUpdateFolderItems);

            foreach (FolderItem fi in LatestUpdateFolderItems)
                items.Concat(fi.getAllLatestSubItems());

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
