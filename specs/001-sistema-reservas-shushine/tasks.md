# Tasks: Sistema Móvil de Gestión y Reservas Shushine Studio

**Input**: Documentos de diseño desde `/specs/001-sistema-reservas-shushine/` ([spec.md](./spec.md), [plan.md](./plan.md), [data-model.md](./data-model.md), [contracts/openapi.yaml](./contracts/openapi.yaml))  
**Prerequisites**: [plan.md](./plan.md) (completado), [spec.md](./spec.md) (completado), [.specify/memory/constitution.md](../../.specify/memory/constitution.md) (aprobado)  

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Ejecutable en paralelo (archivos independientes, sin bloqueo mutuo)
- **[Story]**: Historia de usuario asignada ([US1] a [US6])
- Rutas exactas de archivos especificadas en cada tarea

---

## Phase 1: Setup (Infraestructura y Proyectos Base)

**Propósito**: Inicialización de los proyectos de backend y frontend móvil, repositorios y persistencia base.

- [ ] T001 [P] Crear solución .NET 8/9 y estructura Clean Architecture en `src/backend/ShushineStudio.sln`
- [ ] T002 [P] Inicializar proyecto Flutter con configuración base multiplataforma en `src/mobile/pubspec.yaml`
- [ ] T003 [P] Configurar script DDL y aplicar esquema relacional de 16 tablas en Supabase SQL Editor
- [ ] T004 Configurar buckets de almacenamiento público en Supabase Storage (`servicios-imagenes`, `estilistas-avatares`)
- [ ] T005 [P] Configurar linters y formateadores para C# (`.editorconfig`) y Dart (`analysis_options.yaml`)

---

## Phase 2: Foundational (Prerrequisitos Bloqueantes)

**Propósito**: Componentes transversales obligatorios que deben estar listos antes de implementar las historias de usuario.

**⚠️ CRÍTICO**: Ninguna historia de usuario puede iniciar hasta completar esta fase.

- [ ] T006 Configurar DbContext con `Npgsql.EntityFrameworkCore.PostgreSQL` y cadena de conexión en `src/backend/ShushineStudio.Infrastructure/Data/AppDbContext.cs`
- [ ] T007 [P] Implementar middleware de manejo global de excepciones RFC 7807 Problem Details en `src/backend/ShushineStudio.Api/Middleware/ProblemDetailsMiddleware.cs`
- [ ] T008 [P] Configurar validación de autenticación JwtBearer contra Supabase Auth en `src/backend/ShushineStudio.Api/Program.cs`
- [ ] T009 [P] Configurar cliente HTTP Dio con timeout y headers base en `src/mobile/lib/core/network/dio_client.dart`
- [ ] T010 [P] Implementar `ErrorInterceptor` de Dio para manejo centralizado de respuestas RFC 7807 y detección de 401 en `src/mobile/lib/core/network/error_interceptor.dart`
- [ ] T011 Configurar inyector de dependencias (Service Locator) con `GetIt` en `src/mobile/lib/core/di/injection_container.dart`
- [ ] T012 Crear entidades de base y clases de resultado `Either<Failure, T>` en `src/mobile/lib/core/error/failures.dart`

**Checkpoint**: Cimientos listos — el desarrollo de historias de usuario puede comenzar en paralelo.

---

## Phase 3: User Story 1 - Autenticación Híbrida y Perfil de Usuario (Priority: P1) 🎯 MVP Core

**Goal**: Autenticación segura del cliente y personal vía Supabase Auth, sincronización con PostgreSQL y consulta de perfil en C#.  
**Independent Test**: Registrar un usuario en Flutter, verificar la inserción automática en PostgreSQL vía trigger y consultar exitosamente `/api/auth/me`.

### Tests para User Story 1
- [ ] T013 [P] [US1] Pruebas unitarias para validación de tokens JWT de Supabase en `src/backend/tests/ShushineStudio.UnitTests/Auth/JwtValidationTests.cs`
- [ ] T014 [P] [US1] Pruebas unitarias para el `AuthBloc` en Flutter en `src/mobile/test/presentation/blocs/auth_bloc_test.dart`

### Implementación Backend (Alex Alfaro)
- [ ] T015 [US1] Implementar función de base de datos `handle_new_user()` y trigger `on_auth_user_created` en Supabase
- [ ] T016 [P] [US1] Crear entidad `User` y `Client` en `src/backend/ShushineStudio.Domain/Entities/User.cs`
- [ ] T017 [US1] Crear DTOs de perfil de usuario en `src/backend/ShushineStudio.Application/DTOs/UserProfileResponse.cs`
- [ ] T018 [US1] Implementar servicio de perfil de usuario `IUserService` en `src/backend/ShushineStudio.Application/Services/UserService.cs`
- [ ] T019 [US1] Crear controlador de autenticación y perfil `AuthController` en `src/backend/ShushineStudio.Api/Controllers/AuthController.cs`

