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
    /// Interaction logic for DetailsDialogHistory.xaml
    /// </summary>
    public partial class DetailsDialogHistory : UserControl
    {
        public DetailsDialogHistory()
        {
            InitializeComponent();
            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;

            //FileItem file = (FileItem)this.DataContext;
            //updateDates.ItemsSource = proxy.getFileHistory(file);

        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

       

    }
}
