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
            // Load all available themes and accents
            Themes = new ObservableCollection<string>(SettingsHelper.GetAllThemes());
            Accents = new ObservableCollection<string>(SettingsHelper.GetAllAccents());
            FontFamilies = System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
            FontSizes = GetFontSizes();

            // Load Application settings
            noteDirectory = SettingsHelper.noteDirectory;
            selectedFontFamily = FontFamilies.Where(f => f.Source == SettingsHelper.fontFamily).FirstOrDefault();
            selectedFontSize = double.Parse(SettingsHelper.fontSize);

            // Initiate dialogcoordinator
            dialogCoordinator = instance;
        }

        #endregion

        #region Helper Methods

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

        private List<double> GetFontSizes()
        {
            // Font Sizes
            List<double> fontSizesToReturn = new List<double>();
            fontSizesToReturn.Add(3);
            fontSizesToReturn.Add(4);
            fontSizesToReturn.Add(5);
            fontSizesToReturn.Add(6);
            fontSizesToReturn.Add(6.5);
            fontSizesToReturn.Add(7);
            fontSizesToReturn.Add(7.5);
            fontSizesToReturn.Add(8);
            fontSizesToReturn.Add(8.5);
            fontSizesToReturn.Add(9);
            fontSizesToReturn.Add(9.5);
            fontSizesToReturn.Add(10);
            fontSizesToReturn.Add(10.5);
            fontSizesToReturn.Add(11.5);
            fontSizesToReturn.Add(12);
            fontSizesToReturn.Add(12.5);
            fontSizesToReturn.Add(13.5);
            fontSizesToReturn.Add(14);
            fontSizesToReturn.Add(15);
            fontSizesToReturn.Add(16);
            fontSizesToReturn.Add(17);
            fontSizesToReturn.Add(18);
            fontSizesToReturn.Add(19);
            fontSizesToReturn.Add(20);
            fontSizesToReturn.Add(22);
            fontSizesToReturn.Add(24);
            fontSizesToReturn.Add(26);
            fontSizesToReturn.Add(28);
            fontSizesToReturn.Add(30);
            fontSizesToReturn.Add(32);
            fontSizesToReturn.Add(34);
            fontSizesToReturn.Add(36);
            fontSizesToReturn.Add(38);
            fontSizesToReturn.Add(40);
            fontSizesToReturn.Add(44);
            fontSizesToReturn.Add(48);
            fontSizesToReturn.Add(52);
            fontSizesToReturn.Add(56);
            fontSizesToReturn.Add(60);
            fontSizesToReturn.Add(64);
            fontSizesToReturn.Add(68);
            fontSizesToReturn.Add(72);
            fontSizesToReturn.Add(76);
            fontSizesToReturn.Add(80);
            fontSizesToReturn.Add(88);
            fontSizesToReturn.Add(96);
            fontSizesToReturn.Add(104);
            fontSizesToReturn.Add(112);
            fontSizesToReturn.Add(120);
            fontSizesToReturn.Add(128);
            fontSizesToReturn.Add(136);
            fontSizesToReturn.Add(144);

            return fontSizesToReturn;
        }

        #endregion
    }
}
