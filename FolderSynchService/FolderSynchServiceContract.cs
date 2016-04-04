using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FolderSynchService
{

    [ServiceContract]
    public interface FolderSynchServiceContract
    {
        [OperationContract]
        [FaultContract(typeof(RegistrationFault))]
        void RegisterNewUser(string username, string password);

        [OperationContract]
        void loginUser(string username, string password);

    }
}
