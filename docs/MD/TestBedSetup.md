# TimeMirror — Local Database Test Bed Setup

**Updated:** November 26, 2026 — Iteration 2

## Prerequisites

- Windows x64 with a Visual Studio version supporting WPF/.NET 10
- .NET 10 SDK
- Docker Desktop running Linux containers
- SQL Server Management Studio (SSMS), recommended
- EF Core CLI (`dotnet-ef`, version 10)

## 1. Clone the repository

```powershell
git clone https://github.com/T1-F26/TimeMirror.git
cd TimeMirror
```

## 2. Configure Docker

Copy `.env.example` to `.env`:

```powershell
Copy-Item .env.example .env
```

Edit `.env` and replace the placeholder with a strong local SQL Server password.

Do not commit `.env` to GitHub.

## 3. Start SQL Server

```powershell
docker compose up -d
docker compose ps
```

SQL Server runs locally at `127.0.0.1,14333`.

Docker uses a persistent volume so database records survive container restarts.

## 4. Configure the application connection string

In PowerShell:

```powershell
[Environment]::SetEnvironmentVariable(
    "TIMEMIRROR_CONNECTION_STRING",
    "Server=127.0.0.1,14333;Database=TimeMirrorDB;User Id=sa;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;",
    "User"
)
```

Use the password from your `.env` file.

Restart Visual Studio afterward. For the current PowerShell session, initialize the connection as follows:

```powershell
$devConnection = [Environment]::GetEnvironmentVariable("TIMEMIRROR_CONNECTION_STRING", "User")
$testConnection = $devConnection.Replace("Database=TimeMirrorDB;", "Database=TimeMirrorTestDB;")
$env:TIMEMIRROR_CONNECTION_STRING = $devConnection
```

The persistent user variable remains pointed at development. The test override affects only the current shell and its child processes. Do not share the connection string.

## 5. Install EF Core CLI

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

If already installed, verify:

```powershell
dotnet ef --version
```

## 6. Create the database (fresh installations only)

From the solution directory:

```powershell
dotnet ef database update AddActivityRecords --project TimeMirror.Infrastructure --startup-project TimeMirror.App
```

This creates the schema through activity tracking. The later `SeedDevelopmentData` migration requires the test user and agenda to exist first. Never target this earlier migration on an existing database that is already beyond it: doing so would roll back later migrations.

Do not create a new initial migration.

## 7. Seed sample data

Connect to SQL Server using SSMS:

- Server: `127.0.0.1,14333`
- Authentication: SQL Server Authentication
- Username: `sa`
- Password: your local SQL Server password
- Trust server certificate: enabled for local development

Open `scripts/SeedTestData.sql`, verify its database selection (including any `USE` statement), and execute it against the intended database.

The script creates a test user, an agenda, and sample tasks if they don't already exist.

After verifying the test user and agenda exist, apply the remaining migrations:

```powershell
dotnet ef database update --project TimeMirror.Infrastructure --startup-project TimeMirror.App
```

Repeat Sections 6–7 for a fresh `TimeMirrorTestDB` using `$env:TIMEMIRROR_CONNECTION_STRING = $testConnection` and the corresponding SSMS database. Restore `$devConnection` afterward.

## 8. Run TimeMirror

Open the solution in Visual Studio.

Set `TimeMirror.App` as the startup project and press F5.

The agenda should load the sample tasks from SQL Server.

During development, TimeMirror automatically looks up test@timemirror.local in SQL Server and initializes UserSession using that user's actual ID. Run SeedTestData.sql before launching the application. This is a development-only session, not a production authentication system.

## 9. Verify persistence

Create a task using TimeMirror, close the application, then reopen it.

The task should remain available.

## 10. Iteration 2 — Shared database updates

| Database | Purpose |
|---|---|
| `TimeMirrorDB` | Development and debug builds |
| `TimeMirrorTestDB` | Published test bed and acceptance testing |

Existing developers should pull the latest `main` after the setup PR is merged. If both databases already have the test user and agenda, apply outstanding migrations directly:

```powershell
$env:TIMEMIRROR_CONNECTION_STRING = $devConnection
dotnet ef database update --project TimeMirror.Infrastructure --startup-project TimeMirror.App

$env:TIMEMIRROR_CONNECTION_STRING = $testConnection
dotnet ef database update --project TimeMirror.Infrastructure --startup-project TimeMirror.App

$env:TIMEMIRROR_CONNECTION_STRING = $devConnection
```

Check each command succeeds before continuing. Migrations are generated once and applied independently to each database. Do not edit applied migrations or generate duplicate migrations.

| Migration | Purpose |
|---|---|
| `SeedDevelopmentData` | Adds 50 sample tasks and 100 activity records |
| `AddUserPermissions` | Creates the permissions table |
| `SeedUserPermissions` | Initializes permissions for existing users |
| `BackfillUserPermissions` | Inserts missing permissions without overwriting consent |

Seed dates are relative to migration execution and do not refresh automatically. Docker server time may differ from Windows local time. Sample data is for development and testing only.

Verify in SSMS (repeat for `TimeMirrorTestDB`):

