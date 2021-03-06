﻿using NotebookWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.Helpers
{
    /// <summary>
    /// Database engine interface.
    /// </summary>
    interface IDatabaseEngine
    {
        #region Interface Members

        int Insert<T>(T item);

        bool Update<T>(T item);

        bool Delete<T>(T item);

        List<Notebook> GetNotebooks();

        List<Note> GetFavoriteNotes();

        List<Note> GetNotes(int id);

        List<Note> DeleteNotes(int id);

        string GetNotebookName(int id);

        string GetNoteName(int id);

        bool NoteTitleExists(string title, int noteToEditId);

        #endregion
    }
}
