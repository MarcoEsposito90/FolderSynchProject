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
            {
                size += i.CalculateSize();
            }
            return size;
        }


        /********************************************************************/
        public List<Change> DetectChanges(DateTime lastUpdateTime)
        {
            List<Change> changes = new List<Change>();
            List<String> files = new List<string>(Directory.GetFiles(this.Path));

            // 1) see if old files are still there -----------------------------------------
            foreach (FileItem f in LatestUpdateFileItems)
            {
                int found = files.FindIndex(item => item.Equals(f.Path));

                if (found == -1)
                {
                    // file was deleted
                    changes.Add(new Change(Change.DELETED_FILE, f.Path));
                }
                else
                {

                    FileInfo fi = new FileInfo(files[found]);
                    if (DateTime.Compare(fi.LastWriteTime,lastUpdateTime) > 0)
                    {
                        // file was modified
                        changes.Add(new Change(Change.CHANGED_FILE, f.Path));
                    }

                    files.RemoveAt(found);
                }
            }

            // 2) see new files ----------------------------------------------------------
            foreach (string file in files)
            {
                changes.Add(new Change(Change.NEW_FILE, file));
            }

            List<string> subFolders = new List<string>(Directory.GetDirectories(this.Path));

            // 3) see if old folders are still there ------------------------------------
            foreach (FolderItem fi in LatestUpdateFolderItems)
            {
                int found = subFolders.FindIndex(item => item.Equals(fi.Path));

                if (found == -1)
                {
                    changes.Add(new Change(Change.DELETED_DIRECTORY, fi.Path));
                    // folder deleted. get all its subfiles
                    foreach (Item i in fi.getAllLatestUpdateSubItems())
                    {
                        int type = i.GetType().Equals(typeof(FileItem)) ? Change.DELETED_FILE : Change.DELETED_DIRECTORY;
                        changes.Add(new Change(type, i.Path));
                    }
                }
                else
                {
                    // folder is still there. check for variations inside it
                    List<Change> subCHanges = fi.DetectChanges(lastUpdateTime);
                    foreach (Change sc in subCHanges)
                        changes.Add(sc);

                    subFolders.RemoveAt(found);
                }

            }


            // 4) see new folders ------------------------------------------------------------
            foreach (String sf in subFolders)
            {
                string name = sf.Replace(this.Path + "\\", "");
                changes.Add(new Change(Change.NEW_DIRECTORY, sf));

                FolderItem fi = new FolderItem(name, sf);
                List<Item> subItems = fi.getAllSubItems();

                foreach(Item i in subItems)
                {
                    int type = i.GetType().Equals(typeof(FileItem)) ? Change.NEW_FILE : Change.NEW_DIRECTORY;
                    changes.Add(new Change(type, i.Path));
                }
            }

            return changes;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ METHODS ------------------------------------------- */
        /* ---------------------------------------------------------------- */

        protected ObservableCollection<Item> GetItems()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            List<FileItem> fileItem = getFileItems();
            List<FolderItem> folderItem = getFolderItems();

            foreach (FolderItem ff in folderItem)
                items.Add(ff);

            foreach (FileItem fi in fileItem)
                items.Add(fi);

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
        protected List<Item> getAllLatestUpdateSubItems()
        {
            List<Item> items = new List<Item>();

            foreach (FileItem i in LatestUpdateFileItems)
                items.Add(i);

            foreach (FolderItem fi in LatestUpdateFolderItems)
            {
                items.Add(fi);
                List<Item> subItems = fi.getAllLatestUpdateSubItems();
                foreach (Item si in subItems)
                    items.Add(si);
            }

            return items;
        }


        /************************************************************************/
        protected List<Item> getAllSubItems()
        {

            List<Item> items = new List<Item>();

            List<FileItem> files = getFileItems();

            foreach (FileItem i in files)
                items.Add(i);

            List<FolderItem> folders = getFolderItems();
            foreach(FolderItem f in folders)
            {
                items.Add(f);
                List<Item> subItems = f.getAllSubItems();
                foreach (Item si in subItems)
                    items.Add(si);
            }


            return items;
        }

        /************************************************************************/
        public void setLatestUpdateItems()
        {
            LatestUpdateFolderItems = getFolderItems();
            LatestUpdateFileItems = getFileItems();

            foreach (FolderItem f in LatestUpdateFolderItems)
                f.setLatestUpdateItems();
        }


        /************************************************************************/
        public void printLastUpdateStructure()
        {
            if (LatestUpdateFileItems == null || LatestUpdateFolderItems == null)
                Console.WriteLine("no previous update structure");

            foreach (FileItem file in LatestUpdateFileItems)
                Console.WriteLine(file.Path);

            foreach (FolderItem folder in LatestUpdateFolderItems)
            {
                Console.WriteLine(folder.Path);
                folder.printLastUpdateStructure();
            }
        }
    }
}
