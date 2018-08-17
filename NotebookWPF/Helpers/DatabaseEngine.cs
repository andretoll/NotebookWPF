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
        public bool Insert<T>(T item)
        {
            // Insert an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<T>();
                int numberOfRows = conn.Insert(item);
                // If any rows were affected, return true
                if (numberOfRows > 0)
                    return true;
            }

            // Else, return false
            return false;
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
        public List<Notebook> GetNotebooks<T>()
        {
            // Insert an item
            using (SQLiteConnection conn = new SQLiteConnection(dbFileLocation))
            {
                conn.CreateTable<T>();
                conn.CreateTable<Note>();

                // Get notebooks
                var notebooks = conn.Table<Notebook>().ToList();

                foreach (var item in notebooks)
                {
                    item.NoteCount = conn.Table<Note>().Where(n => n.NotebookId == item.Id).Count();
                }

                return notebooks;
            }
        }

        #endregion
    }
}
