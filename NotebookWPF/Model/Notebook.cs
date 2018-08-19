using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.Model
{
    public class Notebook : INotifyPropertyChanged
    {
        #region Private Members

        private string name;

        #endregion

        #region Properties

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        [Ignore]
        public int NoteCount { get; set; }

        #endregion     

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
