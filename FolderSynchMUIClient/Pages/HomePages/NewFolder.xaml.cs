using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using System.ServiceModel;
using System.IO;

namespace FolderSynchMUIClient.Pages.HomePages
{
    /// <summary>
    /// Interaction logic for NewFolder.xaml
    /// </summary>
    public partial class NewFolder : UserControl
    {
        public NewFolder()
        {
            InitializeComponent();
        }

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedFolderPathEditor.Text = openFolderDialog.FileName.ToString();
            }
        }

        private void btnSynchFolder_Click(object sender, RoutedEventArgs e)
        {

            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            StreamedTransferContractClient streamProxy = application.StreamTransferProxy;

            string[] directories = choosedFolderPathEditor.Text.Split('\\');
            string folderName = directories[directories.Length - 1];

            Console.WriteLine("adding new folder: " + folderName);

            try
            {
                if (application.User != null)
                {
                    proxy.addNewSynchronizedFolder(folderName);

                    string[] files = Directory.GetFiles(choosedFolderPathEditor.Text);

                    foreach(string file in files)
                    {
                        responseLabel.Content += "\ntrying to upload " + file;
                        string[] path = file.Split('\\');
                        string localPath = "";

                        for (int i = path.Length - 1; i >= 0; i--)
                        {
                            if (path[i].Equals(folderName))
                                break;

                            localPath += path[i] + localPath;
                        }

                        using (Stream uploadStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            FileInfo fi = new FileInfo(file);
                            responseLabel.Content += "\nfile size = " + fi.Length;

                            if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                            {
                                responseLabel.Content += "\nproceeding with streamed transfer";
                                streamProxy.uploadFileStreamed(folderName, localPath, application.User.Username, uploadStream);
                            }
                            else
                            {
                                byte[] buffer = new byte[App.MAX_BUFFERED_TRANSFER_FILE_SIZE];
                                uploadStream.Read(buffer, 0, App.MAX_BUFFERED_TRANSFER_FILE_SIZE);
                                proxy.uploadFile(folderName, localPath, buffer);
                            }
                        }

                        responseLabel.Content += "\nuploaded";
                    }
                        
                }
                else
                    responseLabel.Content = "please login";



            }
            catch(FaultException f)
            {
                responseLabel.Content += "\nerror: " + f.Message;
            }
            
        }
    }
}
