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

        private Notebook selectedNotebook;

        private Note selectedNote;

        private bool notebookIsEditing;

        private bool noteIsEditing;

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

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                NotifyPropertyChanged();

                // Get notes
                if (selectedNotebook != null)
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

        public bool NoteIsEditing
        {
            get { return noteIsEditing; }
            set
            {
                noteIsEditing = value;
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

        private ICommand newNoteCommand;
        public ICommand NewNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (newNoteCommand == null)
                    newNoteCommand = new RelayCommand(async p => { await AddNoteAsync(); }, p => true);
                return newNoteCommand;
            }
            set
            {
                newNoteCommand = value;
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

        private ICommand deleteNoteCommand;
        public ICommand DeleteNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (deleteNoteCommand == null)
                    deleteNoteCommand = new RelayCommand(p => { DeleteNote(p); }, p => true);
                return deleteNoteCommand;
            }
            set
            {
                deleteNoteCommand = value;
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

        private ICommand stopNoteEditingCommand;
        public ICommand StopNoteEditingCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (stopNoteEditingCommand == null)
                    stopNoteEditingCommand = new RelayCommand(p => { RenameNote(p); }, p => true);
                return stopNoteEditingCommand;
            }
            set
            {
                stopNoteEditingCommand = value;
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
                AffirmativeButtonText = "Save",
                AnimateHide = false
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
        public async Task AddNoteAsync()
        {
            // If no notebook is selected, return
            if (SelectedNotebook == null)
                return;

            // Open dialog
            var result = await dialogCoordinator.ShowInputAsync(this, "New Note", "Enter a name for your new Note.", new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AffirmativeButtonText = "Save",
                AnimateHide = false
            });

            // If input was cancelled, return
            if (result == null)
                return;

            // If something was entered
            if (result.Count() > 0)
            {
                // Create new Note
                Note newNote = new Note()
                {
                    Title = result,
                    NotebookId = SelectedNotebook.Id,
                    Created = DateTime.Now,
                    Updated = DateTime.Now                    
                };

                // TODO: Create file and save file location

                // Insert Note to database
                dbEngine.Insert(newNote);

                // Insert note into list
                Notes.Insert(0, newNote);
                SelectedNotebook.NoteCount++;
            }
        }

        /// <summary>
        /// Delete existing notebook
        /// </summary>
        /// <param name="notebookId"></param>
        public void DeleteNotebook(object notebookId)
        {
            int id;
            if (!int.TryParse(notebookId.ToString(), out id))
                return;

            // Delete notebook from list
            Notebook notebookToRemove = Notebooks.Where(n => n.Id == id).FirstOrDefault();
            if (notebookToRemove != null)
                Notebooks.Remove(notebookToRemove);

            // Delete notes, if any
            dbEngine.DeleteNotes(id);

            // Delete notebook from database
            dbEngine.Delete(notebookToRemove);
        }

        /// <summary>
        /// Delete existing note
        /// </summary>
        /// <param name="noteId"></param>
        public void DeleteNote(object noteId)
        {
            // If no notebook is selected, return
            if (SelectedNotebook == null)
                return;

            // Delete note from list
            Note noteToRemove = Notes.Where(n => n.Id == (int)noteId).FirstOrDefault();
            if (noteToRemove != null)
            {
                Notes.Remove(noteToRemove);
                SelectedNotebook.NoteCount--;
            }

            // Delete notebook from database
            dbEngine.Delete(noteToRemove);

            // TODO: Delete file from computer
        }

        /// <summary>
        /// Rename existing notebook
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

        /// <summary>
        /// Rename existing note
        /// </summary>
        /// <param name="note"></param>
        public void RenameNote(object note)
        {
            if ((note as Note).Title.Count() > 0)
            {
                // Update database
                dbEngine.Update((note as Note));
            }
            else
            {
                string oldTitle = dbEngine.GetNoteName((note as Note).Id);
                Notes.Where(n => n.Id == (note as Note).Id).FirstOrDefault().Title = oldTitle;
            }
        }

        #endregion
    }
}
