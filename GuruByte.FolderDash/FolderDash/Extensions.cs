using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FolderDash
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true when string is null or empty
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string src)
        {
            return String.IsNullOrEmpty(src); 
        }

        /// <summary>
        /// Adds support for string formatting to all strings
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatString(this string src, params object[] args)
        {
            return string.Format(src, args);
        }
    }

    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Search up the visual tree for a parent of given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns>Ancestor</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;

            T childisparent = child as T;
            if (childisparent != null) return childisparent;

            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// Returns the root element in the visual tree
        /// </summary>
        /// <param name="descendant"></param>
        /// <returns></returns>
        public static FrameworkElement FindRoot(this FrameworkElement descendant)
        {
            if (descendant == null) return null;

            // get parent item
            FrameworkElement parent = (FrameworkElement)VisualTreeHelper.GetParent(descendant);

            if (parent != null) 
            {
                return FindRoot(parent); // recurse through parents to find root
            }

            return descendant; // this is the root element
        }

        /// <summary>
        /// Returns the first element found with Name starting from the root element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Object FindElement(this FrameworkElement element, string Name)
        {
            FrameworkElement root = element.FindRoot();
            return root.FindName(Name);
        }
    }
}
