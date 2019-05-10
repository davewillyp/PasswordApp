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
    /// Interaction logic for SchoolConfirmWindow.xaml
    /// </summary>
    public partial class SchoolConfirmWindow : Window
    {
        public SchoolConfirmWindow()
        {
            InitializeComponent();
            btnYes.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
