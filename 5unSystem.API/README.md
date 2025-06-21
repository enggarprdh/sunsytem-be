# 5unSystem Backend API

## Description
5unSystem is a backend API built with ASP.NET Core (.NET 8) providing JWT authentication, user, role, and dynamic menu management. The project uses Entity Framework Core for SQL Server database access.

## Project Structure

- **5unSystem.API**: Application entry point, contains API controllers, JWT configuration, Swagger, and dependency injection.
- **5unSystem.Core**: Main business logic, database seeding, and data access via Entity Framework.
- **5unSystem.Model**: DTOs, entities, and enums used throughout the application.
- **5unSystem.Utility**: Utilities/helpers such as password hashing and user claim session management.

## How to Run the Project

1. **Clone the repository** and open the project folder in your IDE (e.g., Visual Studio, VS Code).
2. Ensure SQL Server is running and the connection string in `appsettings.json` is correct.
3. Restore dependencies from the root project:
   dotnet restore4. Database migration and seeding will run automatically on first launch.
5. Run the application:
   dotnet run --project 5unSystem.API6. The API will be available at `https://localhost:7294` (or as configured).
7. Open Swagger documentation at `/swagger` to explore endpoints.

## Key Configuration

- **appsettings.json**
  - `ConnectionStrings:Main`: SQL Server connection string.
  - `Jwt`: JWT configuration (key, issuer, audience, expiry).
- **menu.json**: Dynamic menu file seeded to the database on first run.

## Project Flow Diagram
flowchart TD
    A["Startup: Program.cs"] --> B["Register Services (DbContext, JWT, CORS, Swagger)"]
    B --> C["Run GeneratorDB.EnsureDatabaseCreated()"]
    C --> D["Create Database & Tables"]
    C --> E["Seed Initial Data (Admin User, Role, Menu, RoleMenu)"]
    E --> F["menu.json"]
    A --> G["Configure Middleware (Auth, CORS, Swagger, Controllers)"]
    G --> H["API Endpoints"]
    H --> I["Auth: /api/auth/login"]
    H --> J["Role Management: /api/role"]
    H --> K["Dynamic Menu"]
## Project Flow

1. **Startup**: On application start, `Program.cs`:
   - Registers services (DbContext, JWT, CORS, Swagger, etc).
   - Runs `GeneratorDB.EnsureDatabaseCreated()` to create the database, tables, and seed initial data (admin user, role, menu, role-menu).
2. **Authentication**: `/api/auth/login` endpoint accepts username & password, returns JWT token if valid.
3. **Role Management**: `/api/role` endpoint for CRUD operations on roles (JWT authentication required).
4. **Dynamic Menu**: Menu is loaded from the database, seeded from `menu.json`.

## Main Endpoints

- `POST /api/auth/login` : User login, returns JWT token & menu.
- `GET /api/role` : List roles (auth required).
- `POST /api/role` : Add role (auth required).
- `PUT /api/role/{roleId}` : Update role (auth required).
- `DELETE /api/role/{roleId}` : Delete role (auth required).

## Notes
- Default user & role are created automatically on first run.
- Default admin password: `admin123` (hashed in the database).
- For development, use Swagger UI to test endpoints.

---

Feel free to modify the configuration as needed.