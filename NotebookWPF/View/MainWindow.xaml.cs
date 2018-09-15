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

            // Set home as default view
            HomeRadioButton.IsChecked = true;

            // Initiate text editor fonts
            NoteTextEditor.FontFamily = new FontFamily(SettingsHelper.fontFamily);
            NoteTextEditor.FontSize = double.Parse(SettingsHelper.fontSize);

            // Initiate toolbar values
            InitiateToolbarValues();            
        }

        #endregion

        #region Events

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open or close SettingsFlyout
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }
        
        private void BackToNotebooksButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset selected Notebook and Note
            NotebooksListBox.SelectedItem = null;
            NotesListBox.SelectedItem = null;
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
                // Show home
                HomePanel.Visibility = Visibility.Visible;
                ShowHidePanelButton.IsChecked = false;
            }
            else
            {
                // Hide home
                HomePanel.Visibility = Visibility.Collapsed;
                NotebooksListBox.SelectedItem = null;
            }
        }

        private void FavoritesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked ?? true)
            {
                // Show favorites
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
                // Hide favorites
                FavoritePanel.Visibility = Visibility.Collapsed;
                FavoriteNotesListBox.SelectedItem = null;
            }
        }








        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesListBox.SelectedItem != null)
            {
                // If empty note, apply font settings
                TextRange textRange = new TextRange(NoteTextEditor.Document.ContentStart, NoteTextEditor.Document.ContentEnd);
                if (!textRange.Text.Any())
                {
                    NoteTextEditor.FontFamily = new FontFamily(SettingsHelper.fontFamily);
                    NoteTextEditor.FontSize = double.Parse(SettingsHelper.fontSize);
                }

                NoteTextEditor.Focus();

                UpdateToolbarValues();
            }        
        }
                
        private void FavoriteNotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoriteNotesListBox.SelectedItem != null)
            {
                NoteTextEditor.Focus();

                UpdateToolbarValues();
            }
        }     
        
        



















        
        private void NoteTextEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {      
            UpdateToolbarValues();
        }
        
        private void NoteTextEditor_TextChanged(object sender, TextChangedEventArgs e)
        { 
            UpdateToolbarValues();
        }
        
        private void NoteTextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers == ModifierKeys.Control) && _notebookViewModel.noteContentChanged)
            {
                _notebookViewModel.SaveNoteContentAsync();
            }
        }

        private void NoteTextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If enter key is pressed
            if (e.Key == Key.Enter)
            {
                // Create new line
                var newPointer = NoteTextEditor.Selection.Start.InsertLineBreak();
                NoteTextEditor.Selection.Select(newPointer, newPointer);
                UpdateToolbarValues();
                e.Handled = true;
            }
        }




        
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontFamilyProperty, (sender as ComboBox).SelectedItem);
            }

            NoteTextEditor.Focus();
        }
        
        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null)
            {
                NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, (sender as ComboBox).SelectedItem);
            }

            NoteTextEditor.Focus();
        }
        
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
        
        private void UnderlineToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? true)
            {
                NoteTextEditor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
            else
            {
                TextDecorationCollection textDecorations;
                (NoteTextEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                NoteTextEditor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }

            NoteTextEditor.Focus();
        }
        
        private void AlignRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            NoteTextEditor.Focus();
            if (LeftAlignRadioButton.IsChecked ?? true)
                NoteTextEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            else if (CenterAlignRadioButton.IsChecked ?? true)
                NoteTextEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            else if (RightAlignRadioButton.IsChecked ?? true)
                NoteTextEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }
        
        private void IncreaseFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current font size
                double currentFontSize;
                if (double.TryParse(NoteTextEditor.Selection.GetPropertyValue(FontSizeProperty).ToString(), out currentFontSize))
                {
                    // Get font sizes
                    var fontSizes = SettingsHelper.GetFontSizes();

                    // Get next size
                    int newFontSizeIndex = fontSizes.IndexOf(currentFontSize) + 1;
                    var newFontSize = fontSizes[newFontSizeIndex];
                    NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, newFontSize);
                }
                else
                {
                    // If font sizes are mixed, get font size in the end of document and apply
                    TextRange textRange = new TextRange(NoteTextEditor.Selection.Start, NoteTextEditor.Selection.End);
                    var value = (textRange.End.Parent as Run).FontSize;
                    NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, value);
                }
                
                UpdateToolbarValues();
            }
            catch
            {
            }

            NoteTextEditor.Focus();
        }
        
        private void DecreaseFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current font size
                double currentFontSize;
                if (double.TryParse(NoteTextEditor.Selection.GetPropertyValue(FontSizeProperty).ToString(), out currentFontSize))
                {
                    // Get font sizes
                    var fontSizes = SettingsHelper.GetFontSizes();

                    // Get next size
                    int newFontSizeIndex = fontSizes.IndexOf(currentFontSize) - 1;
                    var newFontSize = fontSizes[newFontSizeIndex];
                    NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, newFontSize);
                }
                else
                {
                    // If font sizes are mixed, get font size in the end of document and apply
                    TextRange textRange = new TextRange(NoteTextEditor.Selection.Start, NoteTextEditor.Selection.End);
                    var value = (textRange.End.Parent as System.Windows.Documents.Run).FontSize;
                    NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, value);
                }
                
                UpdateToolbarValues();
            }
            catch
            {
            }

            NoteTextEditor.Focus();
        }
        
        private void BulletListButton_Click(object sender, RoutedEventArgs e)
        {
            List bulletList = new List();
            bulletList.MarkerStyle = TextMarkerStyle.Disc;

            ListItem listItem = new ListItem(new Paragraph(new Run("Listitem")));
            bulletList.ListItems.Add(listItem);

            NoteTextEditor.Document.Blocks.Add(bulletList);
            NoteTextEditor.CaretPosition = NoteTextEditor.CaretPosition.DocumentEnd;
            EditingCommands.MoveToLineStart.Execute(null, NoteTextEditor);
            EditingCommands.SelectToLineEnd.Execute(null, NoteTextEditor);

            NoteTextEditor.Focus();
        }















        

        #endregion

        #region Methods

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
        /// Check if selected text is underlined
        /// </summary>
        /// <param name="textSelection"></param>
        /// <returns></returns>
        private bool CheckUnderLinedText(TextSelection textSelection)
        {
            var value = GetPropertyValue(textSelection, Paragraph.TextDecorationsProperty);

            int propCount;

            // Determine if selected text is underlined
            if (int.TryParse((value as TextDecorationCollection).Count.ToString(), out propCount))
            {
                if (propCount > 0)
                    return true;
            }           

            return false;
        }     

        /// <summary>
        /// Initiate Text editor toolbar values
        /// </summary>
        private void InitiateToolbarValues()
        {
            // Font sizes
            FontSizeComboBox.ItemsSource = SettingsHelper.GetFontSizes();

            // Font families
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            // Text alignment
            LeftAlignRadioButton.IsChecked = true;
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
                FontSizeComboBox.Text = (NoteTextEditor.Selection.GetPropertyValue(Inline.FontSizeProperty)).ToString();

                // Set Bold
                var selectedWeight = NoteTextEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
                BoldToggleButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedWeight.Equals(FontWeights.Bold));

                // Set Italic
                var selectedStyle = NoteTextEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
                ItalicToggleButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

                // Set Underline
                UnderlineToggleButton.IsChecked = CheckUnderLinedText(NoteTextEditor.Selection);             

                // Set Alignment
                var selectedAlignment = NoteTextEditor.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
                switch (selectedAlignment)
                {
                    case TextAlignment.Left:
                        LeftAlignRadioButton.IsChecked = true;
                        break;
                    case TextAlignment.Center:
                        CenterAlignRadioButton.IsChecked = true;
                        break;
                    case TextAlignment.Right:
                        RightAlignRadioButton.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get formatting property value
        /// </summary>
        /// <param name="textRange"></param>
        /// <param name="formattingProperty"></param>
        /// <returns></returns>
        private Object GetPropertyValue(TextRange textRange, DependencyProperty formattingProperty)
        {
            Object value = null;

            var pointer = textRange.Start;

            if (pointer is TextPointer)
            {
                bool needsContinue = true;

                DependencyObject element = ((TextPointer)pointer).Parent as TextElement;

                while (needsContinue && (element is Inline || element is Paragraph || element is TextBlock))
                {
                    value = element.GetValue(formattingProperty);

                    System.Collections.IEnumerable seq = value as System.Collections.IEnumerable;

                    needsContinue = (seq == null) ? value == null : seq.Cast<Object>().Count() == 0;

                    element = element is TextElement ? ((TextElement)element).Parent : null;
                }
            }

            return value;
        }

        #endregion
    }
}
