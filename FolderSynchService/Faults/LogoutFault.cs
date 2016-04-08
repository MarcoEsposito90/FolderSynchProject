using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class LogoutFault : MyBaseFault
    {
        public static string NO_SUCH_USER = "this user is not connected at the moment";

        public LogoutFault(string message): base(message) { }
    }
}
