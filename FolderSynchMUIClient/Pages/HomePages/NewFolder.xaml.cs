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
        public NewFolder()
        {
            InitializeComponent();
        }


        /*----------- BROWSE BUTTON ------------------------------------------------------------- */

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
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

            UploadDialog ud = new UploadDialog(choosedFolderPathEditor.Text);
            ud.ShowDialog();

        }

    }
}
