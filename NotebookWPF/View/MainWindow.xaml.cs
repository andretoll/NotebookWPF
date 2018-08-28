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
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

namespace NotebookWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Private Members

        private NotebookViewModel _notebookViewModel;

        #endregion

        #region Constructor

        public MainWindow()
        {
            // Load app settings
            SettingsHelper.LoadSettings();

            InitializeComponent();

            // Initiate NotebookViewModel and pass in an instance of DialogCoordinator (for metro dialogs)
            _notebookViewModel = new NotebookViewModel(DialogCoordinator.Instance);

            // Set DataContext to ViewModel
            this.DataContext = _notebookViewModel;

            HomeRadioButton.IsChecked = true;
        }

        #endregion

        #region Events

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open or close SettingsFlyout
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void NotebooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count > 0)
            {
                // Check settings for side-by-side panels
                if (!SettingsHelper.sideBySidePanels)
                    NotebooksPanel.Visibility = Visibility.Collapsed;

                NotesPanel.Visibility = Visibility.Visible;
            }
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoteTextEditor.Focus();
        }

        private void FavoriteNotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoteTextEditor.Focus();
        }        

        private void BackToNotebooksButton_Click(object sender, RoutedEventArgs e)
        {
            NotebooksPanel.Visibility = Visibility.Visible;
            NotesPanel.Visibility = Visibility.Collapsed;

            NotebooksListBox.SelectedItem = null;
            NotesListBox.SelectedItem = null;
        }

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent right-click selection
            e.Handled = true;
        }

        private void ShowHidePanelButton_Checked(object sender, RoutedEventArgs e)
        {
            // Toggle show/hide panels
            if ((sender as ToggleButton).IsChecked ?? false)
            {
                ShowHideMenu("sbHideLeftMenu", MainPanel);
            }
            else ShowHideMenu("sbShowLeftMenu", MainPanel);

        }

        private void HomeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked ?? true)
            {
                HomePanel.Visibility = Visibility.Visible;
                ShowHidePanelButton.IsChecked = false;
            }
            else
            {
                HomePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void FavoritesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked ?? true)
            {
                NotebooksListBox.SelectedItem = null;

                if (SettingsHelper.sideBySidePanels)
                    NotesPanel.Visibility = Visibility.Collapsed;

                FavoritePanel.Visibility = Visibility.Visible;
                ShowHidePanelButton.IsChecked = false;

                // If selected note from NotesListBox can be found in FavoriteNotesListBox, select it
                if (NotesListBox.SelectedItem != null)
                {
                    var sourceNote = NotesListBox.SelectedItem as Note;
                    int targetIndex = -1;

                    foreach (var item in FavoriteNotesListBox.Items)
                    {
                        if ((item as Note).Id == sourceNote.Id)
                        {
                            FavoriteNotesListBox.SelectedItem = item;
                            targetIndex = FavoriteNotesListBox.Items.IndexOf(item);
                            FavoriteNotesListBox.SelectedIndex = targetIndex;
                            return;
                        }
                    }
                }

                // Deselect any selected item from NotesListBox
                NotesListBox.SelectedItem = null;
            }
            else
            {
                FavoritePanel.Visibility = Visibility.Collapsed;
                FavoriteNotesListBox.SelectedItem = null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Show or hide side menu
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="pnl"></param>
        private void ShowHideMenu(string Storyboard, DockPanel pnl)
        {
            var sb = this.FindResource(Storyboard);

            // If Storyboard was found, play it
            if (sb != null)
                ((Storyboard)sb).Begin(pnl);
        }

        #endregion        
    }
}
