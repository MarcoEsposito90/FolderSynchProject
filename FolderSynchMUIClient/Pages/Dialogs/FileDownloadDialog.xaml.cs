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
    /// Interaction logic for FileDownloadDialog.xaml
    /// </summary>
    public partial class FileDownloadDialog : ModernDialog
    {
        private Update.UpdateEntry Entry;

        public FileDownloadDialog(Update.UpdateEntry entry)
        {
            InitializeComponent();
            this.Entry = entry;
            
            // define the dialog buttons
            this.Buttons = new Button[] { this.YesButton, this.NoButton };
        }

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
