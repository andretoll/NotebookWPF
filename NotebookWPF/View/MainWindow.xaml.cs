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
using System.Reflection;
using System.Diagnostics;

namespace NotebookWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Private Members

        private NotebookViewModel _notebookViewModel;
        private PropertyInfo[] _colors;
        private List<double> _fontSizes;

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

            // Get font sizes
            _fontSizes = SettingsHelper.GetFontSizes();

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

        private void GoToNotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(SettingsHelper.noteDirectory);
            }
            catch { }
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


        
        private void NoteTextEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {      
            UpdateToolbarValues();
        }
        
        private void NoteTextEditor_TextChanged(object sender, TextChangedEventArgs e)
        { 
            UpdateToolbarValues();
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
        
        private void IncreaseFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {        
                // Get current font size
                double currentFontSize;
                bool b = double.TryParse(NoteTextEditor.Selection.GetPropertyValue(FontSizeProperty).ToString(), out currentFontSize);
                if (!b)
                {
                    // If font sizes are mixed, get font size in the end of document and apply
                    TextRange textRange = new TextRange(NoteTextEditor.Selection.Start, NoteTextEditor.Selection.End);
                    currentFontSize = (textRange.End.Parent as Run).FontSize;
                }

                // Get new font size
                double newFontSize = GetNextFontSize(currentFontSize);                

                // Apply new font size
                NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, newFontSize);                
            }
            catch
            {
            }

            UpdateToolbarValues();
        }
        
        private void DecreaseFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current font size
                double currentFontSize;
                bool b = double.TryParse(NoteTextEditor.Selection.GetPropertyValue(FontSizeProperty).ToString(), out currentFontSize);
                if (!b)
                {
                    // If font sizes are mixed, get font size in the end of document and apply
                    TextRange textRange = new TextRange(NoteTextEditor.Selection.Start, NoteTextEditor.Selection.End);
                    currentFontSize = (textRange.End.Parent as Run).FontSize;
                }

                // Get new font size
                double newFontSize = GetPreviousFontSize(currentFontSize);

                // Apply new font size
                NoteTextEditor.Selection.ApplyPropertyValue(FontSizeProperty, newFontSize);
            }
            catch
            {
            }

            UpdateToolbarValues();
        }

        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var selectedColorName = (sender as ComboBox).SelectedItem;
                NoteTextEditor.Selection.ApplyPropertyValue(ForegroundProperty, selectedColorName);
            }

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
        /// Initiate Text editor toolbar values
        /// </summary>
        private void InitiateToolbarValues()
        {
            // Font sizes
            FontSizeComboBox.ItemsSource = SettingsHelper.GetFontSizes();

            // Font families
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            // Font Colors
            Type brushesType = typeof(Brushes);
            _colors = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            List<string> colorList = new List<string>();
            foreach (var item in _colors)
            {
                string name = item.Name;
                colorList.Add(name);
            }
            FontColorComboBox.ItemsSource = colorList;

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
                double fontSize = 0;
                bool b = double.TryParse(NoteTextEditor.Selection.GetPropertyValue(FontSizeProperty).ToString(), out fontSize);
                if (!b)
                {
                    // If font sizes are mixed, get font size in the end of document and apply
                    TextRange textRange = new TextRange(NoteTextEditor.Selection.Start, NoteTextEditor.Selection.End);
                    var fs = (textRange.Start.Parent as Run).FontSize;
                    fontSize = fs;
                }
                // Resolve font size
                fontSize = ResolveFontSize(fontSize);                    
                FontSizeComboBox.SelectedItem = fontSize;

                // Set FontColor
                var selectedColor = NoteTextEditor.Selection.GetPropertyValue(ForegroundProperty);

                if (selectedColor.GetType() != typeof(SolidColorBrush))
                    FontColorComboBox.SelectedItem = null;
                else
                {
                    string selectedColorName = "";
                    foreach (var item in _colors)
                    {
                        string name = item.Name;
                        SolidColorBrush brush = (SolidColorBrush)item.GetValue(null, null);

                        if (((SolidColorBrush)selectedColor).Color == brush.Color)
                        {
                            selectedColorName = name;
                            break;
                        }
                    }
                    FontColorComboBox.SelectedItem = selectedColorName;
                }                

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

                // List Type
                var listType = GetSelectionListType();
                switch (listType)
                {
                    case "bullets":
                        BulletListButton.IsChecked = true;
                        NumberedListButton.IsChecked = false;
                        break;
                    case "numbers":
                        BulletListButton.IsChecked = false;
                        NumberedListButton.IsChecked = true;
                        break;
                    default:
                        BulletListButton.IsChecked = false;
                        NumberedListButton.IsChecked = false;
                        break;
                }
            }
            catch { }
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
        /// Check if selected text is any type of list
        /// </summary>
        /// <returns></returns>
        private string GetSelectionListType()
        {
            List list = FindListAncestor(NoteTextEditor.Selection.Start.Parent);
            if (list != null)
            {
                if (list.MarkerStyle == TextMarkerStyle.Disc)
                {
                    return "bullets";
                }
                else if (list.MarkerStyle == TextMarkerStyle.Decimal)
                {
                    return "numbers";
                }
            }
            return "";
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

        /// <summary>
        /// Get list style
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private List FindListAncestor(DependencyObject element)
        {
            while (element != null)
            {
                List list = element as List;
                if (list != null)
                {
                    return list;
                }

                element = LogicalTreeHelper.GetParent(element);
            }

            return null;
        }

        /// <summary>
        /// Get closest font size
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private double ResolveFontSize(double fontSize)
        {
            double closest = _fontSizes.Aggregate((x, y) => Math.Abs(x - fontSize) < Math.Abs(y - fontSize) ? x : y);

            return closest;
        }

        /// <summary>
        /// Get next font size from list
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private double GetNextFontSize(double fontSize)
        {
            double closest = _fontSizes.Aggregate((x, y) => Math.Abs(x - fontSize) < Math.Abs(y - fontSize) ? x : y);

            // Get font size index
            int fontSizeIndex = _fontSizes.IndexOf(closest);

            // If last font size
            if (fontSizeIndex == _fontSizes.Count - 1)
                return _fontSizes[fontSizeIndex];
            else
                return _fontSizes[fontSizeIndex + 1];
        }

        /// <summary>
        /// Get previous font size from list
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private double GetPreviousFontSize(double fontSize)
        {
            double closest = _fontSizes.Aggregate((x, y) => Math.Abs(x - fontSize) < Math.Abs(y - fontSize) ? x : y);

            // Get font size index
            int fontSizeIndex = _fontSizes.IndexOf(closest);

            // If last font size
            if (fontSizeIndex == 0)
                return _fontSizes[fontSizeIndex];
            else
                return _fontSizes[fontSizeIndex - 1];
        }

        #endregion        
    }
}
