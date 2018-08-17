using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookWPF.Model
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int NotebookId { get; set; }

        public string Title { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string FileLocation { get; set; }
    }
}
