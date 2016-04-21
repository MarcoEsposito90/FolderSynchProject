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
        Update.UpdateEntry[] updates;
        public DetailsDialogHistory()
        {
            InitializeComponent();
            Console.WriteLine("Inizializzo");
            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            Item i = (Item)this.DataContext;
            Console.WriteLine("DataContext: " + i.Name);

            if (this.DataContext.GetType() == typeof(FileItem))
                Console.WriteLine("I'm file item.");
            else
                Console.WriteLine("I'm folder item.");

            
        }




        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
