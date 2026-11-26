using System;
using TimeMirror.Core.Enums;

namespace TimeMirror.Core.Models
{
    public class Permission
    {
        public int PermissionID { get; set; }
        public int UserID { get; set; }
        public PermissionType Type { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsGranted { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}