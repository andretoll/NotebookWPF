using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NotebookWPF.Model
{
    public class RichTextObject
    {
        public FileStream FileStream { get; set; }

        public TextRange TextRange { get; set; }
    }
}
