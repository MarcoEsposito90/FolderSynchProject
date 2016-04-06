﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicesProject
{

    [ServiceContract (SessionMode = SessionMode.Required)]
    public interface FolderSynchServiceContract
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(RegistrationFault))]
        User RegisterNewUser(string username, string password);

        [OperationContract (IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(LoginFault))]
        User loginUser(string username, string password);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void logoutUser(User user);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        [FaultContract(typeof(MyBaseFault))]
        void addNewSynchronizedFolder(string folderName);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void uploadFile(string baseFolder, string localPath, byte[] data);
    }
}