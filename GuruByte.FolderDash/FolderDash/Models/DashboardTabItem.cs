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
                return dashboard.BackgroundImagePath;
            }

            set
            {
                dashboard.BackgroundImagePath = value;

                try
                {
                    var brush = new ImageBrush(new BitmapImage(new Uri(value)));
                    grid.Background = brush;
                }
                catch
                {
                    grid.Background = null;
                    dashboard.BackgroundImagePath = string.Empty;
                }
                dashboard.Save();
            }
        }

        public DashboardTabItem()
        {

        }

        public DashboardTabItem(Dashboard dash)
        {
            // Set Parent
            dashboard = dash;
            dash.desktopShortcuts.CollectionChanged += desktopShortcuts_CollectionChanged;

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

            // Content Container
            grid = new Grid();
            DefineGridRowsAndCols(grid, 5, 5);
            this.Content = grid;

            // Load background image
            BackgroundImagePath = dash.BackgroundImagePath;

            // Load shortcuts
            foreach (var sc in dashboard.desktopShortcuts) RenderShortcut(sc);
        }

        private void DefineGridRowsAndCols(Grid g, int rows, int cols)
        {
            if (rows <= 0) return;
            if (cols <= 0) return;

            for (int i = 1; i <= rows; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 1; j <= cols; j++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        /// <summary>
        /// Loads icon image for shortcut and adds it to the grid
        /// </summary>
        /// <param name="shortcut"></param>
        /// <returns>Returns the (col,row) coordinates shortcut was added to in the grid.</returns>
        private Tuple<int, int> RenderShortcut(Shortcut shortcut)
        {
            Tuple<int, int> result = new Tuple<int, int>(-1, -1);

            bool EmptyPositionFound = false;

            for (int row = 0; row < grid.RowDefinitions.Count; row++)
            {
                if (EmptyPositionFound == true)
                    break;

                for (int col = 0; col < grid.ColumnDefinitions.Count; col++)
                {
                    // Find first empty position
                    if (grid.Children.Count == 0)
                    {
                        EmptyPositionFound = true;
                    }
                    else
                    {
                        EmptyPositionFound = true;

                        foreach(UIElement ui in grid.Children)
                        {
                            if ((int)ui.GetValue(Grid.ColumnProperty) == col && (int)ui.GetValue(Grid.RowProperty) == row)
                            {
                                EmptyPositionFound = false;
                                break;
                            }
                        }
                    }


                    if (EmptyPositionFound == true)
                    {
                        //someImage.Source = GetIcon(somePath);
                        Image img = new Image();
                        var icon = System.Drawing.Icon.ExtractAssociatedIcon(shortcut.filename);
                        img.Source = ConvertIconToImageSource(icon, typeof(System.Drawing.Bitmap), null, System.Globalization.CultureInfo.InvariantCulture);

                        Grid.SetRow(img, row);
                        Grid.SetColumn(img, col);

                        grid.Children.Add(img);

                        result = new Tuple<int, int>(col, row);

                        break;
                    }
                }
            }

            if (result.Item1 == -1 && result.Item2 == -1) // item was not added
            {
                // Add new row to grid and try again
                grid.RowDefinitions.Add(new RowDefinition());

                result = RenderShortcut(shortcut);
            }

            return result;
        }

        public ImageSource ConvertIconToImageSource(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Icon ico = (value as System.Drawing.Icon);
            System.Drawing.Bitmap bits = ico.ToBitmap();
            MemoryStream strm = new MemoryStream();
            // add the stream to the image streams collection so we can get rid of it later
            //_imageStreams.Add(strm);
            bits.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = strm;
            bitmap.EndInit();
            // freeze it here for performance
            bitmap.Freeze();
            return bitmap;
        }

        void desktopShortcuts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                // Display new shortcuts on desktop
                foreach (var shortcut in e.NewItems.OfType<Shortcut>())
                {
                    RenderShortcut(shortcut);
                }
            }
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

            if (ContextMenu != null)
            {
                ContextMenu.IsEnabled = true;
                ContextMenu.Placement = PlacementMode.MousePoint;
                ContextMenu.IsOpen = true;
            }
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
            // Drag-drop tab items
            if (e.Data.GetDataPresent(typeof(DependencyObject)))
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

                return;
            }

            // Drag-drop files and shortcuts
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in filenames)
                {
                    //System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(file);

                    dashboard.desktopShortcuts.Add(new Shortcut() { filename = file });
                }
            }

            //var formats = e.Data.GetFormats();
            
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
