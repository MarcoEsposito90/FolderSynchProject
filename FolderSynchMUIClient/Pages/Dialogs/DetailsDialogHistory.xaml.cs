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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for DetailsDialogHistory.xaml
    /// </summary>
    public partial class DetailsDialogHistory : UserControl
    {

        public DetailsDialogHistory()
        {
            InitializeComponent();

        }

        private void UserControl_ContentRendered(object sender, EventArgs e)
        {
           
        }


        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            // 1) get the update entry ----------------------------
            Button b = (Button)sender;
            Update.UpdateEntry entry = (Update.UpdateEntry)b.DataContext;
            Console.WriteLine("entry = " + entry.ItemLocalPath + "; " + entry.UpdateNumber);

            // 2) compute the folder name -------------------------
            FileItem fi = (FileItem)this.DataContext;
            Console.WriteLine("file path = " + fi.Path);
            string folderPath = fi.Path.Replace("\\" + entry.ItemLocalPath, "");
            Console.WriteLine("folder Path = " + folderPath);

            // 3) get the LocalFolde3 object ----------------------
            App application = (App)Application.Current;
            int index = application.LocalFolders.ToList().FindIndex(item => item.Path.Equals(folderPath));
            LocalFolder lf = application.LocalFolders.ElementAt(index);

            // 4) display the download dialog ---------------------
            FileDownloadDialog fdd = new FileDownloadDialog(entry, lf);
            fdd.ShowDialog();
        }
    }
}
