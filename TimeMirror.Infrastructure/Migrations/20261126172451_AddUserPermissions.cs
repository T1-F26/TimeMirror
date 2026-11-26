using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO UserPermissions
        (
            UserID,
            PermissionType,
            IsGranted
        )
        SELECT
            u.UserID,
            'ActivityTracking',
            0
        FROM Users u
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM UserPermissions p
            WHERE p.UserID = u.UserID
            AND p.PermissionType = 'ActivityTracking'
        );
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Preserve user permission records and consent decisions.
        }
    }
}
