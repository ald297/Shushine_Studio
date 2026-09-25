# Reglas y Estándares de Arquitectura y Desarrollo — Shushine Studio

> **Archivo de Reglas para Desarrolladores y Asistentes de IA (AGENTS.md / Cursor / Windsurf / Antigravity)**  
> **Proyecto:** Shushine Studio — Sistema Móvil de Gestión y Reservas para Salón de Belleza  
> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)  
> **Equipo:** Alex Fernando Alfaro Diaz (Backend / Scrum Master) & Camila Antonia Calderon Cortez (Móvil / QA)  

---

## 1. Stack Tecnológico Oficial

| Componente | Tecnología | Detalle y Versión |
| :--- | :--- | :--- |
| **Frontend Móvil** | **.NET MAUI (C# / XAML)** | Aplicación nativa multiplataforma (Android & iOS / Windows) con Clean Architecture + MVVM (CommunityToolkit.Mvvm). |
| **Backend API** | **Java Spring Boot 3.3.3** | Web API RESTful con arquitectura en capas bajo paquete de dominio propio (`com.shushinestudio`). |
| **Base de Datos** | **Supabase (PostgreSQL)** | PostgreSQL relacional en la nube (AWS Pooler directo, 16-23 tablas normalizadas). |
| **Seguridad & Auth** | **Spring Security 6 / JJWT** | Autenticación basada en JWT con roles estipulados (`ADMIN`, `CLIENTE`) y autorización granular. |
| **Control de Versiones & CI/CD** | **Azure DevOps / Git** | Repositorios, control de ramas, pull requests y tableros Kanban. |
| **Documentación de API** | **OpenAPI / Swagger UI** | Contrato interactivo con soporte nativo de Bearer Token JWT (`SwaggerConfig`). |
| **Mapeo de Datos** | **ModelMapper** | Desacoplamiento total entre Entidades de base de datos y DTOs por operación. |
| **Testing** | **JUnit 5 / xUnit (.NET MAUI)** | Pruebas unitarias de servicios backend (`t1_crear` a `t6_eliminar`) y ViewModels en frontend. |

---

## 2. Arquitectura de la Aplicación Móvil (Clean Architecture + MVVM)

Para asegurar escalabilidad, testeabilidad y mantenimiento limpio, la aplicación móvil sigue estrictamente los principios de **Clean Architecture** estructurada en tres capas desacopladas:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│     [Pages / Views XAML] ◄───► [ViewModels (MVVM Toolkit)]  │
└───────────────────────────────┬─────────────────────────────┘
                                │ depende de
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                       DOMAIN LAYER                          │
│     [Use Cases]  ───►  [Entities]  ───►  [Repo Interfaces]  │
└───────────────────────────────▲─────────────────────────────┘
                                │ implementado por
                                │
┌─────────────────────────────────────────────────────────────┐
│                        DATA LAYER                           │
│   [Repo Implementations] ──► [Data Sources] ──► [DTOs/JSON] │
│          │                        │                         │
│          ▼                        ▼                         │
│   [Local Cache / SQLite]    [HttpClient / REST Client]      │
└─────────────────────────────────────────────────────────────┘
```

### 2.1 Capa de Presentación (Presentation / UI Layer)
* **Vistas (Pages / Views - XAML):** Archivos puramente declarativos y visuales con data binding reactivo. No contienen lógica de negocio, cálculos de tarifas ni llamadas HTTP directas (ej. `BookingSummaryPage.xaml`).
* **Gestores de Estado (ViewModels):** Basados en `CommunityToolkit.Mvvm` (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`). Controlan el estado reactivo de la pantalla, invocan los **Casos de Uso**, gestionan propiedades (`IsBusy`, `ErrorMessage`, listas) y notifican a la vista para redibujarse.

