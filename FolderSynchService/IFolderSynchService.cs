﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FolderSynchService
{

    [ServiceContract]
    public interface IFolderSynchService
    {
        [OperationContract]
        bool RegisterNewUser(string username, string password);

    }
}
