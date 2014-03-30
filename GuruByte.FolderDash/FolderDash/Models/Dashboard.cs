using System;
using System.Collections.Generic;
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
        private string _name = "[Default]";
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

        public string SettingsFile
        {
            get
            {
                string AppData = Environment.ExpandEnvironmentVariables("%AppData%");

                // Get settings file if it exists
                string settingsfile = Path.Combine(AppData, Name.CleanFilename(), ".dashboard");

                return settingsfile;
            }
        }

        /// <summary>
        /// Load or reload settings from settings file
        /// </summary>
        static public Dashboard Load(string Name)
        {
            Dashboard dashboard = new Dashboard() { Name = Name };

            string AppData = Environment.ExpandEnvironmentVariables("%AppData%");
            string settingsfile = Path.Combine(AppData, Name.CleanFilename(), ".dashboard");

            if (File.Exists(settingsfile))
            {
                XmlSerializer reader = new XmlSerializer(typeof(Dashboard));
                StreamReader file = new StreamReader(settingsfile);

                dashboard = (Dashboard)reader.Deserialize(file);
            }

            return dashboard;
        }

        public void Save()
        {
            XmlSerializer writer = new XmlSerializer(this.GetType());
            StreamWriter file = new StreamWriter(SettingsFile);
            writer.Serialize(file, this);
            file.Close();
        }
    }
}
