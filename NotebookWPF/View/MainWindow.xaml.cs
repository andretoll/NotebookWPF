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
            // Load app settings
            SettingsHelper.LoadSettings();

            InitializeComponent();            

            // Initiate NotebookViewModel and pass in an instance of DialogCoordinator (for metro dialogs)
            notebookViewModel = new NotebookViewModel(DialogCoordinator.Instance);

            // Set DataContext to ViewModel
            this.DataContext = notebookViewModel;
        }        

        #endregion

        #region Events

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count > 0)
            {
                // TODO: Check settings for side-by-side panels
                if (!SettingsHelper.sideBySidePanels)
                    NotebooksPanel.Visibility = Visibility.Collapsed;

                NotesPanel.Visibility = Visibility.Visible;
            }
            else NotesPanel.Visibility = Visibility.Collapsed;
        }

        private void BackToNotebooksButton_Click(object sender, RoutedEventArgs e)
        {
            NotebooksPanel.Visibility = Visibility.Visible;
            NotesPanel.Visibility = Visibility.Collapsed;

            NotebooksListBox.SelectedItem = null;
        }

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
