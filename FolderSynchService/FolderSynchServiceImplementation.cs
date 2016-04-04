using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    public class FolderSynchServiceImplementation : FolderSynchServiceContract
    {
        public void loginUser(string username, string password)
        {
            FolderSynchServer.Instance.LoginUser(username, password);
        }

        public void RegisterNewUser(string username, string password)
        {
            FolderSynchServer.Instance.registerNewUser(username, password);
        }
    }
}
