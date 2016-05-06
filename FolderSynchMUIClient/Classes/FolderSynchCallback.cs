using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchMUIClient.FolderSynchService;

namespace FolderSynchMUIClient
{
    public class FolderSynchCallback : FolderSynchServiceContractCallback
    {
        public FolderSynchCallback() { }

        public void heartbeat()
        {
            Console.WriteLine("answering heartbeat request to the service");
        }
    }
}
