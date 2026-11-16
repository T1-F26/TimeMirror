using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimeMirror.Core.Models;

namespace TimeMirror.Core.Models
{
    public class Agenda
    {
        public int AgendaID { get; set; }
        public int UserID { get; set; }
        public string Description { get; set; } = "";

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