### 2.2 Capa de Dominio (Domain Layer) — *Núcleo Puro de la App*
* **Entidades (Entities):** Objetos de negocio puros de C#, independientes de cualquier framework de UI, base de datos o serialización JSON.
* **Casos de Uso (Use Cases):** Clases con una única responsabilidad (`ExecuteAsync()`). Ejemplos: `GetAvailableSlotsUseCase`, `CreateAppointmentUseCase`, `GetServicesCatalogUseCase`.
* **Contratos de Repositorios (Interfaces):** Clases/interfaces abstractas que declaran los métodos de obtención de datos (ej. `public interface IAppointmentRepository`). La capa de dominio define el contrato, pero no sabe cómo ni de dónde se obtienen los datos.

### 2.3 Capa de Datos (Data Layer)
* **Modelos (DTOs):** Clases que extienden o mapean a las entidades del dominio e implementan la serialización (`System.Text.Json`) conforme a los contratos de la API (ej. `AppointmentDto`, `ServiceDto`).
* **Fuentes de Datos (Data Sources):** 
  * `RemoteDataSource`: Clases dedicadas a peticiones HTTP (GET, POST, PUT, DELETE) mediante `HttpClient` o clientes tipados. Configuran timeouts e inyectan el token JWT en el header `Authorization: Bearer <token>`.
  * `LocalDataSource`: Almacenamiento local (cache, offline fallback, preferencias, `SecureStorage`).
* **Implementación de Repositorios (Repository Implementations):** Clases que implementan los contratos de la capa de dominio. Coordinan si la información se obtiene desde el `RemoteDataSource` o desde la caché local ante fallos de conexión.

---

## 3. Reglas de Interacción entre Agentes del Sistema (API y Móvil)

### 3.1 Fuente Única de Verdad (Single Source of Truth)
* La **Web API es el único agente con autoridad de negocio**.
* Es la única encargada de:
  1. Calcular disponibilidades horarias reales de los estilistas.
  2. Calcular montos, subtotales, descuentos e impuestos.
  3. Gestionar la concurrencia transaccional (evitar doble reserva del mismo slot horario mediante transacciones ACID y bloqueos optimistas/pesimistas).
* La app móvil es un **cliente de presentación**: recolecta entradas del usuario, las envía a la API y renderiza las respuestas. **Nunca** calcula disponibilidad ni aprueba reservas por su cuenta.

### 3.2 Contratos Estrictos de API (API Contracts)
* La API y el cliente móvil se comunican exclusivamente mediante contratos JSON predefinidos y documentados en **OpenAPI (Swagger)**.
* Cualquier modificación en un endpoint, nombre de campo o tipo de dato debe versionarse o acordarse previamente para evitar rupturas en la app móvil.
* Los nombres de propiedades en JSON deben usar `camelCase` estándar.

### 3.3 Aislamiento de Base de Datos y Supabase
* **La app móvil tiene prohibido consultar directamente las tablas de Supabase** (PostgreSQL) usando clientes directos de base de datos.
* El SDK de Supabase en el cliente móvil se utiliza para autenticación y almacenamiento de medios en Storage.
* Todas las operaciones de lectura y escritura del negocio (catálogo, reservas, estilistas, horarios, clientes) se realizan a través de la **Web API**.
* En Supabase, se debe activar **Row Level Security (RLS)** en todas las tablas del esquema `public` para denegar consultas anónimas o directas no autorizadas desde clientes móviles.

