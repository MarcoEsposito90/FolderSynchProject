using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.StreamedTransferService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FolderSynchMUIClient.Classes;
using System.Threading;

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for UploadDialog.xaml
    /// </summary>
    public partial class UploadDialog : ModernDialog
    {

        private string folderPath;

        /* ------------------------------------------------------------------------------ */
        /* ------------------ CONSTRUCTOR ----------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public UploadDialog(string folderPath)
        {
            InitializeComponent();

            this.folderPath = folderPath;
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
            OkButton.IsEnabled = false;
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ AT CONTENT RENDERED --------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {

            UploadBackgroundWorker bw = new UploadBackgroundWorker(folderPath);
            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = true;

            bw.ProgressChanged += UploadWork_ProgressChanged;
            bw.RunWorkerCompleted += UploadWork_Completed;

            bw.RunWorkerAsync();
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------ UPLOAD PROGRESS ------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void UploadWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("progress changed handler called");
            Console.WriteLine("progress = " + e.ProgressPercentage);
            UploadProgressBar.Value = e.ProgressPercentage;
            responseTB.Text=(e.ProgressPercentage).ToString();
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ UPLOAD COMPLETE ------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void UploadWork_Completed(object sender, RunWorkerCompletedEventArgs e)
        {

            if (!(e.Error == null))
                responseTB.Text = ("Error: " + e.Error.Message + " transfer not complete");
            else
                responseTB.Text = "Done!";

            OkButton.IsEnabled = true;
        }
    }
}
