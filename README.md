# Sistema de Turnos Médicos — Backend API (TPI DSW 2026)

API RESTful desarrollada en **C# con ASP.NET Core (.NET 10)** y **Entity Framework Core (SQL Server)** bajo arquitectura en capas (`Api`, `Application`, `Domain`, `Data`, `CrossCutting`).

---

## Integrantes del Grupo

- **Ferreyra Bernabé** — Legajo: 53018
- **Reyes Franco Exequiel** — Legajo: 58223
- **Ruiz Coronel Mariano Agustín** — Legajo: 58286
- **Sosa Ariana Valentina** — Legajo: 58309

---

## Instrucciones para Configurar y Ejecutar el Proyecto Localmente

### 1. Requisitos previos
- **.NET 10 SDK** instalado.
- **SQL Server** o **SQL Server LocalDB** en ejecución.

### 2. Configuración (`appsettings.Development.json`)
En `Dsw2026Tpi.Api/appsettings.Development.json` ya se encuentra preconfigurada la conexión a LocalDB, la clave JWT, las políticas de Rate Limiting, los orígenes CORS permitidos para el frontend (`http://localhost:5173`) y las credenciales iniciales del Administrador:
- **Email Admin inicial:** `admin@admin.com`
- **Password Admin inicial:** `Admin123*`

*(Al iniciar en entorno Development, la aplicación aplica las migraciones automáticamente y ejecuta el seed idempotente de roles, especialidades, doctores y usuario administrador).*

### 3. Compilación y ejecución
Desde la raíz del repositorio (`tpi-back`), ejecutar:

```bash
dotnet restore
dotnet build Dsw2026Tpi.slnx
dotnet run --project Dsw2026Tpi.Api
```

La API quedará disponible en:
- **HTTP:** `http://localhost:5278`
- **Swagger UI:** `http://localhost:5278/swagger`

Para ejecutar las pruebas unitarias:
```bash
dotnet test Dsw2026Tpi.slnx
```

---

## Descripción de Endpoints Implementados

Todos los endpoints (excepto los dos de login) requieren autenticación mediante token **JWT** (`Authorization: Bearer <token>`).

### Módulo de Autenticación (`/api/auth`)
- `POST /api/auth/admin/login`: Autentica a un administrador (`{ email, password }`). Retorna `{ token, role: "ADMINISTRADOR" }`. Protegido con Rate Limiting (5 req/min por IP).
- `POST /api/auth/patient/login`: Autentica a un paciente (`{ email, dni }`). Si el DNI no existe, lo registra automáticamente con rol `PACIENTE`. Protegido con Rate Limiting (10 req/min por IP).
- `POST /api/auth/admin/register`: Permite a un administrador registrar una nueva cuenta administrativa.

### Módulo de Especialidades (`/api/specialties`)
- `GET /api/specialties?pageSize=10&pageIndex=1&name=string`: Lista especialidades activas de forma paginada y filtrada por nombre (accesible para `ADMINISTRADOR` y `PACIENTE`).
- `POST /api/specialties`: Crea una nueva especialidad (`{ name, description }`, solo `ADMINISTRADOR`).
- `PUT /api/specialties/{id}`: Actualiza una especialidad existente (solo `ADMINISTRADOR`).
- `DELETE /api/specialties/{id}`: Eliminación lógica (`deleted = true`) de la especialidad. Retorna `"ok"`.

### Módulo de Médicos (`/api/doctors`)
- `GET /api/doctors?pageSize=10&pageIndex=1&name=string&specialtyId=Guid`: Lista médicos activos paginados con filtro opcional por nombre y especialidad.
- `GET /api/doctors/{id}/availabilities`: Retorna los días y rangos horarios configurados para el médico en el mes en curso.
- `GET /api/doctors/{id}/slots?from=YYYY-MM-DD&to=YYYY-MM-DD`: Retorna los bloques de 30 minutos libres (`AVAILABLE`) y futuros del médico.
- `POST /api/doctors`: Registra un nuevo médico (`{ name, licenseNumber, specialtyId }`, solo `ADMINISTRADOR`).
- `PUT /api/doctors/{id}`: Actualiza los datos de un médico (solo `ADMINISTRADOR`).
- `DELETE /api/doctors/{id}`: Eliminación lógica (`deleted = true`) del médico. Retorna `"ok"`.

### Módulo de Disponibilidades (`/api/availabilities`)
- `POST /api/availabilities`: Crea las reglas de disponibilidad del mes actual (`{ doctorId, days: [{ day, startTime, endTime }] }`) y genera bloques de 30 minutos omitiendo feriados definidos en `holidays.json`.
- `PUT /api/availabilities`: Actualiza las disponibilidades del mes actual para el médico, preservando los turnos ya reservados (`BOOKED`) y regenerando los bloques disponibles futuros.

### Módulo de Turnos / Citas (`/api/appointments`)
- `POST /api/appointments`: Solicita un turno (`{ doctorId, availabilityId, patient: { dni }, reason }`). Valida disponibilidad, horario futuro, motivo (>= 5 caracteres) y controla concurrencia optimista (`RowVersion`). Protegido con Rate Limiting (5 req/min por paciente).
- `GET /api/appointments/patient?dni=number`: Retorna los turnos activos futuros (`BOOKED`) del paciente.
- `DELETE /api/appointments/{id}`: Cancela un turno en estado `BOOKED` y libera el slot (`AVAILABLE`). Retorna `"ok"`.
- `GET /api/appointments?date=YYYY-MM-DD`: Lista todos los turnos de una fecha específica (solo `ADMINISTRADOR`).
- `GET /api/appointments/search?pageSize=10&pageIndex=1&specialtyId=Guid&doctorId=Guid&dni=string&date=YYYY-MM-DD`: Búsqueda combinada y paginada de turnos (solo `ADMINISTRADOR`).
