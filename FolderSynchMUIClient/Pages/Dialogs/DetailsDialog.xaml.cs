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

namespace FolderSynchMUIClient
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class DetailsDialog : ModernDialog
    {

        public static readonly string LOCAL_FOLDER_ACCESS_KEY = "localFolder";

        public DetailsDialog(LocalFolder lf, Item it)
        {
            InitializeComponent();
            this.DataContext = it;

            string localPath = it.Path.Replace(lf.Path + "\\", "");

            App application = (App)Application.Current;
            FolderSynchServiceContractClient proxy = application.FolderSynchProxy;
            it.Updates = new ObservableCollection<Update.UpdateEntry>(proxy.getFileHistory(lf.Name, localPath));

            Console.WriteLine("DataContext: " + it.Name);
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton};
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            tabDetails.DataContext = this.DataContext;
        }
    }
}
