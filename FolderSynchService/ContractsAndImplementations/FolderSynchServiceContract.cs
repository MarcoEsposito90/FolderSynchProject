using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicesProject
{

    [ServiceContract (SessionMode = SessionMode.Required, CallbackContract = typeof(FolderSynchCallbackContract))]
    public interface FolderSynchServiceContract
    {
        /**********************************************************************************************************/
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(RegistrationFault))]
        User RegisterNewUser(string username, string password, string machineName);

        [OperationContract (IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(LoginFault))]
        User loginUser(string username, string password, string machineName); 

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void logoutUser(User user);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void changeCredentials(string oldPassword, string newPassword);

        /**********************************************************************************************************/
        [OperationContract(IsInitiating = false, IsTerminating = false)]
        [FaultContract(typeof(MyBaseFault))]
        void addNewSynchronizedFolder(Folder folderName);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        UpdateTransaction beginUpdate(string baseFolder, DateTime timestamp);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void uploadFile(string transactionID, string baseFolder, string localPath, int type, byte[] data);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void deleteFile(string transactionID, string baseFolder, string localPath);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void addSubDirectory(string transactionID, string baseFolder, string localPath);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void deleteSubDirectory(string transactionID, string baseFolder, string localPath);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Update updateCommit(UpdateTransaction transaction);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void updateAbort(UpdateTransaction transaction);

        /**********************************************************************************************************/
        [OperationContract(IsInitiating = false, IsTerminating = false)]
        List<Update.UpdateEntry> getFileHistory(string baseFolder, string localPath);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        List<Update> getHistory(string baseFolder);

        /**********************************************************************************************************/
        [OperationContract(IsInitiating = false, IsTerminating = false)]
        List<Update.UpdateEntry> getUpdateFileList(Update update);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        RollbackTransaction beginRollback(Update update, DateTime timestamp);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        byte[] downloadFile(string baseFolder, string localPath, int updateNumber);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void commitRollback(RollbackTransaction transaction);


        /**********************************************************************************************************/
        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void changeFolderOptions(string folderName, Folder updatedFolder);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void removeSynchronizedFolder(string folderName);
        
    }
}
