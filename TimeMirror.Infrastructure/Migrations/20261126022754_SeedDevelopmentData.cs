
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeMirror.Infrastructure.Migrations
{
    public partial class SeedDevelopmentData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @UserID INT;
                DECLARE @AgendaID INT;
                DECLARE @Now DATETIME2 = SYSDATETIME();
                DECLARE @Today DATETIME2 = CAST(@Now AS DATE);

                SELECT @UserID = UserID
                FROM Users
                WHERE Email = 'test@timemirror.local';

                SELECT @AgendaID = AgendaID
                FROM Agendas
                WHERE UserID = @UserID;

                IF @UserID IS NULL OR @AgendaID IS NULL
                    THROW 50001, 'Test user or agenda missing.', 1;

                -- Generate 50 tasks
                DECLARE @i INT = 1;
                DECLARE @DayOffset INT;
                DECLARE @Completed BIT;
                DECLARE @Title NVARCHAR(100);

                WHILE @i <= 50
                BEGIN
                    SET @DayOffset = CASE
                        WHEN @i <= 10 THEN 0
                        WHEN @i <= 17 THEN -1
                        WHEN @i <= 30 THEN -2 - (@i % 6)
                        WHEN @i <= 35 THEN 1
                        WHEN @i <= 45 THEN 2 + (@i % 6)
                        ELSE 8 + (@i % 23)
                    END;

                    SET @Completed = CASE
                        WHEN @i BETWEEN 1 AND 4 THEN 1
                        WHEN @i BETWEEN 11 AND 15 THEN 1
                        WHEN @i BETWEEN 18 AND 25 THEN 1
                        ELSE 0
                    END;

                    SET @Title = CASE @i % 15
                        WHEN 0 THEN 'Team Meeting'
                        WHEN 1 THEN 'Review Project Requirements'
                        WHEN 2 THEN 'Complete Assignment'
                        WHEN 3 THEN 'Database Testing'
                        WHEN 4 THEN 'Update Documentation'
                        WHEN 5 THEN 'Code Review'
                        WHEN 6 THEN 'Study Session'
                        WHEN 7 THEN 'Fix Application Bugs'
                        WHEN 8 THEN 'Sprint Planning'
                        WHEN 9 THEN 'Review Pull Requests'
                        WHEN 10 THEN 'Write Unit Tests'
                        WHEN 11 THEN 'Update Class Diagram'
                        WHEN 12 THEN 'Research Design Patterns'
                        WHEN 13 THEN 'Prepare Presentation'
                        ELSE 'Review Lecture Notes'
                    END;

                    INSERT INTO Tasks
                    (
                        AgendaID,
                        Title,
                        Description,
                        DueDate,
                        IsCompleted
                    )
                    VALUES
                    (
                        @AgendaID,
                        @Title,
                        CONCAT('[TM-SEED-V1] Task ', @i),
                        DATEADD(
                            MINUTE,
                            480 + (@i * 47 % 720),
                            DATEADD(DAY, @DayOffset, @Today)
                        ),
                        @Completed
                    );

                    SET @i = @i + 1;
                END;

                -- Generate 100 application activity records
                SET @i = 1;

                DECLARE @Application NVARCHAR(100);
                DECLARE @StartTime DATETIME2;
                DECLARE @EndTime DATETIME2;

                WHILE @i <= 100
                BEGIN
                    SET @Application = CASE @i % 10
                        WHEN 0 THEN 'devenv - Visual Studio'
                        WHEN 1 THEN 'chrome - Google Chrome'
                        WHEN 2 THEN 'Code - Visual Studio Code'
                        WHEN 3 THEN 'devenv - Visual Studio'
                        WHEN 4 THEN 'Ssms - SQL Server Management Studio'
                        WHEN 5 THEN 'chrome - Google Chrome'
                        WHEN 6 THEN 'Discord'
                        WHEN 7 THEN 'EXCEL - Microsoft Excel'
                        WHEN 8 THEN 'powershell'
                        ELSE 'TimeMirror.App - TimeMirror'
                    END;

                    -- Consecutive historical activity sessions.
                    SET @StartTime = DATEADD(
                        MINUTE,
                        -(@i * 40),
                        @Now
                    );

                    SET @EndTime = DATEADD(
                        MINUTE,
                        5 + (@i * 7 % 30),
                        @StartTime
                    );

                    INSERT INTO ActivityRecords
                    (
                        UserID,
                        ApplicationName,
                        StartTime,
                        EndTime
                    )
                    VALUES
                    (
                        @UserID,
                        CONCAT('[TM-SEED-V1] ', @Application),
                        @StartTime,
                        @EndTime
                    );

                    SET @i = @i + 1;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ActivityRecords
                WHERE ApplicationName LIKE '[[]TM-SEED-V1] %'
                AND UserID IN
                (
                    SELECT UserID
                    FROM Users
                    WHERE Email = 'test@timemirror.local'
                );

                DELETE FROM Tasks
                WHERE Description LIKE '[[]TM-SEED-V1] %'
                AND AgendaID IN
                (
                    SELECT a.AgendaID
                    FROM Agendas a
                    INNER JOIN Users u
                        ON a.UserID = u.UserID
                    WHERE u.Email = 'test@timemirror.local'
                );
            ");
        }
    }
}
