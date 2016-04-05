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

    }
}