### 3.4 Estándares Arquitectónicos Oficiales de la API Backend (Spring Boot 3.3.3)
* **Semillero de Datos Automático (`DataInitializer`):** El backend verifica si la base de datos PostgreSQL en Supabase está vacía y puebla automáticamente los roles oficiales de Shushine Studio (`ADMIN`, `CLIENTE`, `RECEPCIONISTA`), usuarios de prueba (`admin`/`admin123`, `cliente`/`cliente123`) y catálogo inicial de belleza, para permitir evaluación inmediata por el docente.
* **Segregación de DTOs por Acción:** Queda estrictamente prohibido exponer entidades de base de datos en controladores. Se deben emplear DTOs especializados (`*Guardar`, `*Modificar`, `*Salida`, `*CambiarEstado`).
* **Endpoints Duales:** Cada módulo debe ofrecer endpoints paginados (`Pageable` con `page`, `size`, `sort`) para volumen de datos y endpoints de listado rápido (`/lista`) para selectores móviles en .NET MAUI.
* **Swagger con Bearer Token Integrado:** La documentación OpenAPI debe incluir el esquema de seguridad HTTP Bearer para autorizar pruebas directamente en el navegador con un clic.
* **Pruebas Unitarias de Servicios:** Cada servicio de negocio debe contar con su clase de pruebas unitarias cubriendo el ciclo CRUD (`t1_crear`, `t2_obtenerTodos`, `t3_obtenerTodosPaginados`, `t4_obtenerPorId`, `t5_editar`, `t6_eliminarPorId`).
* **Identidad y Paquete Propio:** El código del backend se organiza bajo el paquete raíz `com.shushinestudio`, adaptando los patrones pedagógicos de la guía de ESFE AGAPE a la lógica real del salón sin incurrir en duplicaciones arbitrarias.

---

## 4. Flujo de Autenticación y Autorización Híbrido (Supabase Auth + API)

```
[Móvil (.NET MAUI)] ── 1. Login (Email/Pass) ──► [Supabase Auth]
[Móvil (.NET MAUI)] ◄─ 2. Retorna JWT Token ─── [Supabase Auth]
        │
        ▼ 3. Petición HTTP con Header:
             "Authorization: Bearer <Supabase_JWT>"
        │
        ▼
[Backend Web API REST]
        │ 4. Valida firma del JWT con Clave/JWKS de Supabase o Auth Local
        │ 5. Extrae Claims (sub / UserID, Role)
        │ 6. Ejecuta lógica de negocio transaccional
        ▼
[Supabase Database (PostgreSQL)]
```

1. El usuario se autentica en la app móvil mediante **Supabase Auth** o el endpoint de autenticación.
2. Se valida credenciales y entrega un `access_token` (JWT) con tiempo de expiración y un `refresh_token`.
3. Para cualquier consulta a la API, el manejador HTTP (`DelegatingHandler` / Interceptor) de .NET MAUI adjunta el token en la cabecera:
   ```http
   Authorization: Bearer <access_token>
   ```
4. La API valida la firma criptográfica del token JWT.
5. La API extrae el identificador del usuario (`sub`) y valida roles mediante anotaciones/políticas de seguridad (`@PreAuthorize` / `[Authorize]`).

---

## 5. Reglas de Codificación (Clean Code) para el Equipo

### 5.1 Convención de Idiomas y Nomenclatura
* **Código en Inglés:** Todas las variables, métodos, funciones, clases, interfaces, migraciones y nombres de endpoints deben estar en **inglés**.
  * ✅ `GetAvailableSlots()`, `AppointmentService`, `isAvailable`, `CreateAppointmentDto`, `/api/appointments`
  * ❌ `ObtenerDisponibilidad()`, `ServicioReserva`, `estaDisponible`
* **Textos de Interfaz (UI) en Español:** Todos los textos visibles por el usuario final (títulos, botones, mensajes de error, notificaciones) deben estar en **español** neutro.
  * ✅ `"Confirmar Reserva"`, `"Selecciona un estilista"`, `"Error al cargar el catálogo"`
* **Mensajes de Commit en Español:** Los mensajes de commit deben redactarse en **español**, explicando detalladamente los cambios realizados siguiendo la estructura de Conventional Commits.
  * ✅ `feat(reservas): implementar creación de cita con transacción ACID y validación de disponibilidad`
  * ✅ `fix(interceptor): corregir manejo de token expirado 401 y redirección a login en .NET MAUI`

### 5.2 Principio de Responsabilidad Única (Single Responsibility Principle - SRP)
* Cada clase y archivo debe tener una sola razón para cambiar.
* En el backend: los controladores (`Controllers`) solo reciben la petición HTTP y devuelven el status code correspondiente; la lógica de negocio reside en los servicios (`Services`); el acceso a datos reside en repositorios.
* En el frontend: las páginas/vistas XAML solo pintan componentes visuales y enlaces de datos; la gestión de estado y comandos reside en los `ViewModels`; las llamadas a red residen en los `DataSources`.

