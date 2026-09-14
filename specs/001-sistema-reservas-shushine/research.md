# Research & Architectural Decisions: Sistema Móvil de Gestión y Reservas Shushine Studio

**Feature**: `001-sistema-reservas-shushine`  
**Date**: 2026-09-11  
**Status**: Completed & Validated  

---

## 1. Decisiones de Arquitectura y Patrones de Diseño

### Decisión 1: Clean Architecture + BLoC en el Cliente Móvil (Flutter)
* **Decisión:** Implementar Clean Architecture estricta en Flutter dividida en Presentation (Widgets + BLoC), Domain (Entities, Use Cases, Repository Contracts en Dart puro) y Data (DTOs, DataSources con Dio, Repository Implementations).
* **Justificación:** Permite total independencia entre la interfaz visual (wireframes de salón) y la lógica de red. Las pruebas unitarias de los casos de uso se pueden ejecutar sin simular widgets ni frameworks UI. El patrón BLoC (`flutter_bloc`) garantiza flujos de estado predecibles (`Initial`, `Loading`, `Success`, `Error`) ideales para aplicaciones con flujos reactivos como reservas y agendas.
* **Alternativas Evaluadas:**
  * *Provider / ChangeNotifier:* Descartado por permitir lógica de negocio acoplada a la vista y menor escalabilidad para pruebas unitarias formales.
  * *Riverpod:* Descartado para mantener adherencia al estándar académico consensuado del equipo (`flutter_bloc`).

### Decisión 2: Clean Architecture en Web API C# ASP.NET Core (.NET 8/9)
* **Decisión:** Estructurar la API backend en capas: `Api` (Controllers delgados, Middleware RFC 7807, Swagger), `Application` (Services, DTOs, FluentValidation, Use Cases), `Domain` (Entidades del negocio, Enums, Excepciones) e `Infrastructure` (Entity Framework Core con `Npgsql`, Repositorios, Supabase Client).
* **Justificación:** Cumple con el Principio de Responsabilidad Única (SRP) y la Inversión de Dependencias (DIP). Los controladores solo reciben peticiones HTTP y retornan códigos de estado; toda la lógica de cálculo de slots, precios e impuestos reside en la capa de aplicación.
* **Alternativas Evaluadas:**
  * *Minimal APIs en un solo proyecto:* Descartado por mezclar enrutamiento, validación y acceso a datos en arquitecturas empresariales medianas/grandes.

### Decisión 3: Estrategia de Autenticación Híbrida (Supabase Auth + C# JWT Validation)
* **Decisión:** La app móvil autentica al usuario mediante el SDK cliente oficial de Supabase (`supabase_flutter`) para login, registro y refresh tokens. La Web API en C# valida la firma criptográfica del JWT emitido por Supabase mediante `Microsoft.AspNetCore.Authentication.JwtBearer` con las claves públicas/secretas de Supabase.
* **Justificación:** Elimina la necesidad de almacenar y hashear contraseñas en el backend de C#, delegando la seguridad de credenciales y OAuth a Supabase Auth (SOC2 / GDPR compliance), mientras la API C# mantiene el control absoluto y autorización basada en claims/roles (`sub`, `role`).
* **Alternativas Evaluadas:**
  * *Identity propio en ASP.NET Core:* Descartado para aprovechar el BaaS de Supabase requerido en el módulo académico.
  * *API como proxy de Login a Supabase:* Descartado por agregar latencia innecesaria y no aprovechar el refresco local automático de `supabase_flutter`.

### Decisión 4: Manejo de Errores Estandarizado (RFC 7807 Problem Details)
* **Decisión:** Estandarizar todas las respuestas de error en formato `application/problem+json` con `type`, `title`, `status`, `detail`, `instance` y `errors` (diccionario de validación). En Flutter, configurar un `ErrorInterceptor` con `Dio` que deserialice el RFC 7807 y maneje automáticamente 401 (refresh sesión), 403 (prohibido) y 409 (conflicto de horarios).
* **Justificación:** Evita formatos ad-hoc incompatibles y permite que el cliente móvil muestre mensajes de error claros en español directamente al usuario, sin romper los flujos de UI.

### Decisión 5: Prevención de Doble Reserva y Concurrencia de Slots
* **Decisión:** Utilizar transacciones atómicas (ACID) en PostgreSQL a través de Entity Framework Core (`IDbContextTransaction`) con nivel de aislamiento `ReadCommitted` o `RepeatableRead` y comprobación con bloqueo pesimista (`SELECT ... FOR UPDATE`) sobre la franja horaria del estilista al momento de confirmar la cita. Si la franja ya fue tomada, la API aborta la transacción y responde inmediatamente `409 Conflict`.
* **Justificación:** Es la única forma de garantizar 0% de sobreventa cuando múltiples usuarios reservan a la vez el mismo slot horario.

### Decisión 6: Aislamiento de Base de Datos y Row Level Security (RLS)
* **Decisión:** Activar RLS en todas las tablas del esquema `public` en Supabase con política `DENY ALL` para usuarios anónimos o clientes directos. La Web API C# se conecta con cadena de conexión segura (`Npgsql`) utilizando el rol privilegiado de backend.
* **Justificación:** Garantiza que ningún cliente móvil o actor malicioso pueda alterar precios, citas o historiales saltándose la Web API.
