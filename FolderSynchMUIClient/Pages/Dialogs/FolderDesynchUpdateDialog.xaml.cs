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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for FolderDesynchUpdateDialog.xaml
    /// </summary>
    public partial class FolderDesynchUpdateDialog : ModernDialog
    {
        Dictionary<LocalFolder, Update> folderUpdates;

        public FolderDesynchUpdateDialog(Dictionary<LocalFolder,Update> folderAndLatestRemoteUpdates)
        {
            InitializeComponent();
            this.folderUpdates = folderAndLatestRemoteUpdates;

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
        }


        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            OkButton.Visibility = Visibility.Hidden;
            folderList.ItemsSource = folderUpdates.Keys;
        }

        private void btnStartResynch_Click(object sender, RoutedEventArgs e)
        {
            // TODO: FOLDER RESYNCH
            OkButton.Visibility = Visibility.Visible;
        }
    }
}
