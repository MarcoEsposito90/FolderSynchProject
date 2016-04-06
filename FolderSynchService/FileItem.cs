using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [DataContract]
    public class FileItem : Item
    {
        [DataMember]
        public Folder parentFolder { get; set; }

        public FileItem(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
