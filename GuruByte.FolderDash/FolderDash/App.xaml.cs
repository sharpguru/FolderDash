using FolderDash.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FolderDash
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Dashboard CurrentDashboard { get; set; }

        public string ApplicationDataPath
        {
            get
            {
                string AppData = Environment.ExpandEnvironmentVariables("%AppData%");
                string folderPath = Path.Combine(AppData, "FolderDash");

                return folderPath;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            #region Initialize Application directory

            if (!Directory.Exists(ApplicationDataPath))
            {
                Directory.CreateDirectory(ApplicationDataPath);
            }
            #endregion

            #region Load the dashboard
            CurrentDashboard = new Dashboard();

            if (e != null && e.Args != null && e.Args.Count() > 0)
            {
                CurrentDashboard = Dashboard.Load(e.Args[0]);
            }
            #endregion // Load the dashboard

            base.OnStartup(e);
        }
    }
}
