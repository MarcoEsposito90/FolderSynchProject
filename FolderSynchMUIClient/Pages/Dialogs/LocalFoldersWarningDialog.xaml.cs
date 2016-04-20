using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LocalFoldersWarningDialog.xaml
    /// </summary>
    public partial class LocalFoldersWarningDialog : ModernDialog
    {
        private ObservableCollection<Folder> Folders;

        public LocalFoldersWarningDialog(List<Folder> folders)
        {
            InitializeComponent();
            this.Folders = new ObservableCollection<Folder>(folders);
            DataContext = this;
            Owner = Application.Current.MainWindow;
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            missingFoldersList.ItemsSource = Folders;
        }



        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Folder f = (Folder)b.DataContext;

            Console.WriteLine("clicked: " + f.FolderName);
            FolderDownloadDialog dialog = new FolderDownloadDialog(f);

            if(dialog.ShowDialog() == true && dialog.Success)
                Folders.Remove(f);
        }

        
    }
}
