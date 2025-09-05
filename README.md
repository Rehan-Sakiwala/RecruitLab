# RecruitLab

A full-stack recruitment platform built with React and ASP.NET Core.

### Frontend

- React 19
- Vite

### Backend

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

## Prerequisites

- .NET 6.0 or later
- Node.js 16 or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code with C# extension

## Installation

### Backend Setup

1. Navigate to the server directory:

   ```bash
   cd recruitlab.server
   ```

2. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` (if needed):

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RecruitLabDb;Trusted_Connection=True;"
   }
   ```

4. Run database migrations:

   ```bash
   database update
   ```

5. Start the server:
