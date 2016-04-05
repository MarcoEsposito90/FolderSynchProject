using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    //[DataContract]
    public class ItemProvider
    {
        public ObservableCollection<Item> GetItems(string path)
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new Folder(directory.Name, directory.FullName)
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
        public ObservableCollection<Folder> GetFolders(string path)
        {
            ObservableCollection<Folder> folders = new ObservableCollection<Folder>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                Folder f = new Folder(directory.Name, directory.FullName)
                {
                    Items = GetItems(directory.FullName)
                };

                folders.Add(f);
            }
            return folders;
        }

        public long CalculateSize(DirectoryInfo dirInfo)
        {
            long size = 0;
            // Add file sizes.
            foreach (var file in dirInfo.GetFiles())
            {
                size += file.Length;
            }
            // Add subdirectory sizes.
            foreach (var directory in dirInfo.GetDirectories())
            {
                size += CalculateSize(new DirectoryInfo(directory.FullName));
            }
            //Console.WriteLine("Chiamato metodo CalculateSize per " + dirInfo.Name + " " + size.ToString());

            return size;
        }

    }
}

