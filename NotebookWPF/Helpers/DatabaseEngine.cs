using NotebookWPF.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.Helpers
{
    /// <summary>
    /// Database engine that implements the IDatabaseEngine interface
    /// </summary>
    public class DatabaseEngine : IDatabaseEngine
    {
        #region Private Members

        // Path to the database file
        private static readonly string dbFileLocation = Path.Combine(SettingsHelper.mainDirectory, "notebookDb.db");

        #endregion

        #region Methods

        /// <summary>
        /// Delete an item from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Delete<T>(T item)
        {
            // Delete an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<T>();
                int numberOfRows = conn.Delete(item);
                // If any rows were affected, return true
                if (numberOfRows > 0)
                    return true;
            }

            // Else, return false
            return false;
        }        

        /// <summary>
        /// Insert an item into the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Insert<T>(T item)
        {
            // Insert an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                // Create table if not already existing
                conn.CreateTable<T>();

                // Insert item into database
                conn.Insert(item);

                // Get property info of item
                System.Reflection.PropertyInfo pi = item.GetType().GetProperty("Id");

                // If item has a property of 'Id'
                if (pi != null)
                {
                    // Return the Id
                    return int.Parse(pi.GetValue(item).ToString());
                }

                // Else, return 0
                else return 0;
            }
        }

        /// <summary>
        /// Update an item in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Update<T>(T item)
        {
            // Update an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<T>();
                int numberOfRows = conn.Update(item);
                // If any rows were affected, return true
                if (numberOfRows > 0)
                    return true;
            }

            // Else, return false
            return false;
        }

        /// <summary>
        /// Returns all notebooks in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<Notebook> GetNotebooks()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<Notebook>();

                // Get notebooks
                var notebooks = conn.Table<Notebook>().ToList();

                foreach (var item in notebooks)
                {
                    item.NoteCount = conn.Table<Note>().Where(n => n.NotebookId == item.Id).Count();
                }

                // Sort notebooks by number of notes
                notebooks = notebooks.OrderByDescending(n => n.NoteCount).ToList();

                return notebooks;
            }
        }

        /// <summary>
        /// Returns all notes for a notebook
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Note> GetNotes(int id)
        {
            // If the id is greater than 0
            if (id > 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
                {
                    conn.CreateTable<Note>();

                    // Get notebooks
                    var notes = conn.Table<Note>().Where(n => n.NotebookId == id).OrderByDescending(n => n.IsFavorite).ThenByDescending(n => n.Updated).ToList();

                    return notes;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns notebook name from the database
        /// </summary>
        /// <returns></returns>
        public string GetNotebookName(int id)
        {
            // If the Id is greater than 0
            if (id > 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
                {
                    conn.CreateTable<Notebook>();

                    // Get notebooks
                    var notebook = conn.Table<Notebook>().Where(n => n.Id == id).FirstOrDefault();

                    if (notebook != null)
                    {
                        return notebook.Name;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Delete notes of notebook
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Note> DeleteNotes(int id)
        {
            // Insert an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<Note>();

                // Get notes to delete
                var notesToDelete = conn.Table<Note>().Where(n => n.NotebookId == id).ToList();

                int totalNumberOfRows = 0;

                // Delete the notes
                foreach (var note in notesToDelete)
                {
                    int numberOfRows = conn.Delete(note);
                    totalNumberOfRows += numberOfRows;
                }                

                // If any rows were affected, return true
                if (totalNumberOfRows > 0)
                    return notesToDelete;
            }

            // Else, return false
            return null;
        }

        /// <summary>
        /// Returns note name from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNoteName(int id)
        {
            // If the Id is greater than 0
            if (id > 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
                {
                    conn.CreateTable<Note>();

                    // Get notebooks
                    var note = conn.Table<Note>().Where(n => n.Id == id).FirstOrDefault();

                    if (note != null)
                    {
                        return note.Title;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determine if a note title exist
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool NoteTitleExists(string title, int noteToEditId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<Note>();

                // Return true if title already exists
                return conn.Table<Note>().ToList().Exists(n => n.Id != noteToEditId && n.Title == title); ;
            }
        }

        /// <summary>
        /// Returns favorite notes
        /// </summary>
        /// <returns></returns>
        public List<Note> GetFavoriteNotes()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<Note>();

                // Get notebooks
                var favoriteNotes = conn.Table<Note>().Where(n => n.IsFavorite == true).OrderByDescending(n => n.Updated).ToList();

                return favoriteNotes;
            }
        }

        #endregion
    }
}
