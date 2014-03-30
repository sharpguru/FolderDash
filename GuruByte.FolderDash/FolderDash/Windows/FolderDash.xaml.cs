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

        public FolderDash()
        {
            InitializeComponent();

            this.SizeChanged += FolderDash_SizeChanged;

            this.Title = "FolderDash - {0}".FormatString(CurrentApplication.CurrentDashboard.Name);

            #region Load Dashboards
            var dashboardfiles = Directory.EnumerateFiles(CurrentApplication.ApplicationDataPath, "*.dashboard");
            List<Dashboard> dashboardlist = new List<Dashboard>();
            
            foreach (var f in dashboardfiles)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(f);
                dashboardlist.Add(Dashboard.Load(filename));
            }

            if (dashboardlist.Where(db => db.Name == "[Default]").Count() == 0)
            {
                var item = new TreeViewItem();
                item.Header = "[Default]";
                item.MouseDoubleClick += FolderTree_Dashboards_Dashboard_MouseDoubleClick;
                FolderTree_Dashboards.Items.Add(item);
            }
            foreach (var d in dashboardlist.OrderBy(x => x.Name))
            {
                var item = new TreeViewItem();
                item.Header = d.Name;
                item.MouseDoubleClick += FolderTree_Dashboards_Dashboard_MouseDoubleClick;
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
