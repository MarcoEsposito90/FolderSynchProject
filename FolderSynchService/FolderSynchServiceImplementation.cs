using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchService
{
    public class FolderSynchServiceImplementation : FolderSynchServiceContract
    {
        public bool RegisterNewUser(string username, string password)
        {
            return FolderSynchServer.Instance.registerNewUser(username, password);
        }
    }
}