### Implementación Móvil (Camila Calderón)
- [ ] T020 [P] [US1] Configurar cliente `supabase_flutter` en `src/mobile/lib/core/auth/supabase_auth_service.dart`
- [ ] T021 [P] [US1] Implementar modelo `UserDto` con serialización en `src/mobile/lib/data/models/user_dto.dart`
- [ ] T022 [US1] Implementar `AuthRemoteDataSource` y `AuthRepositoryImpl` en `src/mobile/lib/data/repositories/auth_repository_impl.dart`
- [ ] T023 [US1] Implementar casos de uso `LoginUseCase` y `GetCurrentUserUseCase` en `src/mobile/lib/domain/usecases/auth/`
- [ ] T024 [US1] Crear `AuthBloc` (eventos y estados de sesión) en `src/mobile/lib/presentation/blocs/auth/auth_bloc.dart`
- [ ] T025 [US1] Maquetar pantallas de Login y Registro conforme a wireframes en `src/mobile/lib/presentation/screens/auth/login_screen.dart`
- [ ] T026 [US1] Maquetar pantalla de Perfil de Usuario en `src/mobile/lib/presentation/screens/profile/profile_screen.dart`

**Checkpoint**: Flujo de autenticación e identidad completo e independiente.

---

## Phase 4: User Story 2 - Catálogo Categorizado de Servicios y Búsqueda (Priority: P2)

**Goal**: Exploración ágil del catálogo de servicios clasificados por categorías con precios, duración e imágenes de Supabase Storage.  
**Independent Test**: Consultar `/api/categories` y `/api/services?categoryId=1`, verificando renderizado de tarjetas en Flutter.

### Implementación Backend (Alex Alfaro)
- [ ] T027 [P] [US2] Crear entidades `Category` y `Service` en `src/backend/ShushineStudio.Domain/Entities/Service.cs`
- [ ] T028 [P] [US2] Crear DTOs `CategoryDto` y `ServiceDto` en `src/backend/ShushineStudio.Application/DTOs/CatalogDtos.cs`
- [ ] T029 [US2] Implementar servicio de catálogo `ICatalogService` con filtros en `src/backend/ShushineStudio.Application/Services/CatalogService.cs`
- [ ] T030 [US2] Crear `CategoriesController` y `ServicesController` en `src/backend/ShushineStudio.Api/Controllers/`

### Implementación Móvil (Camila Calderón)
- [ ] T031 [P] [US2] Crear DTOs y Entidades de Catálogo en `src/mobile/lib/data/models/service_dto.dart`
- [ ] T032 [US2] Implementar `CatalogRemoteDataSource` con Dio en `src/mobile/lib/data/datasources/catalog_remote_datasource.dart`
- [ ] T033 [US2] Implementar `CatalogRepositoryImpl` y caso de uso `GetServicesCatalogUseCase` en `src/mobile/lib/domain/`
- [ ] T034 [US2] Crear `CatalogBloc` para filtrado y búsqueda reactiva en `src/mobile/lib/presentation/blocs/catalog/catalog_bloc.dart`
- [ ] T035 [US2] Maquetar pantalla de Catálogo con carrusel de categorías en `src/mobile/lib/presentation/screens/catalog/catalog_screen.dart`
- [ ] T036 [US2] Maquetar pantalla de Ficha de Detalle de Servicio en `src/mobile/lib/presentation/screens/catalog/service_detail_screen.dart`

**Checkpoint**: Catálogo navegable y listo para enlazar a reservas.

---

## Phase 5: User Story 3 - Motor de Disponibilidad y Selección de Estilistas (Priority: P3)

**Goal**: Cálculo en tiempo real por la API de las franjas horarias libres de cada profesional cruzando jornadas laborales y citas.  
**Independent Test**: Enviar petición a `/api/stylists/{id}/availability?date=...` y verificar que los bloques libres excluyan citas agendadas y descansos.

### Implementación Backend (Alex Alfaro)
- [ ] T037 [P] [US3] Crear entidades `Stylist`, `StylistSchedule` y `ScheduleBlock` en `src/backend/ShushineStudio.Domain/Entities/Stylist.cs`
- [ ] T038 [US3] Implementar motor de cálculo de franjas horarias `IAvailabilityEngine` en `src/backend/ShushineStudio.Application/Services/AvailabilityEngine.cs`
- [ ] T039 [US3] Crear DTOs de disponibilidad `AvailabilityResponse` y `TimeSlotDto` en `src/backend/ShushineStudio.Application/DTOs/AvailabilityDtos.cs`
- [ ] T040 [US3] Crear controlador `StylistsController` con endpoint de disponibilidad en `src/backend/ShushineStudio.Api/Controllers/StylistsController.cs`
- [ ] T041 [US3] Agregar pruebas unitarias del motor de disponibilidad en `src/backend/tests/ShushineStudio.UnitTests/Services/AvailabilityEngineTests.cs`

