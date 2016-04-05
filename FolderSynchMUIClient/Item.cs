using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    //[DataContract]
    public class Item
    {
        //[DataMember]
        public string Name { get; set; }

       // [DataMember]
        public string Path { get; set; }
    }
}

