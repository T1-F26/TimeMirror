using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeMirror.Infrastructure.Migrations
{
    public partial class BackfillUserPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO dbo.UserPermissions
                (
                    UserID,
                    PermissionType,
                    IsGranted
                )
                SELECT
                    u.UserID,
                    'ActivityTracking',
                    0
                FROM dbo.Users u
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.UserPermissions p
                    WHERE p.UserID = u.UserID
                    AND p.PermissionType = 'ActivityTracking'
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Preserve existing permission decisions.
        }
    }
}