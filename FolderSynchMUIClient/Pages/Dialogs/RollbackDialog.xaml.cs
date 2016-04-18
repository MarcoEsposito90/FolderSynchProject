using FirstFloor.ModernUI.Windows.Controls;
using FolderSynchMUIClient.FolderSynchService;
using Microsoft.WindowsAPICodePack.Dialogs;
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
    /// Interaction logic for RollbackDialog.xaml
    /// </summary>
    public partial class RollbackDialog : ModernDialog
    {
        private LocalFolder localFolder;
        private Update update;

        public RollbackDialog(LocalFolder lf, Update up)
        {
            InitializeComponent();
            this.localFolder = lf;
            this.update = up;

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedPathTextBox.Text = openFolderDialog.FileName.ToString();
            }
        }

        private void btnDownloadOld_Checked(object sender, RoutedEventArgs e)
        {
            btnBrowsePath.Visibility = Visibility.Visible;
            choosedPathTextBox.Visibility = Visibility.Visible;
        }

        private void btnDownloadOld_Unchecked(object sender, RoutedEventArgs e)
        {
            btnBrowsePath.Visibility = Visibility.Hidden;
            choosedPathTextBox.Visibility = Visibility.Hidden;
        }
    }
}