### 5.3 Inyección de Dependencias (Dependency Injection - DI)
* **Frontend (.NET MAUI):** Utilizar el contenedor de servicios nativo en `MauiProgram.cs` mediante `IServiceCollection`:
  ```csharp
  builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
  builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
  builder.Services.AddTransient<BookingSummaryViewModel>();
  builder.Services.AddTransient<BookingSummaryPage>();
  ```
  Queda prohibido instanciar servicios y ViewModels manualmente con `new` dentro de los code-behinds de las páginas.

### 5.4 Manejo Global de Errores y Formato de Respuestas (Estándar RFC 7807 - Problem Details)

* **Backend Web API:** 
  Queda prohibido devolver formatos de error arbitrarios o no estructurados. La Web API debe adherirse al estándar de la industria **RFC 7807 (`application/problem+json`)**:
  * Formato JSON oficial de respuesta para errores de cliente (400, 404, 409) o servidor (500):
    ```json
    {
      "type": "https://httpstatuses.io/409",
      "title": "Conflict",
      "status": 409,
      "detail": "El estilista seleccionado ya no tiene disponible la franja horaria solicitada.",
      "instance": "/api/reservas",
      "errors": {
        "HoraInicio": ["El horario coincide con una reserva previa."]
      }
    }
    ```

* **Frontend (.NET MAUI - DelegatingHandler con HttpClient):**
  Configurar un manejador HTTP centralizado (`ErrorDelegatingHandler` / Interceptor) que deserialice respuestas RFC 7807:
  * **Lectura del error:** Extraer preferentemente el campo `detail`. Si existen campos en `errors`, extraer el primer mensaje legible para el usuario en español.
  * **401 Unauthorized:** El manejador debe intentar refrescar la sesión. Si el refresco falla o expira, debe invalidar la sesión local, limpiar `SecureStorage` y redirigir inmediatamente a `//LoginPage`.
  * **403 Forbidden:** Notificar al usuario mediante diálogo/alerta que su rol no tiene privilegios para realizar esta acción.
  * **400 / 409 / 422:** Extraer `detail` del `ProblemDetails` y renderizarlo mediante `DisplayAlert` o notificación emergente en español.
  * **500 Internal Server Error / Conexión:** Mostrar mensaje amigable: *"Error de conexión con el salón. Intente de nuevo más tarde."* con opción de reintentar.

### 5.5 Gestión Segura de Secretos y Variables de Entorno
* **Prohibido estrictamente** escribir credenciales en duro en archivos de código fuente (`.cs`, `.xaml`, etc.).
* **Backend:**
  * En desarrollo local: usar variables de entorno o perfiles locales registrados en `.gitignore`.
  * Archivos con contraseñas o claves privadas deben figurar en `.gitignore`.
* **Frontend .NET MAUI:**
  * Utilizar `Microsoft.Maui.Storage.SecureStorage` para almacenamiento protegido de tokens JWT y credenciales de sesión.
  * Configuración de API Base URL y claves públicas mediante `appsettings.json` o constantes inyectadas.
  * Los archivos de secretos deben estar siempre registrados en `.gitignore`.

---

## 6. Convenciones de Git y Flujo de Trabajo (Git Workflow en Azure DevOps)

### 6.1 Estrategia de Ramas por Característica (Branch per Feature)
Para mantener un historial limpio y evitar roturas en el código compartido, el equipo sigue estrictamente el flujo **GitFlow**:
* **`main`:** Rama de producción. Solo contiene versiones estables y probadas para entrega final.
* **`develop`:** Rama base de integración continua. Es el tronco común del que nacen y al que se integran las características.
* **`feature/<modulo>-<descripcion>`:** Ramas individuales de desarrollo por cada Historia de Usuario o Tarea técnica.
  * Ejemplos: `feature/api-disponibilidad`, `feature/movil-resumen-reserva`, `feature/auth-custom-claims`
