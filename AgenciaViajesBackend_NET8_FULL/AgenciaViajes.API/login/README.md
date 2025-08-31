# Agencia de Viajes - ASP.NET Core 6 Web API

API básica para una agencia de viajes con:
- Autenticación JWT (registro / login)
- Exploración de países usando REST Countries
- CRUD de viajes por usuario (Firestore)
- Gestión de países favoritos (Firestore)

## Requisitos
- .NET 6 SDK
- Cuenta de Firebase y una base Firestore en modo *production* o *test*
- Archivo de credenciales de cuenta de servicio (serviceAccountKey.json)
- (Frontend sugerido: Angular 16)

## Configuración rápida

1) Clona este proyecto y restaura paquetes:
```bash
dotnet restore
```

2) Configura **Firebase** en `appsettings.json`:
```json
"Firebase": {
  "ProjectId": "tu-proyecto",
  "CredentialsPath": "C:/rutas/tu/serviceAccountKey.json"
}
```

> Alternativa: define la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS` apuntando a tu JSON.

3) Configura **JWT** en `appsettings.json` (cambia la llave):
```json
"Jwt": {
  "Key": "CAMBIA_ESTA_LLAVE_LARGA_Y_SECRETA",
  "Issuer": "AgenciaViajes",
  "Audience": "AgenciaViajesUsers",
  "ExpiresMinutes": 120
}
```

4) Ejecuta la API:
```bash
dotnet run --project AgenciaViajes.API
```
Swagger: https://localhost:7078/swagger

## Colecciones Firestore
- `users` (Id, Email, Name, PasswordHash, RegisteredAt)
- `trips` (Id, UserId, Title, CountryCode, StartDate, EndDate, Status, Description)
- `favorites` (Id, UserId, CountryCode, AddedAt)

## Endpoints principales

### Auth
- `POST /api/auth/register` { email, password, name }
- `POST /api/auth/login` { email, password }
- `GET /api/auth/me` (JWT)

### Users
- `GET /api/users/profile` (JWT)

### Countries
- `GET /api/countries?name=spa`  // lista (opcional filtro por nombre)
- `GET /api/countries/{code}`    // detalle por código (alpha)

### Trips (JWT)
- `GET /api/trips`
- `POST /api/trips`
- `GET /api/trips/{id}`
- `PUT /api/trips/{id}`
- `DELETE /api/trips/{id}`

Body `POST/PUT`:
```json
{
  "title": "Vacaciones",
  "countryCode": "HN",
  "startDate": "2025-12-01T00:00:00Z",
  "endDate": "2025-12-10T00:00:00Z",
  "status": 0,
  "description": "Viaje familiar"
}
```

### Favorites (JWT)
- `GET /api/favorites`
- `POST /api/favorites/{countryCode}`
- `DELETE /api/favorites/{id}`

## CORS
La API permite por defecto `http://localhost:4200` para desarrollo Angular.

## Notas
- Este backend implementa autenticación **JWT** con usuarios almacenados en Firestore (registro y login simples).
- Alineado a los requerimientos básicos del PDF (autenticación, países, viajes CRUD, favoritos, perfil).