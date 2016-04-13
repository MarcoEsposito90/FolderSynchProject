using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class Folder
    {

        public static readonly int DEFAULT_REFRESH_TIME_HOURS = 24;
        public static readonly int DEFAULT_DELETE_TIME_DAYS = 14;

        /* -------------- PROPERTIES -------------------------*/

        /*[DataMember]
        public ObservableCollection<Item> Items
        {
            get;
            set;
        }*/

        [DataMember]
        public ObservableCollection<Update> Updates
        {
            get;
            set;
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long CurrentSize { get; set; }

        [DataMember]
        public string SizeInBytes { get; set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public int ContainedFiles { get; set; }

        [DataMember]
        public int ContainedFolders { get; set; }

        [DataMember]
        public DateTime SynchDate { get; set; }

        [DataMember]
        public int AutoRefreshTime { get; set; }

        [DataMember]
        public int AutoDeleteTime { get; set; }




        /* -------------- CONSTRUCTORS -------------------------*/

        public Folder(string name, string username)
        {
            //this.Items = new ObservableCollection<Item>();
            this.Updates = new ObservableCollection<Update>();
            this.Name = name;
            this.Username = username;
            this.SynchDate = DateTime.Now;
            this.AutoDeleteTime = DEFAULT_DELETE_TIME_DAYS;
            this.AutoRefreshTime = DEFAULT_REFRESH_TIME_HOURS;
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
    }
}
