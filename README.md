# Quiz Game API

REST API developed in ASP.NET Core Web API for managing a question and answer game with progressive levels and prize accumulation.

---

# Technologies

* ASP.NET Core 8
* Entity Framework Core
* SQL Server
* Swagger / OpenAPI
* Serilog
* Docker
* C#

---

# Architecture

The solution was implemented using layered architecture:

* Controllers
* Services
* Data Access
* DTOs
* Middleware
* Entity Framework Core

---

# Features

* Configure categories
* Configure questions
* Start game
* Retrieve random questions
* Answer questions
* Increase game level
* Accumulate prizes
* Withdraw from game
* Win or lose game
* Logging
* Global exception handling

---

# Project Structure

```text
ApiQuizGame
│
├── Controllers
├── Data
├── DTOs
├── Entities
├── Middlewares
├── Services
├── Logs
```

---

# Requirements

* .NET 8 SDK
* SQL Server
* Visual Studio 2022

---

# Database Configuration

Update the connection string in:

```text
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=QuizGameDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

# Run Migrations

Open Package Manager Console and execute:

```powershell
Add-Migration InitialCreate
Update-Database
```

---

# Run Application

```bash
dotnet run
```

Swagger:

```text
https://localhost:xxxx/swagger
```

---

# Docker

Run containers:

```bash
docker-compose up
```

---

# Main Endpoints

## Categories

* GET /api/categories
* POST /api/categories
* PUT /api/categories/{id}
* DELETE /api/categories/{id}

## Questions

* GET /api/questions
* POST /api/questions
* DELETE /api/questions/{id}

## Games

* POST /api/games/start
* GET /api/games/{gameId}/question
* POST /api/games/{gameId}/answer
* POST /api/games/{gameId}/withdraw

---

# Author

Jairo Riaño
