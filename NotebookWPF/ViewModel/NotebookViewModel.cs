using MahApps.Metro.Controls.Dialogs;
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

        private IDialogCoordinator dialogCoordinator;

        private ObservableCollection<Notebook> notebooks;

        private ObservableCollection<Note> notes;

        private Note newNote;

        private bool notebooksExists;

        private Notebook selectedNotebook;

        private Note selectedNote;

        private bool notebookIsEditing;

        #endregion

        #region Properties

        public ObservableCollection<Notebook> Notebooks
        {
            get { NotebookIsEditing = false; return notebooks; }
            set
            {
                notebooks = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                NotifyPropertyChanged();
            }
        }

        public Note NewNote
        {
            get { return newNote; }
            set
            {
                newNote = value;
                NotifyPropertyChanged();
            }
        }

        public bool NotebooksExists
        {
            get { return !Notebooks.Any(); }
            set
            {
                notebooksExists = value;
                NotifyPropertyChanged();
            }
        }

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                NotifyPropertyChanged();

                // Get notes
                GetNotes(selectedNotebook.Id);
            }
        }

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                NotifyPropertyChanged();
            }
        }

        public bool NotebookIsEditing
        {
            get { return notebookIsEditing; }
            set
            {
                notebookIsEditing = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private ICommand newNotebookCommand;
        public ICommand NewNotebookCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (newNotebookCommand == null)
                    newNotebookCommand = new RelayCommand(async p => { await AddNotebookAsync(); }, p => true);
                return newNotebookCommand;
            }
            set
            {
                newNotebookCommand = value;
            }
        }

        private ICommand addNoteCommand;
        public ICommand AddNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (addNoteCommand == null)
                    addNoteCommand = new RelayCommand(p => { AddNote(); }, p => true);
                return addNoteCommand;
            }
            set
            {
                addNoteCommand = value;
            }
        }

        private ICommand cancelNoteCommand;
        public ICommand CancelNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (cancelNoteCommand == null)
                    cancelNoteCommand = new RelayCommand(p => { NewNote = null; }, p => true);
                return cancelNoteCommand;
            }
            set
            {
                cancelNoteCommand = value;
            }
        }

        private ICommand deleteNotebookCommand;
        public ICommand DeleteNotebookCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (deleteNotebookCommand == null)
                    deleteNotebookCommand = new RelayCommand(p => { DeleteNotebook(p); }, p => true);
                return deleteNotebookCommand;
            }
            set
            {
                deleteNotebookCommand = value;
            }
        }

        private ICommand beginNotebookEditingCommand;
        public ICommand BeginNotebookEditingCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (beginNotebookEditingCommand == null)
                    beginNotebookEditingCommand = new RelayCommand(p => { NotebookIsEditing = true; }, p => true);
                return beginNotebookEditingCommand;
            }
            set
            {
                beginNotebookEditingCommand = value;
            }
        }

        private ICommand stopNotebookEditingCommand;
        public ICommand StopNotebookEditingCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (stopNotebookEditingCommand == null)
                    stopNotebookEditingCommand = new RelayCommand(p => { RenameNotebook(p); }, p => true);
                return stopNotebookEditingCommand;
            }
            set
            {
                stopNotebookEditingCommand = value;
            }
        }

        #endregion

        #region Constructor

        public NotebookViewModel(IDialogCoordinator instance)
        {
            // Initiate properties
            dbEngine = new DatabaseEngine();
            Notebooks = new ObservableCollection<Notebook>();
            dialogCoordinator = instance;

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
            var notebooksFromDb = dbEngine.GetNotebooks();

            // Clear notebooks list
            Notebooks.Clear();

            // Add to notebook collection
            foreach (var item in notebooksFromDb)
            {
                Notebooks.Add(item);
            }
        }

        /// <summary>
        /// Get all notes for a specific notebook
        /// </summary>
        public void GetNotes(int id)
        {
            // Get all notes
            var notesFromDb = dbEngine.GetNotes(id);

            // Clear notes list
            Notes = new ObservableCollection<Note>();

            // Add to notes collection
            foreach (var item in notesFromDb)
            {
                Notes.Add(item);
            }
        }

        /// <summary>
        /// Add new notebook
        /// </summary>
        public async Task AddNotebookAsync()
        {
            // Open dialog
            var result = await dialogCoordinator.ShowInputAsync(this, "New Notebook", "Enter a name for your new Notebook.", new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AffirmativeButtonText = "Save"
            });

            // If input was cancelled, return
            if (result == null)
                return;

            // If something was entered
            if (result.Count() > 0)
            {
                // Create new Notebook
                Notebook newNotebook = new Notebook()
                {
                    Name = result
                };

                // Insert Notebook to database
                dbEngine.Insert(newNotebook);

                Notebooks.Insert(0, newNotebook);
            }
        }

        /// <summary>
        /// Add new note
        /// </summary>
        public void AddNote()
        {

        }

        /// <summary>
        /// Delete existing notebook
        /// </summary>
        /// <param name="notebookId"></param>
        public void DeleteNotebook(object notebookId)
        {
            // Delete notebook from list
            Notebook notebookToRemove = Notebooks.Where(n => n.Id == (int)notebookId).FirstOrDefault();
            if (notebookToRemove != null)
                Notebooks.Remove(notebookToRemove);

            // Delete notebook from database
            dbEngine.Delete(notebookToRemove);
        }

        /// <summary>
        /// Rename an existing notebook
        /// </summary>
        /// <param name="newName"></param>
        public void RenameNotebook(object notebook)
        {
            if ((notebook as Notebook).Name.Count() > 0)
            {
                // Update database
                dbEngine.Update((notebook as Notebook));
            }
            else
            {
                string oldName = dbEngine.GetNotebookName((notebook as Notebook).Id);
                Notebooks.Where(n => n.Id == (notebook as Notebook).Id).FirstOrDefault().Name = oldName;
            }
        }

        #endregion
    }
}
