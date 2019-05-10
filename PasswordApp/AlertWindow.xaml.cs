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
using System.Windows.Shapes;

namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        
        public AlertWindow()
        {
            InitializeComponent();
            btnYes.Focus();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
