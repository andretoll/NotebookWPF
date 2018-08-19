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

        private static readonly string filePath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "/Simple Notes", @"/settings.xml");
        private static readonly string directory = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "/Simple Notes");

        #endregion

        #region Methods

        /// <summary>
        /// Save app settings to XML
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SaveSettings(string element, string value)
        {
            // Setup directory and file if necessary
            SetupDirectory();

            // Load settings file
            XDocument doc = XDocument.Load(filePath);

            // Get target setting
            var result = doc.Descendants(element).FirstOrDefault();

            if (result != null && value != null)
            {
                // Change setting
                result.Attribute("color").Value = value;
                // Save changes
                doc.Save(filePath);
            }
        }

        /// <summary>
        /// Load settings from XML
        /// </summary>
        public static void LoadSettings()
        {
            // Setup directory and file if necessary
            SetupDirectory();

            // Load settings file
            XDocument doc = XDocument.Load(filePath);

            // Get settings from file
            var theme = doc.Root.Elements("theme").FirstOrDefault().Attribute("color").Value;
            var accent = doc.Root.Elements("accent").FirstOrDefault().Attribute("color").Value;

            // Apply visual settings
            SettingsHelper.SetAppTheme(theme, accent);
        }

        /// <summary>
        /// Create directory and file if needed
        /// </summary>
        private static void SetupDirectory()
        {
            // If directory does not exist
            if (!Directory.Exists(directory))
            {
                // Create directory
                Directory.CreateDirectory(directory);
            }

            // If file does not exist
            if (!File.Exists(filePath))
            {
                // Create file
                var xdoc = new XDocument(
                new XElement("settings",
                    new XElement("theme",
                        new XAttribute("color", "BaseLight")),
                    new XElement("accent",
                        new XAttribute("color", "Blue"))));

                xdoc.Save(filePath);
            }
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
                {
                    theme = appStyle.Item1.Name;
                }
                else
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
                    ThemeManager.GetAccent("Blue"),
                    ThemeManager.GetAppTheme("BaseLight")
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

            return accents;
        }

        #endregion
    }
}
