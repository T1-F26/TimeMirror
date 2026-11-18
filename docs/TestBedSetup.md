# TimeMirror — Local Database Test Bed Setup

## Prerequisites

- Windows with Visual Studio 2022 (WPF/.NET 10 development support)
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

Restart Visual Studio afterward.

## 5. Install EF Core CLI

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

If already installed, verify:

```powershell
dotnet ef --version
```

## 6. Create the database

From the solution directory:

```powershell
dotnet ef database update --project TimeMirror.Infrastructure
```

This applies the existing migrations and creates the `TimeMirrorDB` schema.

Do not create a new initial migration.

## 7. Seed sample data

Connect to SQL Server using SSMS:

- Server: `127.0.0.1,14333`
- Authentication: SQL Server Authentication
- Username: `sa`
- Password: your local SQL Server password
- Trust server certificate: enabled for local development

Open `scripts/SeedTestData.sql` and execute it.

The script creates a test user, an agenda, and sample tasks if they don't already exist.

## 8. Run TimeMirror

Open the solution in Visual Studio.

Set `TimeMirror.App` as the startup project and press F5.

The agenda should load the sample tasks from SQL Server.

## 9. Verify persistence

Create a task using TimeMirror, close the application, then reopen it.

The task should remain available.

## Troubleshooting

**Connection string not configured:** Verify `TIMEMIRROR_CONNECTION_STRING` exists in your Windows user environment and restart Visual Studio.

**Cannot connect to SQL Server:** Check `docker compose ps` and confirm SQL Server is running.

**No agenda found:** Verify that the development user's `UserID` matches `UserSession.CurrentUserId`.

**Database schema missing:** Run `dotnet ef database update --project TimeMirror.Infrastructure`.

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
