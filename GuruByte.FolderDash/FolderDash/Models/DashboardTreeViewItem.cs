using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolderDash.Models
{
    public class DashboardTreeViewItem : TreeViewItem
    {
        public DashboardTreeViewItem()
        {

        }

        public DashboardTreeViewItem(object Header, RoutedEventHandler OnSelected = null, MouseButtonEventHandler OnMouseDoubleClick = null, MouseButtonEventHandler OnMouseRightButtonUp = null)
        {
            this.Header = Header;
            if (OnMouseDoubleClick != null) MouseDoubleClick += OnMouseDoubleClick;
            if (OnSelected != null) Selected += OnSelected;
            if (OnMouseRightButtonUp != null) MouseRightButtonUp += OnMouseRightButtonUp;
        }
    }
}
