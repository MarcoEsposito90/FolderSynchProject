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

namespace FolderSynchWPFClient
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_DoWork(object sender, RoutedEventArgs e)
        {

            string responseString = "the server answered: ";

            // here, we ask the server to execute the DoWork method
            responseString += App.FolderSynchProxy.DoWork();
            
            responseLabel.Content = responseString;
        }

        private void Button_DoWork2(object sender, RoutedEventArgs e)
        {

            int answer = App.FolderSynchProxy.DoWork2();
            responseLabel.Content = "The server answered:" + answer;
        }

        private void Button_DoWork3(object sender, RoutedEventArgs e)
        {
            bool answer = App.FolderSynchProxy.DoWork3();
            responseLabel.Content = "The server answered:" + answer;
        }
    }
}
