using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMirror.Core.Models
{

    public class TaskItem
    {
        public int TaskID { get; set; }
        public int AgendaID { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DueDate { get; set; }

        // Issue #17 - Vincent: true once the user marks the task as done.
        public bool IsCompleted { get; set; }
    }
}
