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
        private bool isFavorite;
        private DateTime updated;

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

        public DateTime Updated
        {
            get { return updated; }
            set
            {
                updated = value;
                NotifyPropertyChanged();
            }
        }

        public string FileLocation { get; set; }

        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                isFavorite = value;
                NotifyPropertyChanged();
            }
        }

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
