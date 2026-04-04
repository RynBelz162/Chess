# Chess ♟

A multiplayer chess game built with a modern distributed architecture using .NET Orleans for real-time, stateful gameplay across multiple players.

## Overview
This project demonstrates building a real-time multiplayer game using Orleans virtual actors, SignalR for client communication, and Redis for distributed state management. Players can join games, make moves using algebraic notation, and sync game state in real-time.

### Tech Stack 🏗
- **.NET 10** - Latest .NET runtime
- **Microsoft.Orleans 10** - Distributed actor-based runtime for scalable, stateful services
- **SignalR** - Real-time bidirectional communication with clients
- **Redis** - Distributed caching, clustering, and persistence for Orleans
- **Docker** - Containerized deployment with docker-compose orchestration
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for unit tests
- **AwesomeAssertions** - Fluent assertion library for expressive test assertions

## Project Structure
- **Chess.Api** - Main ASP.NET Core API with Orleans grains, SignalR hubs, and Controllers
- **Chess.Shared** - Shared domain models, DTOs, helpers, and extensions
- **Chess.Console** - Console-based client for testing game logic
- **Chess.Api.Tests** - Unit tests for API components
- **Chess.Shared.Tests** - Unit tests for shared models and utilities

### Running 🏃
1. Navigate to the project root directory:
   ```bash
   cd /path/to/Chess
   ```

2. Start all services with docker-compose:
   ```bash
   docker compose up -d
   ```

3. The API will be available at `http://localhost:5062`

4. Access Redis Commander (optional) at `http://localhost:8081` to inspect Redis data

### Local Development
For local development without Docker:
```bash
cd Chess.Api
dotnet run
```

Or use the watch task for live reload:
```bash
dotnet watch run
```

### Running Tests
```bash
dotnet test
```

## Console Client
The Chess.Console project provides an interactive command-line interface for playing chess games. It connects to the Chess.Api server via SignalR and uses Spectre.Console for a rich terminal experience.

### Using the Console Client
1. Ensure the API is running (via Docker or locally)

2. Navigate to the console project:
   ```bash
   cd Chess.Console
   ```

3. Configure the API connection in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Api": "http://localhost:5062/gamehub"
     }
   }
   ```

4. Run the console client:
   ```bash
   dotnet run
   ```

5. Follow the prompts to:
   - **New Game**: Create a new game and wait for an opponent to join
   - **Join Game**: Join an existing game
   - **Play**: Make moves using algebraic notation (e.g., `e2e4`)

### Console Features
- Interactive menu-driven gameplay
- Real-time game updates via SignalR
- Rich terminal UI with Spectre.Console
- Move validation and game state tracking