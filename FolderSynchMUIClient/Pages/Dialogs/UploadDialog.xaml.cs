using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for UploadDialog.xaml
    /// </summary>
    public partial class UploadDialog : ModernDialog
    {

        private string folderPath;

        public UploadDialog(string folderPath)
        {
            InitializeComponent();

            this.folderPath = folderPath;
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
            OkButton.IsEnabled = false;
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {

            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            StreamedTransferContractClient streamProxy = application.StreamTransferProxy;

            string[] directories = folderPath.Split('\\');
            string folderName = directories[directories.Length - 1];

            try
            {
                if (application.User != null)
                {
                    proxy.addNewSynchronizedFolder(folderName);

                    string[] files = Directory.GetFiles(folderPath);

                    foreach (string file in files)
                    {
                        responseTB.Text += "\ntrying to upload " + file;
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
                            responseTB.Text += "\nfile size = " + fi.Length;

                            if (fi.Length > App.MAX_BUFFERED_TRANSFER_FILE_SIZE)
                            {
                                responseTB.Text += "\nproceeding with streamed transfer";
                                streamProxy.uploadFileStreamed(folderName, localPath, application.User.Username, uploadStream);
                            }
                            else
                            {
                                responseTB.Text += "\nproceeding with buffered transfer";
                                byte[] buffer = new byte[App.MAX_BUFFERED_TRANSFER_FILE_SIZE];
                                uploadStream.Read(buffer, 0, App.MAX_BUFFERED_TRANSFER_FILE_SIZE);
                                proxy.uploadFile(folderName, localPath, buffer);
                            }
                        }

                        responseTB.Text += "\nuploaded";
                    }

                }
                else
                    responseTB.Text = "please login";

            }
            catch (FaultException f)
            {
                //responseLabel.Content += "\nerror: " + f.Message;
                Console.WriteLine("error: " + f.Reason);
            }

            this.DialogResult = true;
        }
    }
}
