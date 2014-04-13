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
                    tvi.Tag = drive.Name;
                    tvi.Selected += FolderTree_Drives_DriveSelected;
                }
                else
                {
                    tvi.Header = drive.DriveType.ToString() + " " + drive.Name;
                }
            
                FolderTree_Drives.Items.Add(tvi);
            }

            // TODO: Load Network shares
        }

        void FolderTree_Drives_DriveSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            DriveInfo[] drives = DriveInfo.GetDrives();
            var drive = drives.Where(d => d.Name == item.Tag.ToString()).FirstOrDefault();

            var dir = drive.RootDirectory;
            var subdirs = dir.GetDirectories();
            var files = dir.GetFiles();

            // Folder Image
            Image img = new Image();
            img.Width = 16;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/folder.png");
            logo.EndInit();
            img.Source = logo;

            // Tab label
            TextBlock tablabel = new TextBlock();
            tablabel.Text = item.Header.ToString();

            // Close button
            CrossButton.CrossButton closebutton = new CrossButton.CrossButton();
            closebutton.Click += DashboardTabs_Tab_CloseButton_Click;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.Children.Add(img);
            stackpanel.Children.Add(tablabel);
            stackpanel.Children.Add(closebutton);

            // Content
            ListBox contentList = new ListBox();

            // Directories
            foreach (var subdir in subdirs)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                Label lb = new Label();
                lb.Content = subdir.Name;

                Image i = new Image();
                i.Source = logo;
                i.Width = 16;

                sp.Children.Add(i);
                sp.Children.Add(lb);

                contentList.Items.Add(sp);
            }

            // Document image
            Image docimg = new Image();
            docimg.Width = 16;
            BitmapImage doclogo = new BitmapImage();
            doclogo.BeginInit();
            doclogo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/document.png");
            doclogo.EndInit();
            docimg.Source = doclogo;

            // Files
            foreach (var f in files)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                Label lb = new Label();
                lb.Content = f.Name;

                Image i = new Image();
                i.Source = doclogo;
                i.Width = 16;

                sp.Children.Add(i);
                sp.Children.Add(lb);

                contentList.Items.Add(sp);
            }
            

            TabItem tabitem = new TabItem();
            tabitem.Header = stackpanel;
            tabitem.Content = contentList;
            DashboardTabs.Items.Insert(0, tabitem);

            DashboardTabs.SelectedIndex = 0; 

        }

        void FolderTree_Dashboards_Dashboard_item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            var dash = DashboardList.Find(d => d.Name == item.Header.ToString());

            // Folder Image
            Image img = new Image();
            img.Width = 16;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/folder.ico");
            logo.EndInit();
            img.Source = logo;

            // Tab label
            TextBlock tablabel = new TextBlock();
            tablabel.Text = " " + dash.Name + " - Dashboard ";

            // Close button
            CrossButton.CrossButton closebutton = new CrossButton.CrossButton();
            closebutton.Click += DashboardTabs_Tab_CloseButton_Click;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.Children.Add(img);
            stackpanel.Children.Add(tablabel);
            stackpanel.Children.Add(closebutton);

            TabItem tabitem = new TabItem();
            tabitem.Header = stackpanel;
            DashboardTabs.Items.Insert(0, tabitem);

            DashboardTabs.SelectedIndex = 0; 

        }

        private void DashboardTabs_Tab_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = (Button)sender;
            Control btn = (Control)sender;
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
