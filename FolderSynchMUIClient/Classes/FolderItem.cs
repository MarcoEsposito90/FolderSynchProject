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
        [DataMember]
        public LocalFolder parentFolder { get; set; }
        

        [DataMember]
        public ObservableCollection<Item> Items
        {
            get;
            set;
        }

        public FolderItem(string name, string relativePath)
        {
            this.Name = name;
            this.LocalPath = relativePath;
            this.Items = new ObservableCollection<Item>();
        }

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
    }
}
