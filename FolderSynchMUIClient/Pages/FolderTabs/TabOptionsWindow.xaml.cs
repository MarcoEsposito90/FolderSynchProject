using FolderSynchMUIClient.Classes;
using FolderSynchMUIClient.FolderSynchService;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.IO;

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for TabOptionsWindow.xaml
    /// </summary>
    public partial class TabOptionsWindow : UserControl
    {
        App application;

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public TabOptionsWindow()
        {
            InitializeComponent();
            application = (App)Application.Current;
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------------ AUTO REFRESH ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void autoRefreshUp_Click(object sender, RoutedEventArgs e)
        {
            //Increasing auto-refresh time
            LocalFolder lf = (LocalFolder)(this.DataContext);
            txtAutoRefresh.Text = (lf.AutoRefreshTime + 1).ToString();
           
        }
        

        /********************************************************************************/
        private void autoRefreshDown_Click(object sender, RoutedEventArgs e)
        {
            //Decreasing auto-refresh time
            LocalFolder lf = (LocalFolder)(this.DataContext);
            txtAutoRefresh.Text = (lf.AutoRefreshTime - 1).ToString();
            
        }



        /* ------------------------------------------------------------------------------ */
        /* ------------------------ DELETE ---------------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            LocalFolder lf = (LocalFolder)this.DataContext;
            ConfirmDialog cd = new ConfirmDialog("Do you really want to de-synch this folder?" + "\n" +
                                                 "It won't be deleted anyway from your PC");

            if(cd.ShowDialog() == true)
            {
                Console.WriteLine("User wants to proceed with desynch folder");
                application.FolderSynchProxy.removeSynchronizedFolder(lf.Name);
                Console.WriteLine("server removed folder. removing locally");
                application.removeLocalFolder(lf);
            }
        }




        /* ------------------------------------------------------------------------------ */
        /* ------------------------ MOVE FOLDER ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */


        private void btnBrowseMove_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedFolderPathEditor.Text = openFolderDialog.FileName.ToString();
            }
        }

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ SAVE CHANGES ---------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog cd = new ConfirmDialog("Do you want to save changes?");
            if(cd.ShowDialog() == true)
            {
                LocalFolder lf = (LocalFolder)this.DataContext;
                Folder folder = null;

                // 1) find correspondent remote folder --------------------------------
                foreach (Folder f in application.User.Folders)
                    if (f.FolderName.Equals(lf.Name))
                        folder = f;

                if (folder == null)
                {
                    ErrorDialog ed = new ErrorDialog("An internal error occurred.\nWe suggest you to reboot the program");
                    ed.ShowDialog();
                    return;
                }

                // 2) communicate eventual changes to server ----------------------------
                int newAutoRefresh = int.Parse(txtAutoRefresh.Text);

                if(newAutoRefresh != folder.AutoRefreshTime)
                {
                    folder.AutoRefreshTime = newAutoRefresh;

                    try
                    {
                        application.FolderSynchProxy.changeFolderOptions(folder.FolderName, folder);
                    }
                    catch(FaultException f)
                    {
                        ErrorDialog ed = new ErrorDialog(f.Message);
                        ed.ShowDialog();
                    }
                }

                // 3) move folder if necessary;
                string newPath = choosedFolderPathEditor.Text;

                if(newPath.Trim().Length != 0)
                {
                    if((newPath.Trim() + "\\" + lf.Name).Equals(lf.Path))
                    {
                        ErrorDialog ed = new ErrorDialog("The folder is already placed in:\n" + choosedFolderPathEditor.Text);
                        ed.ShowDialog();
                    }
                    else if (newPath.Trim().Contains(lf.Path))
                    {
                        ErrorDialog ed = new ErrorDialog("You cannot move the folder inside itself");
                        ed.ShowDialog();
                    }
                    else if(Directory.Exists(newPath + "\\" + lf.Name))
                    {
                        ErrorDialog ed = new ErrorDialog("A folder in directory " + newPath + "\\" + lf.Name + " already exists");
                        ed.ShowDialog();
                    }
                    else
                    {
                        Console.WriteLine("proceed moving the folder");

                        // temporary disable updates and manually update
                        application.stopWatching(lf);

                        // check if update is necessary before moving the folder
                        List<Item.Change> changes = lf.DetectChanges(lf.LastUpdateCheck);
                        if(changes.Count > 0)
                        {
                            Console.WriteLine("update before moving necessary");
                            UploadDialog ud = new UploadDialog(lf, changes);
                            ud.ShowDialog();
                        }

                        // now we can move it
                        Directory.Move(lf.Path, newPath);
                        lf.Path = newPath;
                        lf.setLatestUpdateItems();

                        application.startWatching(lf);
                    }

                    labelSaveResult.Content = "Changes made with success.";

                }
            }
        }

        

        private void btnDesynchFolder_Click(object sender, RoutedEventArgs e)
        {
            //TODO: desincronizzare cartella
        }
    }
}
