using TimeMirror.Core.Enums;

namespace TimeMirror.Core.Models;

public class UserPermission
{
    public int PermissionID { get; set; }

    public int UserID { get; set; }

    public PermissionType PermissionType { get; set; }

    public bool IsGranted { get; set; }
}