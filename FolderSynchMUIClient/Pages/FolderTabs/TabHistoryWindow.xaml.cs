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
            Console.WriteLine("Download button pressed!");
            //TODO: implementazione metodo per scaricare una vecchia versione di file/folder
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Reload button pressed!");
            //TODO: implementazione metodo per ripristinare una vecchia versione della cartella
        }
    }
}
