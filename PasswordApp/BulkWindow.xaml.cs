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


namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for BulkWindow.xaml
    /// </summary>
    public partial class BulkWindow : Window
    {
        BackgroundWorker worker;
        string password;
        int totalCount;

        public BulkWindow()
        {
            InitializeComponent();
            wdBulk.Height = 400;
            pbReset.Minimum = 1;


        }

        
        private void BtnBulk_Click(object sender, RoutedEventArgs e)
        {
            if (chk3YO.IsChecked == true)
            {
                DirectoryEntry objDE;

                // Create a new DirectoryEntry with the given path and credentials from properties.  
                objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

                //Search AD
                var deSearch = new DirectorySearcher(objDE)
                { SearchRoot = objDE, Filter = "(&(objectCategory=user)(title=3YO))" };

                SearchResultCollection searchResult = deSearch.FindAll();
                totalCount = searchResult.Count;

                objDE.Close();

                pbReset.Maximum = totalCount;

                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();

            }

            //this.DialogResult = true;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            DirectoryEntry objDE;

            // Create a new DirectoryEntry with the given path and credentials from properties.  
            objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

            //Search AD
            var deSearch = new DirectorySearcher(objDE)
            { SearchRoot = objDE, Filter = "(&(objectCategory=user)(title=3YO))" };

            SearchResultCollection searchResult = deSearch.FindAll();
            int total = searchResult.Count;
            int count = 1;

                foreach (SearchResult sr in searchResult)
                {
                count++;
                password = GrabPassword();
                    //ResultPropertyCollection pc = sr.Properties;
                    //lstUsers.Items.Add(pc["samaccountname"][0].ToString());
                    worker.ReportProgress(count,total);

                }
               


        }

       void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double percent = (e.ProgressPercentage * 100) / 50;
            //pbReset.Maximum = count2;
            pbReset.Value = e.ProgressPercentage;
            lblUser.Content = password;

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Completed!");
        }

        static string GrabPassword()
        {
            WebClient client = new WebClient();
            string myPass = client.DownloadString("https://dinopass.com/password/simple");
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            myPass = ti.ToTitleCase(myPass);
            return myPass;
        }
    }
}
