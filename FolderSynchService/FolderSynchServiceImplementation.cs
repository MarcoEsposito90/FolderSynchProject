using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    public class FolderSynchServiceImplementation : FolderSynchServiceContract
    {
        public void RegisterNewUser(string username, string password)
        {
            FolderSynchServer.Instance.registerNewUser(username, password);
        }
    }
}
