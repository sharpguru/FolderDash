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
        public App CurrentApplication { get { return App.Current as App; }}
        
        public List<Dashboard> DashboardList
        {
            get { return _dashboardlist; }
            set { _dashboardlist = value; }
        }
        private List<Dashboard> _dashboardlist = new List<Dashboard>();

        public FolderDash()
        {
            InitializeComponent();

            this.SizeChanged += FolderDash_SizeChanged;
            this.Title = "FolderDash - {0}".FormatString(CurrentApplication.CurrentDashboard.Name);

            LoadDashboards();
            LoadDrives();

            // TODO: Load Network shares
        }

        private void LoadDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                var tvi = new DashboardTreeViewItem();
                if (drive.IsReady)
                {
                    tvi.Header = drive.VolumeLabel + " " + drive.Name;
                    tvi.Tag = drive.Name;
                    tvi.Selected += FolderTree_Drives_DriveSelected;
                }
                else
                {
                    tvi.Header = drive.DriveType.ToString() + " " + drive.Name;
                }

                FolderTree_Drives.Items.Add(tvi);
            }
        }

        private void LoadDashboards()
        {
            var dashboardfiles = Directory.EnumerateFiles(CurrentApplication.ApplicationDataPath, "*.dashboard");

            foreach (var f in dashboardfiles)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(f);
                DashboardList.Add(Dashboard.Load(filename));
            }

            foreach (var d in DashboardList.OrderBy(x => x.Name))
            {
                var item = new DashboardTreeViewItem(d.Name, FolderTree_Dashboards_Dashboard_item_Selected, 
                    FolderTree_Dashboards_Dashboard_MouseDoubleClick);

                FolderTree_Dashboards.Items.Add(item);
            }
        }

        void FolderTree_Drives_DriveSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            TabItem drivetab = DashboardTabItem.Find(DashboardTabs, item.Header.ToString());

            if (drivetab == null)
            {
                drivetab = new DashboardTabItem(item.Tag.ToString(), item.Header.ToString());
                DashboardTabs.Items.Insert(0, drivetab);
            }

            DashboardTabs.SelectedItem = drivetab;
        }

        void FolderTree_Dashboards_Dashboard_item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            var dash = DashboardList.Find(d => d.Name == item.Header.ToString());
            var dashtab = DashboardTabItem.Find(DashboardTabs, dash);

            if (dashtab == null)
            {
                dashtab = new DashboardTabItem(dash);
                DashboardTabs.Items.Insert(0, dashtab);
            }

            DashboardTabs.SelectedItem = dashtab;
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
