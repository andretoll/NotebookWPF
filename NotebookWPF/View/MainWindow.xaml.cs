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

        private void NotebooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count > 0)
            {
                // Check settings for side-by-side panels
                if (!SettingsHelper.sideBySidePanels)
                    NotebooksPanel.Visibility = Visibility.Collapsed;

                NotesPanel.Visibility = Visibility.Visible;
            }
            else NotesPanel.Visibility = Visibility.Collapsed;

            // Close favorite notes panel if open
            FavoritePanel.Visibility = Visibility.Collapsed;
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count > 0)
            {
                // Check settings for autohide panel
                if (SettingsHelper.autohidePanels)
                {
                    ShowHideMenu("sbHideLeftMenu", pnlLeftMenu);
                    btnLeftMenuShowHide.IsChecked = true;
                }
            }
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

        private void btnLeftMenuShowHide_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Primitives.ToggleButton).IsChecked ?? false)
            {
                ShowHideMenu("sbHideLeftMenu", pnlLeftMenu);
            }
            else ShowHideMenu("sbShowLeftMenu", pnlLeftMenu);

        }

        private void FavoriteNotes_Click(object sender, RoutedEventArgs e)
        {
            NotebooksListBox.SelectedItem = null;

            if (FavoritePanel.Visibility == Visibility.Visible)
            {
                FavoritePanel.Visibility = Visibility.Collapsed;
                NotebooksPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // Check settings for side-by-side panels
                if (!SettingsHelper.sideBySidePanels)
                    NotebooksPanel.Visibility = Visibility.Collapsed;

                FavoritePanel.Visibility = Visibility.Visible;
            }
        }

        private void CloseFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            FavoritePanel.Visibility = Visibility.Collapsed;

            NotebooksPanel.Visibility = Visibility.Visible;
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
            //Storyboard sb = Resources[Storyboard] as Storyboard;
            var sb = this.FindResource(Storyboard);

            // If Storyboard was found, play it
            if (sb != null)
                ((Storyboard)sb).Begin(pnl);
        }


        #endregion
    }
}
