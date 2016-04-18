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
    /// Interaction logic for TabHistoryWindow.xaml
    /// </summary>
    public partial class TabHistoryWindow : UserControl
    {
        public TabHistoryWindow()
        {
            InitializeComponent();
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Update.UpdateEntry entry = (Update.UpdateEntry)b.DataContext;
            LocalFolder lf = (LocalFolder)DataContext;

            FileDownloadDialog dialog = new FileDownloadDialog(entry);
            dialog.ShowDialog();
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Update update = (Update)b.DataContext;
            LocalFolder lf = (LocalFolder)DataContext;

            RollbackDialog rd = new RollbackDialog(lf, update);
            rd.ShowDialog();
        }
    }
}