* **`fix/<descripcion>`:** Ramas de corrección puntual de defectos sobre `develop`.
  * Ejemplo: `fix/error-delegating-handler-401`
* **`hotfix/<descripcion>`:** Ramas de corrección urgente sobre `main`.

### 6.2 Prohibición Estricta de Push Directo a `develop` y `main`
* 🛑 **Queda terminantemente prohibido hacer push directo a las ramas `develop` o `main`.**
* **Todo cambio sin excepción** debe desarrollarse en su propia rama `feature/*` o `fix/*` y subirse a Azure DevOps mediante un **Pull Request (PR)** hacia `develop`.
* **Revisión por Pares Obligatoria:** Todo Pull Request requiere la revisión y aprobación formal del compañero de equipo (**Alex Fernando** o **Camila Antonia**) antes de poder fusionarse.

### 6.3 Convención de Mensajes de Commit en Español
Los mensajes de commit deben escribirse en **español** y seguir el estándar Conventional Commits estructurado:
```text
tipo(alcance): descripción concisa en español

[Cuerpo opcional detallando los cambios realizados, motivos y contexto]
```
* **Tipos permitidos:**
  * `feat:` Nueva funcionalidad o caso de uso.
  * `fix:` Corrección de un bug o error.
  * `docs:` Cambios únicamente en documentación, wireframes o diagramas.
  * `refactor:` Refactorización de código que no altera el comportamiento funcional.
  * `test:` Adición o corrección de pruebas unitarias o de integración.
  * `chore:` Tareas de mantenimiento, dependencias o configuración del proyecto.
* **Ejemplos oficiales:**
  * `feat(reservas): implementar motor de cálculo de disponibilidad de estilistas por franja horaria`
  * `feat(movil): maquetar pantalla de resumen de cita con desglose de servicios e impuestos`
  * `fix(seguridad): remover archivo .env.local del repositorio y blindar .gitignore`
  * `test(backend): agregar pruebas unitarias con xUnit para verificar prevención de doble reserva`

### 6.4 Flujo Paso a Paso para Desarrollar una Tarea
```bash
# 1. Asegurar estar en develop y sincronizado
git checkout develop
git pull origin develop

# 2. Crear y cambiar a la rama de la funcionalidad
git checkout -b feature/api-disponibilidad-slots

# 3. Desarrollar y confirmar cambios con mensajes en español descriptivos
git add .
git commit -m "feat(disponibilidad): agregar servicio de cruce de horarios y reservas existentes"

# 4. Subir la rama a Azure DevOps
git push origin feature/api-disponibilidad-slots

# 5. Crear Pull Request en Azure Repos: feature/api-disponibilidad-slots -> develop
# 6. Esperar la revisión y aprobación de Alex o Camila para completar el merge
```

### 6.5 Autoría Estricta y Prohibición de Crédito para la IA
* **Regla de Oro:** El Asistente de IA (Antigravity, Cursor, Copilot o cualquier otro agente) tiene **estrictamente prohibido adjudicarse el crédito** de los commits, pull requests, tareas de Azure Boards o modificaciones de código.
* **Prohibición de Marcas de IA:** Queda terminantemente prohibido incluir frases, firmas o metadatos como `"Co-authored-by: assistant"`, `"Generated by AI"`, `"Created by Antigravity"` o similares en mensajes de commit, descripciones de PRs o archivos fuente.
* **Atribución Nominal Obligatoria:** Todos los commits, tareas y cambios deben figurar **única y exclusivamente** bajo el nombre y usuario de los integrantes humanos del equipo:
  * **Alex Fernando Alfaro Diaz** (`ald297` / `lalafaro6@gmail.com`)
  * **Camila Antonia Calderon Cortez** (`cc25003@esfe.agape.edu.sv`)
* Las contribuciones en Git siempre deben realizarse respetando la identidad del desarrollador local (`user.name` y `user.email`).

