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
        public FolderDash()
        {
            InitializeComponent();

            this.SizeChanged += FolderDash_SizeChanged;

            // TODO: Load Dashboards

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
            System.Diagnostics.Process dashboard = new System.Diagnostics.Process();
            string filename = this.GetType().Assembly.Location;
            dashboard.StartInfo.FileName = filename;

            dashboard.Start();
        }
    }
}
