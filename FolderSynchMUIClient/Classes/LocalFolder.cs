using FolderSynchMUIClient.FolderSynchService;
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
    public class LocalFolder : Item
    {

        /* -------------- PROPERTIES -------------------------*/

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /* ---------------------------------------------------------------- */
        /* ------------ SERIALIZABLE PROPERTIES --------------------------- */
        /* ---------------------------------------------------------------- */

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string FolderName { get; private set; }

        [DataMember]
        public List<Item> LatestUpdateItems { get; private set; }

        [DataMember]
        public Update LastUpdate { get; set; }

        [DataMember]
        public int AutoRefreshTime { get; set; }

        [DataMember]
        public int AutoDeleteTime { get; set; }

        [DataMember]
        public DateTime SynchDate { get; set; }


        /* ---------------------------------------------------------------- */
        /* ------------ TEMPORARY PROPERTIES ------------------------------ */
        /* ---------------------------------------------------------------- */

        public ObservableCollection<Item> Items
        {
            get
            {
                return GetItems(LocalPath);
            }
        }

        public ObservableCollection<Update> Updates
        {
            get;
            set;
        }

        public int ContainedFiles
        {
            get
            {
                return Directory.GetFiles(this.LocalPath, "*", SearchOption.AllDirectories).Length;
            }
        }

        public int ContainedFolders
        {
            get
            {
                return Directory.GetDirectories(this.LocalPath, "*", SearchOption.AllDirectories).Length;
            }
        }
        

        /* ---------------------------------------------------------------- */
        /* ------------ CONSTRUCTORS -------------------------------------- */
        /* ---------------------------------------------------------------- */

        public LocalFolder(string username, string folderName, string localPath)
        {

            this.Updates = new ObservableCollection<Update>();
            this.Username = username;
            this.FolderName = folderName;
            this.LocalPath = localPath;

        }

        /* ---------------------------------------------------------------- */
        /* ------------ PROPERTY COMPUTING -------------------------------- */
        /* ---------------------------------------------------------------- */

        /*********************************************************************/
        public override long CalculateSize(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            long size = 0;

            foreach (var file in dirInfo.GetFiles())
            {
                size += file.Length;
            }

            foreach (var directory in dirInfo.GetDirectories())
            {
                size += CalculateSize(directory.FullName);
            }
            //Console.WriteLine("Chiamato metodo CalculateSize per " + dirInfo.Name + " " + size.ToString());

            return size;
        }


        /*********************************************************************/
        private ObservableCollection<Item> GetItems(string path)
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new FolderItem(directory.Name, directory.FullName)
                {
                    Items = GetItems(directory.FullName)

                };

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileItem(file.Name, file.FullName);

                items.Add(item);
            }

            return items;
        }


        /*********************************************************************/
        private ObservableCollection<FolderItem> GetFolders(string path)
        {
            ObservableCollection<FolderItem> folders = new ObservableCollection<FolderItem>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                FolderItem f = new FolderItem(directory.Name, directory.FullName)
                {
                    Items = GetItems(directory.FullName)
                };

                folders.Add(f);
            }
            return folders;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ SCANNING ------------------------------------------ */
        /* ---------------------------------------------------------------- */
        public void scanForChanges()
        {

        }


    }
}
