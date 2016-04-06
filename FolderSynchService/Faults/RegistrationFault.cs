using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class RegistrationFault : MyBaseFault
    {

        public static string USERNAME_UNAVAILABLE = "Username not available";
        public static string SERVER_ERROR = "A server error occurred";

        
        public RegistrationFault(string message) : base(message) { }
    }
}
