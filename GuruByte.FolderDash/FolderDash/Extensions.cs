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
        /// Removes invalid characters from string in an attempt to produce a valid filename
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string CleanFilename(this string src)
        {
            if (src == null) throw new ArgumentNullException();

            string result = src;

            // Get a list of invalid file characters. 
            char[] invalidFileChars = Path.GetInvalidFileNameChars();

            if (invalidFileChars.Count() > 0)
            {
                foreach (char c in invalidFileChars.ToList())
                {
                    result = result.Replace(c.ToString(), "");
                }
            }

            return result;
        }

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
    }
}
