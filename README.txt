# Project Carros 

his project was developed as part of the P1 Evaluation for Module III of the **UpSkill** course. It is a car workshop management system built with **C#** and **.NET 9**, focused on implementing modern architectural patterns and data persistence in SQL Server.

## Technologies Used

* **Language:** C#
* **Framework:** .NET 9
* **Database:** SQL Server
* **Design Pattern:** Repository Pattern (Generic Repository)
* **Authentication:** JWT Bearer Token

## Project Architecture

The project follows a structured organization across separate projects to ensure scalability and ease of maintenance:

### 3-Tier Architecture
* **Frontend:** JavaScript-based frontend.
* **Service Layer:** ASP.NET Core MInimal API, establishing communication between the frontend and backend via the HTTP protocol.
* **Data Layer:** Class Library containing the data model and repositories with core business logic.

### Technical Components
* **Models:** Domain entities representing the database tables (Cars, Accounts, Brands, Models).
* **ADO.NET:** Implementation of low-level persistence using ADO.NET (via the **DalPro** library) for direct data stream manipulation.
* **Repositories:** Implementation of a **Generic Repository** to centralize Data Access Logic (CRUD).
* **Helpers:** Utility classes for authentication handling and database connection management.
* **Loggers:** Error logging and exception tracking.
* **Dependency Injection (DI):** Decoupling of the Service Layer (API) and the Persistence Layer (Repositories) through Interface-based injection.

## Main Features
- [x] Vehicle Management (CRUD).
- [x] Search filtering.

## How to Run the Project

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/LuizaDiBlasio/P1_ModuloIII.git](https://github.com/LuizaDiBlasio/P1_ModuloIII.git)
