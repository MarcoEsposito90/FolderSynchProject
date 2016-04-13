using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchMUIClient
{
    [DataContract]
    public abstract class Item
    {
        [DataMember]
        public string Name { get; set; }

        public long CurrentSize
        {
            get
            {
                return CalculateSize(this.LocalPath);
            }
        }

        public string SizeInBytes
        {
            get
            {
                return SizeSuffix(CalculateSize(this.LocalPath));
            }
        }

        [DataMember]
        public string LocalPath { get; set; }
        
        
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public abstract long CalculateSize(string path);


        /*********************************************************************/
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
