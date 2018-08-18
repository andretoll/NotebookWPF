using NotebookWPF.Commands;
using NotebookWPF.Helpers;
using NotebookWPF.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotebookWPF.ViewModel
{
    public class NotebookViewModel : BaseViewModel
    {
        #region Private Members

        private DatabaseEngine dbEngine;

        private ObservableCollection<Notebook> notebooks;

        private Notebook newNotebook;

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

        public Notebook NewNotebook
        {
            get { return newNotebook; }
            set
            {
                newNotebook = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private ICommand addNotebookCommand;
        public ICommand AddNotebookCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (addNotebookCommand == null)
                    addNotebookCommand = new RelayCommand(p => { AddNotebook(); }, p => true);
                return addNotebookCommand;
            }
            set
            {
                addNotebookCommand = value;
            }
        }

        private ICommand cancelNotebookCommand;
        public ICommand CancelNotebookCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (cancelNotebookCommand == null)
                    cancelNotebookCommand = new RelayCommand(p => { NewNotebook = null; }, p => true);
                return cancelNotebookCommand;
            }
            set
            {
                cancelNotebookCommand = value;
            }
        }

        #endregion

        #region Constructor

        public NotebookViewModel()
        {
            // Initiate properties
            dbEngine = new DatabaseEngine();
            Notebooks = new ObservableCollection<Notebook>();

            // Get notebooks
            GetNotebooks();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get all notebooks
        /// </summary>
        public void GetNotebooks()
        {
            // Get all notebooks
            var dbHelper = new DatabaseEngine();
            var notebooks = dbHelper.GetNotebooks();

            // Clear notebooks list
            Notebooks.Clear();

            // Add to notebook collection
            foreach (var item in notebooks)
            {
                Notebooks.Add(item);
            }
        }

        /// <summary>
        /// Add new notebook
        /// </summary>
        public void AddNotebook()
        {
            // Add new notebook to collection
            var newNotebookId = dbEngine.Insert(NewNotebook);
            NewNotebook.Id = newNotebookId;

            // If the new notebook returned an Id
            if (newNotebookId > 0)
            {
                // Place new notebook at top of list
                Notebooks.Insert(0, NewNotebook);
            }

            // Reset new notebook
            NewNotebook = null;

            // Trigger event
            NewNotebookAdded(NewNotebook, null);
        }

        #endregion

        #region Events

        public event EventHandler NewNotebookAdded;

        #endregion
    }
}
