using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [DataContract]
    public class Item
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public string SizeInBytes { get; set; }
    }
}
