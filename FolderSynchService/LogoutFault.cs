using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [DataContract]
    public class LogoutFault : MyBaseFault
    {

        public LogoutFault(string message): base(message) { }
    }
}
