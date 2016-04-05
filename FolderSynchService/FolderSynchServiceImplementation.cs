using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.PerSession,
                        ConcurrencyMode = ConcurrencyMode.Single)]
    public class FolderSynchServiceImplementation : FolderSynchServiceContract
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
            FolderSynchServer.Instance.logoutUser(user);
            currentUser = null;
        }

        public User RegisterNewUser(string username, string password)
        {
            currentUser = FolderSynchServer.Instance.registerNewUser(username, password);
            return currentUser;
        }


        /* ------------------------ FOLDERS ---------------------------------------- */

        public void uploadNewFile(UploadFileStreamMessage message)
        {
            Console.WriteLine(currentUser.Username + "wants to add a new file");
            FolderSynchServer.Instance.addNewFile(currentUser, message.baseFolder, message.localPath, message.data);
        }

        public void addNewSynchronizedFolder(string folderName)
        {
            Console.WriteLine(currentUser.Username + " wants to add a new folder: " + folderName);
            FolderSynchServer.Instance.AddNewFolder(currentUser, folderName);
        }
        
    }
}
