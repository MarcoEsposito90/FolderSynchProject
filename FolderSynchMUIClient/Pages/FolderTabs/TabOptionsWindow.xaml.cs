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

namespace FolderSynchMUIClient.Pages
{
    /// <summary>
    /// Interaction logic for TabOptionsWindow.xaml
    /// </summary>
    public partial class TabOptionsWindow : UserControl
    {
        public Folder f;

        public TabOptionsWindow()
        {
            InitializeComponent();
        }

        private void autoRefreshUp_Click(object sender, RoutedEventArgs e)
        {
            f = (Folder)(this.DataContext);
            txtAutoRefresh.Text = (f.AutoRefreshTime + 1).ToString();
           
        }
        
        private void autoRefreshDown_Click(object sender, RoutedEventArgs e)
        {
            f = (Folder)(this.DataContext);
            txtAutoRefresh.Text = (f.AutoRefreshTime - 1).ToString();
            
        }

        private void autoDeleteUp_Click(object sender, RoutedEventArgs e)
        {
            f = (Folder)(this.DataContext);
            txtAutoDelete.Text = (f.AutoDeleteTime + 1).ToString();
            
        }

        private void autoDeleteDown_Click(object sender, RoutedEventArgs e)
        {
            f = (Folder)(this.DataContext);
            txtAutoDelete.Text = (f.AutoDeleteTime - 1).ToString();
            
        }

        
    }
}
