using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchServiceNamespace
{
    [DataContract]
    class LogoutFault
    {

        [DataMember]
        public string Message
        {
            get;
            private set;
        }
        
        public LogoutFault(string message)
        {
            this.Message = message;
        }
    }
}
