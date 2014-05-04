using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace FolderDash.Models
{
    /// <summary>
    /// Custom TabItem class specific to dashboards
    /// </summary>
    public class DashboardTabItem : TabItem
    {
        Dashboard dashboard { get; set; }
        Image img { get; set; }
        BitmapImage logo { get; set; }
        public TextBlock Title { get; set; }
        CrossButton.CrossButton closebutton { get; set; }
        StackPanel stackpanelHeader { get; set; }
        ContextMenu contextmenu { get; set; }
        Grid grid { get; set; }

        public string BackgroundImagePath
        { 
            get
            {
                return backgroundImagePath;
            }

            set
            {
                backgroundImagePath = value;
                var brush = new ImageBrush(new BitmapImage(new Uri(backgroundImagePath)));

                try
                {
                    grid.Background = brush;
                    dashboard.Save();
                }
                catch
                {
                    grid.Background = null;
                }
            }
        }
        private string backgroundImagePath = null;

        public DashboardTabItem()
        {

        }

        public DashboardTabItem(Dashboard dash)
        {
            // Set Parent
            dashboard = dash;

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
            closebutton.Click += CloseButton_Click;
            closebutton.MouseEnter += closebutton_MouseEnter;
            closebutton.MouseLeave += closebutton_MouseLeave;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            stackpanelHeader = new StackPanel();
            stackpanelHeader.Orientation = Orientation.Horizontal;
            stackpanelHeader.Children.Add(img);
            stackpanelHeader.Children.Add(Title);
            stackpanelHeader.Children.Add(closebutton);

            this.Header = stackpanelHeader;

            // Context menu
            contextmenu = new System.Windows.Controls.ContextMenu();
            MenuItem mnuClose = new MenuItem();
            mnuClose.Header = "Close";
            mnuClose.Click += CloseButton_Click;
            MenuItem mnuBackground = new MenuItem();
            mnuBackground.Header = "Set Background Image";
            mnuBackground.Click += mnuBackground_Click;
            contextmenu.Items.Add(mnuClose);
            contextmenu.Items.Add(mnuBackground);
            ContextMenu = contextmenu;
            ContextMenu.IsEnabled = false;
            ContextMenu.IsOpen = false;
            MouseRightButtonUp += DashboardTabItem_MouseRightButtonUp;

            // Drag and Drop
            PreviewMouseMove += DashboardTabItem_PreviewMouseMove;
            Drop += DashboardTabItem_Drop;
            AllowDrop = true;

            // Background image
            //backgroundImagePath = @"C:\Work\Screen Backgrounds\firstlight21600.jpg";

            grid = new Grid();
            //var brush = new ImageBrush(new BitmapImage(new Uri(backgroundImagePath)));
            //grid.Background = brush;

            this.Content = grid;
        }

        void mnuBackground_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // set background image
                string filename = dlg.FileName;
                BackgroundImagePath = filename;
            }
        }

        void DashboardTabItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenContextMenu(sender);
        }

        public void OpenContextMenu(object sender)
        {
            var root = ((FrameworkElement)sender).FindElement("DashboardTabs");
            ContextMenu.IsEnabled = true;
            ContextMenu.Placement = PlacementMode.MousePoint;
            ContextMenu.IsOpen = true;
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
            closebutton.Click += CloseButton_Click;
            closebutton.MouseEnter += closebutton_MouseEnter;
            closebutton.MouseLeave += closebutton_MouseLeave;
            closebutton.Margin = new Thickness(4);
            closebutton.Width = 12;

            // Tab Label Header stackpanel container
            stackpanelHeader = new StackPanel();
            stackpanelHeader.Orientation = Orientation.Horizontal;
            stackpanelHeader.Children.Add(img);
            stackpanelHeader.Children.Add(Title);
            stackpanelHeader.Children.Add(closebutton);

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

            Header = stackpanelHeader;
            Content = contentList;

            // Drag and Drop
            PreviewMouseMove += DashboardTabItem_PreviewMouseMove;
            Drop += DashboardTabItem_Drop;
            AllowDrop = true;
        }

        public void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = this;
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
