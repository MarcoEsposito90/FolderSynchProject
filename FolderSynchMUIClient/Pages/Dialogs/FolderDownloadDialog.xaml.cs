using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.ComponentModel;
using System.IO;

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for FolderDownloadDialog.xaml
    /// </summary>
    public partial class FolderDownloadDialog : ModernDialog
    {
        private Folder Folder;
        private LocalFolder localFolder;
        private string downloadFolderName;
        public bool Success { get; private set; }

        public FolderDownloadDialog(Folder folder)
        {
            InitializeComponent();

            this.Title = "Download " + folder.FolderName;
            this.Folder = folder;
            Success = false;
            Owner = Application.Current.MainWindow;

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------ AT CONTENT RENDERED --------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {

            choosedPathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DownloadProgressBar.Visibility = Visibility.Hidden;
            OkButton.Visibility = Visibility.Hidden;
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ BROWSER --------------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedPathTextBox.Text = openFolderDialog.FileName.ToString();
            }
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ DOWNLOAD -------------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnStartDownload_Click(object sender, RoutedEventArgs e)
        {
            downloadFolderName = choosedPathTextBox.Text + "\\" + Folder.FolderName;
            Directory.CreateDirectory(downloadFolderName);
            bool deleteCurrent = false;

            App application = (App)Application.Current;
            localFolder = new LocalFolder(application.User.Username, Folder.FolderName, "");
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            List<Update> Updates = new List<Update>(proxy.getHistory(Folder.FolderName));
            Update latest = Updates.ElementAt(Updates.Count - 1);

            DownloadBackgroundWorker bw = new DownloadBackgroundWorker(localFolder, latest, downloadFolderName, deleteCurrent);

            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += DownloadWork_ProgressChanged;
            bw.RunWorkerCompleted += DownloadWork_Completed;
            bw.RunWorkerAsync();
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ DOWNLOAD COMPLETED ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void DownloadWork_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("BackgroundWorker error: " + e.Error);
                warningTB.Text = "Error: " + e.Error + ". Unable to complete";
                return;
            }

            DownloadBackgroundWorker.DownloadWorkerResponse response = (DownloadBackgroundWorker.DownloadWorkerResponse)e.Result;
            if (!response.Success)
            {
                warningTB.Text = response.ErrorMessage;
                return;
            }

            localFolder.Path = downloadFolderName;
            App application = (App)Application.Current;
            application.addLocalFolder(Folder, localFolder);

            warningTB.Text = "Operation completed succesfully!";
            CancelButton.Visibility = Visibility.Hidden;
            OkButton.Visibility = Visibility.Visible;
            Success = true;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ DOWNLOAD PROGRESS ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void DownloadWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("progress at: " + e.ProgressPercentage);
            DownloadProgressBar.Value = e.ProgressPercentage;
            responseTB.Text = (e.ProgressPercentage).ToString();
        }
    }
}
