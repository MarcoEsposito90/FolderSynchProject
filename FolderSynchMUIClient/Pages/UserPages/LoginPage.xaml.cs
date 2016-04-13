using FolderSynchMUIClient.FolderSynchService;
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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // perform login ---------------------------------------------------------------------------
                App application = (App)Application.Current;
                FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
                application.User = proxy.loginUser(TBLoginUsername.Text.ToString(), TBLoginPassword.Password.ToString());
                responseLabel.Content = "login successful";

                if (CheckBoxRemember.IsChecked.Value)
                    application.AddKnownUser(TBLoginUsername.Text, TBLoginPassword.Password);

                // change window ---------------------------------------------------------------------------
                SecondWindow sw = new SecondWindow();
                sw.Show();

                Application.Current.MainWindow.Close();

                // check if some synced folder is missing on this device -----------------------------------
                List<LocalFolder> localFolders = application.getLocalFolders();
                List<Folder> missingFolders = new List<Folder>();
                foreach(Folder f in application.User.Folders)
                {
                    bool missing = true;
                    foreach (LocalFolder lf in localFolders)
                        if (lf.FolderName.Equals(f.Name))
                        {
                            missing = false;
                            break;
                        }

                    if (missing)
                        missingFolders.Add(f);
                }
                
                if(missingFolders.Count > 0)
                {
                    LocalFoldersWarningDialog dialog = new LocalFoldersWarningDialog(missingFolders);
                    dialog.ShowDialog();
                }


            }
            catch (FaultException f)
            {
                responseLabel.Content = "error: " + f.Message;
            }
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;

            if (application.User == null)
                responseLabel.Content = "must login before logout";

            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            proxy.logoutUser(application.User);
            application.User = null;
        }
    }
}
