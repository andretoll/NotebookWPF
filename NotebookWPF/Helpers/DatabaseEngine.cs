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
        #region Public Members

        // Path to the database file
        public static string dbFileLocation = Path.Combine(Environment.CurrentDirectory, "notebookDb.db");

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
            // Insert an item
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
            // Insert an item
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
                var notebooks = conn.Table<Notebook>().OrderByDescending(n => n.Id).ToList();

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
        /// Returns notebook name from the database
        /// </summary>
        /// <returns></returns>
        public string GetNotebookName(int id)
        {
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

        #endregion
    }
}
