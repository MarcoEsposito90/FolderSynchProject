using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
   // [DataContract]
    class FileItem : Item
    {
        //[DataMember]
        public Folder parentFolder { get; set; }

        public FileItem(string name, string path) {
            this.Name = name;
            this.Path = path;
        }
    }
}
