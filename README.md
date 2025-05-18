README.txt

==================================================
.NET Core Backend – User and Document Management
==================================================

📌 PURPOSE
----------
This project is a .NET Core backend service designed to manage user authentication, document handling, and ingestion controls. It serves as a microservice that communicates with a Spring Boot backend for ingestion processing.

==================================================
🔧 TECHNOLOGIES & TOOLS
-----------------------
- Framework: ASP.NET Core (.NET 8/9 preferred)
- Language: C#
- ORM: Entity Framework Core (with PostgreSQL)
- Authentication: JWT (JSON Web Tokens)
- Communication: HttpClient or gRPC
- Architecture: Microservices
- Version Control: Git

==================================================
🔑 KEY FEATURES & APIs
-----------------------

1. **Authentication APIs**
   - `POST /api/auth/register` – Register a new user
   - `POST /api/auth/login` – Authenticate user and issue JWT
   - `POST /api/auth/logout` – Logout user and invalidate session
   - Role-based access: Supports roles like `Admin`, `Editor`, `Viewer`

2. **User Management APIs** (Admin-only)
   - `GET /api/users` – List all users
   - `PUT /api/users/{id}/role` – Update a user's role
   - `DELETE /api/users/{id}` – Remove a user

3. **Document Management APIs**
   - `GET /api/documents` – Retrieve all documents
   - `GET /api/documents/{id}` – Get a single document
   - `POST /api/documents` – Upload or create a document
   - `PUT /api/documents/{id}` – Update a document
   - `DELETE /api/documents/{id}` – Delete a document

4. **Ingestion Trigger API**
   - `POST /api/ingestion/trigger` – Initiate ingestion process
   - Calls Spring Boot backend using `HttpClient` or `gRPC`

5. **Ingestion Management API**
   - `GET /api/ingestion/status/{id}` – Get ingestion status
   - `DELETE /api/ingestion/cancel/{id}` – Cancel ingestion process

==================================================
🔐 AUTHENTICATION & AUTHORIZATION
----------------------------------
- JWT used to secure endpoints.
- Claims are used to enforce role-based access (Admin, Editor, Viewer).
- Middleware verifies token validity and role claims.

==================================================
💾 DATABASE SETUP
------------------
- PostgreSQL is the backing database.
- Use Entity Framework Core to generate and apply migrations.

Example command:
> dotnet ef migrations add InitialCreate  
> dotnet ef database update

==================================================
🚀 RUNNING THE APPLICATION
---------------------------
1. Ensure PostgreSQL is running and accessible.
2. Configure `appsettings.json` with the correct connection string and JWT settings.
3. Build and run the API using:

> dotnet build  
> dotnet run

The API will be available at:  
> https://localhost:{PORT}/api

==================================================
🔄 COMMUNICATION WITH SPRING BOOT BACKEND
------------------------------------------
- The ingestion-related endpoints call the Spring Boot service using REST or gRPC.
- Endpoint URLs and credentials can be configured in `appsettings.json`.

==================================================
📂 PROJECT STRUCTURE
---------------------
- `Controllers/` – API controllers for each domain (Auth, Users, Documents, Ingestion)
- `Models/` – Data models and DTOs
- `Services/` – Business logic
- `Repositories/` – Data access layer
- `Middleware/` – JWT authentication and role-based authorization
- `Data/` – EF DbContext and Migrations

==================================================
📫 CONTACT
----------
For any queries or issues, please contact the assignment supervisor or course administrator.

