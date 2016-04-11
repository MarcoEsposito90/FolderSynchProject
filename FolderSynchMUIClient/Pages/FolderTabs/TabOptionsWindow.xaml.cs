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
        public static readonly int MAX_REFRESH_TIME_HOURS = 48;
        public static readonly int MAX_DELETE_TIME_DAYS = 31;
        public static readonly int MIN_TIME = 1;
        public int currRefresh;
        public int currDelete;

        public TabOptionsWindow()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            currRefresh = application.Folder.AutoRefreshTime;
            currDelete = application.Folder.AutoDeleteTime;
        }

        private void autoRefreshUp_Click(object sender, RoutedEventArgs e)
        {
            //int currTime = int.Parse(txtAutoRefresh.Text.ToString());
            currRefresh += 1;
            txtAutoRefresh.Text = (currRefresh).ToString();
            if (currRefresh == MAX_REFRESH_TIME_HOURS)
                autoRefreshUp.IsEnabled = false;
            if (!autoRefreshDown.IsEnabled)
                autoRefreshDown.IsEnabled = true;
        }
        
        private void autoRefreshDown_Click(object sender, RoutedEventArgs e)
        {
            //int currTime = int.Parse(txtAutoRefresh.Text.ToString());
            currRefresh -= 1;
            txtAutoRefresh.Text = (currRefresh).ToString();
            if (currRefresh == MIN_TIME)
                autoRefreshDown.IsEnabled = false;
            if (!autoRefreshUp.IsEnabled)
                autoRefreshUp.IsEnabled = true;
        }

        private void autoDeleteUp_Click(object sender, RoutedEventArgs e)
        {
            //int currTime = int.Parse(txtAutoDelete.Text.ToString());
            currDelete += 1;
            txtAutoDelete.Text = (currDelete).ToString();
            if (currDelete == MAX_DELETE_TIME_DAYS)
                autoDeleteUp.IsEnabled = false;
            if (!autoDeleteDown.IsEnabled)
                autoDeleteDown.IsEnabled = true;
        }

        private void autoDeleteDown_Click(object sender, RoutedEventArgs e)
        {
            //int currTime = int.Parse(txtAutoDelete.Text.ToString());
            currDelete -= 1;
            txtAutoDelete.Text = (currDelete).ToString();
            if (currDelete == MIN_TIME)
                autoDeleteDown.IsEnabled = false;
            if (!autoDeleteUp.IsEnabled)
                autoDeleteUp.IsEnabled = true;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            App application = (App)Application.Current;
            application.Folder.AutoRefreshTime = currRefresh;
            application.Folder.AutoDeleteTime = currDelete;

        }
    }
}
