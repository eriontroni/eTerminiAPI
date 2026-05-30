# eTermini — Platforma Digjitale e Termineve

eTermini është një platformë e menaxhimit të termineve për institucionet publike në Kosovë. Qytetarët mund të rezervojnë termine në institucione si spitale, komuna dhe institucione të tjera publike, ndërkohë që stafi dhe administratorët menaxhojnë oraret, shërbimet dhe departamentet.

---

## Përmbajtja

- [Arkitektura e Sistemit](#arkitektura-e-sistemit)
- [Stack Teknologjik](#stack-teknologjik)
- [Struktura e Projektit](#struktura-e-projektit)
- [Konfigurimi dhe Instalimi](#konfigurimi-dhe-instalimi)
- [Autentifikimi dhe Autorizimi](#autentifikimi-dhe-autorizimi)
- [API Referenca — eTerminiAPI](#api-referenca--eterminiapi)
- [API Referenca — eTerminiAdminAPI](#api-referenca--eterminiadminapi)
- [Baza e të Dhënave](#baza-e-të-dhënave)
- [Funksionet Real-time](#funksionet-real-time)
- [Multi-tenancy](#multi-tenancy)
- [Variablat e Mjedisit](#variablat-e-mjedisit)

---

## Arkitektura e Sistemit

```
┌─────────────────────────────────────────────────────────────────┐
│                        KLIENTI (Browser)                        │
│                                                                 │
│   ┌───────────────────────┐   ┌───────────────────────────┐    │
│   │   eTerminiUI           │   │   eTerminiAdminUI          │    │
│   │   (Portal Qytetar)     │   │   (Panel Administrativ)    │    │
│   │   React 19 + Vite      │   │   React 19 + Vite          │    │
│   │   Port: 5173           │   │   Port: 5175               │    │
│   └──────────┬────────────┘   └────────────┬──────────────┘    │
└──────────────┼─────────────────────────────┼───────────────────┘
               │ HTTP + SignalR               │ HTTP
               ▼                             ▼
┌──────────────────────────┐   ┌──────────────────────────────┐
│      eTerminiAPI          │   │      eTerminiAdminAPI         │
│   (API Publike)           │   │   (API Administrative)        │
│   .NET 10 / C#            │   │   .NET 10 / C#                │
│   Port: 44305             │   │   Port: 44306                 │
│                           │   │                               │
│  ┌─────────────────────┐  │   │  ┌─────────────────────────┐ │
│  │  Application Layer  │  │   │  │   Application Layer      │ │
│  │  (DTOs, Interfaces) │  │   │  │   (DTOs, Interfaces)     │ │
│  └─────────────────────┘  │   │  └─────────────────────────┘ │
│  ┌─────────────────────┐  │   │  ┌─────────────────────────┐ │
│  │  Domain Layer       │  │   │  │   Domain Layer           │ │
│  │  (Entities, Enums)  │  │   │  │   (Entities, Enums)      │ │
│  └─────────────────────┘  │   │  └─────────────────────────┘ │
│  ┌─────────────────────┐  │   │  ┌─────────────────────────┐ │
│  │  Infrastructure     │  │   │  │   Infrastructure         │ │
│  │  (EF Core, Redis,   │  │   │  │   (EF Core, Services)   │ │
│  │   Services, Repos)  │  │   │  └─────────────────────────┘ │
│  └─────────────────────┘  │   └──────────────┬───────────────┘
└──────────────┬────────────┘                  │
               │                               │
               └──────────────┬────────────────┘
                              ▼
              ┌───────────────────────────────┐
              │        SQL Server              │
              │   (Azure SQL — eTerminiDB)     │
              └───────────────────────────────┘
                              │
              ┌───────────────┴───────────────┐
              │         Redis Cache            │
              │     localhost:6379             │
              │   (Slot disponueshmëri)        │
              └───────────────────────────────┘
```

### Modeli Arkitekturor

Projekti ndjek **Clean Architecture** me 3 shtresa:

| Shtresa | Projekti | Përmbajtja |
|---------|----------|------------|
| **Domain** | `eTerminiAPI.Domain` | Entitetet, Enum-et, Kontratat e autorizimit |
| **Application** | `eTerminiApi.Application` | DTO-të, Interfaqet e shërbimeve |
| **Infrastructure** | `eTerminiAPI.Infrastructure` | EF Core DbContext, Implementimet e shërbimeve, Repozitoriet |

---

## Stack Teknologjik

### Backend
| Teknologjia | Versioni | Qëllimi |
|------------|---------|---------|
| .NET | 10.0 | Runtime i APIs |
| C# | 13 | Gjuha e programimit |
| ASP.NET Core | 10.0 | Web framework |
| Entity Framework Core | 10.0.5 | ORM — akses i bazës së të dhënave |
| SQL Server | Azure SQL | Baza e të dhënave |
| Redis | — | Cache i sloteve të disponueshme |
| SignalR | 10.0 | Komunikim real-time (WebSocket) |
| JWT Bearer | — | Autentifikim me token |
| Serilog | — | Logging (Console + File) |
| Swagger | — | Dokumentim interaktiv i API |

### Frontend
| Teknologjia | Versioni | Qëllimi |
|------------|---------|---------|
| React | 19.2.4 | UI framework |
| Vite | 8.0.4 | Build tool dhe dev server |
| TailwindCSS | 4.3.0 | Stilizim |
| Axios | 1.16.0 | HTTP klient |
| React Router DOM | 7.15.0 | Routing |
| Microsoft SignalR | 10.0.0 | Real-time komunikim |
| QRCode.React | 4.2.0 | Gjenerimi i QR kodeve |
| Lucide React | — | Ikonat |

---

## Struktura e Projektit

```
eTermini/
│
├── eTerminiAPI/                        # API Publike
│   ├── eTerminiAPI/
│   │   ├── Controllers/               # 9 kontrollorë
│   │   │   ├── AuthController.cs
│   │   │   ├── AppointmentsController.cs
│   │   │   ├── TimeSlotsController.cs
│   │   │   ├── CatalogController.cs
│   │   │   ├── DepartmentsController.cs
│   │   │   ├── InstitutionsController.cs
│   │   │   ├── TenantsController.cs
│   │   │   └── StatisticsController.cs
│   │   ├── Hubs/
│   │   │   └── AppointmentHub.cs      # SignalR Hub
│   │   ├── Program.cs                 # Konfigurimi i aplikacionit
│   │   └── appsettings.json
│   │
│   ├── eTerminiApi.Application/
│   │   ├── DTOs/                      # Request/Response modelet
│   │   └── Interfaces/                # Kontratat e shërbimeve
│   │
│   ├── eTerminiAPI.Domain/
│   │   ├── Entities/                  # 19 entitete të bazës së të dhënave
│   │   ├── Enums/                     # UserRole, AppointmentStatus
│   │   └── Authorization/             # Definicionet e permiseve
│   │
│   └── eTerminiAPI.Infrastructure/
│       ├── Persistence/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/
│       ├── Services/                  # 7 shërbime biznesi
│       └── Repositories/
│
├── eTerminiAdminAPI/                  # API Administrative
│   ├── eTerminiAdminAPI.API/
│   │   ├── Controllers/               # 9 kontrollorë adminësh
│   │   │   ├── AdminAuthController.cs
│   │   │   ├── DashboardController.cs
│   │   │   ├── TenantsController.cs
│   │   │   ├── InstitutionsController.cs
│   │   │   ├── DepartmentsController.cs
│   │   │   ├── WorkersController.cs
│   │   │   ├── AdministratorsController.cs
│   │   │   ├── RolesController.cs
│   │   │   └── SystemController.cs
│   │   ├── Authorization/
│   │   │   └── HasPermissionAttribute.cs
│   │   └── Program.cs
│   │
│   ├── eTerminiAdminAPI.Application/
│   └── eTerminiAdminAPI.Infrastructure/
│
├── eTerminiUI/                        # Frontend Qytetar
│   ├── src/
│   │   ├── api/                       # Axios instancat
│   │   ├── components/                # Komponentët React
│   │   ├── context/                   # Auth Context
│   │   ├── pages/                     # Faqet
│   │   ├── routes/                    # Konfigurimi i rutave
│   │   └── hooks/
│   ├── .env
│   └── package.json
│
└── eTerminiAdminUi/                   # Frontend Administrativ
    ├── src/
    │   ├── api/
    │   ├── components/
    │   ├── pages/
    │   └── routes/
    └── package.json
```

---

## Konfigurimi dhe Instalimi

### Kërkesat paraprake

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (ose lidhje me Azure SQL)
- Redis (lokalisht ose Docker)

### 1. Klono Repozitorin

```bash
git clone https://github.com/your-org/eTermini.git
cd eTermini
```

### 2. Konfiguro Bazën e të Dhënave

Ndrysho `ConnectionStrings:DefaultConnection` në `appsettings.json` të çdo API:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=eTerminiDB;User Id=YOUR_USER;Password=YOUR_PASS;...",
    "Redis": "localhost:6379"
  }
}
```

### 3. Apliko Migracionet

```bash
cd eTerminiAPI
dotnet ef database update --project eTerminiAPI.Infrastructure --startup-project eTerminiAPI
```

### 4. Nis APIs

```bash
# API Publike
cd eTerminiAPI/eTerminiAPI
dotnet run

# API Administrative (terminal tjetër)
cd eTerminiAdminAPI/eTerminiAdminAPI.API
dotnet run
```

### 5. Instalo dhe Nis Frontend

```bash
# Portal Qytetar
cd eTerminiUI
npm install
npm run dev

# Panel Administrativ (terminal tjetër)
cd eTerminiAdminUi
npm install
npm run dev
```

### 6. Ports Default

| Shërbimi | URL |
|----------|-----|
| eTerminiAPI | https://localhost:44305 |
| eTerminiAdminAPI | https://localhost:44306 |
| eTerminiUI | http://localhost:5173 |
| eTerminiAdminUI | http://localhost:5175 |
| Swagger (API) | https://localhost:44305/swagger |
| Swagger (Admin) | https://localhost:44306/swagger |

---

## Autentifikimi dhe Autorizimi

### Rrjedha e Autentifikimit

```
Qytetari                    eTerminiAPI
    │                           │
    │── POST /api/auth/login ──►│
    │                           │  Verifikon kredencialet
    │◄── { accessToken,         │  Gjeneron JWT + RefreshToken
    │       refreshToken } ─────│
    │                           │
    │── GET /api/appointments ──►│ (me Authorization: Bearer <token>)
    │   [Header: Bearer token]  │  Verifikon token-in
    │◄── 200 OK ────────────────│  Kthen të dhënat
    │                           │
    │  [Token skadon pas 60 min]│
    │── POST /api/auth/refresh ─►│ (me refreshToken)
    │◄── { accessToken } ───────│  Token i ri
```

### JWT Token — Claims

| Claim | Vlera |
|-------|-------|
| `sub` / `NameIdentifier` | User ID (Guid) |
| `email` | Email e përdoruesit |
| `role` | Citizen / Staff / InstitutionAdmin / SuperAdmin |
| `fullName` | Emri i plotë |
| `tenantId` | ID e tenantit (për multi-tenancy) |

### Rolet e Përdoruesit

| Roli | Përshkrimi |
|------|-----------|
| `Citizen` | Qytetar — rezervon termine |
| `Staff` | Staf institucioni — menaxhon terminet |
| `InstitutionAdmin` | Admin institucioni — menaxhon stafin dhe oraret |
| `SuperAdmin` | Admin i sistemit — akses i plotë |

### Autorizimi Administrativ — Sistemi i Permiseve

AdminAPI përdor autorizim të bazuar në permise (RBAC). Çdo rol admin ka një listë permisesh.

**Formati i permisit**: `modul.veprim`

| Permisia | Përshkrimi |
|---------|-----------|
| `dashboard.view` | Shiko statistikat e dashboard |
| `tenants.view` | Shiko tenantët |
| `tenants.create_update` | Krijo / ndrysho tenantë |
| `tenants.delete` | Fshi tenantë |
| `institutions.view` | Shiko institucionet |
| `institutions.create_update` | Krijo / ndrysho institucione |
| `workers.view` | Shiko punëtorët |
| `workers.create_update` | Krijo / ndrysho punëtorë |
| `administrators.view` | Shiko adminëtë |
| `administrators.create_update` | Krijo / ndrysho adminë |
| `administrators.delete` | Fshi adminë |
| `system.view` | Shiko logjet dhe auditet e sistemit |

> **SuperAdmin** ka qasje automatike në të gjitha permiset, pavarësisht rolit të caktuar.

---

## API Referenca — eTerminiAPI

**Base URL**: `https://localhost:44305/api`

Çdo endpoint që kërkon autentifikim duhet të ketë header:
```
Authorization: Bearer <accessToken>
```

---

### Autentifikimi — `/api/auth`

#### `POST /api/auth/register`
Regjistron një qytetar të ri.

**Body:**
```json
{
  "firstName": "Artan",
  "lastName": "Krasniqi",
  "email": "artan@example.com",
  "password": "P@ssword123",
  "phoneNumber": "+38344123456",
  "tenantId": "guid-tenant-id"
}
```

**Response `200`:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123def456..."
}
```

---

#### `POST /api/auth/login`
Kyçja e përdoruesit.

**Body:**
```json
{
  "email": "artan@example.com",
  "password": "P@ssword123"
}
```

**Response `200`:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123def456..."
}
```

---

#### `GET /api/auth/me` `[Authorized]`
Kthen të dhënat e përdoruesit aktual.

**Response `200`:**
```json
{
  "id": "guid",
  "firstName": "Artan",
  "lastName": "Krasniqi",
  "email": "artan@example.com",
  "role": "Citizen",
  "tenantId": "guid"
}
```

---

#### `POST /api/auth/refresh`
Rifresko access token-in me refresh token.

**Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

---

#### `POST /api/auth/revoke`
Anulon refresh token-in (çkyçja).

**Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

---

### Terminet — `/api/appointments` `[Authorized]`

#### `POST /api/appointments`
Krijon një termin të ri.

**Body:**
```json
{
  "serviceId": "guid",
  "staffMemberId": "guid",
  "appointmentDate": "2026-06-15",
  "timeSlotId": "guid",
  "notes": "Kontroll rutinë"
}
```

---

#### `GET /api/appointments`
Kthen të gjitha terminet e tenantit aktual.

---

#### `GET /api/appointments/my`
Kthen terminet e përdoruesit të kyçur.

**Response `200`:**
```json
[
  {
    "id": "guid",
    "serviceName": "Kontroll i Përgjithshëm",
    "staffName": "Dr. Blerim Hoxha",
    "appointmentDate": "2026-06-15",
    "status": "Pending",
    "notes": "Kontroll rutinë"
  }
]
```

---

#### `GET /api/appointments/{id}`
Kthen detajet e një termini.

---

#### `PUT /api/appointments/{id}/status`
Ndryshon statusin e terminit.

**Body:**
```json
{
  "status": "Confirmed"
}
```

**Statuset e mundshme:** `Pending`, `Confirmed`, `Cancelled`, `Completed`, `NoShow`

---

#### `PUT /api/appointments/{id}/reschedule`
Ri-cakton datën dhe kohën e terminit.

**Body:**
```json
{
  "newDate": "2026-06-20",
  "newTimeSlotId": "guid"
}
```

---

#### `DELETE /api/appointments/{id}`
Fshin terminin (soft delete).

---

### Slotet Kohore — `/api/timeslots` `[Authorized]`

#### `GET /api/timeslots/available`
Kthen slotet e disponueshme për një staf të caktuar në një datë.

**Query Parameters:**

| Parametri | Tipi | Detyrues | Përshkrimi |
|-----------|------|----------|-----------|
| `doctorId` | Guid | Po | ID e stafit/mjekut |
| `date` | string | Po | Data (`yyyy-MM-dd`) |
| `durationMinutes` | int | Jo | Kohëzgjatja (default: 30) |

**Shembull:**
```
GET /api/timeslots/available?doctorId=guid&date=2026-06-15&durationMinutes=30
```

**Response `200`:**
```json
[
  {
    "id": "guid",
    "startTime": "09:00",
    "endTime": "09:30",
    "isAvailable": true
  }
]
```

> Slotet cache-ohen në Redis për 5 minuta. Ndryshimet transmetohen në kohë reale nëpërmjet SignalR.

---

#### `GET /api/timeslots/check`
Kontrollon nëse një slot specifik është i lirë.

**Query Parameters:**

| Parametri | Tipi | Detyrues |
|-----------|------|----------|
| `doctorId` | Guid | Po |
| `startTime` | DateTime | Po |
| `durationMinutes` | int | Jo |

---

### Katalogu — `/api/catalog` `[AllowAnonymous]`

#### `GET /api/catalog/categories`
Kthen të gjitha kategoritë e shërbimeve.

**Response `200`:**
```json
[
  {
    "id": "guid",
    "name": "Shëndetësi",
    "description": "Shërbime shëndetësore"
  }
]
```

---

#### `GET /api/catalog/institutions`
Kthen listën e institucioneve.

**Query Parameters:**

| Parametri | Tipi | Detyrues |
|-----------|------|----------|
| `categoryId` | Guid | Jo |

---

#### `GET /api/catalog/services`
Kthen listën e shërbimeve.

**Query Parameters:**

| Parametri | Tipi | Detyrues |
|-----------|------|----------|
| `institutionId` | Guid | Jo |
| `categoryId` | Guid | Jo |

---

#### `GET /api/catalog/services/{serviceId}`
Kthen detajet e një shërbimi.

---

#### `GET /api/catalog/services/{serviceId}/providers`
Kthen listën e stafit që ofron këtë shërbim.

---

### Departamentet — `/api/departments` `[Authorized]`

#### `GET /api/departments`
Kthen të gjitha departamentet e tenantit aktual.

#### `GET /api/departments/{id}`
Kthen detajet e një departamenti.

#### `POST /api/departments`
Krijon departament të ri.

**Body:**
```json
{
  "name": "Kardiologji",
  "description": "Departamenti i sëmundjeve të zemrës",
  "institutionId": "guid",
  "branchId": "guid"
}
```

#### `PUT /api/departments/{id}`
Ndryshon departamentin.

#### `DELETE /api/departments/{id}`
Fshin departamentin (soft delete).

---

### Institucionet — `/api/institutions` `[Authorized]`

#### `GET /api/institutions/context`
Kthen kontekstin e institucionit të përdoruesit aktual (qyteti + emri i tenantit).

**Response `200`:**
```json
{
  "city": "Prishtinë",
  "tenantName": "Komuna e Prishtinës"
}
```

---

#### `GET /api/institutions/search`
Kërkon institucione sipas emrit.

**Query Parameters:**

| Parametri | Tipi | Kushtëzim |
|-----------|------|-----------|
| `q` | string | Min. 2 karaktere |

**Response:** Maksimum 20 rezultate.

---

### Tenantët — `/api/tenants` `[AllowAnonymous]`

#### `GET /api/tenants`
Kthen të gjithë tenantët aktiv (me faqëzim).

**Query Parameters:**

| Parametri | Tipi | Detyrues |
|-----------|------|----------|
| `page` | int | Jo (default: 1) |
| `pageSize` | int | Jo (default: 10) |

---

#### `GET /api/tenants/count`
Kthen numrin e tenantëve aktiv.

---

### Statistikat — `/api/statistics` `[AllowAnonymous]`

#### `GET /api/statistics`
Kthen statistikat e platformës.

**Response `200`:**
```json
{
  "totalCities": 38,
  "totalInstitutions": 142,
  "totalUsers": 15340
}
```

---

## API Referenca — eTerminiAdminAPI

**Base URL**: `https://localhost:44306/api/admin`

Të gjitha endpoint-et kërkojnë:
1. `Authorization: Bearer <adminAccessToken>`
2. Permisin e duhur (kontrolluar nga `[HasPermission]`)

---

### Autentifikimi Administrativ — `/api/admin/auth`

#### `POST /api/admin/auth/login`

**Body:**
```json
{
  "email": "admin@etermini.com",
  "password": "AdminPass123!"
}
```

**Response `200`:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "xyz789...",
  "role": "SuperAdmin",
  "permissions": ["tenants.view", "institutions.view", "..."]
}
```

---

#### `POST /api/admin/auth/refresh`
Rifresko token-in e adminit.

---

#### `PATCH /api/admin/auth/change-password` `[Authorized]`

**Body:**
```json
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}
```

---

### Dashboard — `/api/admin/dashboard`

#### `GET /api/admin/dashboard/stats` `[Permission: dashboard.view]`

**Response `200`:**
```json
{
  "totalTenants": 38,
  "totalInstitutions": 142,
  "totalWorkers": 890,
  "totalAppointments": 45230,
  "appointmentsThisMonth": 1234
}
```

---

### Tenantët (Admin) — `/api/admin/tenants`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/tenants` | `tenants.view` | Lista e tenantëve |
| `POST` | `/api/admin/tenants` | `tenants.create_update` | Krijon tenant |
| `DELETE` | `/api/admin/tenants/{id}` | `tenants.delete` | Fshin tenant |

**Body (POST):**
```json
{
  "name": "Komuna e Gjilanit",
  "slug": "gjilan",
  "logoUrl": "https://..."
}
```

---

### Institucionet (Admin) — `/api/admin/institutions`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/institutions` | `institutions.view` | Lista |
| `GET` | `/api/admin/institutions/{id}` | `institutions.view` | Detajet |
| `POST` | `/api/admin/institutions` | `institutions.create_update` | Krijo |
| `PUT` | `/api/admin/institutions/{id}` | `institutions.create_update` | Ndrysho |
| `PATCH` | `/api/admin/institutions/{id}/toggle-active` | `institutions.create_update` | Aktivizo/Çaktivizo |

---

### Departamentet (Admin) — `/api/admin/departments`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/departments` | `institutions.view` | Departamentet sipas institucionit |

---

### Punëtorët — `/api/admin/workers`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/workers` | `workers.view` | Lista e punëtorëve |
| `GET` | `/api/admin/workers/{id}` | `workers.view` | Detajet |
| `POST` | `/api/admin/workers` | `workers.create_update` | Krijon punëtor |
| `PUT` | `/api/admin/workers/{id}` | `workers.create_update` | Ndrysho |
| `PATCH` | `/api/admin/workers/{id}/toggle-active` | `workers.create_update` | Aktivizo/Çaktivizo |
| `PATCH` | `/api/admin/workers/{id}/assign-institution` | `workers.create_update` | Cakto në institucion |

---

### Administratorët — `/api/admin/administrators`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/administrators` | `administrators.view` | Lista e adminëve |
| `POST` | `/api/admin/administrators` | `administrators.create_update` | Krijon admin |
| `PATCH` | `/api/admin/administrators/{id}/toggle-active` | `administrators.create_update` | Aktivizo/Çaktivizo |

---

### Rolet — `/api/admin/roles`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/roles` | `administrators.view` | Lista e roleve |
| `GET` | `/api/admin/roles/permissions-catalog` | `administrators.view` | Katalog i permiseve |
| `GET` | `/api/admin/roles/{id}` | `administrators.view` | Detajet e rolit |
| `POST` | `/api/admin/roles` | `administrators.create_update` | Krijon rol |
| `PUT` | `/api/admin/roles/{id}` | `administrators.create_update` | Ndrysho rol |
| `DELETE` | `/api/admin/roles/{id}` | `administrators.delete` | Fshin rol |

**Body (POST/PUT):**
```json
{
  "name": "Menaxher Institucioni",
  "description": "Menaxhon institucionin dhe stafin",
  "permissions": [
    "institutions.view",
    "workers.view",
    "workers.create_update"
  ]
}
```

---

### Sistemi — `/api/admin/system`

| Metoda | Endpoint | Permisia | Përshkrimi |
|--------|---------|---------|-----------|
| `GET` | `/api/admin/system/logs` | `system.view` | Logjet e auditit (me faqëzim) |
| `GET` | `/api/admin/system/users` | `system.view` | Lista e të gjithë përdoruesve |

**Loget — Query Parameters:**

| Parametri | Tipi | Default |
|-----------|------|---------|
| `page` | int | 1 |
| `pageSize` | int | 50 |
| `from` | DateTime | — |
| `to` | DateTime | — |

---

## Baza e të Dhënave

### Entitetet Kryesore

```
┌──────────┐     ┌─────────────┐     ┌──────────────┐
│  Tenant  │────►│ Institution │────►│  Department  │
└──────────┘     └─────────────┘     └──────┬───────┘
     │                                      │
     │           ┌─────────────┐            │
     └──────────►│    User     │     ┌──────▼──────────┐
                 └──────┬──────┘     │  PublicService  │
                        │            └──────┬──────────┘
                        │                   │
                 ┌──────▼──────┐    ┌───────▼────────┐
                 │StaffMember  │────│    TimeSlot    │
                 └─────────────┘    └───────┬────────┘
                        │                   │
                 ┌──────▼──────────────────▼──────┐
                 │           Appointment           │
                 └─────────────────────────────────┘
```

### Tabelat

| Tabela | Qëllimi |
|--------|---------|
| `Tenants` | Komunat / Rajonet (multi-tenancy) |
| `Users` | Të gjithë përdoruesit e sistemit |
| `AdminRoles` | Rolet e adminëve me permiset e tyre |
| `Institutions` | Institucionet publike |
| `InstitutionBranches` | Degët e institucioneve |
| `Departments` | Departamentet brenda institucioneve |
| `PublicServices` | Shërbimet e ofruara |
| `ServiceCategories` | Kategoritë e shërbimeve |
| `ServiceRequirements` | Kërkesat për çdo shërbim |
| `StaffMembers` | Profilet e stafit / mjekëve |
| `StaffSchedules` | Oraret javore të punës |
| `TimeSlots` | Slotet e disponueshme |
| `Appointments` | Terminet e rezervuara |
| `AppointmentStatusHistory` | Historia e ndryshimeve të statusit |
| `Notifications` | Njoftimet e përdoruesve |
| `NotificationTemplates` | Shabllonet e njoftimeve |
| `RefreshTokens` | Token-et e rifreskimit |
| `TenantSettings` | Konfigurimi sipas tenantit |
| `AuditLogs` | Regjistri i auditit të sistemit |

### Soft Delete

Çdo entitet kryesor ka kolona `IsDeleted` dhe `IsActive`. Fshirja nuk e heq rekorden nga baza, por e shënon si të fshirë. EF Core aplikon automatikisht filtrin global `IsDeleted == false` për të gjitha queries.

---

## Funksionet Real-time

eTerminiAPI përdor **SignalR** për të transmetuar ndryshimet e disponueshmërisë së sloteve.

### Lidhja me Hub

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:44305/hubs/appointments", {
    accessTokenFactory: () => localStorage.getItem("accessToken")
  })
  .withAutomaticReconnect()
  .build();

await connection.start();
```

### Dëgjo ndryshimet e sloteve

```javascript
// Bashkohu në grupin e sloteve për mjekun dhe datën
await connection.invoke("JoinSlotGroup", doctorId, date);

// Dëgjo ndryshimet
connection.on("SlotUpdated", (slot) => {
  console.log("Slot u përditësua:", slot);
});

// Largohesh nga grupi
await connection.invoke("LeaveSlotGroup", doctorId, date);
```

### Formati i Grupit SignalR

Grupet quhen sipas formatit: `slots:{doctorId}:{date}`

Shembull: `slots:3f2504e0-4f89-11d3-9a0c-0305e82c3301:2026-06-15`

---

## Multi-tenancy

Sistemi mbështet multi-tenancy bazuar në **Tenant** (komunë/rajon).

- Çdo entitet kryesor ka kolonën `TenantId`
- JWT token-i përmban `tenantId` të përdoruesit
- Të gjitha queries filtrohen automatikisht sipas `TenantId`
- Citizen-ët mund të shohin vetëm të dhënat e tenantit të tyre

### Rrjedha e Zgjedhjes së Tenantit

```
1. Qytetari hap platformën
2. Zgjedh komunën / qytetin (Tenant)
3. Regjistrohet → tenantId ruhet në llogarinë e tij
4. Kur kyçet → JWT token përfshin tenantId
5. Çdo kërkesë API filtrohet sipas atij tenant
```

---

## Variablat e Mjedisit

### eTerminiAPI — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:SERVER,1433;Initial Catalog=eTerminiDB;...",
    "Redis": "localhost:6379"
  },
  "Cache": {
    "AvailableSlotsTtlMinutes": 5,
    "InstanceName": "eTermini:"
  },
  "Reminders": {
    "PollIntervalMinutes": 5,
    "LeadTimeMinutes": 1440,
    "ToleranceMinutes": 30
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "eTerminiAPI",
    "Audience": "eTerminiUI",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Auth": {
    "PasswordSalt": "YOUR_GLOBAL_SALT"
  }
}
```

### eTerminiUI — `.env`

```env
VITE_API_URL=https://localhost:44305/api
VITE_HUB_URL=https://localhost:44305
```

### eTerminiAdminUI — `.env`

```env
VITE_ADMIN_API_URL=https://localhost:44306/api/admin
```

---

## Kodi i Gabimeve HTTP

| Kodi | Përshkrimi |
|------|-----------|
| `200 OK` | Kërkesa u realizua me sukses |
| `201 Created` | Resursi u krijua me sukses |
| `400 Bad Request` | Të dhëna të pavlefshme në body/query |
| `401 Unauthorized` | Token mungon ose ka skaduar |
| `403 Forbidden` | Nuk ke permisin e nevojshëm |
| `404 Not Found` | Resursi nuk u gjet |
| `409 Conflict` | Konfuzion (p.sh. slot tashmë i zënë) |
| `500 Internal Server Error` | Gabim i brendshëm i serverit |

---

## Logging

Aplikacioni përdor **Serilog** me dy destinations:

- **Console** — për debugging gjatë zhvillimit
- **File** — skedarë rotatif ditore në `logs/log-.txt`

Niveli default: `Information` (me `Warning` për ASP.NET Core internals).

---

## Licenca

Projekti është zhvilluar si pjesë e projektit të diplomës. Të gjitha të drejtat e rezervuara.