### Implementación Móvil (Camila Calderón)
- [ ] T042 [P] [US3] Crear modelos `StylistDto` y `TimeSlotDto` en `src/mobile/lib/data/models/stylist_dto.dart`
- [ ] T043 [US3] Implementar `StylistRemoteDataSource` y repositorio en `src/mobile/lib/data/repositories/stylist_repository_impl.dart`
- [ ] T044 [US3] Crear caso de uso `GetStylistAvailabilityUseCase` en `src/mobile/lib/domain/usecases/stylists/`
- [ ] T045 [US3] Implementar `BookingBloc` (manejo de selección de estilista, fecha y hora) en `src/mobile/lib/presentation/blocs/booking/booking_bloc.dart`
- [ ] T046 [US3] Maquetar selector de estilistas y calendario de slots horarios en `src/mobile/lib/presentation/screens/booking/select_datetime_screen.dart`

**Checkpoint**: Disponibilidad en tiempo real verificable desde móvil.

---

## Phase 6: User Story 4 - Motor Transaccional de Citas y Concurrencia (Priority: P4)

**Goal**: Creación atómica de citas (ACID), bloqueo pesimista contra doble reserva, generación de código único y gestión de estado.  
**Independent Test**: Ejecutar dos peticiones paralelas idénticas al endpoint de citas; la primera debe aprobarse (201) y la segunda fallar con 409 Conflict.

### Implementación Backend (Alex Alfaro)
- [ ] T047 [P] [US4] Crear entidades `Appointment` y `AppointmentDetail` en `src/backend/ShushineStudio.Domain/Entities/Appointment.cs`
- [ ] T048 [P] [US4] Crear validadores FluentValidation para `CreateAppointmentRequest` en `src/backend/ShushineStudio.Application/Validators/CreateAppointmentValidator.cs`
- [ ] T049 [US4] Implementar servicio transaccional `IAppointmentService` con `IDbContextTransaction` y verificación de colisión en `src/backend/ShushineStudio.Application/Services/AppointmentService.cs`
- [ ] T050 [US4] Implementar generación de correlativo único (`SHU-YYYY-NNNN`) en `src/backend/ShushineStudio.Application/Common/CodeGenerator.cs`
- [ ] T051 [US4] Crear controlador `AppointmentsController` con endpoints de creación, consulta y cancelación en `src/backend/ShushineStudio.Api/Controllers/AppointmentsController.cs`
- [ ] T052 [US4] Pruebas de integración para validar prevención de doble reserva concurrente en `src/backend/tests/ShushineStudio.IntegrationTests/Appointments/ConcurrencyBookingTests.cs`

### Implementación Móvil (Camila Calderón)
- [ ] T053 [P] [US4] Crear modelos `AppointmentDto` y `AppointmentDetailDto` en `src/mobile/lib/data/models/appointment_dto.dart`
- [ ] T054 [US4] Implementar `AppointmentRemoteDataSource` y repositorio en `src/mobile/lib/data/repositories/appointment_repository_impl.dart`
- [ ] T055 [US4] Implementar casos de uso `CreateAppointmentUseCase` y `GetMyAppointmentsUseCase` en `src/mobile/lib/domain/usecases/appointments/`
- [ ] T056 [US4] Maquetar pantalla de Resumen de Reserva con desglose de costos e impuestos en `src/mobile/lib/presentation/screens/booking/booking_summary_screen.dart`
- [ ] T057 [US4] Maquetar pantalla de Confirmación Exitosa de Cita en `src/mobile/lib/presentation/screens/booking/booking_success_screen.dart`
- [ ] T058 [US4] Maquetar pantalla de Mis Citas con tabs de citas activas e historial en `src/mobile/lib/presentation/screens/appointments/my_appointments_screen.dart`

**Checkpoint**: Motor de reservas transaccional 100% operativo.

---

## Phase 7: User Story 5 - Agenda Timeline y Gestión de Citas para Estilistas / Admin (Priority: P5)

**Goal**: Timeline diario interactivo para estilistas y recepcionistas, actualización de estados de cita y registro de walk-ins.  
**Independent Test**: Consultar `/api/admin/appointments/timeline?date=...` y cambiar estado de cita a `En_Progreso`.

### Implementación Backend (Alex Alfaro)
- [ ] T059 [US5] Implementar servicio de agenda `ITimelineService` con filtros por fecha y estilista en `src/backend/ShushineStudio.Application/Services/TimelineService.cs`
- [ ] T060 [US5] Implementar endpoint para registro de clientes presenciales Walk-in en `src/backend/ShushineStudio.Api/Controllers/AppointmentsController.cs`
- [ ] T061 [US5] Implementar transición de estados de cita (`Pendiente` -> `Confirmada` -> `En_Progreso` -> `Completada`) en `src/backend/ShushineStudio.Application/Services/AppointmentService.cs`

