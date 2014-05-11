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
        private string _name = "[Default]";

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
        /// Returns the settings file from the application data path
        /// </summary>
        public string SettingsFile
        {
            get
            {
                string AppDataPath = Environment.ExpandEnvironmentVariables(CurrentApplication.ApplicationDataPath);

                // Get settings file if it exists
                string settingsfile = Path.Combine(AppDataPath, Name.CleanFilename() + ".dashboard");

                return settingsfile;
            }
        }

        public string BackgroundImagePath
        {
            get
            {
                return backgroundImagePath;
            }

            set
            {
                backgroundImagePath = value;
            }
        }
        private string backgroundImagePath = string.Empty;

        public ObservableCollection<Shortcut> desktopShortcuts { get; set; }

        /// <summary>
        /// Load or reload settings from settings file
        /// </summary>
        static public Dashboard Load(string filename)
        {
            Dashboard dashboard = new Dashboard();

            string AppDataPath = Environment.ExpandEnvironmentVariables(dashboard.CurrentApplication.ApplicationDataPath);
            string settingsfile = Path.Combine(AppDataPath, filename.CleanFilename() + ".dashboard");

            if (File.Exists(settingsfile))
            {
                XmlSerializer reader = new XmlSerializer(typeof(Dashboard));
                using (StreamReader file = new StreamReader(settingsfile))
                {
                    dashboard = (Dashboard)reader.Deserialize(file);
                }
            }

            return dashboard;
        }

        /// <summary>
        /// Save dashboard file
        /// </summary>
        public void Save()
        {
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
        }

        public void Rename(string NewName)
        {
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
                Name = NewName;
                Save();
            }
        }
    }
}
