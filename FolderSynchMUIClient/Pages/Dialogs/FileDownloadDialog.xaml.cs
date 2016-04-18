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
    /// Interaction logic for FileDownloadDialog.xaml
    /// </summary>
    public partial class FileDownloadDialog : ModernDialog
    {
        private Update.UpdateEntry Entry;
        public string DownloadPath;

        public FileDownloadDialog(Update.UpdateEntry entry)
        {
            InitializeComponent();
            this.Entry = entry;
            
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton};
        }

        private void FileDownloadDialog_ContentRendered(object sender, EventArgs e)
        {
            this.OkButton.Visibility = Visibility.Hidden;
            DownloadProgressBar.Visibility = Visibility.Hidden;
            responseTB.Visibility = Visibility.Hidden;
            TBDownloadQuestion.Text += Entry.EntryTimestamp.ToString() + "?";
            choosedFolderPathEditor.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            //Showing folder picker dialog
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                choosedFolderPathEditor.Text = openFolderDialog.FileName.ToString();
                DownloadPath = choosedFolderPathEditor.Text;
            }
        }

        private void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Entry.ItemLocalPath;
            string[] tokens = filePath.Split('\\');
            string name = tokens[tokens.Length - 1];

            Console.WriteLine("downloaded file path will be: " + DownloadPath + "\\" + name);
            DownloadProgressBar.Visibility = Visibility.Visible;
            responseTB.Visibility = Visibility.Visible;
        }
    }
}
