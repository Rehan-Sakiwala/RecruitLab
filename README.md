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

### Frontend Setup

1.  Navigate to the client directory:

    ```bash
    cd recruitlab.client
    ```

2.  Install dependencies:

    ```bash
    npm install
    ```

3.  Start the development server:

    ```bash
    npm run dev
    ```

    The frontend will be available at `http://localhost:5173`

### Backend Setup

1.  Navigate to the server directory:

    ```bash
    cd recruitlab.server
    ```

2.  Restore NuGet packages:

    ```bash
    dotnet restore
    ```

3.  Update `appsettings.json` with your database connection string:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database={db_name};Trusted_Connection=True;"
    }
    ```

4.  Update `appsettings.json` with your **Email** and **JWT** settings.

    - **EmailSettings**: Use a service like Mailtrap for development or Gmail/SendGrid for production. (For Gmail, you must use a 16-digit "App Password").
    - **Jwt**: Set a strong, secret key.

    ```json
    "EmailSettings": {
      "MailServer": "{Server here}",
      "MailPort": "{Port here}",
      "SenderName": "{Sender name}",
      "SenderEmail": "{Sender email}",
      "Password": "{App Password}"
    },
    "Jwt": {
      "Key": "{key here}",
      "Issuer": "{Issuer here}",
      "Audience": "{Audience here}"
    }
    ```

5.  Run database migrations (this will also seed the database):

    ```bash
    update-database
    ```

6.  Start the server (Run `Program.cs` from Visual Studio or use the command line):
    ```bash
    dotnet run
    ```
