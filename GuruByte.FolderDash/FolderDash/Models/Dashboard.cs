using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FolderDash.Models
{
    [Serializable]
    public class Dashboard
    {
        public Dashboard()
        {
            // empty ctor
        }

        public string Name 
        { 
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
        private string _name = "My Dashboard";

        /// <summary>
        /// Returns reference to the currently running FolderDash application
        /// </summary>
        public App CurrentApplication
        {
            get
            {
                return App.Current as FolderDash.App;
            }
        }

        /// <summary>
        /// Path to dashboard directory
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// Returns the settings file
        /// </summary>
        public string SettingsFile
        {
            get
            {
                // Get settings file if it exists
                string settingsfile = Path.Combine(FolderPath, Name + ".dashboard");

                return settingsfile;
            }
        }

        private string backgroundImagePath = string.Empty;
        public string BackgroundImagePath
        {
            get
            {
                string result = backgroundImagePath;

                if (string.IsNullOrEmpty(backgroundImagePath) && (!string.IsNullOrEmpty(BackgroundImageFileName)))
                {
                    result = Path.Combine(FolderPath, BackgroundImageFileName);
                }

                return result;
            }

            set
            {
                backgroundImagePath = value;
            }
        }

        public string BackgroundImageFileName { get; set; }

        private ObservableCollection<Shortcut> _desktopShortcuts = new ObservableCollection<Shortcut>();
        public ObservableCollection<Shortcut> desktopShortcuts 
        { 
            get
            {
                return _desktopShortcuts;
            }

            set
            {
                _desktopShortcuts = value;
            }
        }

        /// <summary>
        /// Open a dashboard from a directory path e.g. C:\Users\Brian\Documents\aaa.dashboard
        /// </summary>
        static public Dashboard Open(string folderPath)
        {
            Dashboard dashboard = new Dashboard();

            string path = folderPath.Substring(0, folderPath.LastIndexOf('\\'));
            string filename = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);

            string settingsfile = Path.Combine(folderPath, filename);

            if (File.Exists(settingsfile))
            {
                XmlSerializer reader = new XmlSerializer(typeof(Dashboard));
                using (StreamReader file = new StreamReader(settingsfile))
                {
                    dashboard = (Dashboard)reader.Deserialize(file);
                }
            }

            dashboard.FolderPath = folderPath;

            return dashboard;
        }

        /// <summary>
        /// Save dashboard file
        /// </summary>
        public void Save()
        {
            string settingsfile = Path.Combine(FolderPath, Name + ".dashboard");

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            XmlSerializer writer = new XmlSerializer(this.GetType());
            using (StreamWriter file = new StreamWriter(SettingsFile))
            {
                writer.Serialize(file, this);
                file.Close();
            }
        }

        public void Delete()
        {
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
            }

            if (Directory.Exists(FolderPath))
            {
                Directory.Delete(FolderPath);
            }
        }

        public void Rename(string NewName)
        {
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
                string newFolderPath = FolderPath.Substring(0, FolderPath.LastIndexOf('\\')) + NewName;
                Directory.Move(FolderPath, newFolderPath);
                Name = NewName;
                Save();
            }
        }
    }
}
