using FolderDash.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace FolderDash.Windows
{
    /// <summary>
    /// Interaction logic for FolderDash.xaml
    /// </summary>
    public partial class FolderDash : Window
    {
        public App CurrentApplication
        {
            get
            {
                return App.Current as App;
            }
        }

        private List<Dashboard> _dashboardlist = new List<Dashboard>();
        public List<Dashboard> DashboardList
        {
            get
            {
                return _dashboardlist;
            }

            set
            {
                _dashboardlist = value; 
            }
        }

        public FolderDash()
        {
            InitializeComponent();

            this.SizeChanged += FolderDash_SizeChanged;

            this.Title = "FolderDash - {0}".FormatString(CurrentApplication.CurrentDashboard.Name);

            #region Load Dashboards
            var dashboardfiles = Directory.EnumerateFiles(CurrentApplication.ApplicationDataPath, "*.dashboard");
            
            foreach (var f in dashboardfiles)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(f);
                DashboardList.Add(Dashboard.Load(filename));
            }

            if (DashboardList.Where(db => db.Name == "[Default]").Count() == 0)
            {
                DashboardList.Add(new Dashboard() { Name = "[Default]" });
            }

            foreach (var d in DashboardList.OrderBy(x => x.Name))
            {
                var item = new TreeViewItem();
                item.Header = d.Name;
                item.MouseDoubleClick += FolderTree_Dashboards_Dashboard_MouseDoubleClick;
                item.Selected += FolderTree_Dashboards_Dashboard_item_Selected;
                FolderTree_Dashboards.Items.Add(item);
            }
            #endregion // Load Dashboards

            // Load Computer drives
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                var tvi = new TreeViewItem();
                if (drive.IsReady)
                {
                    tvi.Header = drive.VolumeLabel + " " + drive.Name;
                }
                else
                {
                    tvi.Header = drive.DriveType.ToString() + " " + drive.Name;
                }
            
                tviComputer.Items.Add(tvi);
            }

            // TODO: Load Network shares
        }

        void FolderTree_Dashboards_Dashboard_item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            var dash = DashboardList.Find(d => d.Name == item.Header.ToString());

            DashboardTabs.Items.Clear();

            // Folder Image
            Image img = new Image();
            img.Width = 24;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/folder.ico");
            logo.EndInit();
            img.Source = logo;

            // Tab label
            TextBlock tablabel = new TextBlock();
            tablabel.Text = " " + dash.Name + " - Dashboard ";

             // Close Button
            Button closebutton = new Button();
            closebutton.Click += DashboardTabs_Tab_CloseButton_Click;
            closebutton.Content = "X";

            // Tab Label Header stackpanel container
            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.Children.Add(img);
            stackpanel.Children.Add(tablabel);
            stackpanel.Children.Add(closebutton);

            TabItem tabitem = new TabItem();
            tabitem.Header = stackpanel;
            DashboardTabs.Items.Add(tabitem);

        }

        private void DashboardTabs_Tab_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel sp = (StackPanel)btn.Parent;
            TabItem tab = (TabItem)(sp.Parent);
            DashboardTabs.Items.Remove(tab);
        }

        void FolderDash_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualHeight = this.ActualHeight;
            FolderTree.MaxHeight = actualHeight;
        }

        private void MainMenu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void MainMenu_File_NewDashboard_Click(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputBox() { Title = "New Dashboard", Prompt = "Name:" };
            var result = inputWindow.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (inputWindow.Value.IsNullOrEmpty())
                {
                    MessageBox.Show("Name is required!");
                }
                else
                {
                    Dashboard dashboard = new Dashboard() { Name = inputWindow.Value };
                    dashboard.Save();
                }
            }
        }

        private void FolderTree_Dashboards_Dashboard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process dashboard = new System.Diagnostics.Process();
            string location = this.GetType().Assembly.Location;
            dashboard.StartInfo.FileName = location;
            dashboard.StartInfo.Arguments = ((TreeViewItem)sender).Header.ToString();

            dashboard.Start();
        }
    }
}
