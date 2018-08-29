﻿using MahApps.Metro.Controls;
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

            InitiateToolbarValues();
        }

        #endregion

        #region Events

        /// <summary>
        /// On clicking SettingsButton, open FlyOut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open or close SettingsFlyout
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        /// <summary>
        /// On clicking BackToNotebooksButton, hide Notes panel and show Notebooks panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToNotebooksButton_Click(object sender, RoutedEventArgs e)
        {
            NotebooksPanel.Visibility = Visibility.Visible;
            NotesPanel.Visibility = Visibility.Collapsed;

            NotebooksListBox.SelectedItem = null;
            NotesListBox.SelectedItem = null;
        }

        /// <summary>
        /// On clicking ShowHidePanelButton, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHidePanelButton_Checked(object sender, RoutedEventArgs e)
        {
            // Toggle show/hide panels
            if ((sender as ToggleButton).IsChecked ?? false)
            {
                ShowHideMenu("sbHideLeftMenu", MainPanel);
            }
            else ShowHideMenu("sbShowLeftMenu", MainPanel);

        }

        /// <summary>
        /// On selecting Notebook, close or open NotesPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// On selecting Note, focus TextEditor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListBox.SelectedItem != null)
            {                
                NoteTextEditor.Focus();

                UpdateToolbarValues();
            }
        }

        /// <summary>
        /// On selecting Favorite Note, focus TextEditor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoriteNotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteNotesListBox.SelectedItem != null)
            {
                NoteTextEditor.Focus();

                UpdateToolbarValues();
            }
        }           

        /// <summary>
        /// On checking HomeRadioButton, show all Notebooks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// On checking FavoriteRadioButton, show favorite Notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// On Note text editor selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoteTextEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateToolbarValues();
        }

        #region Text Editor Events

        /// <summary>
        /// On chaning Font Family
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fontFamily = (sender as ComboBox).SelectedItem;

            NoteTextEditor.Selection.ApplyPropertyValue(FontFamilyProperty, fontFamily);

            NoteTextEditor.Focus();
        }

        /// <summary>
        /// On changing font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fontSize = (sender as ComboBox).SelectedItem;

            NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, fontSize);

            NoteTextEditor.Focus();
        }

        /// <summary>
        /// On changing bold text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoldToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? true)
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontWeightProperty, FontWeights.Bold);
            }
            else
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontWeightProperty, FontWeights.Normal);
            }

            NoteTextEditor.Focus();
        }

        /// <summary>
        /// On chaning italic text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItalicToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? true)
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontStyleProperty, FontStyles.Italic);
            }
            else
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontStyleProperty, FontStyles.Normal);
            }

            NoteTextEditor.Focus();
        }

        #endregion

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

        /// <summary>
        /// Initiate Text editor toolbar values
        /// </summary>
        private void InitiateToolbarValues()
        {
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

            FontSizeComboBox.ItemsSource = fontSizes;
        }

        /// <summary>
        /// Update Text editor toolbar values
        /// </summary>
        private void UpdateToolbarValues()
        {
            try
            {
                // Set FontFamily
                var fontFamily = NoteTextEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
                FontFamilyComboBox.SelectedItem = fontFamily;

                // Set FontSize
                var fontSize = NoteTextEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
                foreach (var item in FontSizeComboBox.Items)
                {
                    if ((double)item == (double)fontSize)
                    {
                        FontSizeComboBox.SelectedItem = item;
                        break;
                    }

                    FontSizeComboBox.SelectedIndex = 15;
                }

                // Set Bold
                var isBold = NoteTextEditor.Selection.GetPropertyValue(FontWeightProperty);
                if ((FontWeight)isBold == FontWeights.Bold)
                {
                    BoldToggleButton.IsChecked = true;
                }
                else BoldToggleButton.IsChecked = false;

                // Set Italic
                var isItalic = NoteTextEditor.Selection.GetPropertyValue(FontStyleProperty);
                if ((FontStyle)isItalic == FontStyles.Italic)
                {
                    ItalicToggleButton.IsChecked = true;
                }
                else ItalicToggleButton.IsChecked = false;
            }
            catch { }
        }

        #endregion        
    }
}
