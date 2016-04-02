using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [DataContract]
    public class User
    {

        /* -------------- PROPERTIES -------------------------*/

        [DataMember]
        public string Password
        {
            get;
            private set;
        }

        [DataMember]
        public string Username
        {
            get;
            private set;
        }


        /* -------------- CONSTRUCTORS -------------------------*/

        public User(string username, string password)
        {

            this.Username = username;
            this.Password = password;
        }



        /* -------------- METHODS ---------------------------- */

        public bool changePassword(string oldPassword, string newPassword)
        {

            if (oldPassword.Equals(this.Password))
            {
                this.Password = newPassword;
                return true;
            }

            return false;
        }
    }
}
