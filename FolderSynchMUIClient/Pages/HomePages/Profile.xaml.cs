using FolderSynchMUIClient.FolderSynchService;
using FolderSynchMUIClient.Pages;
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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : UserControl
    {
        public Profile()
        {
            InitializeComponent();
            App application = (App)Application.Current;

            TBUsername.Text = application.User.Username;
            PBPassword.Password = application.User.Password;
        }

        private void editPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDialog cpd = new ChangePasswordDialog();
            cpd.ShowDialog();
            

        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            // check if some synced folder is missing on this device -----------------------------------
            App application = (App)Application.Current;
            List<LocalFolder> localFolders = application.LocalFolders.ToList();
            List<Folder> missingFolders = new List<Folder>();
            foreach (Folder f in application.User.Folders)
            {
                int found = localFolders.FindIndex(item => item.Name.Equals(f.FolderName));

                if (found == -1)
                    missingFolders.Add(f);
            }

            if (missingFolders.Count > 0)
            {
                LocalFoldersWarningDialog dialog = new LocalFoldersWarningDialog(missingFolders);
                dialog.ShowDialog();
               

            }
            else
            {
                MessageBoxResult result = MessageBox.Show("You have no unsynched folders.","Confirmation", MessageBoxButton.OK);
            }
        }
    }
}
