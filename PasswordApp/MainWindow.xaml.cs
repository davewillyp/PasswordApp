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
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Globalization;

namespace PasswordApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User CurrentUser = new User();
        

        public MainWindow()
        {
            InitializeComponent();
            lblTitle.Visibility = Visibility.Hidden;
            lblDisplayName.Visibility = Visibility.Hidden;
            lblMail.Visibility = Visibility.Hidden;
            lblDepartment.Visibility = Visibility.Hidden;
            
            txtUsername.Focus();
            wdMain.Height = 140;
           // Properties.Settings.Default.RootOU = "";
           // Properties.Settings.Default.Save();

            //Check if can find curent School
            CheckSchool();

            //If No School Set or Can't find, Ask for Code
            if (Properties.Settings.Default.RootOU == "")
            {
                bool? result = null;
                do
                {
                    SchoolWindow schoolWindow = new SchoolWindow();
                    if (result == false)
                    {
                        schoolWindow.lblError.Visibility = Visibility.Visible;
                    }
                    result = schoolWindow.ShowDialog();
                    

                } while (result != true);



            }
            
            var WritePermission = CheckWritePermission();
            if (WritePermission == false)
            {
                MessageBox.Show("You do not have permission to run this application." +
                    " Contact your System Administrator.", "Access Denied!", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
            }
            
            lblSchoolName.Content = Properties.Settings.Default.OuName;
        }

        private void TxtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            lblTitle.Content = "";
            lblDisplayName.Content = "";
            lblMail.Content = "";
            string userName = "";
            userName = txtUsername.Text;

            DisplayUser(userName);
            DisplayUserList(userName);
        }

        private void DisplayUser(string userName1)
        {
            lblDisplayName.Visibility = Visibility.Hidden;
            lblMail.Visibility = Visibility.Hidden;
            lblTitle.Visibility = Visibility.Hidden;
            lblDepartment.Visibility = Visibility.Hidden;
            btnUnlock.Visibility = Visibility.Hidden;
                        
           
            if (userName1 != "")
            {


                DirectoryEntry objDE;

                // Create a new DirectoryEntry with the given path and credentials from properties.  
                objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

                //Search AD
                var deSearch = new DirectorySearcher(objDE)
                { SearchRoot = objDE, Filter = "(&(objectCategory=user)(cn=" + userName1 + "*))" };
                deSearch.PropertiesToLoad.Add("title");
                deSearch.PropertiesToLoad.Add("sn");
                deSearch.PropertiesToLoad.Add("givenName");
                deSearch.PropertiesToLoad.Add("mail");
                deSearch.PropertiesToLoad.Add("department");
                deSearch.PropertiesToLoad.Add("sAMAccountName");

                SearchResult searchResult = deSearch.FindOne();


                if (searchResult != null)
                {
                    CurrentUser.UserName = searchResult.Properties["sAMAccountName"][0].ToString();
                    
                   
                    try
                    {
                        CurrentUser.GivenName = searchResult.Properties["givenName"][0].ToString();
                        CurrentUser.Surname = searchResult.Properties["sn"][0].ToString();
                        CurrentUser.DisplayName = CurrentUser.GivenName + " " + CurrentUser.Surname;
                        lblDisplayName.Content = CurrentUser.DisplayName;
                        
                        
                    }
                    catch
                    {

                    }
                    try
                    {
                        CurrentUser.Mail = searchResult.Properties["mail"][0].ToString();
                        lblMail.Content = CurrentUser.Mail;

                    }
                    catch
                    {
                        lblMail.Content = "No Email";
                    }
                    try
                    {
                        CurrentUser.Title = searchResult.Properties["title"][0].ToString();
                        lblTitle.Content = CurrentUser.Title;
                    }
                    catch
                    {
                        lblTitle.Content = "No Title";
                    }
                    try
                    {
                        CurrentUser.Department = searchResult.Properties["department"][0].ToString();
                        lblDepartment.Content = CurrentUser.Department;
                    }
                    catch
                    {
                        lblDepartment.Content = "No Department";
                    }
                    

                    CurrentUser.Path = searchResult.Path;
                    CurrentUser.UserOU = CurrentUser.Path.Substring(CurrentUser.Path.IndexOf(',') + 1);

                    deSearch.Dispose();
                    objDE.Close();

                    lblDisplayName.Visibility = Visibility.Visible;
                    lblMail.Visibility = Visibility.Visible;
                    lblTitle.Visibility = Visibility.Visible;
                    lblDepartment.Visibility = Visibility.Visible;
                    btnUnlock.Visibility = Visibility.Hidden;
                   


                    // set up domain context
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain,"CEWA",CurrentUser.UserOU);

                    // find a user
                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, CurrentUser.UserName);

                    if (user != null)
                    {
                        CurrentUser.AccountStatus = user.Enabled;
                        CurrentUser.Lockout = user.IsAccountLockedOut();
                    }

                    user.Dispose();
                    ctx.Dispose();

                                      

                    if (CurrentUser.AccountStatus == false)
                    {
                        DisabledUser();
                    }
                    else
                    {
                        if (CurrentUser.Lockout == true)
                        {
                           LockedUser();
                        }
                        else
                        {
                           EnabledUser();
                        }
                    }
                    


                    wdMain.Height = 370;
                }
                else
                {
                    wdMain.Height = 140;
                }
            }
            else
            {
                wdMain.Height = 140;
            }
        }

        private void DisplayUserList(string userName1)
        {
            lstUsers.Items.Clear();
            if (userName1 != "")
            {


                DirectoryEntry objDE;

                // Create a new DirectoryEntry with the given path and credentials from properties.  
                objDE = new DirectoryEntry(Properties.Settings.Default.RootOU);

                //Search AD
                var deSearch = new DirectorySearcher(objDE)
                { SearchRoot = objDE, Filter = "(&(objectCategory=user)(cn=" + userName1 + "*))" };

                SearchResultCollection searchResult = deSearch.FindAll();

                foreach (SearchResult sr in deSearch.FindAll())
                {
                    ResultPropertyCollection pc = sr.Properties;
                    lstUsers.Items.Add(pc["samaccountname"][0].ToString());
                }

                deSearch.Dispose();
                objDE.Close();
            }
        }
        private void DisabledUser()
        {
            lblDisplayName.Foreground = new SolidColorBrush(Colors.Red);
            lblMail.Foreground = new SolidColorBrush(Colors.Red);
            lblTitle.Foreground = new SolidColorBrush(Colors.Red);
            lblDepartment.Foreground = new SolidColorBrush(Colors.Red);
            
        }

        private void EnabledUser()
        {
            lblDisplayName.Foreground = new SolidColorBrush(Colors.White);
            lblMail.Foreground = new SolidColorBrush(Colors.White);
            lblTitle.Foreground = new SolidColorBrush(Colors.White);
            lblDepartment.Foreground = new SolidColorBrush(Colors.White);
            
        }

        private void LockedUser()
        {
            lblDisplayName.Foreground = new SolidColorBrush(Colors.Orange);
            lblMail.Foreground = new SolidColorBrush(Colors.Orange);
            lblTitle.Foreground = new SolidColorBrush(Colors.Orange);
            lblDepartment.Foreground = new SolidColorBrush(Colors.Orange);
            btnUnlock.Visibility = Visibility.Visible;
           
        }

        static string GrabPassword()
        {
            WebClient client = new WebClient();
            string myPass = client.DownloadString("https://dinopass.com/password/simple");
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            myPass = ti.ToTitleCase(myPass);
            return myPass;
        }

        private void WdMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8)
            {
               ResetPassword(CurrentUser.Path);
           
            }

            if (e.Key == Key.Escape)
            {
                System.Windows.Application.Current.Shutdown();

            }

            if (e.Key == Key.F12)
            {
                BulkWindow bulkWindow = new BulkWindow();
                bulkWindow.ShowDialog();
                

            }
        }

      
        private void LstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (lstUsers.SelectedItem != null)
            {
                string userSelected = Convert.ToString(lstUsers.SelectedItem);
                DisplayUser(userSelected);
                //txtUsername.Text = userSelected;
            }           
        }

        private void ResetPassword(string userNamePath)
        {

            try
            {
                
                AlertWindow alertWindow = new AlertWindow();
                alertWindow.lblUser.Content = CurrentUser.DisplayName;
                var changePass = alertWindow.ShowDialog();
                
                if (changePass == true)
                {
                    
                    string newPassword = GrabPassword();
                    
                    DirectoryEntry objDE;
                    objDE = new DirectoryEntry(userNamePath);
                    
                    objDE.Invoke("SetPassword", new object[] { newPassword });
                    objDE.Properties["LockOutTime"].Value = 0;
                    
                    if (alertWindow.chkForceChange.IsChecked == true)
                    {
                        objDE.Properties["pwdLastSet"].Value = 0;
                    }
                    else
                    {
                        objDE.Properties["pwdLastSet"].Value = -1;
                    }

                    objDE.CommitChanges();
                    objDE.Close();

                    PasswordWindow passwordWindow = new PasswordWindow();
                    passwordWindow.lblNewPassword.Content = newPassword;
                    passwordWindow.ShowDialog();

                    txtUsername.Text = "";
                    txtUsername.Focus();
                    wdMain.Height = 140;

                }




            }
            catch (System.Reflection.TargetInvocationException)
            {

                MessageBox.Show("Login as user with password reset permissions",
                    "Access Denied!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                                
            }
        }

        static bool CheckWritePermission()
        {
            string checkOU = Properties.Settings.Default.RootOU;
            checkOU = checkOU.Substring(7, checkOU.Length - 7);
            checkOU = "LDAP://OU=Staff," + checkOU;
            DirectoryEntry de1 = new DirectoryEntry(checkOU);
            var deSearch = new DirectorySearcher(de1)
            { SearchRoot = de1, Filter = "(&(objectCategory=user))" };
            SearchResult searchResult = deSearch.FindOne();
            string path = searchResult.Path;
            
            using (DirectoryEntry de = new DirectoryEntry(path))
            {
                de.RefreshCache(new string[] { "allowedAttributesEffective" });
                return de.Properties["allowedAttributesEffective"].Value != null;

            }

            
        }

        private void CheckSchool()
        {
            //Get Current User
            string thisuser = Environment.UserName;
            try
            {
                //Find Current User School Code from extensionAttribute8
                DirectoryEntry de1 = new DirectoryEntry("LDAP://OU=Schools,OU=CEWA,DC=cewa,DC=edu,DC=au");
                var deSearch = new DirectorySearcher(de1)
                { SearchRoot = de1, Filter = "(&(objectCategory=user)(cn=" + thisuser + "))" };
                deSearch.PropertiesToLoad.Add("extensionAttribute8");
                SearchResult searchResult = deSearch.FindOne();
                string schoolCode = searchResult.Properties["extensionAttribute8"][0].ToString();
                deSearch.Dispose();
                de1.Close();

                //Search for School OU
                DirectoryEntry de2 = new DirectoryEntry("LDAP://OU=Schools,OU=CEWA,DC=cewa,DC=edu,DC=au");
                var deSearch2 = new DirectorySearcher(de2)
                { SearchRoot = de2, Filter = "(&(objectCategory=organizationalUnit)(ou=" + schoolCode + "*))" };
                deSearch2.PropertiesToLoad.Add("ou");
                SearchResult searchResult2 = deSearch2.FindOne();
                string schoolPath = searchResult2.Path;
                string ouName = searchResult2.Properties["ou"][0].ToString();
                ouName = ouName.Substring(5, ouName.Length - 5);
                deSearch2.Dispose();
                de2.Close();

                //Set Settings File
                Properties.Settings.Default.RootOU = schoolPath;
                Properties.Settings.Default.OuName = ouName;
                Properties.Settings.Default.Save();
            }
            catch 
            {
                
            }
        }

        private void UnlockUser()
        {
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "CEWA", CurrentUser.UserOU);

            // find a user
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, CurrentUser.UserName);

            if (user != null)
            {
                user.UnlockAccount();
                user.Save();
            }

            user.Dispose();
            ctx.Dispose();
            btnUnlock.Visibility = Visibility.Hidden;
            DisplayUser(CurrentUser.UserName);
        }

        private void WdMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void BtnBulk_Click(object sender, RoutedEventArgs e)
        {
            BulkWindow bulkWindow = new BulkWindow();
            bulkWindow.ShowDialog();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetPassword(CurrentUser.Path);
        }

        private void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            UnlockUser();
        }
    }

    class User
    {
        public string Path { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Title { get; set; }
        public string Mail { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        public bool? AccountStatus { get; set; }
        public string Password { get; set; }
        public bool Lockout { get; set; }
        public string UserOU { get; set; }
        public string LastLogin { get; set; }
    }

}
