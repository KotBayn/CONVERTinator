<div align="center">
    <a href="https://convertinator.onrender.com" target="_blank">
<img width="190" height="199" alt="image_2026-06-26_16-13-08" src="https://github.com/user-attachments/assets/d081ae0e-7ff7-4f03-b3c2-2553a4931eda" />
    </a>
</div>
    

## Overview to
    ╔═══════════════════════════════════════════════════════════════════════════════════════════════╗
    ║██████ ██████ ██   ██ ██   ██ ██████ █████  ██████ ██████ ██   ██   ███   ██████ ██████ █████  ║
    ║██     ██  ██ ███  ██ ██   ██ ██     ██  ██   ██     ██   ███  ██  ██ ██    ██   ██  ██ ██  ██ ║
    ║██     ██  ██ ██ █ ██ ██   ██ ██     █████    ██     ██   ██ █ ██ ███████   ██   ██  ██ █████  ║
    ║██     ██  ██ ██ █ ██  ██ ██  █████  ██  ██   ██     ██   ██ █ ██ ███████   ██   ██  ██ ██  ██ ║
    ║██     ██  ██ ██  ███  ██ ██  ██     ██  ██   ██     ██   ██  ███ ██   ██   ██   ██  ██ ██  ██ ║
    ║██████ ██████ ██   ██    █    ██████ ██  ██   ██   ██████ ██   ██ ██   ██   ██   ██████ ██  ██ ║
    ╚═══════════════════════════════════════════════════════════════════════════════════════════════╝
**High-Performance Currency Conversion Platform**

[Live Demo](https://convertinator.onrender.com) | [Author LinkedIn](https://www.linkedin.com/in/t-drohoman/)

---

CONVERTinator is a robust, lightweight currency conversion engine designed to convert a lot of currencies at once. By aggregating multiple financial data sources, it calculates accurate median rates and provides context-aware conversion logic. 

The solution is built on .NET 8, adhering strictly to Clean Architecture principles to ensure high testability, maintainability, and strict separation of concerns.

## Technical Highlights

### Core Engineering
- **Asynchronous Execution:** Utilizes `Task.WhenAll` for parallel polling across multiple financial APIs, ensuring response times are bound only by the fastest available server.
- **Resource Management:** Implements `IHttpClientFactory` to prevent socket exhaustion during high-frequency external API calls.
- **Dependency Injection:** Fully migrated to constructor injection, facilitating strict mocking and unit testing across the domain and service layers.
- **Background Processing:** Employs `IServiceScopeFactory` within background services (`SyncWorker`, `CacheSyncService`) to resolve captive dependencies safely.

### Domain Logic & Modes
- **Context-Aware Routing (Travel Mode):** Automatically detects user geolocation to provide local rates and bordering currencies, filtering out irrelevant global data. For website now is only IP, but it will be improved.
- **Analytical Routing (Business Mode):** Provides deep financial analytics, including cross-rate calculations via base currencies (USD/EUR) and regional median evaluations. Not use fo now.
- **Precision:** Engineered for high-precision calculations, maintaining accuracy up to 4 decimal places.

## Architecture

The project follows Clean Architecture, isolating core business logic from external frameworks and UI.

```text
CONVERTinator/
├── CONVERTinator.Domain/          # Core business logic, entities, and interfaces
├── CONVERTinator.WebAPI/          # ASP.NET Core REST API layer
├── CONVERTinator/                 # Console application (CLI)
├── CONVERTinator.Tests/           # Unit and integration test suites
└── CONVERTinator.sln              # Solution configuration
```

## Technology Stack

| Category | Technology |
| :--- | :--- |
| **Runtime** | .NET 8.0 / C# 12 |
| **Web Framework** | ASP.NET Core Web API |
| **Data Access** | Entity Framework Core |
| **DataBase** | SQLite |
| **Testing** | xUnit, FluentAssertions, Moq |
| **Containerization**| Docker |
| **Design Patterns** | Factory, Facade, Dependency Injection, Repository |

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Local Development

1. Clone the repository:
   ```bash
   git clone https://github.com/KotBayn/CONVERTinator.git
   cd CONVERTinator
   ```

2. Run the Web API:
   ```bash
   cd CONVERTinator.WebAPI
   dotnet run
   ```

3. Run the Console Application:
   ```bash
   cd CONVERTinator
   dotnet run
   ```

## Testing

The codebase is backed by a comprehensive suite of unit and integration tests, ensuring reliability across the domain and service layers.

```bash
cd CONVERTinator.Tests
dotnet test
```
Tests cover controller logic, service layer behavior with mocked dependencies, and edge-case boundary conditions using the EF Core In-Memory provider.

## Docker Deployment

Build and run the application in a containerized environment:

```bash
# Build the image
docker build -t convertinator .

# Run the container
docker run -d -p 8080:8080 --name convertinator-app convertinator
```
The application will be accessible at `http://localhost:8080`.

## License

**Proprietary License**

- **Personal Use:** Free to download, compile, and use for personal, non-commercial tasks.
- **Commercial & Enterprise Use:** This project is NOT open-source. All rights are reserved. 
- **Restrictions:** Without explicit written permission, it is strictly prohibited to:
  1. Use the code or its derivatives in commercial projects.
  2. Embed the software into other proprietary software products.
  3. Modify the logic and distribute modified versions (forks) under a different name.

For commercial licensing, integration, or collaboration inquiries, please reach out via LinkedIn.

## Contact

**T. Drohoman aka KotBayn**  
- **LinkedIn:** [linkedin.com/in/t-drohoman](https://www.linkedin.com/in/t-drohoman/)  
- **GitHub:** [github.com/KotBayn](https://github.com/KotBayn)  
- **Live Project:** [convertinator.onrender.com](https://convertinator.onrender.com)
```
