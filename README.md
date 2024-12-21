# Redis Example Project

This project demonstrates the use of Redis for caching in an ASP.NET Core application. It includes examples of how to use `SemaphoreSlim` to control concurrent access to the cache and how to handle cache invalidation using `MediatR`.

---

## Project Structure

- **`Controllers/TodosController.cs`**: Handles HTTP requests for managing ToDo items.
- **`EventHandler/CacheInvalidationTodoHandler.cs`**: Handles cache invalidation events.
- **`Infrastructure/Caching/CacheService.cs`**: Provides caching functionality using Redis.
- **Models, Entities, Events**: Define the data models, entities, and events used in the application.

---

## Key Components

### TodosController
Handles CRUD operations for ToDo items and uses the `CacheService` to cache the list of ToDo items.

### CacheService
Provides methods to:
- Retrieve or create cached values.
- Remove cached values.

Uses `SemaphoreSlim` to ensure that only one thread can access the cache creation logic at a time.

### CacheInvalidationTodoHandler
Handles events related to ToDo item creation and deletion to invalidate the cache.

---

## Running the Project

1. Ensure you have **Docker** installed and running.
2. Start the services with the following command:
   ```bash
   docker compose up -d
    ```
3. The application uses in-memory operations for the database.

### Note on Cache Invalidation in Multi-Server Environments
In multi-server environments, consider using a message broker (e.g., RabbitMQ, Kafka) to handle cache invalidation across servers, ensuring data consistency.