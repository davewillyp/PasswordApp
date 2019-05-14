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
using System.Net;
using System.Globalization;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Collections;

namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for BulkWindow.xaml
    /// </summary>
    public partial class BulkWindow : Window
    {
        BackgroundWorker worker;
        BulkChange bulkChange = new BulkChange();
        List<ChangeUser> myUsers = new List<ChangeUser>();

        public BulkWindow()
        {
            InitializeComponent();
            WdBulk.Height = 300;
            pbReset.Minimum = 0;          
            
        }

        
        private void BtnBulk_Click(object sender, RoutedEventArgs e)
        {
            BulkConfirmWindow bulkConfirmWindow = new BulkConfirmWindow();
            var proceed = bulkConfirmWindow.ShowDialog();

            if (proceed == true)
            {
                WdBulk.Height = 400;
                if (chkForce.IsChecked == true)
                {
                    bulkChange.Force = true;
                }
                else
                {
                    bulkChange.Force = false;
                }
                
                //Generate CSV
                bulkChange.CSVPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                bulkChange.CSVPath += @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
                using (StreamWriter file = new StreamWriter(bulkChange.CSVPath, true))
                {
                    file.WriteLine(@"FirstName,LastName,Username,Password,Email,School_Year,Roll_Group");
                }

                //Build AD Search filter based on Checkboxes
                bulkChange.Filter = "(&(objectCategory=user)(|";


                if (chk3YO.IsChecked == true)
                {
                    bulkChange.Filter += "(title=3YO)";

                }

                if (chkKK.IsChecked == true)
                {
                    bulkChange.Filter += "(title=K4)";

                }

                if (chkPP.IsChecked == true)
                {
                    bulkChange.Filter += "(title=PP)";

                }

                if (chkY01.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y01)";

                }

                if (chkY02.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y02)";

                }

                if (chkY03.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y03)";

                }

                if (chkY04.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y04)";
                }

                if (chkY05.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y05)";

                }
                if (chkY06.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y06)";

                }

                if (chkY07.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y07)";

                }

                if (chkY08.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y08)";

                }

                if (chkY09.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y09)";

                }

                if (chkY10.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y10)";

                }

                if (chkY11.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y11)";

                }

                if (chkY12.IsChecked == true)
                {
                    bulkChange.Filter += "(title=Y12)";

                }



                bulkChange.Filter += "))";

                //Connect and Search AD, then build List
                DirectoryEntry objDE;
                objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

                var deSearch = new DirectorySearcher(objDE)
                { SearchRoot = objDE, Filter = bulkChange.Filter };

                SearchResultCollection searchResult = deSearch.FindAll();
                bulkChange.TotalCount = searchResult.Count;



                foreach (SearchResult sr in searchResult)
                {
                    ResultPropertyCollection pc = sr.Properties;
                    string mA;
                    string rG;
                    try
                    {
                        mA = pc["mail"][0].ToString();
                    }
                    catch
                    {
                        mA = "None";
                    }

                    try
                    {
                        rG = pc["department"][0].ToString();
                    }
                    catch
                    {
                        rG = "None";
                    }

                    myUsers.Add(new ChangeUser()
                    {
                        Path = sr.Path,
                        UserName = pc["samaccountname"][0].ToString(),
                        Title = pc["title"][0].ToString(),
                        FirstName = pc["givenName"][0].ToString(),
                        LastName = pc["sn"][0].ToString(),
                        Mail = mA,
                        RollGroup = rG

                    });

                    objDE.Close();
                }

                //Set Progress bar Maximum
                pbReset.Maximum = bulkChange.TotalCount;
                //Set Progress Label
                lblUser.Content = "0 of " + bulkChange.TotalCount;

                //Start worker
                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();
            }
            else
            {
                ClearAll();
            }
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            bulkChange.Count = 0;

            var users = from user in myUsers
                        orderby user.Title, user.RollGroup, user.LastName ascending 
                        select user;

            foreach (var user in users)
            {
                string newPassword = GrabPassword();
                string text = user.FirstName + ","
                    + user.LastName + ","
                    + user.UserName + ","
                    + newPassword + ","
                    + user.Mail + ","
                    + user.Title + ","
                    + user.RollGroup;

                //Write line to CSV
                using (StreamWriter file = new StreamWriter(bulkChange.CSVPath, true))
                {
                    file.WriteLine(text);
                }

                //Reset Password
                
                DirectoryEntry objDE;
                objDE = new DirectoryEntry(user.Path);
                objDE.Invoke("SetPassword", new object[] { newPassword });
                objDE.Properties["LockOutTime"].Value = 0;

                if (bulkChange.Force == true)
                {
                    objDE.Properties["pwdLastSet"].Value = 0;
                }
                else
                {
                    objDE.Properties["pwdLastSet"].Value = -1;
                }
                
                objDE.CommitChanges();
                objDE.Close();
                
                
                bulkChange.Count++;
                worker.ReportProgress(bulkChange.Count, bulkChange.TotalCount);
            }

            
           

        }

       void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double percent = (e.ProgressPercentage * 100) / 50;
            pbReset.Value = e.ProgressPercentage;
            lblUser.Content = bulkChange.Count + " of " + bulkChange.TotalCount;          
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            MessageBox.Show("CSV Saved to " + bulkChange.CSVPath,"Completed!");
            ClearAll();

        }

        static string GrabPassword()
        {
            WebClient client = new WebClient();
            string myPass = client.DownloadString("https://dinopass.com/password/simple");
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            myPass = ti.ToTitleCase(myPass);
            return myPass;
        }

        public void ClearAll()
        {
            pbReset.Value = 0;
            lblUser.Content = "";
            myUsers.Clear();
            chkForce.IsChecked = false;
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
            WdBulk.Height = 300;

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

        private void ChkAll_Checked(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private void ChkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WdBulk_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();

            }

        }

    }

    public class BulkChange
    {
        public int TotalCount { get; set; }
        public int Count { get; set; }
        public string CSVPath { get; set; }
        public string Filter { get; set; }
        public bool Force { get; set; }
    }

    public class ChangeUser
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Path { get; set; }
        public string RollGroup { get; set; }
    }
}
