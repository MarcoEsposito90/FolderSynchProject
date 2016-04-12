﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FolderSynchMUIClient.FolderSynchService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FolderSynchService.FolderSynchServiceContract", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface FolderSynchServiceContract {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FolderSynchServiceContract/RegisterNewUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/RegisterNewUserResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServicesProject.RegistrationFault), Action="http://tempuri.org/FolderSynchServiceContract/RegisterNewUserRegistrationFaultFau" +
            "lt", Name="RegistrationFault", Namespace="http://schemas.datacontract.org/2004/07/ServicesProject")]
        ServicesProject.User RegisterNewUser(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FolderSynchServiceContract/RegisterNewUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/RegisterNewUserResponse")]
        System.Threading.Tasks.Task<ServicesProject.User> RegisterNewUserAsync(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FolderSynchServiceContract/loginUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/loginUserResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServicesProject.LoginFault), Action="http://tempuri.org/FolderSynchServiceContract/loginUserLoginFaultFault", Name="LoginFault", Namespace="http://schemas.datacontract.org/2004/07/ServicesProject")]
        ServicesProject.User loginUser(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FolderSynchServiceContract/loginUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/loginUserResponse")]
        System.Threading.Tasks.Task<ServicesProject.User> loginUserAsync(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsTerminating=true, IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/logoutUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/logoutUserResponse")]
        void logoutUser(ServicesProject.User user);
        
        [System.ServiceModel.OperationContractAttribute(IsTerminating=true, IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/logoutUser", ReplyAction="http://tempuri.org/FolderSynchServiceContract/logoutUserResponse")]
        System.Threading.Tasks.Task logoutUserAsync(ServicesProject.User user);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/addNewSynchronizedFolder", ReplyAction="http://tempuri.org/FolderSynchServiceContract/addNewSynchronizedFolderResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServicesProject.MyBaseFault), Action="http://tempuri.org/FolderSynchServiceContract/addNewSynchronizedFolderMyBaseFault" +
            "Fault", Name="MyBaseFault", Namespace="http://schemas.datacontract.org/2004/07/ServicesProject")]
        void addNewSynchronizedFolder(ServicesProject.Folder folderName);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/addNewSynchronizedFolder", ReplyAction="http://tempuri.org/FolderSynchServiceContract/addNewSynchronizedFolderResponse")]
        System.Threading.Tasks.Task addNewSynchronizedFolderAsync(ServicesProject.Folder folderName);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/beginUpdate", ReplyAction="http://tempuri.org/FolderSynchServiceContract/beginUpdateResponse")]
        ServicesProject.UpdateTransaction beginUpdate(string baseFolder, System.DateTime timestamp);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/beginUpdate", ReplyAction="http://tempuri.org/FolderSynchServiceContract/beginUpdateResponse")]
        System.Threading.Tasks.Task<ServicesProject.UpdateTransaction> beginUpdateAsync(string baseFolder, System.DateTime timestamp);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/uploadFile", ReplyAction="http://tempuri.org/FolderSynchServiceContract/uploadFileResponse")]
        void uploadFile(string transactionID, string baseFolder, string localPath, byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/uploadFile", ReplyAction="http://tempuri.org/FolderSynchServiceContract/uploadFileResponse")]
        System.Threading.Tasks.Task uploadFileAsync(string transactionID, string baseFolder, string localPath, byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/addSubDirectory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/addSubDirectoryResponse")]
        void addSubDirectory(string transactionID, string baseFolder, string localPath);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/addSubDirectory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/addSubDirectoryResponse")]
        System.Threading.Tasks.Task addSubDirectoryAsync(string transactionID, string baseFolder, string localPath);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/updateCommit", ReplyAction="http://tempuri.org/FolderSynchServiceContract/updateCommitResponse")]
        void updateCommit(ServicesProject.UpdateTransaction transaction);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/updateCommit", ReplyAction="http://tempuri.org/FolderSynchServiceContract/updateCommitResponse")]
        System.Threading.Tasks.Task updateCommitAsync(ServicesProject.UpdateTransaction transaction);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/getFileHistory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/getFileHistoryResponse")]
        ServicesProject.Update.UpdateEntry[] getFileHistory(string baseFolder, string localPath);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/getFileHistory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/getFileHistoryResponse")]
        System.Threading.Tasks.Task<ServicesProject.Update.UpdateEntry[]> getFileHistoryAsync(string baseFolder, string localPath);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/getHistory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/getHistoryResponse")]
        ServicesProject.Update[] getHistory(string baseFolder);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/getHistory", ReplyAction="http://tempuri.org/FolderSynchServiceContract/getHistoryResponse")]
        System.Threading.Tasks.Task<ServicesProject.Update[]> getHistoryAsync(string baseFolder);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/downloadFile", ReplyAction="http://tempuri.org/FolderSynchServiceContract/downloadFileResponse")]
        byte[] downloadFile(string baseFolder, string localPath, int updateNumber);
        
        [System.ServiceModel.OperationContractAttribute(IsInitiating=false, Action="http://tempuri.org/FolderSynchServiceContract/downloadFile", ReplyAction="http://tempuri.org/FolderSynchServiceContract/downloadFileResponse")]
        System.Threading.Tasks.Task<byte[]> downloadFileAsync(string baseFolder, string localPath, int updateNumber);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface FolderSynchServiceContractChannel : FolderSynchMUIClient.FolderSynchService.FolderSynchServiceContract, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FolderSynchServiceContractClient : System.ServiceModel.ClientBase<FolderSynchMUIClient.FolderSynchService.FolderSynchServiceContract>, FolderSynchMUIClient.FolderSynchService.FolderSynchServiceContract {
        
        public FolderSynchServiceContractClient() {
        }
        
        public FolderSynchServiceContractClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FolderSynchServiceContractClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FolderSynchServiceContractClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FolderSynchServiceContractClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ServicesProject.User RegisterNewUser(string username, string password) {
            return base.Channel.RegisterNewUser(username, password);
        }
        
        public System.Threading.Tasks.Task<ServicesProject.User> RegisterNewUserAsync(string username, string password) {
            return base.Channel.RegisterNewUserAsync(username, password);
        }
        
        public ServicesProject.User loginUser(string username, string password) {
            return base.Channel.loginUser(username, password);
        }
        
        public System.Threading.Tasks.Task<ServicesProject.User> loginUserAsync(string username, string password) {
            return base.Channel.loginUserAsync(username, password);
        }
        
        public void logoutUser(ServicesProject.User user) {
            base.Channel.logoutUser(user);
        }
        
        public System.Threading.Tasks.Task logoutUserAsync(ServicesProject.User user) {
            return base.Channel.logoutUserAsync(user);
        }
        
        public void addNewSynchronizedFolder(ServicesProject.Folder folderName) {
            base.Channel.addNewSynchronizedFolder(folderName);
        }
        
        public System.Threading.Tasks.Task addNewSynchronizedFolderAsync(ServicesProject.Folder folderName) {
            return base.Channel.addNewSynchronizedFolderAsync(folderName);
        }
        
        public ServicesProject.UpdateTransaction beginUpdate(string baseFolder, System.DateTime timestamp) {
            return base.Channel.beginUpdate(baseFolder, timestamp);
        }
        
        public System.Threading.Tasks.Task<ServicesProject.UpdateTransaction> beginUpdateAsync(string baseFolder, System.DateTime timestamp) {
            return base.Channel.beginUpdateAsync(baseFolder, timestamp);
        }
        
        public void uploadFile(string transactionID, string baseFolder, string localPath, byte[] data) {
            base.Channel.uploadFile(transactionID, baseFolder, localPath, data);
        }
        
        public System.Threading.Tasks.Task uploadFileAsync(string transactionID, string baseFolder, string localPath, byte[] data) {
            return base.Channel.uploadFileAsync(transactionID, baseFolder, localPath, data);
        }
        
        public void addSubDirectory(string transactionID, string baseFolder, string localPath) {
            base.Channel.addSubDirectory(transactionID, baseFolder, localPath);
        }
        
        public System.Threading.Tasks.Task addSubDirectoryAsync(string transactionID, string baseFolder, string localPath) {
            return base.Channel.addSubDirectoryAsync(transactionID, baseFolder, localPath);
        }
        
        public void updateCommit(ServicesProject.UpdateTransaction transaction) {
            base.Channel.updateCommit(transaction);
        }
        
        public System.Threading.Tasks.Task updateCommitAsync(ServicesProject.UpdateTransaction transaction) {
            return base.Channel.updateCommitAsync(transaction);
        }
        
        public ServicesProject.Update.UpdateEntry[] getFileHistory(string baseFolder, string localPath) {
            return base.Channel.getFileHistory(baseFolder, localPath);
        }
        
        public System.Threading.Tasks.Task<ServicesProject.Update.UpdateEntry[]> getFileHistoryAsync(string baseFolder, string localPath) {
            return base.Channel.getFileHistoryAsync(baseFolder, localPath);
        }
        
        public ServicesProject.Update[] getHistory(string baseFolder) {
            return base.Channel.getHistory(baseFolder);
        }
        
        public System.Threading.Tasks.Task<ServicesProject.Update[]> getHistoryAsync(string baseFolder) {
            return base.Channel.getHistoryAsync(baseFolder);
        }
        
        public byte[] downloadFile(string baseFolder, string localPath, int updateNumber) {
            return base.Channel.downloadFile(baseFolder, localPath, updateNumber);
        }
        
        public System.Threading.Tasks.Task<byte[]> downloadFileAsync(string baseFolder, string localPath, int updateNumber) {
            return base.Channel.downloadFileAsync(baseFolder, localPath, updateNumber);
        }
    }
}
