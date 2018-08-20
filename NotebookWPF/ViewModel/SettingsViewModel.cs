using NotebookWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.ViewModel
{
    /// <summary>
    /// A ViewModel for all settings
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        private string selectedAccent;

        private string selectedTheme;

        private string noteDirectory;

        #endregion

        #region Public Members

        // Collection of available themes
        public ObservableCollection<string> Themes { get; set; }

        // Collection of available accents
        public ObservableCollection<string> Accents { get; set; }

        #endregion

        #region Properties

        public string SelectedTheme
        {
            get { return SettingsHelper.GetCurrentTheme(); }
            set
            {
                selectedTheme = value;
                SettingsHelper.SetAppTheme(value, null);

                // Save changes
                SettingsHelper.SaveSettings("theme", value);

                // Notify property changed
                NotifyPropertyChanged();
            }
        }

        public string SelectedAccent
        {
            get { return SettingsHelper.GetCurrentAccent(); }
            set
            {
                selectedAccent = value;
                SettingsHelper.SetAppTheme(null, value);

                // Save changes
                SettingsHelper.SaveSettings("accent", value);

                // Notify property changed
                NotifyPropertyChanged();
            }
        }

        public string NoteDirectory
        {
            get { return SettingsHelper.noteDirectory; }
            set
            {
                noteDirectory = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public SettingsViewModel()
        {
            // Load all available themes and accents
            Themes = new ObservableCollection<string>(SettingsHelper.GetAllThemes());
            Accents = new ObservableCollection<string>(SettingsHelper.GetAllAccents());
        }

        #endregion
    }
}
