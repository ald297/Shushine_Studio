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

- [X] T001 [P] Inicializar proyecto de Backend con arquitectura en capas (Java 21 Spring Boot 3.3.3 - `com.shushinestudio`) y dependencias base (`pom.xml`)
- [X] T002 [P] Inicializar proyecto .NET MAUI con configuración base multiplataforma en `src/mobile/`
- [X] T003 [P] Configurar script DDL y aplicar esquema relacional de 16-23 tablas en base de datos PostgreSQL Supabase (`docs/migration_23_tables_shunshine.sql`)
- [X] T004 Configurar buckets de almacenamiento público en Supabase Storage (`servicios-imagenes`, `estilistas-avatares`, `categorias-imagenes`)
- [X] T005 [P] Configurar análisis estático y reglas de código en .NET MAUI (`.editorconfig`)

---

## Phase 2: Foundational (Prerrequisitos Bloqueantes de la Guía ESFE)

**Propósito**: Componentes transversales obligatorios que deben estar listos antes de implementar las historias de usuario.

**⚠️ CRÍTICO**: Ninguna historia de usuario puede iniciar hasta completar esta fase.

- [X] T006 Configurar persistencia relacional con soporte dual (H2/MySQL para evaluación rápida y PostgreSQL Supabase)
- [X] T007 [P] Configurar documentación interactiva OpenAPI / Swagger UI con soporte de seguridad Bearer JWT (`SwaggerConfig`)
- [X] T008 [P] Configurar filtros de seguridad JWT (`JwtAuthenticationFilter`) y autorización granular por roles (`SecurityConfig`)
- [X] T009 [P] Implementar semillero automático de datos de prueba (`DataInitializer`) que cree roles (`ADMIN`, `USER`), usuarios por defecto (`admin`/`admin123`, `user`/`user123`) y catálogo inicial si la BD está vacía
- [X] T010 [P] Implementar configuración de mapeo de DTOs (`ModelMapperConfig` / `AutoMapper`) para aislar entidades de datos expuestos
- [X] T011 [P] Configurar cliente HTTP con `HttpClientFactory`, timeout y headers base en `src/mobile/Core/Network/`
- [X] T012 [P] Implementar `ErrorDelegatingHandler` para manejo centralizado de respuestas RFC 7807 y detección de 401 en `src/mobile/Core/Network/`
- [X] T013 Configurar inyector de dependencias en `src/mobile/MauiProgram.cs` (`IServiceCollection`)
- [X] T014 Crear entidades base y contratos de repositorio en `src/mobile/Domain/`

**Checkpoint**: Cimientos listos — base de datos inicializada con usuarios de prueba y Swagger disponible para pruebas.

---

## Phase 3: User Story 1 - Autenticación y Perfil de Usuario (Priority: P1) 🎯 MVP Core

**Goal**: Autenticación segura de clientes y administradores con JWT, almacenamiento con BCrypt en PostgreSQL y endpoints de login y registro en la API.  
**Independent Test**: Registrar un usuario en `/api/auth/registro`, iniciar sesión en `/api/auth/login`, obtener el JWT y consultar exitosamente `/api/auth/me`.

### Tests para User Story 1
- [X] T015 [P] [US1] Pruebas unitarias para generación y validación de tokens en `src/backend/src/test/java/com/shushinestudio/seguridad/JwtServiceTest.java`
- [X] T016 [P] [US1] Pruebas unitarias para el `AuthViewModel` en .NET MAUI con xUnit
- [X] T017 [P] [US1] Crear entidades JPA `Usuario`, `Rol` y `Cliente` en `src/backend/src/main/java/com/shushinestudio/modelos/`
- [X] T018 [P] [US1] Crear repositorios `UsuarioRepository` y `RolRepository` en `src/backend/src/main/java/com/shushinestudio/repositorios/`
- [X] T019 [US1] Crear DTOs `UsuarioLogin`, `UsuarioRegistrar` y `UsuarioToken` en `src/backend/src/main/java/com/shushinestudio/dtos/auth/`
- [X] T020 [US1] Implementar servicios `UsuarioService` y `JwtService` en `src/backend/src/main/java/com/shushinestudio/seguridad/servicios/`
- [X] T021 [US1] Crear controlador `AuthController` con endpoints `/api/auth/login`, `/api/auth/registro` y `/api/auth/me` en `src/backend/src/main/java/com/shushinestudio/controladores/AuthController.java`

