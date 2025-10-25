## DentaCare Skeleton

Backend: .NET 8, EF Core (Pomelo MySQL), Serilog, Swagger.
Frontend: React + Vite, proxy to backend.

### Prerequisites
- .NET SDK 8
- Node.js 20+
- MySQL 8 with database `DentaCare`

### Configure DB
Edit `backend/WebApi/appsettings.Development.json` connection string:

```
"ConnectionStrings": {
  "DentaCare": "server=127.0.0.1;port=3306;database=DentaCare;user id=root;password=YOUR_PASSWORD;TreatTinyAsBoolean=false;SslMode=None;"
}
```

### Run backend
```
cd backend/WebApi
dotnet run --launch-profile https
```
Health: https://localhost:5001/health
Swagger: https://localhost:5001/swagger

### Run frontend
```
cd frontend
npm run dev
```
Open http://localhost:5173

### Notes
- Tenancy scaffold via `ITenantProvider`; single tenant for now.
- Global tenant filter in `DentaCareDbContext`.
- Patients GET `/api/patients` returns up to 100 entries.


