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
    public class LocalFolder
    {

        /* -------------- PROPERTIES -------------------------*/

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        [DataMember]
        public ObservableCollection<Item> Items
        {
            get;
            set;
        }

        [DataMember]
        public ObservableCollection<Update> Updates
        {
            get;
            set;
        }

        [DataMember]
        public string Username
        {
            get;
            private set;
        }


        [DataMember]
        public string FolderName
        {
            get;
            private set;
        }

        [DataMember]
        public string LocalPath
        {
            get;
            private set;
        }

        [DataMember]
        public Update LastUpdate
        {
            get;
            set;
        }

        [DataMember]
        public int ContainedFiles { get; set; }

        [DataMember]
        public int ContainedFolders { get; set; }

        [DataMember]
        public long CurrentSize { get; set; }

        [DataMember]
        public string SizeInBytes { get; set; }

        [DataMember]
        public int AutoRefreshTime { get; set; }

        [DataMember]
        public int AutoDeleteTime { get; set; }


        public LocalFolder(string username, string folderName, string localPath)
        {
            this.Items = new ObservableCollection<Item>();
            this.Updates = new ObservableCollection<Update>();
            this.Username = username;
            this.FolderName = folderName;
            this.LocalPath = localPath;

        }

        /* -------------- METHODS ---------------------------- */

        public void CalculateProperties(string path)
        {

            this.ContainedFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            this.ContainedFolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;

        }

        public long CalculateSize(string path)
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

        public string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public ObservableCollection<Item> GetItems(string path)
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

        public ObservableCollection<FolderItem> GetFolders(string path)
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



    }
}
