using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FolderSynchService
{

    public class FolderSynchService : IFolderSynchService
    {
        public string DoWork()
        {
            return "the server executed the DoWork at: " + DateTime.Now.ToString();
        }
    }
}
