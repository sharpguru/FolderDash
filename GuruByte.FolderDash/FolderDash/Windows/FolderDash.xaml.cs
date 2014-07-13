using FolderDash.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public string FolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FolderDash");
        
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
            var dashboardfolders = Directory.EnumerateDirectories(FolderPath, "*.dashboard");

            foreach (var f in dashboardfolders)
            {
                //string name = f.Substring(0, f.LastIndexOf('.'));
                DashboardList.Add(Dashboard.Open(f));
            }

            foreach (var d in DashboardList.OrderBy(x => x.Name))
            {
                FolderTree_InsertDashboard(d);
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
            CreateNewDashboard();
        }

        private void CreateNewDashboard()
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
                    string name = inputWindow.Value;
                    Dashboard dashboard = new Dashboard();
                    dashboard.Name = name;
                    dashboard.FolderPath = System.IO.Path.Combine(FolderPath, name + ".dashboard"); 

                    dashboard.Save();
                    DashboardList.Insert(DashboardList.Count(), dashboard);
                    FolderTree_InsertDashboard(dashboard);
                }
            }
        }

        private DashboardTreeViewItem FolderTree_InsertDashboard(Dashboard dashboard)
        {
            var item = new DashboardTreeViewItem(dashboard.Name, FolderTree_Dashboards_Dashboard_item_Selected,
            FolderTree_Dashboards_Dashboard_MouseDoubleClick, FolderTree_Dashboards_Dashboard_MouseRightButtonUp);
            FolderTree_Dashboards.Items.Add(item);
            DashboardList.Add(dashboard);

            return item;
        }

        private void FolderTree_Dashboards_Dashboard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void FolderTree_Dashboards_Dashboard_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            item.ContextMenu = FolderTree_Dashboard_BuddyMenu;
            item.ContextMenu.IsEnabled = true;
            item.ContextMenu.PlacementTarget = item;
            item.ContextMenu.Placement = PlacementMode.Bottom;
            item.ContextMenu.IsOpen = true;
        }

        private void FolderTree_Dashboard_BuddyMenu_Delete(object sender, RoutedEventArgs e)
        {
            var OkToDelete = MessageBox.Show("OK to Delete?", "Confirm Delete", MessageBoxButton.OKCancel);

            if (OkToDelete == MessageBoxResult.OK)
            {
                TreeViewItem item = FolderTree.SelectedItem as TreeViewItem;
                var dash = DashboardList.Find(d => d.Name == item.Header.ToString());
                
                dash.Delete();
                RemoveDashboard(item, dash);
            }
        }

        private void RemoveDashboard(TreeViewItem item, Dashboard dash)
        {
            DashboardList.Remove(dash);
            FolderTree_Dashboards.Items.Remove(item);

            var dashtab = DashboardTabItem.Find(DashboardTabs, dash);
            DashboardTabs.Items.Remove(dashtab);
        }

        private void FolderTree_Dashboard_BuddyMenu_New(object sender, RoutedEventArgs e)
        {
            CreateNewDashboard();
        }

        private void FolderTree_Dashboard_BuddyMenu_Open(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process dashboard = new System.Diagnostics.Process();
            string location = this.GetType().Assembly.Location;
            dashboard.StartInfo.FileName = location;

            TreeViewItem item = FolderTree.SelectedItem as TreeViewItem;
            dashboard.StartInfo.Arguments = item.Header.ToString();

            dashboard.Start();
        }

        private void FolderTree_Dashboard_BuddyMenu_Rename(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = FolderTree.SelectedItem as TreeViewItem;
            var dash = DashboardList.Find(d => d.Name == item.Header.ToString());

            var inputWindow = new InputBox() { Title = "New Dashboard Name", Prompt = "Name:", DefaultResponse = dash.Name };
            var result = inputWindow.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RemoveDashboard(item, dash);
                dash.Rename(inputWindow.Value);
                DashboardList.Insert(DashboardList.Count(), dash);
                FolderTree_InsertDashboard(dash);
            }
        }

        private void FolderTree_DashboardTab_BuddyMenu_Close(object sender, RoutedEventArgs e)
        {
            var item = DashboardTabs.SelectedItem;

            if (item is DashboardTabItem)
            {
                var dashitem = (DashboardTabItem)item;
                dashitem.CloseButton_Click(sender, e);
            }
        }

        private void FolderTree_DashboardTab_BuddyMenu_SetBackground(object sender, RoutedEventArgs e)
        {

        }

        private void FolderTree_DashboardTabs_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tab = (DashboardTabItem)DashboardTabs.SelectedItem;
            
            if (tab != null) tab.OpenContextMenu(tab);
        }
    }
}
