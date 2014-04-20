using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FolderDash.Models
{
    /// <summary>
    /// Custom TabItem class specific to dashboards
    /// </summary>
    public class DashboardTabItem : TabItem
    {
        Image img { get; set; }
        BitmapImage logo { get; set; }
        public TextBlock Title { get; set; }
        CrossButton.CrossButton closebutton { get; set; }
        StackPanel stackpanel { get; set; }

        public DashboardTabItem()
        {

        }

        public DashboardTabItem(Dashboard dash)
        {
            // Folder Image
            img = new Image();
            img.Width = 16;
            logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/folder.ico");
            logo.EndInit();
            img.Source = logo;

            // Tab label
            Title = new TextBlock();
            Title.Text = FormatTitle(dash);

            // Close button
            closebutton = new CrossButton.CrossButton();
            closebutton.Click += closebutton_Click;
            closebutton.MouseEnter += closebutton_MouseEnter;
            closebutton.MouseLeave += closebutton_MouseLeave;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.Children.Add(img);
            stackpanel.Children.Add(Title);
            stackpanel.Children.Add(closebutton);

            this.Header = stackpanel;

            // Drag and Drop
            PreviewMouseMove += DashboardTabItem_PreviewMouseMove;
            Drop += DashboardTabItem_Drop;
            AllowDrop = true;
        }

        public DashboardTabItem(string DriveName, string TabLabel)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            var drive = drives.Where(d => d.Name == DriveName).FirstOrDefault();

            var dir = drive.RootDirectory;
            var subdirs = dir.GetDirectories();
            var files = dir.GetFiles();

            // Folder Image
            img = new Image();
            img.Width = 16;
            logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/FolderDash;component/Assets/folder.png");
            logo.EndInit();
            img.Source = logo;

            // Tab label
            Title = new TextBlock();
            Title.Text = TabLabel;

            // Close button
            closebutton = new CrossButton.CrossButton();
            closebutton.Click += closebutton_Click;
            closebutton.MouseEnter += closebutton_MouseEnter;
            closebutton.MouseLeave += closebutton_MouseLeave;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.Children.Add(img);
            stackpanel.Children.Add(Title);
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

            Header = stackpanel;
            Content = contentList;

            // Drag and Drop
            PreviewMouseMove += DashboardTabItem_PreviewMouseMove;
            Drop += DashboardTabItem_Drop;
            AllowDrop = true;
        }

        void closebutton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject child = (DependencyObject)sender;
            TabItem tab = child.FindParent<TabItem>();
            TabControl DashboardTabs = tab.Parent as TabControl;
            DashboardTabs.Items.Remove(tab);
        }

        void DashboardTabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var dep = sender as DependencyObject;
            var tabItem = dep.FindParent<DashboardTabItem>();

            if (tabItem == null)
                return;

            if (AllowDrop && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        void DashboardTabItem_Drop(object sender, DragEventArgs e)
        {
            var deptarget = sender as DependencyObject;
            var tabItemTarget = deptarget.FindParent<DashboardTabItem>();

            var depsource = e.Data.GetData(typeof(DashboardTabItem)) as DependencyObject;
            var tabItemSource = depsource.FindParent<DashboardTabItem>();

            if (tabItemSource == null)
                return;

            if (!tabItemTarget.Equals(tabItemSource))
            {
                var tabControl = tabItemTarget.Parent as TabControl;
                int sourceIndex = tabControl.Items.IndexOf(tabItemSource);
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);

                tabControl.Items.Remove(tabItemTarget);
                tabControl.Items.Insert(sourceIndex, tabItemTarget);
            }
        }

        void closebutton_MouseEnter(object sender, MouseEventArgs e)
        {
            AllowDrop = false;
        }

        void closebutton_MouseLeave(object sender, MouseEventArgs e)
        {
            AllowDrop = true;
        }

        static public string FormatTitle(Dashboard dash)
        {
            string title = " " + dash.Name + " - Dashboard ";
            return title;
        }

        static public DashboardTabItem Find(TabControl tabControl, Dashboard dash)
        {
            return Find(tabControl, FormatTitle(dash));
        }

        static public DashboardTabItem Find(TabControl tabControl, string TabLabel)
        {
            DashboardTabItem found = null;

            foreach (object obj in tabControl.Items)
            {
                if (obj is DashboardTabItem)
                {
                    var itm = (DashboardTabItem)obj;

                    if (itm.Title.Text == TabLabel)
                    {
                        found = itm;
                        break;
                    }
                }
            }

            return found;
        }
    }
}
