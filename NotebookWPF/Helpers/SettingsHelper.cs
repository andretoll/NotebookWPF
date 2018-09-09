using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace NotebookWPF.Helpers
{
    /// <summary>
    /// A static class to manage reading and writing settings
    /// </summary>
    public static class SettingsHelper
    {
        #region Private Members

        // Directories
        private static readonly string mainDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simple Notes");
        private static readonly string filePath = Path.Combine(mainDirectory, @"settings.xml");
        private static readonly string noteDefaultDirectory = Path.Combine(mainDirectory, "Notes");

        // Default Settings
        private static readonly string defaultTheme = "BaseLight";
        private static readonly string defaultAccent = "Blue";

        #endregion

        #region Public Members

        // Writable public Settings
        public static string noteDirectory;

        #endregion

        #region Methods

        /// <summary>
        /// Save app settings to XML
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SaveSettings(string element, SettingsProperties property, string value)
        {
            // Setup directory and file if necessary
            SetupDirectory();

            // Load settings file
            XDocument doc = XDocument.Load(filePath);

            // Get target setting
            var result = doc.Descendants(element).FirstOrDefault();

            if (result != null && value != null && !string.IsNullOrEmpty(property.ToString()))
            {
                // Change setting
                result.Attribute(property.ToString()).Value = value;
                // Save changes
                doc.Save(filePath);
            }

            // Reload all settings
            LoadSettings();
        }

        /// <summary>
        /// Load settings from XML
        /// </summary>
        public static void LoadSettings()
        {
            // Setup directory and file if necessary
            SetupDirectory();

            try
            {
                // Load settings file
                XDocument doc = XDocument.Load(filePath);

                // Get appearance settings from file
                var theme = doc.Root.Elements("theme").FirstOrDefault().Attribute("color").Value;
                var accent = doc.Root.Elements("accent").FirstOrDefault().Attribute("color").Value;

                // Apply visual settings
                SetAppTheme(theme, accent);

                // Get application settings from file
                noteDirectory = doc.Root.Elements("noteDirectory").FirstOrDefault().Attribute("path").Value;     
            }
            catch
            {
                // If any errors occurs on loading, repair settings
                RepairSettings();
            }            
        }

        /// <summary>
        /// Create directory and file if needed
        /// </summary>
        private static void SetupDirectory()
        {
            // If main directory does not exist
            if (!Directory.Exists(mainDirectory))
            {
                // Create main directory
                Directory.CreateDirectory(mainDirectory);
            }

            // If note directory does not exist
            if (!Directory.Exists(noteDefaultDirectory))
            {
                // Create note directory
                Directory.CreateDirectory(noteDefaultDirectory);
            }

            // If file does not exist
            if (!File.Exists(filePath))
            {
                // Create default settings
                CreateDefaultSettings();                
            }
        }

        /// <summary>
        /// Create default settings for settings file
        /// </summary>
        private static void CreateDefaultSettings()
        {
            // Create file
            var xdoc = new XDocument(
            new XElement("settings",
                new XElement("theme",
                    new XAttribute("color", defaultTheme)),
                new XElement("accent",
                    new XAttribute("color", defaultAccent)),
                new XElement("noteDirectory",
                    new XAttribute("path", noteDefaultDirectory))));

            xdoc.Save(filePath);
        }

        /// <summary>
        /// Recreate settings file with default values
        /// </summary>
        public static void RepairSettings()
        {
            // Delete file
            File.Delete(filePath);

            // Create default settings
            CreateDefaultSettings();

            LoadSettings();
        }

        /// <summary>
        /// Set App Theme and Accent
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="accent"></param>
        public static void SetAppTheme(string theme, string accent)
        {
            // If either theme or accent is empty
            if ((string.IsNullOrEmpty(theme)) || (string.IsNullOrEmpty(accent)))
            {
                // Get current theme or accent from ThemeManager
                Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

                // Apply either theme or accent
                if (string.IsNullOrEmpty(theme))
                    theme = appStyle.Item1.Name;
                
                if (string.IsNullOrEmpty(accent))
                    accent = appStyle.Item2.Name;
            }

            try
            {
                // Try applying theme and accent
                ThemeManager.ChangeAppStyle(
                    Application.Current,
                    ThemeManager.GetAccent(accent),
                    ThemeManager.GetAppTheme(theme)
                );
            }
            catch
            {
                // If any errors occur, set base theme and accent
                ThemeManager.ChangeAppStyle(
                    Application.Current,
                    ThemeManager.GetAccent(defaultAccent),
                    ThemeManager.GetAppTheme(defaultTheme)
                );
            }
        }

        /// <summary>
        /// Get current theme
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTheme()
        {
            // Get current theme
            return ThemeManager.DetectAppStyle(Application.Current).Item1.Name;
        }

        /// <summary>
        /// Get current accent
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAccent()
        {
            // Get current accent
            return ThemeManager.DetectAppStyle(Application.Current).Item2.Name;
        }

        /// <summary>
        /// Load available themes
        /// </summary>
        public static List<string> GetAllThemes()
        {
            // Populate collection
            List<string> themes = new List<string>();
            themes.Add("BaseDark");
            themes.Add("BaseLight");

            return themes;
        }

        /// <summary>
        /// Load available accents
        /// </summary>
        public static List<string> GetAllAccents()
        {
            // Populate collection
            List<string> accents = new List<string>();
            accents.Add("Blue");
            accents.Add("Red");
            accents.Add("Green");
            accents.Add("Orange");
            accents.Add("Yellow");
            accents.Add("Pink");
            accents.Add("Purple");
            accents.Add("Lime");
            accents.Add("Emerald");
            accents.Add("Teal");
            accents.Add("Cobalt");
            accents.Add("Indigo");
            accents.Add("Violet");
            accents.Add("Magenta");
            accents.Add("Crimson");
            accents.Add("Amber");
            accents.Add("Brown");
            accents.Add("Olive");
            accents.Add("Steel");
            accents.Add("Mauve");
            accents.Add("Taupe");
            accents.Add("Sienna");

            // Return list ordered alphabetically
            return accents.OrderBy(a => a).ToList();
        }

        #endregion

        public enum SettingsProperties
        {
            color,
            path,
            enabled
        }
    }
}
