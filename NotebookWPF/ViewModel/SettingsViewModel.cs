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

            // Load Application settings
            noteDirectory = SettingsHelper.noteDirectory;

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
        }

        #endregion
    }
}