### Implementación Móvil (Camila Calderón)
- [X] T022 [P] [US1] Configurar `SecureStorage` para almacenamiento protegido del token en `src/mobile/Core/Auth/TokenStorage.cs`
- [X] T023 [P] [US1] Implementar modelo `UserDto` con serialización en `src/mobile/Data/Models/UserDto.cs`
- [X] T024 [US1] Implementar `AuthRemoteDataSource` y `AuthRepositoryImpl` en `src/mobile/Data/Repositories/AuthRepositoryImpl.cs`
- [X] T025 [US1] Implementar casos de uso `LoginUseCase` y `GetCurrentUserUseCase` en `src/mobile/Domain/UseCases/Auth/`
- [X] T026 [US1] Crear `AuthViewModel` (comandos y propiedades reactivas con MVVM Toolkit) en `src/mobile/Presentation/ViewModels/Auth/`
- [X] T027 [US1] Maquetar páginas XAML de Login y Registro conforme a wireframes en `src/mobile/Presentation/Views/Auth/`
- [X] T028 [US1] Maquetar página de Perfil de Usuario en `src/mobile/Presentation/Views/Profile/ProfilePage.xaml`

**Checkpoint**: Flujo de autenticación e identidad completo e independiente.

---

## Phase 4: User Story 2 - Catálogo Categorizado de Servicios y Búsqueda (Priority: P2)

**Goal**: Exploración ágil del catálogo de servicios clasificados por categorías con precios, duración e imágenes de Supabase Storage.  
**Independent Test**: Consultar `/api/categorias` y `/api/servicios/lista`, verificando renderizado de tarjetas en .NET MAUI.

### Implementación Backend (Alex Alfaro)
- [X] T029 [P] [US2] Crear entidades `Categoria` y `Servicio` en `src/backend/src/main/java/com/shushinestudio/modelos/`
- [X] T030 [P] [US2] Crear repositorios `ICategoriaRepository` y `IServicioRepository` en `src/backend/src/main/java/com/shushinestudio/repositorios/`
- [X] T031 [P] [US2] Crear DTOs estandarizados (`CategoriaGuardar`, `CategoriaModificar`, `CategoriaSalida`, `ServicioGuardar`, `ServicioModificar`, `ServicioSalida`) en `src/backend/src/main/java/com/shushinestudio/dtos/`
- [X] T032 [US2] Implementar servicios `ICategoriaService` y `IServicioService` con soporte paginado y `/lista` en `src/backend/src/main/java/com/shushinestudio/servicios/`
- [X] T033 [US2] Crear `CategoriaController` y `ServicioController` en `src/backend/src/main/java/com/shushinestudio/controladores/`
- [X] T034 [US2] Pruebas unitarias CRUD de servicios (`t1_crear` a `t6_eliminar`) en `src/backend/src/test/java/com/shushinestudio/servicios/CategoriaServiceTest.java`

### Implementación Móvil (Camila Calderón)
- [X] T035 [P] [US2] Crear DTOs y Entidades de Catálogo en `src/mobile/Data/Models/ServiceDto.cs`
- [X] T036 [US2] Implementar `CatalogRemoteDataSource` con `HttpClient` en `src/mobile/Data/DataSources/CatalogRemoteDataSource.cs`
- [X] T037 [US2] Implementar `CatalogRepositoryImpl` y caso de uso `GetServicesCatalogUseCase` en `src/mobile/Domain/`
- [X] T038 [US2] Crear `CatalogViewModel` para filtrado y búsqueda reactiva en `src/mobile/Presentation/ViewModels/Catalog/`
- [X] T039 [US2] Maquetar página de Catálogo con carrusel de categorías en `src/mobile/Presentation/Views/Catalog/CatalogPage.xaml`
- [X] T040 [US2] Maquetar página de Ficha de Detalle de Servicio en `src/mobile/Presentation/Views/Catalog/ServiceDetailPage.xaml`

**Checkpoint**: Catálogo navegable y listo para enlazar a reservas.

---

## Phase 5: User Story 3 - Motor de Disponibilidad y Selección de Estilistas (Priority: P3)

**Goal**: Cálculo en tiempo real por la API de las franjas horarias libres de cada profesional cruzando jornadas laborales y citas.  
**Independent Test**: Enviar petición a `/api/estilistas/{id}/disponibilidad?fecha=...` y verificar que los bloques libres excluyan citas agendadas y descansos.

