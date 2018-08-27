using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NotebookWPF.Helpers
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static FileStream GetFileStream(DependencyObject obj)
        {
            return (FileStream)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, FileStream value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly DependencyProperty DocumentXamlProperty =
          DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(FileStream),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj, e) =>
                {
                    var richTextBox = (RichTextBox)obj;

                    // Parse the XAML to a document (or use XamlReader.Parse())
                    var fileStream = GetFileStream(richTextBox);
                    var doc = new FlowDocument();
                    var range = new TextRange(doc.ContentStart, doc.ContentEnd);

                    try
                    {
                        range.Load(fileStream, DataFormats.Rtf);

                        // Set the document
                        richTextBox.Document = doc;
                    }
                    catch
                    {
                    }                    

                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        SetDocumentXaml(richTextBox, fileStream);
                    };
                }
            });

        private static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
