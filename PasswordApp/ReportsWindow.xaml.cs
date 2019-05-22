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
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        string searchString;
        string csvPath;
        int count;
        int totalCount;
        List<UserList> userList = new List<UserList>();
        BackgroundWorker worker;
        UserList comboChoice = new UserList();

        public ReportsWindow()
        {
            InitializeComponent();
            pbReport.Visibility = Visibility.Hidden;
            btnOpen.Visibility = Visibility.Hidden;

            List<UserProperty> userProperties = new List<UserProperty>();
            userProperties.Add(new UserProperty { Key = "First Name", Value = "givenName" });
            userProperties.Add(new UserProperty { Key = "Last Name", Value = "sn" });
            userProperties.Add(new UserProperty { Key = "Display Name", Value = "displayName" });
            userProperties.Add(new UserProperty { Key = "Logon Name", Value = "sAMAccountName" });
            userProperties.Add(new UserProperty { Key = "E-mail", Value = "mail" });
            userProperties.Add(new UserProperty { Key = "School Year", Value = "title" });
            userProperties.Add(new UserProperty { Key = "Roll Group", Value = "department" });
            userProperties.Add(new UserProperty { Key = "School", Value = "company" });
            userProperties.Add(new UserProperty { Key = "Maze Key", Value = "extensionAttribute11" });
            userProperties.Add(new UserProperty { Key = "WASN / Exam Number", Value = "extensionAttribute9" });
            userProperties.Add(new UserProperty { Key = "--None--", Value = "none" });


            var cmbList = new[] {cmbColumn1, cmbColumn2, cmbColumn3, cmbColumn4,
            cmbColumn5, cmbColumn6, cmbColumn7, cmbColumn8, cmbColumn9, cmbColumn10};

            foreach (var combobox in cmbList)
            {
                combobox.DisplayMemberPath = "Key";
                combobox.SelectedValuePath = "Value";
                combobox.ItemsSource = userProperties;
            }

            cmbColumn1.SelectedValue = "givenName";
            cmbColumn2.SelectedValue = "sn";
            cmbColumn3.SelectedValue = "displayName";
            cmbColumn4.SelectedValue = "sAMAccountName";
            cmbColumn5.SelectedValue = "mail";
            cmbColumn6.SelectedValue = "title";
            cmbColumn7.SelectedValue = "department";
            cmbColumn8.SelectedValue = "company";
            cmbColumn9.SelectedValue = "extensionAttribute11";
            cmbColumn10.SelectedValue = "extensionAttribute9";

            pbReport.Minimum = 0;
        }
        

        private void ChkAll_Checked(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        public void ClearAll()
        {
            chk3YO.IsChecked = false;
            chkKK.IsChecked = false;
            chkPP.IsChecked = false;
            chkY01.IsChecked = false;
            chkY02.IsChecked = false;
            chkY03.IsChecked = false;
            chkY04.IsChecked = false;
            chkY05.IsChecked = false;
            chkY06.IsChecked = false;
            chkY07.IsChecked = false;
            chkY08.IsChecked = false;
            chkY09.IsChecked = false;
            chkY10.IsChecked = false;
            chkY11.IsChecked = false;
            chkY12.IsChecked = false;
           
        }

        public void SelectAll()
        {
            chk3YO.IsChecked = true;
            chkKK.IsChecked = true;
            chkPP.IsChecked = true;
            chkY01.IsChecked = true;
            chkY02.IsChecked = true;
            chkY03.IsChecked = true;
            chkY04.IsChecked = true;
            chkY05.IsChecked = true;
            chkY06.IsChecked = true;
            chkY07.IsChecked = true;
            chkY08.IsChecked = true;
            chkY09.IsChecked = true;
            chkY10.IsChecked = true;
            chkY11.IsChecked = true;
            chkY12.IsChecked = true;

        }

        private void ChkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void GetSearchString()
        {
            searchString = "(&(objectCategory=user)(|";

            if (chk3YO.IsChecked == true)
            {
                searchString += "(title=3YO)";

            }

            if (chkKK.IsChecked == true)
            {
                searchString += "(title=K4)";

            }

            if (chkPP.IsChecked == true)
            {
                searchString += "(title=PP)";

            }

            if (chkY01.IsChecked == true)
            {
                searchString += "(title=Y01)";

            }

            if (chkY02.IsChecked == true)
            {
                searchString += "(title=Y02)";

            }

            if (chkY03.IsChecked == true)
            {
                searchString += "(title=Y03)";

            }

            if (chkY04.IsChecked == true)
            {
                searchString += "(title=Y04)";
            }

            if (chkY05.IsChecked == true)
            {
                searchString += "(title=Y05)";

            }
            if (chkY06.IsChecked == true)
            {
                searchString += "(title=Y06)";

            }

            if (chkY07.IsChecked == true)
            {
                searchString += "(title=Y07)";

            }

            if (chkY08.IsChecked == true)
            {
                searchString += "(title=Y08)";

            }

            if (chkY09.IsChecked == true)
            {
                searchString += "(title=Y09)";

            }

            if (chkY10.IsChecked == true)
            {
                searchString += "(title=Y10)";

            }

            if (chkY11.IsChecked == true)
            {
                searchString += "(title=Y11)";

            }

            if (chkY12.IsChecked == true)
            {
                searchString += "(title=Y12)";

            }
                        
            searchString += "))";
        }

   

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            pbReport.Visibility = Visibility.Visible;

            //Get Year Groups To Report
            GetSearchString();

            //Create CSV File with Headers
            BuildCsv();
            
            totalCount = GetCount();

            pbReport.Maximum = totalCount;
            lblStatus.Content = "0 of " + totalCount;

            comboChoice.Col1 = cmbColumn1.SelectedValue.ToString();
            comboChoice.Col2 = cmbColumn2.SelectedValue.ToString();
            comboChoice.Col3 = cmbColumn3.SelectedValue.ToString();
            comboChoice.Col4 = cmbColumn4.SelectedValue.ToString();
            comboChoice.Col5 = cmbColumn5.SelectedValue.ToString();
            comboChoice.Col6 = cmbColumn6.SelectedValue.ToString();
            comboChoice.Col7 = cmbColumn7.SelectedValue.ToString();
            comboChoice.Col8 = cmbColumn8.SelectedValue.ToString();
            comboChoice.Col9 = cmbColumn9.SelectedValue.ToString();
            comboChoice.Col10 = cmbColumn10.SelectedValue.ToString();


            //Start BackGround Worker
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
                                   
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string col1;
            string col2;
            string col3;
            string col4;
            string col5;
            string col6;
            string col7;
            string col8;
            string col9;
            string col10;

            BackgroundWorker worker = sender as BackgroundWorker;
            count = 0;

            DirectoryEntry objDE;
            objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

            var deSearch = new DirectorySearcher(objDE)
            {
                SearchRoot = objDE,
                Filter = searchString
            };

            SearchResultCollection searchResult = deSearch.FindAll();


            foreach (SearchResult sr in searchResult)
            {
                ResultPropertyCollection pc = sr.Properties;

                try
                {
                    col1 = pc[comboChoice.Col1][0].ToString();
                }
                catch
                {
                    col1 = "";
                }

                try
                {
                    col2 = pc[comboChoice.Col2][0].ToString();
                }
                catch
                {
                    col2 = "";
                }

                try
                {
                    col3 = pc[comboChoice.Col3][0].ToString();
                }
                catch
                {
                    col3 = "";
                }

                try
                {
                    col4 = pc[comboChoice.Col4][0].ToString();
                }
                catch
                {
                    col4 = "";
                }

                try
                {
                    col5 = pc[comboChoice.Col5][0].ToString();
                }
                catch
                {
                    col5 = "";
                }
                try
                {
                    col6 = pc[comboChoice.Col6][0].ToString();
                }
                catch
                {
                    col6 = "";
                }

                try
                {
                    col7 = pc[comboChoice.Col7][0].ToString();
                }
                catch
                {
                    col7 = "";
                }
                try
                {
                    col8 = pc[comboChoice.Col8][0].ToString();
                }
                catch
                {
                    col8 = "";
                }
                try
                {
                    col9 = pc[comboChoice.Col9][0].ToString();
                }
                catch
                {
                    col9 = "";
                }
                try
                {
                    col10 = pc[comboChoice.Col10][0].ToString();
                }
                catch
                {
                    col10 = "";
                }

                userList.Add(new UserList()
                {
                    Col1 = col1,
                    Col2 = col2,
                    Col3 = col3,
                    Col4 = col4,
                    Col5 = col5,
                    Col6 = col6,
                    Col7 = col7,
                    Col8 = col8,
                    Col9 = col9,
                    Col10 = col10
                });

                count++;
                worker.ReportProgress(count, totalCount);
            }

            deSearch.Dispose();
            searchResult.Dispose();
            objDE.Dispose();

            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double percent = (e.ProgressPercentage * 100) / 50;
            pbReport.Value = e.ProgressPercentage;
            lblStatus.Content = count + " of " + totalCount;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var users = from user in userList
                        select user;

            foreach (var user in users)
            {

                string text = "";
                            
                text += user.Col1 + ",";
               
                text += user.Col2 + ",";
                
                text += user.Col3 + ",";
                
                text += user.Col4 + ",";
                
                text += user.Col5 + ",";
                
                text += user.Col6 + ",";
                
                text += user.Col7 + ",";
                
                text += user.Col8 + ",";
                
                text += user.Col9 + ",";
                
                text += user.Col10 + ",";

                using (StreamWriter file = new StreamWriter(csvPath, true))
                {
                    file.WriteLine(text);
                }

            }

            btnOpen.Visibility = Visibility.Visible;
            ClearAll();
            lblStatus.Content = "CSV Saved to: " + csvPath;

        }

        private int GetCount()
        {
            DirectoryEntry objDE;
            objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

            var deSearch = new DirectorySearcher(objDE)
            {
                SearchRoot = objDE,
                Filter = searchString
            };

            SearchResultCollection searchResult = deSearch.FindAll();

            totalCount = searchResult.Count;

            searchResult.Dispose();
            deSearch.Dispose();
            objDE.Dispose();

            return totalCount;
        }

        private void BuildCsv()
        {
            //Generate CSV
            csvPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            csvPath += @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            using (StreamWriter file = new StreamWriter(csvPath, true))
            {
                if (cmbColumn1.SelectedValue.ToString() != "none")
                file.Write(cmbColumn1.SelectedValue.ToString() + ",");

                if (cmbColumn2.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn2.SelectedValue.ToString() + ",");

                if (cmbColumn3.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn3.SelectedValue.ToString() + ",");

                if (cmbColumn4.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn4.SelectedValue.ToString() + ",");

                if (cmbColumn5.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn5.SelectedValue.ToString() + ",");

                if (cmbColumn6.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn6.SelectedValue.ToString() + ",");

                if (cmbColumn7.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn7.SelectedValue.ToString() + ",");

                if (cmbColumn8.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn8.SelectedValue.ToString() + ",");

                if (cmbColumn9.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn9.SelectedValue.ToString() + ",");

                if (cmbColumn10.SelectedValue.ToString() != "none")
                    file.Write(cmbColumn10.SelectedValue.ToString() + ",");

                file.WriteLine("");   
            }
        
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            Process.Start(csvPath);
        }

        private void WdReports_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();

            }
        }
    }
    public class UserProperty
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class UserList
    {
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
        public string Col8 { get; set; }
        public string Col9 { get; set; }
        public string Col10 { get; set; }
    }
}
