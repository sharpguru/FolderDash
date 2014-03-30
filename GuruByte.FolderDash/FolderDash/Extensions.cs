﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
