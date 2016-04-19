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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for RollbackDialog.xaml
    /// </summary>
    public partial class RollbackDialog : ModernDialog
    {
        private LocalFolder localFolder;
        private Update update;

        /* ------------------------------------------------------------------------------ */
        /* ------------------ CONSTRUCTOR ----------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public RollbackDialog(LocalFolder localFolder, Update update)
        {
            InitializeComponent();
            this.localFolder = localFolder;
            this.update = update;

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ AT CONTENT RENDERED --------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            OkButton.Visibility = Visibility.Hidden;
            btnStartDownload.Visibility = Visibility.Hidden;
            btnBrowsePath.Visibility = Visibility.Hidden;
            choosedPathTextBox.Visibility = Visibility.Hidden;
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
        /* ------------------ RADIO BUTTONS --------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Equals(btnDownloadOld))
            {
                btnBrowsePath.Visibility = Visibility.Visible;
                choosedPathTextBox.Visibility = Visibility.Visible;
            }

            btnStartDownload.Visibility = Visibility.Visible;
        }

        private void radioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            btnBrowsePath.Visibility = Visibility.Hidden;
            choosedPathTextBox.Visibility = Visibility.Hidden;
            btnStartDownload.Visibility = Visibility.Hidden;
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------ START DOWNLOAD -------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnStartDownload_Click(object sender, RoutedEventArgs e)
        {
            RollbackConfirmDialog.Option op;

            if (btnDeleteOld.IsChecked == true)
                op = RollbackConfirmDialog.Option.DeleteCurrent;
            else if (btnKeepOld.IsChecked == true)
                op = RollbackConfirmDialog.Option.KeepCurrent;
            else if (btnDownloadOld.IsChecked == true)
                op = RollbackConfirmDialog.Option.SimpleDownload;
            else
            {
                warningTB.Text = "Please select an option before proceeding!";
                return;
            }
            

            RollbackConfirmDialog dialog = new RollbackConfirmDialog(op, update);
            if (dialog.ShowDialog() == true)
            {

                Console.WriteLine("asked to proceed with rollback/download");
                DownloadBackgroundWorker bw = new DownloadBackgroundWorker(localFolder, update, "");

                bw.WorkerSupportsCancellation = false;
                bw.WorkerReportsProgress = true;
                bw.ProgressChanged += DownloadWork_ProgressChanged;
                bw.RunWorkerCompleted += DownloadWork_Completed;
                bw.RunWorkerAsync();
            }
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ COMPLETED DOWNLOAD ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void DownloadWork_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed download");
        }


        /* ------------------------------------------------------------------------------ */
        /* ------------------ PROGRESS DOWNLOAD ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void DownloadWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("progress at: " + e.ProgressPercentage);
        }
    }
}
