using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NotebookWPF.Helpers;
using NotebookWPF.Model;
using NotebookWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotebookWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Private Members

        private NotebookViewModel notebookViewModel;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // Load app settings
            SettingsHelper.LoadSettings();

            // Set data context
            notebookViewModel = new NotebookViewModel();
            this.DataContext = notebookViewModel;

            // Subscribe to the NewNotebookAdded event
            notebookViewModel.NewNotebookAdded += NotebookViewModel_NewNotebookAdded;
        }
        

        #endregion

        #region Events

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void NewNotebookButton_Click(object sender, RoutedEventArgs e)
        {
            notebookViewModel.NewNotebook = new Notebook()
            {
                Name = "New Notebook"
            };

            NewNotebookTextBox.Focus();
            NewNotebookTextBox.SelectAll();
        }

        private void NotebookViewModel_NewNotebookAdded(object sender, EventArgs e)
        {
            NotebookListBox.SelectedIndex = 0;
        }

        #endregion
    }
}
