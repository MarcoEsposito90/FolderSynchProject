using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchServiceNamespace
{
    [DataContract]
    public class RegistrationFault
    {

        public static string USERNAME_UNAVAILABLE = "Username not available";
        public static string SERVER_ERROR = "A server error occurred";


        [DataMember]
        public string Message
        {
            get;
            private set;
        }



        public RegistrationFault(string message)
        {
            this.Message = message;
        }

    }
}
