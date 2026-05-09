# AI Context — TrackYourTasksAPI (In-Memory Cache Integration)

Last updated: 2026-05-09

## Purpose
This document captures everything an AI (or a developer unfamiliar with the repo) needs to understand the current TrackYourTasksAPI implementation with the in-memory cache. It documents project metadata, file layout, important code behaviors, configuration keys, how caching is implemented, and operational notes for local/dev and deployment.

---

## Project metadata
- Target framework: `.NET 8`
- C# language version: `12.0`
- Project root repository: `TrackYourTasksAPI` (branch: `master`)
- Primary runtime: Kestrel via `dotnet TrackYourTasksAPI.dll`

---

## File map (important files)
- `Program.cs` — app startup and DI registration
- `Controllers/TasksController.cs` — HTTP API surface (`GET`, `POST`, `PUT`, `DELETE`)
- `Services/MongoTaskService.cs` — data access + in-memory caching
- `Models/TrackTask.cs` — data model persisted in MongoDB
- `Dockerfile` — multi-stage Docker build used for Render deployment
- `appsettings.json`, `appsettings.Development.json` — configuration (Mongo connection, optional cache configs)
- `AI_CONTEXT.md` — this file

---

## Data model
`Models/TrackTask.cs` (fields)
- `Id` (string): MongoDB ObjectId, decorated with `[BsonId]` and `[BsonRepresentation(BsonType.ObjectId)]`
- `Title` (string)
- `Description` (string)
- `IsCompleted` (bool)
- `IsSkipped` (bool)
- `IsPartiallyCompleted` (bool)
- `CreatedAt` (DateTime): default `DateTime.Now`

---

## Controller surface
`Controllers/TasksController.cs`
- Route: `api/tasks`
- Endpoints:
  - `GET api/tasks` => `Get()` returns `Task<List<TrackTask>>`
  - `POST api/tasks` => `Create(TrackTask task)` returns `Task`
  - `PUT api/tasks/{id}` => `Update(string id, TrackTask task)` returns `Task`
  - `DELETE api/tasks/{id}` => `Delete(string id)` returns `Task`
- No route-level caching; caching is implemented in the service layer.

---

## Service: `Services/MongoTaskService.cs` — responsibilities
- Connects to MongoDB using `IConfiguration` keys:
  - `MongoDb:ConnectionString` (env var alternative `MongoDb__ConnectionString`)
  - `MongoDb:DatabaseName` (env var alternative `MongoDb__DatabaseName`)
- Performs CRUD against collection `TYTTasks`
- Implements in-memory caching via `IMemoryCache` (injected via DI)

Key behaviors:
- Cache key for full-list: `tasks:all` (constant `AllTasksCacheKey`)
- `GetAsync()` uses:
  - `_cache.GetOrCreateAsync(AllTasksCacheKey, async entry => { ... })`
  - Cache policy inside factory: `AbsoluteExpirationRelativeToNow = 5 minutes`, `SlidingExpiration = 2 minutes`
  - On cache miss, reads all tasks from Mongo and returns them; the result is cached.
- `CreateAsync(TrackTask)`, `UpdateAsync(string, TrackTask)`, `DeleteAsync(string)`:
  - Perform the DB operation, then call `_cache.Remove(AllTasksCacheKey)` to invalidate the cached full-list.

Notes on injection:
- `Program.cs` registers:
  - `builder.Services.AddMemoryCache();`
  - `builder.Services.AddSingleton<MongoTaskService>();` — service constructor signature is `MongoTaskService(IConfiguration config, IMemoryCache cache)`

Concurrency/consistency notes:
- `GetOrCreateAsync` provides atomic creation per cache instance but does not provide distributed locks across multiple processes. For the single-instance/small user base scenario this is acceptable.
- Cache is per application instance (in-memory). If you scale to multiple instances, the cache will not be shared — consider Redis for distributed caching.

---

## Configuration keys & environment variables
- Mongo:
  - `MongoDb:ConnectionString` or `MongoDb__ConnectionString`
  - `MongoDb:DatabaseName` or `MongoDb__DatabaseName`
- No Redis/other cache configuration required for in-memory mode.

---

## Deployment and runtime notes
- Dockerfile (multi-stage) builds and publishes the app for .NET 8.
- For Render or other PaaS:
  - Ensure `ASPNETCORE_URLS` is set to bind to their `$PORT` (Dockerfile uses `ENV ASPNETCORE_URLS=http://+:$PORT`)
  - Add the MongoDB connection values as environment variables (do not embed secrets in the image)
- Local run:
  - `dotnet run` from project folder
  - Or __F5__ / __Ctrl+F5__ in Visual Studio

---

## Quick troubleshooting & best practices
- If GET returns stale data, create/update/delete should remove `tasks:all` — verify those methods run successfully and that the cache key matches exactly.
- Avoid hardcoding URLs in code (for Render/prod): search for `UseUrls("http://localhost:5000")`. If present, prefer this guard: