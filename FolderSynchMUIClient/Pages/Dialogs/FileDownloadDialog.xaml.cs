using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for FileDownloadDialog.xaml
    /// </summary>
    public partial class FileDownloadDialog : ModernDialog
    {
        private LocalFolder LocalFolder;
        private Update.UpdateEntry Entry;
        public string DownloadPath;


        /* ------------------------------------------------------------------------------ */
        /* ------------------ CONSTRUCTOR ----------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public FileDownloadDialog(Update.UpdateEntry entry, LocalFolder localFolder)
        {
            InitializeComponent();
            this.Entry = entry;
            this.LocalFolder = localFolder;

            Owner = Application.Current.MainWindow;
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton};
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ AT CONTENT RENDERED --------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void FileDownloadDialog_ContentRendered(object sender, EventArgs e)
        {
            this.OkButton.Visibility = Visibility.Hidden;
            DownloadProgressBar.Visibility = Visibility.Hidden;
            responseTB.Visibility = Visibility.Hidden;
            TBDownloadQuestion.Text += Entry.EntryTimestamp.ToString() + "?";
            choosedFolderPathEditor.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ CHANDE DIRECTORY ------------------------------------------ */
        /* ------------------------------------------------------------------------------ */

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            //Showing folder picker dialog
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedFolderPathEditor.Text = openFolderDialog.FileName.ToString();
                DownloadPath = choosedFolderPathEditor.Text;
            }
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ START DOWNLOAD -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Entry.ItemLocalPath;
            string[] tokens = filePath.Split('\\');
            string name = tokens[tokens.Length - 1];
            name = DownloadPath + "\\" + name;

            Console.WriteLine("downloaded file path will be: " + name);
            DownloadProgressBar.Visibility = Visibility.Visible;
            responseTB.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Hidden;

            DownloadBackgroundWorker bw = new DownloadBackgroundWorker(LocalFolder, Entry, name);
            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = true;

            bw.ProgressChanged += Download_ProgressChanged;
            bw.RunWorkerCompleted += Download_Complete;

            bw.RunWorkerAsync();
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ DOWNLOAD PROGRESS ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void Download_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // should not get here
            Console.WriteLine("progress download: " + e.ProgressPercentage);
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ DOWNLOAD COMPLETE ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void Download_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            DownloadBackgroundWorker.DownloadWorkerResponse response = (DownloadBackgroundWorker.DownloadWorkerResponse)e.Result;
            DownloadProgressBar.Visibility = Visibility.Hidden;

            if (response.Success)
                responseTB.Text = "Download completed succesfully";
            else
                responseTB.Text = response.ErrorMessage + "\nUnable to complete download";

            OkButton.Visibility = Visibility.Visible;
        }
    }
}
