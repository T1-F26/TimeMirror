using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimeMirror.Core.Models;

namespace TimeMirror.Core.Validators
{

    public class AgendaValidator
    {
        public bool IsValid(Agenda agenda) =>
            !string.IsNullOrWhiteSpace(agenda.Description);
    }

}