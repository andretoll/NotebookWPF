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
    public class Note : INotifyPropertyChanged
    {
        #region Private Members

        private string title;

        #endregion

        #region Properties

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int NotebookId { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string FileLocation { get; set; }

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