```sql
USE TimeMirrorDB;
SELECT * FROM dbo.__EFMigrationsHistory;
SELECT * FROM dbo.Users;
SELECT * FROM dbo.Agendas;
SELECT * FROM dbo.UserPermissions;
SELECT COUNT(*) AS SeededTasks FROM dbo.Tasks
WHERE Description LIKE '[[]TM-SEED-V1] %';
SELECT COUNT(*) AS SeededActivities FROM dbo.ActivityRecords
WHERE ApplicationName LIKE '[[]TM-SEED-V1] %';
```

Expected immediately after seeding: 50 marked tasks, 100 marked activities and an `ActivityTracking` permission for the test user with `IsGranted = 0`. IDs may differ between databases.

## 11. Publish and launch the test bed

After integration, close the previous test application and publish a fresh self-contained Windows x64 build:

```powershell
dotnet publish TimeMirror.App -c Release -r win-x64 --self-contained true -o ./TestBed/PublishSelfContained
```

Select the test database and verify the database name without exposing credentials:

```powershell
$env:TIMEMIRROR_CONNECTION_STRING = $testConnection
$builder = [System.Data.Common.DbConnectionStringBuilder]::new()
$builder.ConnectionString = $env:TIMEMIRROR_CONNECTION_STRING
$builder["Database"]
```

Expected: `TimeMirrorTestDB`. Launch from the same shell:

```powershell
.\TestBed\PublishSelfContained\TimeMirror.App.exe
```

After closing the application, restore development:

```powershell
$env:TIMEMIRROR_CONNECTION_STRING = $devConnection
```

Keep `.env`, `TestBed/Publish/` and `TestBed/PublishSelfContained/` out of Git. Commit source, migrations and documentation instead.

## 12. View Permissions (#99) — Developer handoff

The shared model, enum, DbContext configuration and migrations establish the database contract. Assigned developers implement the repository, service, ViewModel, view, DI and navigation according to the design class and sequence diagrams.

| Column | Type / constraint |
|---|---|
| PermissionID | INT, primary key |
| UserID | INT, foreign key |
| PermissionType | NVARCHAR(50), string-backed enum |
| IsGranted | BIT, default false |

The combination `(UserID, PermissionType)` is unique. The initial permission type is `ActivityTracking`.

This story is read-only. The flag represents stored application-level consent, not Windows permissions. Modification and enforcement belong to #22. Until enforcement is implemented, the flag does not actually stop activity tracking. New-user permission initialization remains future work.

## 13. Acceptance testing

Record actual results, screenshots and build identifiers in the TAC test documentation. Do not mark unexecuted tests as passed.

### Dashboard (#91)

Verify automatic loading on navigation, today's tasks, upcoming tasks, completion progress, tracked time, refresh and empty-state behavior. The published dashboard has been observed running with seeded data; formal acceptance results are recorded separately.

### View Permissions (#99)

| Test ID | Scenario | Expected result | Status |
|---|---|---|---|
| VP-01 | Open Permissions | Page opens and loads current user's permissions | Not executed |
| VP-02 | Denied permission | Not Granted displayed | Not executed |
| VP-03 | Granted permission | Granted displayed | Not executed |
| VP-04 | Navigate away and return | Current status reloads | Not executed |
| VP-05 | No permission records | Appropriate empty state | Not executed |
| VP-06 | Inspect controls | No modification controls | Not executed |
| VP-07 | Multiple users | Only current user's permissions displayed | Not executed |

VP-05 and VP-07 require additional controlled test fixtures. Do not delete genuine user records.

To prepare the granted test scenario after implementation:

```sql
USE TimeMirrorTestDB;
UPDATE dbo.UserPermissions
SET IsGranted = 1
WHERE PermissionType = 'ActivityTracking'
AND UserID IN (SELECT UserID FROM dbo.Users WHERE Email = 'test@timemirror.local');
```

Reopen the Permissions page and verify the displayed status. Restore the default denied fixture afterward:

```sql
USE TimeMirrorTestDB;
UPDATE dbo.UserPermissions
SET IsGranted = 0
WHERE PermissionType = 'ActivityTracking'
AND UserID IN (SELECT UserID FROM dbo.Users WHERE Email = 'test@timemirror.local');
```

These SQL changes are test fixture preparation, not implementation of #22.

## Troubleshooting

**Connection string not configured:** Verify `TIMEMIRROR_CONNECTION_STRING` exists in your Windows user environment and restart Visual Studio.

**Cannot connect to SQL Server:** Check `docker compose ps` and confirm SQL Server is running.

**No agenda found:** Verify that the development user's `UserID` matches `UserSession.CurrentUserId`.

**Database schema missing:** Run `dotnet ef database update --project TimeMirror.Infrastructure --startup-project TimeMirror.App`.

**Migration name already exists:** Apply the existing migration using `database update` rather than generating it again.

**Permission table empty:** Check user records and migration history. Apply the backfill migration if pending. Editing an applied migration does not rerun it.

**Published app requires .NET:** Republish using the self-contained command in Section 11.

**Wrong database:** Verify the connection string database field and launch from the correct PowerShell session.

**NU1902/NU1903 warnings:** Review and update vulnerable NuGet dependencies separately. A successful build does not resolve these warnings.

## Cleanup

Stop containers without deleting database data:

```powershell
docker compose down
```

Restart later with:

```powershell
docker compose up -d
```

Avoid `docker compose down -v` unless you intentionally want to delete the database volume and its data.
