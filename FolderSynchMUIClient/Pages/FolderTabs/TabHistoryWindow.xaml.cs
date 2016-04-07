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
using ServicesProject;

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
            /*
            ItemProvider itemProvider = new ItemProvider();

            App application = (App)Application.Current;
            if (application.Folder != null)
            {
                //qui al posto delle folder dovrò mettere le date di modifica --> lista di date di modifica
                ObservableCollection<Folder> ItemsList = itemProvider.GetFolders(application.Folder.Path);
                folderEditDates.ItemsSource = ItemsList;
            }*/

        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Download button pressed!");
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Reload button pressed!");
        }
    }
}
