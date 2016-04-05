using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderSynchMUIClient
{

    //[DataContract]
    public class Folder : Item
    {

        /* -------------- PROPERTIES -------------------------*/

        //[DataMember]
        public ObservableCollection<Item> Items {
            get;
            set;
        }
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public int ContainedFiles { get; set; }

        public int ContainedFolders { get; set;}

        public DirectoryInfo dirInfo { get; set; }


        /* -------------- CONSTRUCTORS -------------------------*/

        public Folder(string name, string path) {
            this.Items = new ObservableCollection<Item>();
            this.Name = name;
            this.Path = path;
            this.dirInfo = new DirectoryInfo(path);
            this.Size = CalculateSize(this.dirInfo);
            this.SizeInBytes = SizeSuffix(this.Size);
            this.ContainedFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            this.ContainedFolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;
        }

        /* -------------- METHODS ---------------------------- */

        public long CalculateSize(DirectoryInfo dirInfo)
        {

            long size = 0;

            foreach (var file in dirInfo.GetFiles())
            {
                size += file.Length;
            }

            foreach (var directory in dirInfo.GetDirectories())
            {
                size += CalculateSize(new DirectoryInfo(directory.FullName));
            }
            Console.WriteLine("Chiamato metodo CalculateSize per " + dirInfo.Name + " " + size.ToString());

            return size;
        }

        static string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}
