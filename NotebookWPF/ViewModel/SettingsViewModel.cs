using MahApps.Metro.Controls.Dialogs;
using NotebookWPF.Commands;
using NotebookWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace NotebookWPF.ViewModel
{
    /// <summary>
    /// A ViewModel for all settings
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        private IDialogCoordinator dialogCoordinator;

        private string selectedAccent;

        private string selectedTheme;

        private string noteDirectory;

        private FontFamily selectedFontFamily;

        private double selectedFontSize;

        #endregion

        #region Public Members

        // Collection of available themes
        public ObservableCollection<string> Themes { get; set; }

        // Collection of available accents
        public ObservableCollection<string> Accents { get; set; }

        // Collection of available font families
        public List<System.Windows.Media.FontFamily> FontFamilies { get; set; }

        // Collection of available font sizes
        public List<double> FontSizes { get; set; }

        #endregion

        #region Properties

        public string SelectedTheme
        {
            get { return SettingsHelper.GetCurrentTheme(); }
            set
            {
                selectedTheme = value;
                NotifyPropertyChanged();

                // Save changes
                SettingsHelper.SaveSettings("theme", SettingsHelper.SettingsProperties.color, value);
            }
        }

        public string SelectedAccent
        {
            get { return SettingsHelper.GetCurrentAccent(); }
            set
            {
                selectedAccent = value;
                NotifyPropertyChanged();

                // Save changes
                SettingsHelper.SaveSettings("accent", SettingsHelper.SettingsProperties.color, value);
            }
        }

        public string NoteDirectory
        {
            get { return noteDirectory; }
            set
            {
                noteDirectory = value;
                NotifyPropertyChanged();

                // Save Changes
                SettingsHelper.SaveSettings(nameof(noteDirectory), SettingsHelper.SettingsProperties.path, value);
            }
        }

        public FontFamily SelectedFontFamily
        {
            get { return selectedFontFamily; }
            set
            {
                selectedFontFamily = value;
                NotifyPropertyChanged();

                // Save Changes
                SettingsHelper.SaveSettings("fontFamily", SettingsHelper.SettingsProperties.value, (value as FontFamily).Source);
            }
        }

        public double SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                selectedFontSize = double.Parse(value.ToString());
                NotifyPropertyChanged();

                // Save Changes
                SettingsHelper.SaveSettings("fontSize", SettingsHelper.SettingsProperties.value, value.ToString());
            }
        }

        #endregion

        #region Commands

        private ICommand defaultSettingsCommand;
        public ICommand DefaultSettingsCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (defaultSettingsCommand == null)
                    defaultSettingsCommand = new RelayCommand(async p => { await ApplyDefaultSettingsAsync(); }, p => true);
                return defaultSettingsCommand;
            }
            set
            {
                defaultSettingsCommand = value;
            }
        }

        #endregion

        #region Constructor

        public SettingsViewModel(IDialogCoordinator instance)
        {
            // Load all themes, accents, font families and font sizes
            Themes = new ObservableCollection<string>(SettingsHelper.GetAllThemes());
            Accents = new ObservableCollection<string>(SettingsHelper.GetAllAccents());
            FontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
            FontSizes = SettingsHelper.GetFontSizes();

            // Load Application settings
            noteDirectory = SettingsHelper.noteDirectory;
            selectedFontFamily = FontFamilies.Where(f => f.Source == SettingsHelper.fontFamily).FirstOrDefault();
            selectedFontSize = double.Parse(SettingsHelper.fontSize);

            // Initiate dialogcoordinator
            dialogCoordinator = instance;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply default settings
        /// </summary>
        /// <returns></returns>
        private async Task ApplyDefaultSettingsAsync()
        {
            var result = await dialogCoordinator.ShowMessageAsync(this, "Revert to default settings?", "Are you sure you want to revert to default settings?\n\nThis action is irreversible.", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
                SettingsHelper.RepairSettings();
            else return;

            // Get new settings
            SelectedTheme = SettingsHelper.GetCurrentTheme();
            SelectedAccent = SettingsHelper.GetCurrentAccent();
            NoteDirectory = SettingsHelper.noteDirectory;
            SelectedFontSize = double.Parse(SettingsHelper.fontSize);
        }        

        #endregion
    }
}
