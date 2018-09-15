﻿using MahApps.Metro.Controls.Dialogs;
using NotebookWPF.Commands;
using NotebookWPF.Helpers;
using NotebookWPF.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace NotebookWPF.ViewModel
{
    /// <summary>
    /// Main Viewmodel for Notebook application
    /// </summary>
    public class NotebookViewModel : BaseViewModel
    {
        #region Private Members

        private IDatabaseEngine dbEngine;

        private IDialogCoordinator dialogCoordinator;

        private ObservableCollection<Notebook> notebooks;

        private ObservableCollection<Note> notes;

        private ObservableCollection<Note> favoriteNotes;

        private Notebook selectedNotebook;

        private Note selectedNote;

        private string noteContent;

        private string clientMessage;

        private bool notebookIsEditing;

        private bool noteIsEditing;

        private bool selectedNoteIsFavorite;

        #endregion

        #region Public Members

        public bool noteContentChanged;

        public bool clientMessageActive;

        #endregion

        #region Properties

        public ObservableCollection<Notebook> Notebooks
        {
            get
            {
                NotebookIsEditing = false;
                return notebooks;
            }
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

        public ObservableCollection<Note> FavoriteNotes
        {
            get { return favoriteNotes; }
            set
            {
                favoriteNotes = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Notebook> AvailableNotebooks
        {
            get
            {
                ObservableCollection<Notebook> notebooksAvailable = new ObservableCollection<Notebook>(Notebooks);
                notebooksAvailable.Remove(SelectedNotebook);
                return notebooksAvailable;
            }
            private set {}
        }

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                NotifyPropertyChanged();

                if (selectedNotebook != null)
                    GetNotes(selectedNotebook.Id);

                NoteIsEditing = false;
            }
        }

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                // If changes were made without saving
                if (noteContentChanged)
                {
                    SaveNoteContentAsync();
                }

                selectedNote = value;
                NotifyPropertyChanged();       

                // Empty content
                noteContent = null;

                // If a note is selected, get its content
                if (value != null)
                    GetNoteContent();                

                noteContentChanged = false;
            }
        }

        public bool NotebookIsEditing
        {
            get { return notebookIsEditing; }
            set
            {
                notebookIsEditing = value;
                NotifyPropertyChanged();

                // Add client message
                if (value)
                    SetClientMessage("Editing notebooks.");
            }
        }

        public bool NoteIsEditing
        {
            get { return noteIsEditing; }
            set
            {
                noteIsEditing = value;
                NotifyPropertyChanged();

                // Add client message
                if (value)
                    SetClientMessage("Editing notes.");
            }
        }        

        public bool SelectedNoteIsFavorite
        {
            get
            {
               try
                {
                    return selectedNote.IsFavorite;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                selectedNoteIsFavorite = value;
                NotifyPropertyChanged();

                ToggleNoteFavorite(value);
            }
        }

        public string NoteContent
        {
            get { return noteContent; }
            set
            {
                if (noteContent != value && noteContent != null && value != null)
                    noteContentChanged = true;

                noteContent = value;
                NotifyPropertyChanged();
            }
        }

        public string ClientMessage
        {
            get { return clientMessage; }
            set
            {
                clientMessage = value;
                NotifyPropertyChanged();
            }
        }

        public bool ClientMessageActive
        {
            get { return clientMessageActive; }
            set
            {
                clientMessageActive = value;
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

        private ICommand moveNoteCommand;
        public ICommand MoveNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (moveNoteCommand == null)
                    moveNoteCommand = new RelayCommand(p => { MoveNote(p); }, p => true);
                return moveNoteCommand;
            }
            set
            {
                moveNoteCommand = value;
            }
        }

        private ICommand copyNoteCommand;
        public ICommand CopyNoteCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (copyNoteCommand == null)
                    copyNoteCommand = new RelayCommand(async p => { await CopyNoteAsync(); }, p => true);
                return copyNoteCommand;
            }
            set
            {
                copyNoteCommand = value;
            }
        }

        private ICommand saveNoteContentCommand;
        public ICommand SaveNoteContentCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (saveNoteContentCommand == null)
                    saveNoteContentCommand = new RelayCommand(p => { SaveNoteContentAsync(); }, p => noteContentChanged);
                return saveNoteContentCommand;
            }
            set
            {
                saveNoteContentCommand = value;
            }
        }

        private ICommand discardChangesCommand;
        public ICommand DiscardChangesCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (discardChangesCommand == null)
                    discardChangesCommand = new RelayCommand(p => { DiscardChangesAsync(); }, p => noteContentChanged);
                return discardChangesCommand;
            }
            set
            {
                discardChangesCommand = value;
            }
        }

        private ICommand importFromFileCommand;
        public ICommand ImportFromFileCommand
        {
            get
            {
                // Create new RelayCommand and pass method to be executed and a boolean value whether or not to execute
                if (importFromFileCommand == null)
                    importFromFileCommand = new RelayCommand(async p => { await ImportFromFileAsync(); }, p => true);
                return importFromFileCommand;
            }
            set
            {
                importFromFileCommand = value;
            }
        }

        #endregion

        #region Constructor

        public NotebookViewModel(IDialogCoordinator instance)
        {
            // Initiate properties
            dbEngine = new DatabaseEngine();
            Notebooks = new ObservableCollection<Notebook>();
            FavoriteNotes = new ObservableCollection<Note>();
            dialogCoordinator = instance;

            // Get notebooks
            GetNotebooks();

            // Get favorite notes
            GetFavoriteNotes();
        }

        #endregion

        #region Methods

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
        /// Get all favorite notes
        /// </summary>
        public void GetFavoriteNotes()
        {
            var favoriteNotesFromDb = dbEngine.GetFavoriteNotes();

            FavoriteNotes = new ObservableCollection<Note>();

            foreach (var item in favoriteNotesFromDb)
            {
                FavoriteNotes.Add(item);
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

                // Add message
                SetClientMessage("Notebook added.");
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
            var result = await dialogCoordinator.ShowInputAsync(this, "New Note", "Enter a title for your new Note.", new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AffirmativeButtonText = "Save",
                AnimateHide = false
            });

            // If note title already exists, accept new input
            if (dbEngine.NoteTitleExists(result, 0))
            {
                while (dbEngine.NoteTitleExists(result, 0) && result != null)
                {
                    result = await dialogCoordinator.ShowInputAsync(this, "Title taken", "A Note with that title already exists! \n\nEnter a title for your new Note.", new MetroDialogSettings()
                    {
                        ColorScheme = MetroDialogColorScheme.Theme,
                        AffirmativeButtonText = "Save",
                        AnimateHide = false,
                        DefaultText = result
                    });
                }
            }

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

                // Create file and save file location
                string fileName = string.Concat(result, ".rtf");
                string filePath = Path.Combine(SettingsHelper.noteDirectory, fileName);
                try
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    // If adding fails, show error message and return
                    await dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                    return;
                }

                newNote.FileLocation = filePath;

                // Insert Note to database
                dbEngine.Insert(newNote);

                // Insert note into list
                Notes.Insert(0, newNote);
                SelectedNotebook.NoteCount++;

                // Add message
                SetClientMessage("Note added.");
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

            Notebook notebookToRemove = Notebooks.Where(n => n.Id == id).FirstOrDefault();

            if (SelectedNotebook != null)
            {
                if (SelectedNotebook.Id == notebookToRemove.Id)
                    SelectedNotebook = null;
            }

            // Delete notebook from list            
            if (notebookToRemove != null)
                Notebooks.Remove(notebookToRemove);

            // Return and Delete notes, if any
            var notesToDelete = dbEngine.DeleteNotes(id);

            // Delete notebook from database
            dbEngine.Delete(notebookToRemove);

            if (notesToDelete != null)
            {
                // Delete notes from device
                try
                {
                    foreach (var note in notesToDelete)
                    {
                        File.Delete(note.FileLocation);                        
                    }
                }
                catch
                {
                }
            }

            if (SelectedNote != null)
            {
                if (SelectedNote.NotebookId == notebookToRemove.Id)
                    SelectedNote = null;
            }            

            // Refresh Favorite Notes
            GetFavoriteNotes();

            // Add message
            SetClientMessage("Notebook deleted.");
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

            // Get note to remove
            Note noteToRemove = Notes.Where(n => n.Id == (int)noteId).FirstOrDefault();

            // Delete file from computer
            try
            {
                File.Delete(noteToRemove.FileLocation);
            }
            catch (Exception ex)
            {
                // If deletion fails, show error message and return
                dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                return;
            }

            // Delete note from list
            if (noteToRemove != null)
            {
                Notes.Remove(noteToRemove);
                SelectedNotebook.NoteCount--;
            }

            // Remove from favorites, if possible
            Note noteToRemoveFromFavorites = favoriteNotes.Where(f => f.Id == noteToRemove.Id).FirstOrDefault();
            if (noteToRemoveFromFavorites != null)
                FavoriteNotes.Remove(noteToRemoveFromFavorites);

            // Delete notebook from database
            dbEngine.Delete(noteToRemove);

            // Add message
            SetClientMessage("Note deleted.");
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
            // If the Note has any characters
            if ((note as Note).Title.Count() > 0)
            {
                // If note title already exists
                if (dbEngine.NoteTitleExists((note as Note).Title, (note as Note).Id))
                {
                    string oldTitle = dbEngine.GetNoteName((note as Note).Id);
                    Notes.Where(n => n.Id == (note as Note).Id).FirstOrDefault().Title = oldTitle;

                    dialogCoordinator.ShowMessageAsync(this, "Title taken", "A Note with that title already exists!");
                }

                // Update file name
                try
                {
                    string newFileName = (note as Note).Title;
                    string oldFileLocation = (note as Note).FileLocation;
                    string newFileLocation = Path.Combine(Path.GetDirectoryName(oldFileLocation), newFileName + ".rtf");
                    File.Move(oldFileLocation, newFileLocation);

                    (note as Note).FileLocation = newFileLocation;
                }
                catch (Exception ex)
                {
                    dialogCoordinator.ShowMessageAsync(this, "Could not rename Note.", ex.Message);

                    return;
                }

                // Update database
                dbEngine.Update((note as Note));

                // Display message
                SetClientMessage("Note renamed.");
            }
            else
            {
                string oldTitle = dbEngine.GetNoteName((note as Note).Id);
                Notes.Where(n => n.Id == (note as Note).Id).FirstOrDefault().Title = oldTitle;
            }
        }

        /// <summary>
        /// Move Note to another Notebook
        /// </summary>
        /// <param name="note"></param>
        public void MoveNote(object notebookId)
        {
            int id;

            // If Id is not an integer, return
            if (!int.TryParse(notebookId.ToString(), out id))
                return;

            // Get Note object
            Note noteToMove = SelectedNote;

            // Get Notebook
            Notebook targetNotebook = Notebooks.SingleOrDefault(n => n.Id == id);

            // If Note or Notebook can't be found, return
            if (noteToMove == null || targetNotebook == null)
                return;

            // Update database
            noteToMove.NotebookId = targetNotebook.Id;
            dbEngine.Update(noteToMove);

            // Remove Note from current Notebook
            Notes.Remove(noteToMove);
            SelectedNotebook.NoteCount--;
            targetNotebook.NoteCount++;

            // Add message
            SetClientMessage("Note moved.");
        }

        /// <summary>
        /// Copy Note into the same Notebook
        /// </summary>
        /// <param name="noteId"></param>
        public async Task CopyNoteAsync()
        {
            // If no Notebook or Note is selected, show error and return
            if (SelectedNote == null || SelectedNotebook == null)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error", "The Note could not be copied.\n\nPlease restart the application if the pro");

                return;
            }

            // Create new Note
            Note newNote = new Note()
            {
                NotebookId = SelectedNotebook.Id,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            // Add different title if necessary
            string newNoteTitle = SelectedNote.Title;
            int i = 1;
            while (dbEngine.NoteTitleExists(newNoteTitle, 0))
            {                
                newNoteTitle = SelectedNote.Title + $"({i})";
                i++;
            }
            newNote.Title = newNoteTitle;

            // Create file and save file location
            string fileName = string.Concat(newNote.Title, ".rtf");
            string filePath = Path.Combine(SettingsHelper.noteDirectory, fileName);
            try
            {
                File.Copy(SelectedNote.FileLocation, filePath);
            }
            catch (Exception ex)
            {
                // If adding fails, show error message and return
                await dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                return;
            }

            newNote.FileLocation = filePath;

            // Insert Note to database
            dbEngine.Insert(newNote);

            // Insert note into list
            Notes.Insert(0, newNote);
            SelectedNotebook.NoteCount++;

            // Add message
            SetClientMessage("Note copied.");
        }

        /// <summary>
        /// Make Note Favorite
        /// </summary>
        /// <param name="noteId"></param>
        public void ToggleNoteFavorite(bool favorite)
        {
            SelectedNote.IsFavorite = favorite;
            dbEngine.Update(SelectedNote);

            // If Notes are populated, change note favorite value
            if (Notes != null || Notes.Count != 0)
            {
                var note = Notes.Where(n => n.Id == SelectedNote.Id).FirstOrDefault();

                if (note != null)
                    note.IsFavorite = favorite;
            }

            if (favorite)
            {
                FavoriteNotes.Add(SelectedNote);
                SetClientMessage("Favorite added.");
            }
            else
            {
                // Find favorite note
                var noteToRemove = FavoriteNotes.Where(n => n.Id == SelectedNote.Id).FirstOrDefault();

                // Remove note from favorites
                if (noteToRemove != null)
                    FavoriteNotes.Remove(noteToRemove);

                SetClientMessage("Favorite removed.");
            }
        }

        /// <summary>
        /// Get Note content from file
        /// </summary>
        public void GetNoteContent()
        {
            // Get Note content from file
            if (!File.Exists(SelectedNote.FileLocation))
            {
                dialogCoordinator.ShowMessageAsync(this, "File not found.", "The file could not be found. It may have been deleted or moved.");
                DeleteNote(SelectedNote.Id);
                return;
            }

            NoteContent = File.ReadAllText(SelectedNote.FileLocation);
        }

        /// <summary>
        /// Save Note content to file
        /// </summary>
        public async void SaveNoteContentAsync()
        {
            if (SelectedNote != null && NoteContent != null)
            {
                // Save Note
                File.WriteAllText(SelectedNote.FileLocation, NoteContent);

                // Update Note
                SelectedNote.Updated = DateTime.Now;
                dbEngine.Update(SelectedNote);

                noteContentChanged = false;

                SetClientMessage("Changes saved!");
            }
            else await dialogCoordinator.ShowMessageAsync(this, "Error", "An error occured when saving the Note. Please try again.");
        }

        /// <summary>
        /// Discard changes made to Note content
        /// </summary>
        public async void DiscardChangesAsync()
        {
            if (SelectedNote != null && NoteContent != null)
            {
                var result = await dialogCoordinator.ShowMessageAsync(this, "Discard Changes", "Would you like to discard all changes made to this Note?", MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    GetNoteContent();
                    noteContentChanged = false;

                    SetClientMessage("Changes discarded.");
                }                
            }
            else await dialogCoordinator.ShowMessageAsync(this, "Error", "An error occured when saving the Note. Please try again.");
        }

        /// <summary>
        /// Set a message in the client
        /// </summary>
        /// <param name="message"></param>
        public void SetClientMessage(string message)
        {
            ClientMessageActive = false;
            ClientMessage = message;
            ClientMessageActive = true;
        }

        /// <summary>
        /// Import a file and add it to the Notebook
        /// </summary>
        public async Task ImportFromFileAsync()
        {
            // If no Notebook is selected, show error message and return
            if (SelectedNotebook == null)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error", "Please select a Notebook and try again.");
                return;
            }

            // Open file dialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Rich Text Format Files (*.rtf)|*.rtf|Text Document (*.txt)|*.txt";

            var result = openFileDialog.ShowDialog();

            // If a file was selected
            if (result == true)
            {
                int numberOfFiles = openFileDialog.FileNames.Count();

                // Loop through each file
                foreach (var file in openFileDialog.FileNames)
                {
                    // Validate extension
                    if (Path.GetExtension(file) != ".rtf" && Path.GetExtension(file) != ".txt")
                    {
                        numberOfFiles--;
                        break;
                    }

                    // Create Note object
                    Note newNote = new Note()
                    {
                        NotebookId = SelectedNotebook.Id,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };

                    // Get filename
                    string importedFileName = Path.GetFileNameWithoutExtension(file);
                    string newNoteTitle = importedFileName;
                    int i = 1;
                    while (dbEngine.NoteTitleExists(newNoteTitle, 0))
                    {
                        newNoteTitle = importedFileName + $"({i})";
                        i++;
                    }
                    newNote.Title = newNoteTitle;
                    newNote.FileLocation = Path.Combine(SettingsHelper.noteDirectory, newNoteTitle + ".rtf");

                    switch (Path.GetExtension(file))
                    {
                        case ".rtf":
                            // Copy imported file to Notes directory
                            try
                            {
                                File.Copy(file, newNote.FileLocation);
                            }
                            catch (Exception ex)
                            {
                                // If adding fails, show error message and return
                                await dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                                return;
                            }
                            break;

                        case ".txt":
                            // Create new file
                            try
                            {
                                File.WriteAllText(newNote.FileLocation, File.ReadAllText(file));
                            }
                            catch (Exception ex)
                            {
                                // If adding fails, show error message and return
                                await dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                                return;
                            }
                            break;
                    }            
                    
                    // Add Note to db
                    dbEngine.Insert(newNote);

                    // Insert note into list
                    Notes.Insert(0, newNote);
                    SelectedNotebook.NoteCount++;
                }                

                // Add message
                SetClientMessage($"Successfully imported {numberOfFiles} files.");
            }            
        }

        #endregion
    }
}