### Implementación Backend (Alex Alfaro)
- [X] T041 [P] [US3] Crear entidades `Estilista`, `HorarioEstilista` y `BloqueoHorario` en `src/backend/src/main/java/com/shushinestudio/modelos/`
- [X] T042 [P] [US3] Crear repositorios `IEstilistaRepository`, `IHorarioEstilistaRepository` y `IBloqueoHorarioRepository` en `src/backend/src/main/java/com/shushinestudio/repositorios/`
- [X] T043 [US3] Implementar motor de cálculo de franjas horarias `AvailabilityEngine` en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/DisponibilidadService.java`
- [X] T044 [US3] Crear DTOs de disponibilidad (`DisponibilidadSalida`, `FranjaHorariaDto`) en `src/backend/src/main/java/com/shushinestudio/dtos/disponibilidad/`
- [X] T045 [US3] Crear controlador `EstilistaController` con endpoint de disponibilidad en `src/backend/src/main/java/com/shushinestudio/controladores/EstilistaController.java`
- [X] T046 [US3] Agregar pruebas unitarias del motor de disponibilidad en `src/backend/src/test/java/com/shushinestudio/servicios/DisponibilidadServiceTest.java`

### Implementación Móvil (Camila Calderón)
- [X] T047 [P] [US3] Crear modelos `StylistDto` y `TimeSlotDto` en `src/mobile/Data/Models/StylistDto.cs`
- [X] T048 [US3] Implementar `StylistRemoteDataSource` y repositorio en `src/mobile/Data/Repositories/StylistRepositoryImpl.cs`
- [X] T049 [US3] Crear caso de uso `GetStylistAvailabilityUseCase` en `src/mobile/Domain/UseCases/Stylists/`
- [X] T050 [US3] Implementar `BookingViewModel` (manejo de selección de estilista, fecha y hora) en `src/mobile/Presentation/ViewModels/Booking/`
- [X] T051 [US3] Maquetar selector de estilistas y calendario de slots horarios en `src/mobile/Presentation/Views/Booking/SelectDateTimePage.xaml`

**Checkpoint**: Disponibilidad en tiempo real verificable desde móvil.

---

## Phase 6: User Story 4 - Motor Transaccional de Citas y Concurrencia (Priority: P4)

**Goal**: Creación atómica de citas (ACID), bloqueo pesimista contra doble reserva, generación de código único y gestión de estado.  
**Independent Test**: Ejecutar dos peticiones paralelas idénticas al endpoint de citas; la primera debe aprobarse (201) y la segunda fallar con 409 Conflict.

### Implementación Backend (Alex Alfaro)
- [X] T052 [P] [US4] Crear entidades `Cita` y `CitaServicio` en `src/backend/src/main/java/com/shushinestudio/modelos/`
- [X] T053 [P] [US4] Crear DTOs estandarizados (`CitaGuardar`, `CitaModificar`, `CitaCambiarEstado`, `CitaSalida`) en `src/backend/src/main/java/com/shushinestudio/dtos/cita/`
- [X] T054 [US4] Implementar servicio transaccional `ICitaService` con `@Transactional`, verificación de colisión y generación de código (`SHU-YYYY-NNNN`) en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/CitaService.java`
- [X] T055 [US4] Crear controlador `CitaController` con endpoints de creación, consulta paginada, lista y cambio de estado en `src/backend/src/main/java/com/shushinestudio/controladores/CitaController.java`
- [X] T056 [US4] Pruebas de integración para validar prevención de doble reserva concurrente en `src/backend/src/test/java/com/shushinestudio/servicios/CitaConcurrenciaTest.java`

### Implementación Móvil (Camila Calderón)
- [X] T057 [P] [US4] Crear modelos `AppointmentDto` y `AppointmentDetailDto` en `src/mobile/Data/Models/AppointmentDto.cs`
- [X] T058 [US4] Implementar `AppointmentRemoteDataSource` y repositorio en `src/mobile/Data/Repositories/AppointmentRepositoryImpl.cs`
- [X] T059 [US4] Implementar casos de uso `CreateAppointmentUseCase` y `GetMyAppointmentsUseCase` en `src/mobile/Domain/UseCases/Appointments/`
- [X] T060 [US4] Maquetar pantalla de Resumen de Reserva con desglose de costos e impuestos en `src/mobile/Presentation/Views/Booking/BookingSummaryPage.xaml`
- [X] T061 [US4] Maquetar pantalla de Confirmación Exitosa de Cita en `src/mobile/Presentation/Views/Booking/BookingSuccessPage.xaml`
- [X] T062 [US4] Maquetar pantalla de Mis Citas con tabs de citas activas e historial en `src/mobile/Presentation/Views/Appointments/MyAppointmentsPage.xaml`

**Checkpoint**: Motor de reservas transaccional 100% operativo.

---

## Phase 7: User Story 5 - Agenda Timeline y Gestión de Citas para Estilistas / Admin (Priority: P5)

