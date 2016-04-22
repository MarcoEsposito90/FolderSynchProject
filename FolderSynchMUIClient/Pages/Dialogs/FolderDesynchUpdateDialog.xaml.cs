using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
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
    /// Interaction logic for FolderDesynchUpdateDialog.xaml
    /// </summary>
    public partial class FolderDesynchUpdateDialog : ModernDialog
    {
        Dictionary<LocalFolder, Update> folderUpdates;
        List<DownloadBackgroundWorker> BackgroundWorkers;

        /* ---------------------------------------------------------------- */
        /* ------------ CONSTRUCTOR --------------------------------------- */
        /* ---------------------------------------------------------------- */

        public FolderDesynchUpdateDialog(Dictionary<LocalFolder,Update> folderAndLatestRemoteUpdates)
        {
            InitializeComponent();
            this.folderUpdates = folderAndLatestRemoteUpdates;
            BackgroundWorkers = new List<DownloadBackgroundWorker>();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
        }


        /* ---------------------------------------------------------------- */
        /* ------------ AT CONTENT RENDERED ------------------------------- */
        /* ---------------------------------------------------------------- */

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            OkButton.Visibility = Visibility.Hidden;
            folderList.ItemsSource = folderUpdates.Keys;
        }


        /* ---------------------------------------------------------------- */
        /* ------------ START RESYNCH ------------------------------------- */
        /* ---------------------------------------------------------------- */

        private void btnStartResynch_Click(object sender, RoutedEventArgs e)
        {
            // TODO: FOLDER RESYNCH
            btnStartResynch.Visibility = Visibility.Hidden;

            foreach(LocalFolder lf in folderUpdates.Keys)
            {
                Console.WriteLine("creating rollback downloader for folder: " + lf.Name);
                Update targetUpdate = null;
                folderUpdates.TryGetValue(lf, out targetUpdate);

                DownloadBackgroundWorker bw = new DownloadBackgroundWorker(lf, targetUpdate, lf.Path, true);
                BackgroundWorkers.Add(bw);
                bw.WorkerSupportsCancellation = false;
                bw.WorkerReportsProgress = true;

                bw.ProgressChanged += Update_ProgressChanged;
                bw.RunWorkerCompleted += Update_Completed;

                bw.RunWorkerAsync();
            }
        }



        /* ---------------------------------------------------------------- */
        /* ------------ UPDATE PROGRESS ----------------------------------- */
        /* ---------------------------------------------------------------- */

        private void Update_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadBackgroundWorker bw = (DownloadBackgroundWorker)sender;
            LocalFolder lf = bw.LocalFolder;
            ContentPresenter cp = folderList.ItemContainerGenerator.ContainerFromItem(lf) as ContentPresenter;
            ProgressBar pb = cp.ContentTemplate.FindName("DownloadProgressBar", cp) as ProgressBar;
            pb.Value = e.ProgressPercentage;
        }



        /* ---------------------------------------------------------------- */
        /* ------------ UPDATE COMPLETE ----------------------------------- */
        /* ---------------------------------------------------------------- */

        private void Update_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            DownloadBackgroundWorker bw = sender as DownloadBackgroundWorker;
            BackgroundWorkers.Remove(bw);

            if (BackgroundWorkers.Count == 0)
                OkButton.Visibility = Visibility.Visible;
        }

    }
}
