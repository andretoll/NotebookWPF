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
        #region Members

        // Directories
        public static readonly string mainDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simple Notes");
        private static readonly string settingsFilePath = Path.Combine(mainDirectory, @"settings.xml");
        private static readonly string noteDefaultDirectory = Path.Combine(mainDirectory, "Notes");

        // Default Settings
        private static readonly string defaultTheme = "BaseDark";
        private static readonly string defaultAccent = "Blue";
        private static readonly string defaultFontFamily = "Lato";
        private static readonly string defaultFontSize = "16";

        // Writable public Settings
        public static string noteDirectory;
        public static string fontFamily;
        public static string fontSize;

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
            XDocument doc = XDocument.Load(settingsFilePath);

            // Get target setting
            var result = doc.Descendants(element).FirstOrDefault();

            if (result != null && value != null && !string.IsNullOrEmpty(property.ToString()))
            {
                // Change setting
                result.Attribute(property.ToString()).Value = value;
                // Save changes
                doc.Save(settingsFilePath);
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
                XDocument doc = XDocument.Load(settingsFilePath);

                // Get appearance settings from file
                var theme = doc.Root.Elements("theme").FirstOrDefault().Attribute("color").Value;
                var accent = doc.Root.Elements("accent").FirstOrDefault().Attribute("color").Value;

                // Apply visual settings
                SetAppTheme(theme, accent);

                // Get application settings from file
                noteDirectory = doc.Root.Elements("noteDirectory").FirstOrDefault().Attribute("path").Value;

                // Get font settings
                fontFamily = doc.Root.Elements("fontFamily").FirstOrDefault().Attribute("value").Value;
                fontSize = doc.Root.Elements("fontSize").FirstOrDefault().Attribute("value").Value;
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
            if (!File.Exists(settingsFilePath))
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
                    new XAttribute("path", noteDefaultDirectory)),
                new XElement("fontFamily",
                    new XAttribute("value", defaultFontFamily)),
                new XElement("fontSize",
                    new XAttribute("value", defaultFontSize))));

            xdoc.Save(settingsFilePath);
        }

        /// <summary>
        /// Recreate settings file with default values
        /// </summary>
        public static void RepairSettings()
        {
            // Delete file
            File.Delete(settingsFilePath);

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

        /// <summary>
        /// Return all font sizes
        /// </summary>
        /// <returns></returns>
        public static List<double> GetFontSizes()
        {
            // Font Sizes
            List<double> fontSizes = new List<double>();
            fontSizes.Add(3);
            fontSizes.Add(4);
            fontSizes.Add(5);
            fontSizes.Add(6);
            fontSizes.Add(6.5);
            fontSizes.Add(7);
            fontSizes.Add(7.5);
            fontSizes.Add(8);
            fontSizes.Add(8.5);
            fontSizes.Add(9);
            fontSizes.Add(9.5);
            fontSizes.Add(10);
            fontSizes.Add(10.5);
            fontSizes.Add(11.5);
            fontSizes.Add(12);
            fontSizes.Add(12.5);
            fontSizes.Add(13.5);
            fontSizes.Add(14);
            fontSizes.Add(15);
            fontSizes.Add(16);
            fontSizes.Add(17);
            fontSizes.Add(18);
            fontSizes.Add(19);
            fontSizes.Add(20);
            fontSizes.Add(22);
            fontSizes.Add(24);
            fontSizes.Add(26);
            fontSizes.Add(28);
            fontSizes.Add(30);
            fontSizes.Add(32);
            fontSizes.Add(34);
            fontSizes.Add(36);
            fontSizes.Add(38);
            fontSizes.Add(40);
            fontSizes.Add(44);
            fontSizes.Add(48);
            fontSizes.Add(52);
            fontSizes.Add(56);
            fontSizes.Add(60);
            fontSizes.Add(64);
            fontSizes.Add(68);
            fontSizes.Add(72);
            fontSizes.Add(76);
            fontSizes.Add(80);
            fontSizes.Add(88);
            fontSizes.Add(96);
            fontSizes.Add(104);
            fontSizes.Add(112);
            fontSizes.Add(120);
            fontSizes.Add(128);
            fontSizes.Add(136);
            fontSizes.Add(144);

            return fontSizes;
        }

        #endregion

        public enum SettingsProperties
        {
            color,
            path,
            value
        }
    }
}
