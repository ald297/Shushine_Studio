# Quickstart Validation Guide: Sistema Móvil de Gestión y Reservas Shushine Studio

**Feature**: `001-sistema-reservas-shushine`  
**Date**: 2026-09-11  
**Status**: Ready for Execution  

---

## 1. Prerrequisitos de Entorno

* **.NET SDK:** .NET 8.0 o 9.0 instalado (`dotnet --version`).
* **Flutter SDK:** Flutter 3.19+ con Dart 3.3+ (`flutter --version`).
* **Supabase CLI / Acceso:** Organización activa en Supabase con PostgreSQL y Supabase Auth habilitados.
* **Herramientas de prueba:** `curl` o Postman / ThunderClient para validación HTTP, y un emulador Android/iOS o navegador Chrome para Flutter.

---

## 2. Configuración Inicial del Proyecto

### 2.1 Variables de Entorno del Backend (ASP.NET Core)
En la carpeta del proyecto de API o mediante `dotnet user-secrets`:
```bash
dotnet user-secrets set "ConnectionStrings:SupabasePostgres" "Host=aws-0-us-east-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.acikahicfjtojuvqcvxv;Password=TU_PASSWORD_AQUI;SSL Mode=Require;Trust Server Certificate=true"
dotnet user-secrets set "Jwt:Authority" "https://acikahicfjtojuvqcvxv.supabase.co/auth/v1"
dotnet user-secrets set "Jwt:Audience" "authenticated"
```

### 2.2 Variables de Compilación en Flutter
Ejecutar la app móvil pasando los parámetros seguros de Supabase:
```bash
flutter run --dart-define=SUPABASE_URL=https://acikahicfjtojuvqcvxv.supabase.co \
            --dart-define=SUPABASE_ANON_KEY=TU_ANON_KEY_PUBLICA \
            --dart-define=API_BASE_URL=http://localhost:5000/api
```

---

## 3. Escenarios de Validación de Punta a Punta (E2E)

### Escenario 1: Verificación de Catálogo de Servicios (Público)
```bash
# 1. Consultar categorías activas
curl -X GET "http://localhost:5000/api/categories" -H "Accept: application/json"

# 2. Consultar servicios filtrados por categoría de cabello (ID: 1)
curl -X GET "http://localhost:5000/api/services?categoryId=1" -H "Accept: application/json"
```
* **Resultado Esperado:** Código HTTP `200 OK` con arreglo de categorías y servicios con precios y tiempos de duración en minutos.

---

### Escenario 2: Verificación del Motor de Disponibilidad de Estilistas
```bash
# Consultar disponibilidad de un estilista para una fecha específica con servicio de 60 minutos
curl -X GET "http://localhost:5000/api/stylists/1/availability?date=2026-09-15&serviceDuration=60" \
     -H "Accept: application/json"
```
* **Resultado Esperado:** Código HTTP `200 OK` con los slots horarios libres (`09:00`, `10:00`, etc.) excluyendo horas de almuerzo y citas previamente registradas.

---

### Escenario 3: Creación Transaccional de Cita con Token JWT de Supabase
```bash
# Enviar reserva con token de autenticación
curl -X POST "http://localhost:5000/api/appointments" \
     -H "Authorization: Bearer <SUPABASE_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "stylistId": 1,
       "appointmentDate": "2026-09-15",
       "startTime": "10:00:00",
       "serviceIds": [1],
       "notes": "Prueba de validacion rapida"
     }'
```
* **Resultado Esperado:** Código HTTP `201 Created` con el código generado (ej. `'SHU-2026-0001'`), estado `'Confirmada'`, subtotal y desglose de IVA (13%).

---

### Escenario 4: Verificación de Prevención de Doble Reserva (Control de Concurrencia)
```bash
# Reintentar la misma petición con el mismo estilista, fecha y hora inmediatamente
curl -X POST "http://localhost:5000/api/appointments" \
     -H "Authorization: Bearer <SUPABASE_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "stylistId": 1,
       "appointmentDate": "2026-09-15",
       "startTime": "10:00:00",
       "serviceIds": [1]
     }'
```
* **Resultado Esperado:** Código HTTP `409 Conflict` estructurado en formato RFC 7807 (`application/problem+json`):
```json
{
  "type": "https://httpstatuses.io/409",
  "title": "Conflict",
  "status": 409,
  "detail": "El estilista seleccionado ya no tiene disponible la franja horaria solicitada.",
  "instance": "/api/appointments"
}
```
