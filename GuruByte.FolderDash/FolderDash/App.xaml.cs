using FolderDash.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentDashboard = new Dashboard();

            if (e != null && e.Args != null && e.Args.Count() > 0)
            {
                CurrentDashboard = Dashboard.Load(e.Args[0]);
            }

            base.OnStartup(e);
        }
    }
}
