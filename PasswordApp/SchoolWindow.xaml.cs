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
using System.DirectoryServices;

namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for SchoolWindow.xaml
    /// </summary>
    public partial class SchoolWindow : Window
    {
        public SchoolWindow()
        {
            InitializeComponent();
            lblError.Visibility = Visibility.Hidden;
            txtSchoolCode.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string schoolCode = txtSchoolCode.Text;
            this.DialogResult = CheckSchool(schoolCode);
                         
        }



        private bool? CheckSchool(string schoolCode)
        {
            try
            {
                DirectoryEntry de1 = new DirectoryEntry("LDAP://OU=Schools,OU=CEWA,DC=cewa,DC=edu,DC=au");
                var deSearch = new DirectorySearcher(de1)
                { SearchRoot = de1, Filter = "(&(objectCategory=organizationalUnit)(ou=" + schoolCode + "*))" };
                deSearch.PropertiesToLoad.Add("ou");
                SearchResult searchResult = deSearch.FindOne();
               
                string schoolPath = searchResult.Path;
                string ouName = searchResult.Properties["ou"][0].ToString();
                ouName = ouName.Substring(5, ouName.Length - 5);

                SchoolConfirmWindow schoolConfirmWindow = new SchoolConfirmWindow();
                schoolConfirmWindow.lblSchoolName.Content = ouName;
                var schoolCheck = schoolConfirmWindow.ShowDialog();

                if (schoolCheck == false)
                {
                    lblError.Visibility = Visibility.Hidden;
                    txtSchoolCode.Text = "";
                    return null;
                }
                else
                {
                    Properties.Settings.Default.RootOU = schoolPath;
                    Properties.Settings.Default.OuName = ouName;
                    Properties.Settings.Default.Save();
                    return true;
                }                            
                
            }
            catch
            {
                return false;
            }
        }

        private void WdSchoolWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string schoolCode = txtSchoolCode.Text;
                this.DialogResult = CheckSchool(schoolCode);

            }
        }
    }
}