**Goal**: Timeline diario interactivo para estilistas y recepcionistas, actualización de estados de cita y registro de walk-ins.  
**Independent Test**: Consultar `/api/citas/timeline?fecha=...` y cambiar estado de cita a `EN_PROCESO`.

### Implementación Backend (Alex Alfaro)
- [X] T063 [US5] Implementar servicio de agenda `ITimelineService` con filtros por fecha y estilista en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/TimelineService.java`
- [X] T064 [US5] Implementar endpoint para registro de clientes presenciales Walk-in en `src/backend/src/main/java/com/shushinestudio/controladores/CitaController.java`
- [X] T065 [US5] Implementar transición de estados de cita (`PENDIENTE` -> `CONFIRMADA` -> `EN_PROCESO` -> `COMPLETADA`) en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/CitaService.java`

### Implementación Móvil (Camila Calderón)
- [X] T066 [P] [US5] Crear `TimelineViewModel` para gestión de agenda de estilista en `src/mobile/Presentation/ViewModels/Timeline/`
- [X] T067 [US5] Maquetar pantalla de Agenda Timeline interactiva con filtros de día en `src/mobile/Presentation/Views/Admin/StylistTimelinePage.xaml`
- [X] T068 [US5] Maquetar modal/pantalla de Registro Rápido de Walk-in en `src/mobile/Presentation/Views/Admin/RegisterWalkinPopup.xaml`

**Checkpoint**: Operatividad del salón en cabina cubierta.

---

## Phase 8: User Story 6 - Pagos, Facturación y Dashboard Gerencial (Priority: P6)

**Goal**: Registro de transacciones de pago multimetodo, emisión de facturas correlativas con IVA y visualización de KPIs gerenciales.  
**Independent Test**: Registrar un pago para una cita completada y consultar métricas en `/api/admin/dashboard`.

### Implementación Backend (Alex Alfaro)
- [X] T069 [P] [US6] Crear entidades `Pago` y `Factura` en `src/backend/src/main/java/com/shushinestudio/modelos/`
- [X] T070 [US6] Implementar servicio de facturación y cálculo de IVA `IFacturaService` en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/FacturaService.java`
- [X] T071 [US6] Implementar servicio de métricas de Dashboard `IDashboardService` en `src/backend/src/main/java/com/shushinestudio/servicios/implementaciones/DashboardService.java`
- [X] T072 [US6] Crear `PagoController` y `DashboardController` en `src/backend/src/main/java/com/shushinestudio/controladores/`

### Implementación Móvil (Camila Calderón)
- [X] T073 [P] [US6] Crear modelos `InvoiceDto` y `DashboardMetricsDto` en `src/mobile/Data/Models/`
- [X] T074 [US6] Maquetar pantalla de Cobro y Factura en `src/mobile/Presentation/Views/Checkout/InvoicePage.xaml`
- [X] T075 [US6] Maquetar Dashboard de Métricas Gerenciales del Salón en `src/mobile/Presentation/Views/Admin/AdminDashboardPage.xaml`

**Checkpoint**: Ciclo contable y administrativo cerrado.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Propósito**: Seguridad final, validaciones cruzadas, pruebas E2E y compilación de artefactos.

- [X] T076 [P] Auditar y verificar que ningún secreto, JWT key o contraseña figure en el repositorio
- [X] T077 [P] Documentar endpoints y probar contrato Swagger interactivo en `http://localhost:8080/swagger-ui/index.html`
- [X] T078 Ejecutar pruebas de carga concurrentes para revalidar prevención de doble reserva
- [X] T079 [P] Compilar y verificar paquete Release de .NET MAUI (`dotnet build -c Release`)
- [X] T080 Validar escenarios del documento [quickstart.md](./quickstart.md) de punta a punta

---

## Dependencies & Execution Order

```
[Phase 1: Setup]
       │
       ▼
[Phase 2: Foundational] ◄─── (BLOQUEANTE: Todos los componentes base de API y .NET MAUI)
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

* **Alex Fernando Alfaro Diaz (Backend):** Fases 1 a 8 en `src/backend/` desarrollando controladores REST, servicios con interfaces, repositorios Spring Data JPA, DTOs segregados, seguridad con Spring Security/JJWT, semillero `DataInitializer`, transacciones ACID y pruebas unitarias con JUnit 5.
* **Camila Antonia Calderon Cortez (Móvil / QA):** Fases 1 a 8 en `src/mobile/` implementando Clean Architecture, ViewModels (`CommunityToolkit.Mvvm`), integración con HttpClient, páginas XAML conforme a los wireframes y plan de pruebas.
