using NotebookWPF.Helpers;
using NotebookWPF.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.ViewModel
{
    public class NotebookViewModel : BaseViewModel
    {
        #region Private Members

        private ObservableCollection<Notebook> notebooks;

        #endregion

        #region Properties

        public ObservableCollection<Notebook> Notebooks
        {
            get { return notebooks; }
            set
            {
                notebooks = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public NotebookViewModel()
        {
            // Initiate properties
            Notebooks = new ObservableCollection<Notebook>();

            // Get notebooks
            GetNotebooks();
        }

        #endregion

        #region Helper Methods

        public void GetNotebooks()
        {
            var dbHelper = new DatabaseEngine();

            var notebooks = dbHelper.GetNotebooks<Notebook>();

            Notebooks.Clear();

            foreach (var item in notebooks)
            {
                Notebooks.Add(item);
            }
        }

        #endregion
    }
}