### Implementación Móvil (Camila Calderón)
- [ ] T062 [P] [US5] Crear `TimelineBloc` para gestión de agenda de estilista en `src/mobile/lib/presentation/blocs/timeline/timeline_bloc.dart`
- [ ] T063 [US5] Maquetar pantalla de Agenda Timeline interactiva con filtros de día en `src/mobile/lib/presentation/screens/admin/stylist_timeline_screen.dart`
- [ ] T064 [US5] Maquetar modal/pantalla de Registro Rápido de Walk-in en `src/mobile/lib/presentation/screens/admin/register_walkin_dialog.dart`

**Checkpoint**: Operatividad del salón en cabina cubierta.

---

## Phase 8: User Story 6 - Pagos, Facturación y Dashboard Gerencial (Priority: P6)

**Goal**: Registro de transacciones de pago multimetodo, emisión de facturas correlativas con IVA y visualización de KPIs gerenciales.  
**Independent Test**: Registrar un pago para una cita completada y consultar métricas en `/api/admin/dashboard`.

### Implementación Backend (Alex Alfaro)
- [ ] T065 [P] [US6] Crear entidades `PaymentTransaction` y `Invoice` en `src/backend/ShushineStudio.Domain/Entities/Invoice.cs`
- [ ] T066 [US6] Implementar servicio de facturación y cálculo de IVA `IInvoiceService` en `src/backend/ShushineStudio.Application/Services/InvoiceService.cs`
- [ ] T067 [US6] Implementar servicio de métricas de Dashboard `IDashboardService` en `src/backend/ShushineStudio.Application/Services/DashboardService.cs`
- [ ] T068 [US6] Crear `PaymentsController` y `DashboardController` en `src/backend/ShushineStudio.Api/Controllers/`

### Implementación Móvil (Camila Calderón)
- [ ] T069 [P] [US6] Crear modelos `InvoiceDto` y `DashboardMetricsDto` en `src/mobile/lib/data/models/`
- [ ] T070 [US6] Maquetar pantalla de Cobro y Factura en `src/mobile/lib/presentation/screens/checkout/invoice_screen.dart`
- [ ] T071 [US6] Maquetar Dashboard de Métricas Gerenciales del Salón en `src/mobile/lib/presentation/screens/admin/admin_dashboard_screen.dart`

**Checkpoint**: Ciclo contable y administrativo cerrado.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Propósito**: Seguridad final, validaciones cruzadas, pruebas E2E y compilación de artefactos.

- [ ] T072 [P] Auditar y verificar que ningún secreto, JWT key o cadena de conexión figure en el repositorio
- [ ] T073 [P] Documentar endpoints y probar contrato Swagger interactivo en `https://localhost:5001/swagger`
- [ ] T074 Ejecutar pruebas de carga concurrentes para revalidar prevención de doble reserva
- [ ] T075 [P] Compilar y verificar artefacto APK Release en Flutter (`flutter build apk --release`)
- [ ] T076 Validar escenarios del documento [quickstart.md](./quickstart.md) de punta a punta

---

## Dependencies & Execution Order

```
[Phase 1: Setup]
       │
       ▼
[Phase 2: Foundational] ◄─── (BLOQUEANTE: Todos los componentes base de API y Flutter)
       │
       ├───────────────────────────────┐
       ▼                               ▼
[Phase 3: US1 - Auth & Perfil]   [Phase 4: US2 - Catálogo]
       │                               │
       └──────────────┬────────────────┘
                      ▼
       [Phase 5: US3 - Disponibilidad Estilistas]
                      │
                      ▼
       [Phase 6: US4 - Reserva Transaccional & Concurrencia] 🎯 MVP Completo
                      │
                      ├───────────────────────────────┐
                      ▼                               ▼
       [Phase 7: US5 - Agenda Timeline]   [Phase 8: US6 - Pagos & Dashboard]
                      │                               │
                      └──────────────┬────────────────┘
                                     ▼
                      [Phase 9: Polish & Entrega Final]
```

---

## Estrategia de Trabajo en Pareja (Alex y Camila)

* **Alex Fernando Alfaro Diaz (Backend):** Fases 1 a 8 en `src/backend/` desarrollando controllers, servicios de aplicación, EF Core DbContext, transacciones ACID y OpenAPI.
* **Camila Antonia Calderon Cortez (Móvil / QA):** Fases 1 a 8 en `src/mobile/` implementando Clean Architecture, BLoCs, integración con Dio, widgets conforme a los wireframes y plan de pruebas.
