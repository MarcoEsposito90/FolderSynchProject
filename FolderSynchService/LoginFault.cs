using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchServiceNamespace
{
    [DataContract]
    class LoginFault
    {
        public static string WRONG_USERNAME_OR_PASSWORD = "Wrong username or password";
        public static string USER_ALREADY_IN = "This is user is already logged in";
        public static string SERVER_ERROR = "Service unavailable at the moment. Please try later";


        [DataMember]
        public string Message
        {
            get;
            private set;
        }

        public LoginFault(string message)
        {
            this.Message = message;
        }
    }
}
