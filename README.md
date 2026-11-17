# TestProject — Clean Architecture Full-Stack Template

โปรเจกต์โครงสร้างมาตรฐานระดับ Production พร้อมใช้งานทันที (Plug & Play) ด้วย Docker Compose หรือรันแยกบนเครื่อง:

- **Backend**: C# / **.NET 10**, **ASP.NET Core Web API**, **Swagger (OpenAPI)**, **Entity Framework Core**, **PostgreSQL**
- **Frontend**: **Vue 3** (`<script setup>` + TypeScript), **Vuetify 3**, **Pinia**, **Vue Router**, **Axios**, **Vite**
- **Architecture & Patterns**:
  - **Clean Architecture** (4 Layers: `Domain` → `Application` → `Infrastructure` → `WebApi`)
  - **CQRS** (Command-Query Responsibility Segregation) แยกฝั่ง Command (`IProductRepository` + `IUnitOfWork`) และฝั่ง Query (`IProductReadStore` แบบ `AsNoTracking()`)
  - **Mediator Pattern** (`IMediator`, `ICommand<T>`, `IQuery<T>`, `IPipelineBehavior<,>` พร้อม `LoggingBehavior` และ `ValidationBehavior` ด้วย FluentValidation)
  - **SOLID Principles** (DIP, SRP, OCP, ISP)

---

## 1. Quick Start (วิธีที่ง่ายที่สุด — ใช้ Docker Compose)

> เพียงมี Docker Desktop ก็รันได้ทั้ง **PostgreSQL + .NET 10 API + Vue 3 Frontend** ทันที โดยอัตโนมัติ (สร้าง Database, Run EF Core Migrations และ Seed ข้อมูลตัวอย่างให้เลย)

```bash
docker compose up --build
```

เมื่อรันเสร็จ สามารถเปิดใช้งานได้ทันที:

| Service | URL |
|---|---|
| **Frontend (Vue 3 + Vuetify)** | http://localhost:3000 |
| **Swagger UI (.NET 10 API)** | http://localhost:5000/swagger |
| **Health Check** | http://localhost:5000/health |
| **PostgreSQL** | `localhost:5432` (`db: testproject_db`, `user/pass: postgres`) |

---

## 2. Project Structure

```text
Test-Project/
├── docker-compose.yml
├── .env.example
├── backend/
│   ├── TestProject.sln
│   ├── Directory.Build.props              # Shared .NET 10 & analyzer settings
│   ├── Directory.Packages.props           # Central Package Management (CPM)
│   ├── Dockerfile
│   ├── src/
│   │   ├── TestProject.Domain/            # Entities, Enums, Domain Events, Result<T>, Exceptions (Zero dependencies)
│   │   ├── TestProject.Application/       # CQRS Commands/Queries, Mediator, Pipeline Behaviors, Validators, Interfaces
│   │   ├── TestProject.Infrastructure/    # EF Core, Npgsql (PostgreSQL), Repositories, ReadStores, Migrations, Seeder
│   │   └── TestProject.WebApi/            # Controllers, Swagger, ProblemDetails ExceptionHandler, HealthChecks
│   └── tests/
│       └── TestProject.UnitTests/         # xUnit tests for Domain & CQRS Handlers + Mediator Pipeline
└── frontend/
    ├── Dockerfile
    ├── nginx.conf                         # Reverse proxy (/api, /swagger) + SPA routing
    ├── vite.config.ts
    └── src/
        ├── plugins/vuetify.ts             # Vuetify 3 theme & component defaults
        ├── router/index.ts
        ├── services/                      # Typed Axios HTTP clients
        ├── stores/                        # Pinia state management
        ├── layouts/                       # AppBar + Navigation Drawer + Dark/Light toggle
        ├── components/                    # Reusable dialogs (Form, Confirm Delete)
        └── views/                         # Dashboard & Products CRUD Data Table
```

---

## 3. Run Locally Without Docker (สำหรับ Development ผ่าน Kestrel + Windows PostgreSQL)

### 3.1 Database (PostgreSQL)
- **กรณีใช้ PostgreSQL ที่ติดตั้งเป็น Windows Service บนเครื่องอยู่แล้ว**: ตรวจสอบให้แน่ใจว่า service ทำงานที่พอร์ต `5432` และปรับ `ConnectionStrings:DefaultConnection` ใน `backend/src/TestProject.WebApi/appsettings.Development.json` ให้ตรงกับ user/password ของเครื่อง
- **กรณีไม่มี PostgreSQL บนเครื่อง**: เปิดเฉพาะ DB ผ่าน Docker ได้ด้วย:
  ```bash
  docker compose up -d postgres
  ```

### 3.2 Run Backend (.NET 10 Kestrel)
```bash
cd backend/src/TestProject.WebApi
dotnet run
```
- Swagger UI: `https://localhost:7114/swagger` (และเปิด HTTP ที่ `http://localhost:5000` ควบคู่กัน)
- ระบบจะรัน EF Core Migration และ Seed ข้อมูลตัวอย่างให้อัตโนมัติเมื่อเริ่มทำงานในโหมด `Development`

### 3.3 Run Frontend (Vue 3 + Vuetify)
```bash
cd frontend
pnpm install
# หากใช้ pnpm v10/v11 และติดแจ้งเตือน build scripts ให้รัน: pnpm approve-builds (เลือก esbuild, vue-demi)
pnpm dev
```
- Web UI: `http://localhost:5173` (Vite proxy `/api` → `http://localhost:5000`)

---

## 4. EF Core Migration Commands

```bash
cd backend
dotnet ef migrations add <MigrationName> \
  --project src/TestProject.Infrastructure \
  --startup-project src/TestProject.WebApi \
  --output-dir Persistence/Migrations
```

---

## 5. API Endpoints (`/api/v1/products`)

| Method | Endpoint | Pattern | Description |
|---|---|---|---|
| `GET` | `/api/v1/products` | Query | List products with search, filter, sort, pagination |
| `GET` | `/api/v1/products/summary` | Query | Dashboard inventory summary metrics |
| `GET` | `/api/v1/products/{id}` | Query | Get product details by ID |
| `POST` | `/api/v1/products` | Command | Create a new product (validated via FluentValidation) |
| `PUT` | `/api/v1/products/{id}` | Command | Update an existing product |
| `DELETE` | `/api/v1/products/{id}` | Command | Delete a product |
