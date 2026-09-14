# CoffeeNChill Canteen Management System-Part 1

## Overview

CoffeeNChill is a campus canteen management system being modernized from paper-based menus, order slips, and filing-cabinet recipe documents into a cloud-enabled microservices system. The project is built incrementally across three parts, covering Azure Storage, Serverless Functions, Docker Containerization, Web APIs, JWT Authentication, and VPS Hosting.

This repository covers **Part 1 -Azure Functions, Tables, Files & Docker Hub**, which lays the cloud foundation: Azure Table Storage for the digital menu, an Azure File Share for staff operational documents, and HTTP-triggered Azure Functions running locally in isolated Docker containers via Azurite emulation.

## Project Structure

```
/
├── docs/
│   └── screenshots/
├── Properties/
├── Functions/
├── Models/
├── Services/
├── .dockerignore
├── .gitignore
├── CoffeeNChill.csproj
├── CoffeeNChill.slnx
├── Dockerfile
├── host.json
├── local.settings.json    (gitignored — not committed)
├── Program.cs
└── README.md
```

## Local Setup

1. Clone the repository.
2. Open `CoffeeNChill.slnx` in Visual Studio 2022+.
3. Restore NuGet packages: `Azure.Data.Tables`, `Microsoft.Azure.Functions.Worker.Extensions.Http`, and (for document functions) `Azure.Storage.Files.Shares`, `Microsoft.AspNetCore.WebUtilities`.
4. Ensure `local.settings.json` contains:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     }
   }
   ```
5. Start Azurite (locally or via Docker — see below) before running the Functions project.
6. Run the project (F5 or `dotnet run`) to start the Functions host locally at `http://localhost:7071`.

## Docker Commands

Run Azurite in its own container, binding the default storage ports:

```
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

Build and tag the Functions image:

```
docker build -t <dockerhub_username>/coffeenchill-functions:v1.0 .
```

Run the Functions container standalone, connecting it to the Azurite container host:

```
docker run -p 7071:80 -e AzureWebJobsStorage="UseDevelopmentStorage=true" <dockerhub_username>/coffeenchill-functions:v1.0
```
### Overview
The CoffeeNChill Functions app is containerized using a multi-stage Dockerfile,
built on the official Azure Functions .NET 10 isolated-worker runtime image.
No Docker Compose is used in Part 1 — each container is built and run as a
standalone unit via `docker run`, per the assignment requirements.

### Dockerfile
The Dockerfile has four stages:
- `base` — the lightweight Azure Functions runtime image the final container runs on.
- `build` — uses the full .NET 10 SDK to restore and compile the project.
- `publish` — publishes the compiled output for deployment.
- `final` — copies only the published output onto the lightweight base image,
  keeping the final image small and free of build-time tooling.

### Verification
Both containers were confirmed running via `docker ps`, and the Functions
container's logs (`docker logs coffeenchill-api`) confirmed a successful
connection to Azurite with no storage errors. The full Postman collection
(all 8 endpoints) was run against the Dockerized container to confirm
end-to-end functionality.

## Endpoints

### Menu (Azure Table Storage)

| Method | Route | Description |
|---|---|---|
| POST | `/api/menu` | Creates a new menu item |
| GET | `/api/menu` | Returns all menu items |
| GET | `/api/menu/category/{category}` | Returns menu items filtered by category |
| PUT | `/api/menu/{category}/{id}` | Updates price and/or availability of a menu item |
| DELETE | `/api/menu/{category}/{id}` | Deletes a menu item |

### Documents (Azure File Share — in progress)

| Method | Route | Description |
|---|---|---|
| POST | `/api/documents/upload` | Uploads a file to the staff-docs share |
| GET | `/api/documents` | Lists all stored operational files |
| GET | `/api/documents/download/{fileName}` | Downloads a stored file |

## Progress So Far

### Completed (Menu Functions)

- Created project folder structure (`Functions`, `Models`, `Services`, `docs`).
- Implemented `MenuItem` model (`Models/MenuItem.cs`) implementing `ITableEntity`, with `PartitionKey` (category), `RowKey` (SKU), `Name`, `Description`, `Price`, and `IsAvailable`.
- Implemented `TableStorageService` (`Services/TableStorageService.cs`) with Create, GetAll, GetByCategory, Update, and Delete methods against the `MenuItems` table, including ETag-based optimistic concurrency on updates.
- Implemented all five Menu HTTP-triggered functions: `CreateMenuItem`, `GetAllMenuItems`, `GetMenuItemsByCategory`, `UpdateMenuItem`, `DeleteMenuItem`.
- Registered `TableStorageService` in `Program.cs` for constructor injection into the Menu functions.
- Removed the default `Function1.cs` scaffold stub.
- Installed the `Azure.Data.Tables` NuGet package (12.12.0).
- Installed the `Azure.Blob.Storage` NuGet package
- Completed Document Functions (`UploadStaffDocument`, `ListStaffDocuments`, `DownloadStaffDocument`) and `FileShareService`. 

### In Progress / Pending

- Docker containerization: finalizing the Dockerfile, building and pushing `coffeenchill-functions:v1.0` and `coffeenchill-azurite:v1.0` to Docker Hub.
- Postman collection covering all eight endpoints.
- Full integration test against the containerized setup.
- Demo video recording.

## Team Contributions

*To be completed once all parts are merged — each member's section should summarize their contribution area (Menu Functions, Document Functions, Docker & Containerization, Postman/README/Video).*

## Reference List

Microsoft, 2025. *Azure.Data.Tables client library for .NET*. [Source code] Available at: <https://learn.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme> [Accessed 02 September 2026].

Microsoft, 2025. *Azure Functions HTTP trigger for .NET isolated worker functions*. [online] Available at: <https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?pivots=programming-language-csharp> [Accessed 02 September 2026].

Microsoft, 2025. *Dependency injection in .NET Azure Functions*. [online] Available at: <https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection> [Accessed 02 September 2026].

Microsoft, 2025. *Quickstart: Azure Blob Storage Client Library for .NET*. [omline]
Available at: <https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=visual-studio%2Cmanaged-identity%2Croles-azure-portal%2Csign-in-azure-cli%2Cidentity-visual-studio&pivots=blob-storage-quickstart-scratch>
[Accessed 09 September 2026].

Microsoft, 2025. *Azure Functions HTTP trigger*. [online]
Available at: <https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=python-v2%2Cisolated-process%2Cnodejs-v4%2Cfunctionsv2&pivots=programming-language-csharp>[Accessed 09 September 2026].

Microsoft, 2026. *Guide for running C# Azure Functions in an isolated worker model*.[online] Available at: <https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=ihostapplicationbuilder%2Cconfig%2Cwindows>[Accessed 09 September].

## Annexure

Disclosure of AI Usage in our Assesment 

Section within the assessment in which generative AI was used: Blob Storage

Name of AI tool used: Claude

Purpose: Correcting and generate the right code

Date: 10 September 2026

Link: <https://claude.ai/chat/c8712ed9-d9d6-4503-9ca5-ba62c56df2fb>


