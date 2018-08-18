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
        int Insert<T>(T item);

        bool Update<T>(T item);

        bool Delete<T>(T item);

        List<Notebook> GetNotebooks();
    }
}