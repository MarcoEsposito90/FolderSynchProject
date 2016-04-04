using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FolderSynchService
{

    [ServiceContract (SessionMode = SessionMode.Required)]
    public interface FolderSynchServiceContract
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(RegistrationFault))]
        void RegisterNewUser(string username, string password);

        [OperationContract (IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(LoginFault))]
        User loginUser(string username, string password);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void logoutUser(User user);
    }
}
