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
        public static string USER_NOT_CONNECTED = "This user is not connected or does not exist";
        public static string NO_TRANSACTION_ACTIVE = "There is no transaction active for this update";
        public static string INCONSISTENT_UPDATE = "The upload brings to an inconsistent state";
        public static string NO_UPDATE_FOUND = "The update object relative to this upload is missing";


        public FileTransferFault(string message) : base(message) { }
    }
}
