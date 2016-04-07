using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.PerSession,
                        ConcurrencyMode = ConcurrencyMode.Single)]
    public class FolderSynchImplementation : FolderSynchServiceContract
    {
        User currentUser = null;

        
        /* ------------------------ USER ---------------------------------------- */
        public User loginUser(string username, string password)
        {
            currentUser = FolderSynchServer.Instance.LoginUser(username, password);
            return currentUser;
        }

        public void logoutUser(User user)
        {
            Console.WriteLine(user.Username + "is logging out");
            FolderSynchServer.Instance.logoutUser(user);
            currentUser = null;
        }

        public User RegisterNewUser(string username, string password)
        {
            currentUser = FolderSynchServer.Instance.registerNewUser(username, password);
            return currentUser;
        }


        /* ------------------------ FILE TRANSFER ---------------------------------------- */

        public void uploadFile(string baseFolder, string localPath, byte[] data)
        {
            Console.WriteLine(currentUser.Username + "wants to add a new file");
            FolderSynchServer.Instance.addNewFile(currentUser, baseFolder, localPath, data);
        }


        /* ------------------------ FOLDERS --------------------------------------------- */
        public void addNewSynchronizedFolder(string folderName)
        {
            Console.WriteLine(currentUser.Username + " wants to add a new folder: " + folderName);
            FolderSynchServer.Instance.AddNewFolder(currentUser, folderName);
        }
        
    }
}
