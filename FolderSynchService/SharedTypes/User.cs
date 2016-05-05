using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class User
    {

        /* -------------- PROPERTIES -------------------------*/

        [DataMember]
        public string Username
        {
            get;
            private set;
        }

        [DataMember]
        public string Password
        {
            get;
            set;
        }

        [DataMember]
        public List<Folder> Folders
        {
            get;
            private set;
        }

        [DataMember]
        public List<Installation> Installations
        {
            get;
            private set;
        }

        [DataMember]
        public Installation LastAccessDevice
        {
            get;
            set;
        }

        /* -------------- CONSTRUCTORS -------------------------*/

        public User(string username, string password)
        {

            this.Username = username;
            this.Password = password;
            Folders = new List<Folder>();
            Installations = new List<Installation>();
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
