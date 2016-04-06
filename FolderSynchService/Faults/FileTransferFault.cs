using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    public class FileTransferFault : MyBaseFault
    {
        public static string UNKNOWN_BASE_FOLDER = "specified base folder does not exist";

        public FileTransferFault(string message) : base(message) { }
    }
}
