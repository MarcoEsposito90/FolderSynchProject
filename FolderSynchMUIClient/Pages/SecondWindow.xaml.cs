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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for SecondWindow.xaml
    /// </summary>
    public partial class SecondWindow : ModernWindow
    {
        public SecondWindow()
        {
            InitializeComponent();
        }

        private void ModernWindow_ContentRendered(object sender, EventArgs e)
        {
            //Console.WriteLine("Secondwindow contentrendered called");
            //App application = (App)Application.Current;

            //// check if some synced folder is missing on this device ---------------------------------------------
            //List<Folder> missingFolders = new List<Folder>();
            //Dictionary<LocalFolder, Update> needResynch = new Dictionary<LocalFolder, Update>();

            //foreach (Folder f in application.User.Folders)
            //{
            //    LocalFolder lf = null;

            //    foreach (LocalFolder locFold in application.LocalFolders)
            //        if (locFold.Name.Equals(f.FolderName))
            //            lf = locFold;

            //    if (lf == null)
            //    {
            //        Console.WriteLine(f.FolderName + " is missing");
            //        missingFolders.Add(f);
            //    }
            //    else
            //    {
            //        // check if local folder needs to be resynched ----------------------------------------------
            //        Update lastLocalUpdate = lf.Updates.ElementAt(lf.Updates.Count - 1);
            //        List<Update> remoteUpdates = new List<Update>(application.FolderSynchProxy.getHistory(lf.Name));
            //        Update lastRemoteUpdate = remoteUpdates.ElementAt(remoteUpdates.Count - 1);

            //        Console.WriteLine("******************************************************************************");
            //        Console.WriteLine("Checking synchronization for folder: " + lf.Name);

            //        Console.WriteLine("------------------------------------------------------");
            //        Console.WriteLine("local update: ");
            //        printUpdate(lastLocalUpdate);

            //        Console.WriteLine("------------------------------------------------------");
            //        Console.WriteLine("remote update: ");
            //        printUpdate(lastRemoteUpdate);

            //        if (!lastRemoteUpdate.TransactionID.Equals(lastLocalUpdate.TransactionID))
            //        {
            //            Console.WriteLine(lf.Name + " is not synchronized! localUpdate: " +
            //                                lf.Updates.ElementAt(lf.Updates.Count - 1).TransactionID +
            //                                "; remoteUpdate: " + remoteUpdates.ElementAt(remoteUpdates.Count - 1).TransactionID);

            //            needResynch.Add(lf, lastRemoteUpdate);
            //        }
            //    }
            //}

            //// show dialogs if necessary ---------------------------------------------------------------
            //if (missingFolders.Count > 0)
            //{
            //    LocalFoldersWarningDialog dialog1 = new LocalFoldersWarningDialog(missingFolders);
            //    dialog1.ShowDialog();
            //}

            //if (needResynch.Count > 0)
            //{
            //    FolderDesynchUpdateDialog dialog2 = new FolderDesynchUpdateDialog(needResynch);
            //    dialog2.ShowDialog();
            //}

            //application.startWatching();
        }



        /* ---------------------------------------------------------------- */
        /* ------------ AUXILIARY ----------------------------------------- */
        /* ---------------------------------------------------------------- */

        private void printUpdate(Update update)
        {
            Console.WriteLine("Update number: " + update.Number);
            Console.WriteLine("timestamp: " + update.Timestamp);
            Console.WriteLine("transaction: " + update.TransactionID);
            Console.WriteLine("number of changes: " + update.UpdateEntries.Length);
        }
    }
}
