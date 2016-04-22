using FirstFloor.ModernUI.Windows.Controls;
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
    /// Interaction logic for ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : ModernDialog
    {
        string message;
        public ConfirmDialog(string message)
        {
            InitializeComponent();

            this.message = message;
            // define the dialog buttons
            this.Buttons = new Button[] { this.YesButton, this.NoButton };
        }

        private void ModernDialog_ContentRendered(object sender, EventArgs e)
        {
            warningMessageTB.Text = message;
        }
    }
}
