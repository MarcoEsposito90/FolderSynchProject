﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FolderSynchMUIClient.StreamedTransferService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="StreamedTransferService.StreamedTransferContract")]
    public interface StreamedTransferContract {
        
        // CODEGEN: Generazione di un contratto di messaggio perché l'operazione uploadFileStreamed non è RPC né incapsulata da documenti.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/uploadFileStreamed", ReplyAction="http://tempuri.org/StreamedTransferContract/uploadFileStreamedResponse")]
        FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse uploadFileStreamed(FolderSynchMUIClient.StreamedTransferService.FileStreamMessage request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/uploadFileStreamed", ReplyAction="http://tempuri.org/StreamedTransferContract/uploadFileStreamedResponse")]
        System.Threading.Tasks.Task<FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse> uploadFileStreamedAsync(FolderSynchMUIClient.StreamedTransferService.FileStreamMessage request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/downloadFileStreamed", ReplyAction="http://tempuri.org/StreamedTransferContract/downloadFileStreamedResponse")]
        System.IO.Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/downloadFileStreamed", ReplyAction="http://tempuri.org/StreamedTransferContract/downloadFileStreamedResponse")]
        System.Threading.Tasks.Task<System.IO.Stream> downloadFileStreamedAsync(string username, string baseFolder, string localPath, int updateNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/heartBeat", ReplyAction="http://tempuri.org/StreamedTransferContract/heartBeatResponse")]
        void heartBeat(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StreamedTransferContract/heartBeat", ReplyAction="http://tempuri.org/StreamedTransferContract/heartBeatResponse")]
        System.Threading.Tasks.Task heartBeatAsync(string username);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="FileStreamMessage", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class FileStreamMessage {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string baseFolder;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string localPath;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string transactionID;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public int updateType;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string username;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public System.IO.Stream data;
        
        public FileStreamMessage() {
        }
        
        public FileStreamMessage(string baseFolder, string localPath, string transactionID, int updateType, string username, System.IO.Stream data) {
            this.baseFolder = baseFolder;
            this.localPath = localPath;
            this.transactionID = transactionID;
            this.updateType = updateType;
            this.username = username;
            this.data = data;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class uploadFileStreamedResponse {
        
        public uploadFileStreamedResponse() {
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface StreamedTransferContractChannel : FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class StreamedTransferContractClient : System.ServiceModel.ClientBase<FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract>, FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract {
        
        public StreamedTransferContractClient() {
        }
        
        public StreamedTransferContractClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public StreamedTransferContractClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StreamedTransferContractClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StreamedTransferContractClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract.uploadFileStreamed(FolderSynchMUIClient.StreamedTransferService.FileStreamMessage request) {
            return base.Channel.uploadFileStreamed(request);
        }
        
        public void uploadFileStreamed(string baseFolder, string localPath, string transactionID, int updateType, string username, System.IO.Stream data) {
            FolderSynchMUIClient.StreamedTransferService.FileStreamMessage inValue = new FolderSynchMUIClient.StreamedTransferService.FileStreamMessage();
            inValue.baseFolder = baseFolder;
            inValue.localPath = localPath;
            inValue.transactionID = transactionID;
            inValue.updateType = updateType;
            inValue.username = username;
            inValue.data = data;
            FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse retVal = ((FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract)(this)).uploadFileStreamed(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse> FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract.uploadFileStreamedAsync(FolderSynchMUIClient.StreamedTransferService.FileStreamMessage request) {
            return base.Channel.uploadFileStreamedAsync(request);
        }
        
        public System.Threading.Tasks.Task<FolderSynchMUIClient.StreamedTransferService.uploadFileStreamedResponse> uploadFileStreamedAsync(string baseFolder, string localPath, string transactionID, int updateType, string username, System.IO.Stream data) {
            FolderSynchMUIClient.StreamedTransferService.FileStreamMessage inValue = new FolderSynchMUIClient.StreamedTransferService.FileStreamMessage();
            inValue.baseFolder = baseFolder;
            inValue.localPath = localPath;
            inValue.transactionID = transactionID;
            inValue.updateType = updateType;
            inValue.username = username;
            inValue.data = data;
            return ((FolderSynchMUIClient.StreamedTransferService.StreamedTransferContract)(this)).uploadFileStreamedAsync(inValue);
        }
        
        public System.IO.Stream downloadFileStreamed(string username, string baseFolder, string localPath, int updateNumber) {
            return base.Channel.downloadFileStreamed(username, baseFolder, localPath, updateNumber);
        }
        
        public System.Threading.Tasks.Task<System.IO.Stream> downloadFileStreamedAsync(string username, string baseFolder, string localPath, int updateNumber) {
            return base.Channel.downloadFileStreamedAsync(username, baseFolder, localPath, updateNumber);
        }
        
        public void heartBeat(string username) {
            base.Channel.heartBeat(username);
        }
        
        public System.Threading.Tasks.Task heartBeatAsync(string username) {
            return base.Channel.heartBeatAsync(username);
        }
    }
}
