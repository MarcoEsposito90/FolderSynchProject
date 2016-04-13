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
    /// Interaction logic for LogoutPage.xaml
    /// </summary>
    public partial class LogoutPage : UserControl
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        private void btnConfirmLogout_Click(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;
            if (application.User != null)
            {
                FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
                proxy.logoutUser(application.User);
                application.User = null;

                // change window ---------------------------------------------------------------------------
                MainWindow mw = new MainWindow();
                mw.Show();

                Application.Current.Windows[0].Close();

            }
        }

       
    }
}
