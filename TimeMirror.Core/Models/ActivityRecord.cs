
using System;

namespace TimeMirror.Core.Models
{
    public class ActivityRecord
    {
        public int ActivityID { get; set; }
        public int UserID { get; set; }
        public string ApplicationName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
