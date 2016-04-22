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
using System.ComponentModel;
using FirstFloor.ModernUI.Windows.Controls;

namespace FolderSynchMUIClient.Pages.HomePages
{
    /// <summary>
    /// Interaction logic for NewFolder.xaml
    /// </summary>
    public partial class NewFolder : UserControl
    {
        private int[] daysArray = Enumerable.Range(1, 31).ToArray();
        private int[] hoursArray = Enumerable.Range(1, 48).ToArray();

        public NewFolder()
        {
            InitializeComponent();

            RefreshComboBox.ItemsSource = hoursArray;
            RefreshComboBox.SelectedIndex = 23;
            
        }


        /*----------- BROWSE BUTTON ------------------------------------------------------------- */

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            //Showing folder picker dialog
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedFolderPathEditor.Text = openFolderDialog.FileName.ToString();
            }
        }



        /*----------- SYNCH BUTTON ------------------------------------------------------------- */

        private void btnSynchFolder_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("adding new folder: " + choosedFolderPathEditor.Text);

            // 1) get service references
            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;

            // 2) get folder name 
            string[] directories = choosedFolderPathEditor.Text.Split('\\');
            string folderName = directories[directories.Length - 1];

            // 3) add the new folder on the server
            try
            {
                Folder newFold = new Folder();
                newFold.FolderName = folderName;
                newFold.Username = application.User.Username;
                newFold.AutoRefreshTime = int.Parse(RefreshComboBox.SelectedItem.ToString());

                DateTime synchDate = DateTime.Now;
                newFold.SynchDate = synchDate;

                proxy.addNewSynchronizedFolder(newFold);

                // 3b) add new folder to user's list
                Folder[] newFoldersList = new Folder[application.User.Folders.Length + 1];

                for (int i = 0; i < application.User.Folders.Length; i++)
                    newFoldersList[i] = application.User.Folders[i];

                newFoldersList[application.User.Folders.Length] = newFold;
                application.User.Folders = newFoldersList;

                // 4) proceed to upload
                LocalFolder lf = new LocalFolder(application.User.Username, newFold.FolderName, choosedFolderPathEditor.Text);
                lf.AutoRefreshTime = int.Parse(RefreshComboBox.SelectedItem.ToString());

                UploadDialog ud = new UploadDialog(lf);
                ud.ShowDialog();

                if(ud.success)
                {
                    Console.WriteLine("update succeeded");
                    lf.SynchDate = synchDate;
                    application.addLocalFolder(newFold, lf);
                }

            }
            catch (FaultException f)
            {
                Console.WriteLine("fault: " + f.Message);

                ErrorDialog errDialog = new ErrorDialog(f.Message);
                errDialog.ShowDialog();
            }

        }

    }
}
