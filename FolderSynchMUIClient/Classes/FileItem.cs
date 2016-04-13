using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public class FileItem : Item
    {
        [DataMember]
        public LocalFolder parentFolder { get; set; }

        [DataMember]
        public string RelativePath { get; private set; }

        public FileItem(string name, string relativePath)
        {
            this.Name = name;
            this.RelativePath = relativePath;
        }
    }
}
