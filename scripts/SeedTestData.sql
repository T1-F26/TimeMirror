
USE TimeMirrorDB;
GO

-- 1. Create a test user only if it does not exist
IF NOT EXISTS (
    SELECT 1 FROM dbo.Users
    WHERE Email = 'test@timemirror.local'
)
BEGIN
    INSERT INTO dbo.Users (Name, Email)
    VALUES ('Test User', 'test@timemirror.local');
END;

-- 2. Find the test user's actual ID
DECLARE @UserID INT;

SELECT @UserID = UserID
FROM dbo.Users
WHERE Email = 'test@timemirror.local';

-- 3. Create the user's agenda if missing
IF NOT EXISTS (
    SELECT 1 FROM dbo.Agendas
    WHERE UserID = @UserID
)
BEGIN
    INSERT INTO dbo.Agendas (UserID, Description)
    VALUES (@UserID, 'My Test Agenda');
END;

-- 4. Find the agenda's actual ID
DECLARE @AgendaID INT;

SELECT @AgendaID = AgendaID
FROM dbo.Agendas
WHERE UserID = @UserID;

-- 5. Insert sample tasks only if missing
IF NOT EXISTS (
    SELECT 1 FROM dbo.Tasks
    WHERE AgendaID = @AgendaID
      AND Title = 'Team Meeting'
)
BEGIN
    INSERT INTO dbo.Tasks
        (AgendaID, Title, Description, DueDate)
    VALUES
        (@AgendaID,
         'Team Meeting',
         'COMP231 project meeting',
         DATEADD(DAY, 1, GETDATE()));
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Tasks
    WHERE AgendaID = @AgendaID
      AND Title = 'Complete Assignment'
)
BEGIN
    INSERT INTO dbo.Tasks
        (AgendaID, Title, Description, DueDate)
    VALUES
        (@AgendaID,
         'Complete Assignment',
         'Finish TimeMirror testing',
         DATEADD(DAY, 2, GETDATE()));
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Tasks
    WHERE AgendaID = @AgendaID
      AND Title = 'Database Review'
)
BEGIN
    INSERT INTO dbo.Tasks
        (AgendaID, Title, Description, DueDate)
    VALUES
        (@AgendaID,
         'Database Review',
         'Verify EF Core integration',
         DATEADD(DAY, 3, GETDATE()));
END;

-- 6. Verify seeded records
SELECT
    u.UserID,
    u.Name,
    a.AgendaID,
    t.TaskID,
    t.Title,
    t.DueDate
FROM dbo.Users u
INNER JOIN dbo.Agendas a
    ON u.UserID = a.UserID
LEFT JOIN dbo.Tasks t
    ON a.AgendaID = t.AgendaID
WHERE u.Email = 'test@timemirror.local'
ORDER BY t.DueDate;
GO
