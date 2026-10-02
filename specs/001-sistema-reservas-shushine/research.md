# Research & Architectural Decisions: Sistema Móvil de Gestión y Reservas Shushine Studio

**Feature**: `001-sistema-reservas-shushine`  
**Date**: 2026-09-11  
**Status**: Completed & Validated  

---

## 1. Decisiones de Arquitectura y Patrones de Diseño

### Decisión 1: Clean Architecture + MVVM en el Cliente Móvil (.NET MAUI)
* **Decisión:** Implementar Clean Architecture estricta en .NET MAUI dividida en Presentation (Pages XAML + ViewModels con `CommunityToolkit.Mvvm`), Domain (Entities, Use Cases, Repository Contracts en C# puro) y Data (DTOs, DataSources con `HttpClient`, Repository Implementations).
* **Justificación:** Permite total independencia entre la interfaz visual (wireframes de salón) y la lógica de red. Las pruebas unitarias de los casos de uso y ViewModels se pueden ejecutar sin simular controles visuales ni frameworks UI. El patrón MVVM (`CommunityToolkit.Mvvm`) con `[ObservableProperty]` y `[RelayCommand]` garantiza flujos de estado predecibles (`IsBusy`, `ErrorMessage`, `ObservableCollection`) ideales para aplicaciones con flujos reactivos como reservas y agendas.
* **Alternativas Evaluadas:**
  * *Code-behind tradicional:* Descartado por acoplar la lógica de negocio a la vista y dificultar las pruebas unitarias.
  * *ReactiveUI:* Descartado para mantener simplicidad y el estándar oficial de Microsoft (`CommunityToolkit.Mvvm`).

### Decisión 2: Clean Architecture en Web API Spring Boot 3.3.3 (`com.shushinestudio`)
* **Decisión:** Estructurar la API backend en capas desacopladas: `controladores` (@RestController delgados, enrutamiento y Swagger), `servicios` (lógica de negocio, transacciones @Transactional y mapeo de DTOs con ModelMapper), `repositorios` (Spring Data JPA) y `modelos` (entidades relacionales del dominio).
* **Justificación:** Cumple con el Principio de Responsabilidad Única (SRP) y la Inversión de Dependencias (DIP). Los controladores solo reciben peticiones HTTP y retornan códigos de estado; toda la lógica de cálculo de slots, precios e impuestos reside en los servicios.
* **Alternativas Evaluadas:**
  * *Spring Boot sin capas (lógica en controladores):* Descartado por violar buenas prácticas y dificultar el testing unitario.

### Decisión 3: Autenticación Stateless con JWT y Spring Security 6
* **Decisión:** Implementar autenticación autónoma en la API mediante endpoints `/api/auth/login` y `/api/auth/registro`, validando credenciales contra la tabla `usuarios` cifradas con `BCryptPasswordEncoder` y emitiendo tokens JWT firmados con HMAC-SHA512. La app móvil en .NET MAUI almacena el JWT en `Microsoft.Maui.Storage.SecureStorage` y lo adjunta en la cabecera `Authorization: Bearer <token>`.
* **Justificación:** Permite que tanto Swagger UI como la app móvil prueben y consuman la API de forma autónoma, sin depender de librerías externas de autenticación para validar permisos y roles (`ADMIN`, `CLIENTE`).
* **Alternativas Evaluadas:**
  * *Sesiones Stateful con cookies/JSESSIONID:* Descartado por no ser apto para clientes móviles ni cumplir con la arquitectura REST.

### Decisión 4: Manejo de Errores Estandarizado (RFC 7807 Problem Details)
* **Decisión:** Estandarizar todas las respuestas de error en formato `application/problem+json` con `type`, `title`, `status`, `detail`, `instance` y `errors` (diccionario de validación). En .NET MAUI, configurar un `ErrorDelegatingHandler` con `HttpClient` que deserialice el RFC 7807 y maneje automáticamente 401 (refresh sesión), 403 (prohibido) y 409 (conflicto de horarios).
* **Justificación:** Evita formatos ad-hoc incompatibles y permite que el cliente móvil muestre mensajes de error claros en español directamente al usuario, sin romper los flujos de UI.

### Decisión 5: Prevención de Doble Reserva y Concurrencia de Slots
* **Decisión:** Utilizar transacciones atómicas (ACID) en PostgreSQL a través de Entity Framework Core (`IDbContextTransaction`) con nivel de aislamiento `ReadCommitted` o `RepeatableRead` y comprobación con bloqueo pesimista (`SELECT ... FOR UPDATE`) sobre la franja horaria del estilista al momento de confirmar la cita. Si la franja ya fue tomada, la API aborta la transacción y responde inmediatamente `409 Conflict`.
* **Justificación:** Es la única forma de garantizar 0% de sobreventa cuando múltiples usuarios reservan a la vez el mismo slot horario.

### Decisión 6: Aislamiento de Base de Datos y Row Level Security (RLS)
* **Decisión:** Activar RLS en todas las tablas del esquema `public` en Supabase con política `DENY ALL` para usuarios anónimos o clientes directos. La Web API se conecta con credenciales seguras utilizando el rol privilegiado de backend.
* **Justificación:** Garantiza que ningún cliente móvil o actor malicioso pueda alterar precios, citas o historiales saltándose la Web API.

### Decisión 7: Adopción de la Forma Arquitectónica de Spring Boot con Dominio Propio (`com.shushinestudio`)
* **Decisión:** Adoptar la arquitectura en capas y patrones observados en la guía de referencia del docente de ESFE AGAPE, pero adaptada con identidad propia para el salón de belleza Shushine Studio sin hacer copia y pega:
  1. Paquete raíz del dominio: `com.shushinestudio` (no `org.esfe`).
  2. Capas limpias: Controladores (`@RestController`), Servicios con interfaces (`@Service`), Repositorios JPA (`JpaRepository`), Entidades relacionales del salón, DTOs específicos por operación y configuración de seguridad con Spring Security 6.
  3. Desacoplamiento total con .NET MAUI: La app móvil consume la API a través de endpoints REST estándar en formato JSON con tokens JWT Bearer, siendo 100% independiente del lenguaje del backend.
* **Justificación:** Cumple con la estructura técnica y pedagógica requerida por la institución, pero con un código fuente profesional, original y adaptado con rigor al modelo de negocio de Shushine Studio.

### Decisión 8: Semillero de Datos Automático (`DataInitializer`) en PostgreSQL Supabase con Roles Oficiales
* **Decisión:** Implementar un inicializador de datos en el arranque del backend (`CommandLineRunner`) que apunte directamente a **PostgreSQL en Supabase** (sin base de datos H2 intermedia) y compruebe si las tablas maestras están vacías para insertar:
  1. Roles oficiales estipulados en el modelo de Shushine Studio: **`ADMIN`** (Administrador) y **`CLIENTE`** (Cliente), con soporte opcional para `RECEPCIONISTA`.
  2. Usuarios de prueba con contraseñas encriptadas con BCrypt:
     - `admin` (`admin123`) con rol `ADMIN`.
     - `cliente` (`cliente123`) con rol `CLIENTE`.
  3. Datos maestros del salón de belleza: categorías (Corte y Peinado, Colorimetría, Manicura y Pedicura, Cuidado Facial), servicios base con duración y precio, estilistas y citas de demostración.
* **Justificación:** Garantiza que al evaluar la API en Swagger o al abrir la app móvil, existan datos reales de prueba en la nube sin requerir migraciones manuales.

### Decisión 9: Estructura de DTOs Segregados por Operación (`Guardar`, `Modificar`, `Salida`, `CambiarEstado`)
* **Decisión:** Prohibir la exposición directa de entidades JPA / EF Core en los controladores y definir DTOs especializados por intención:
  - `*Guardar`: Datos requeridos para inserción (sin ID autogenerado).
  - `*Modificar`: Datos completos con ID obligatorio para actualización total (`PUT`).
  - `*Salida`: Datos formateados para respuesta al cliente con relaciones legibles (nombres en lugar de solo IDs).
  - `*CambiarEstado`: DTO específico para actualizaciones atómicas de estado vía `PATCH`.
* **Justificación:** Desacopla la base de datos de la API pública, previene ataques de asignación masiva (*Mass Assignment*) y estandariza la serialización para .NET MAUI y Swagger.

### Decisión 10: Estrategia Dual de Consulta (Paginación + Listas Rápidas)
* **Decisión:** Proveer dos variantes de endpoints de lectura para cada catálogo:
  1. `GET /api/{recurso}`: Endpoint paginado que recibe `page`, `size` y `sort` (vía `Pageable`), retornando metadatos (`totalPages`, `totalElements`, `content`).
  2. `GET /api/{recurso}/lista`: Endpoint que retorna la lista completa en un array simple.
* **Justificación:** La versión paginada optimiza la carga en listas infinitas de la app móvil y tablas grandes, mientras que `/lista` permite alimentar desplegables y selectores rápidos en .NET MAUI sin sobrecarga.

### Decisión 11: Documentación Swagger / OpenAPI con Autenticación Bearer
* **Decisión:** Configurar OpenAPI con esquema de seguridad `SecuritySchemeType.HTTP`, `scheme = "bearer"`, `bearerFormat = "JWT"`, aplicando `@SecurityRequirement` globalmente.
* **Justificación:** Permite a los evaluadores autenticarse directamente en la interfaz web de Swagger pulsando "Authorize", ingresar el token JWT obtenido de `/api/auth/login` y probar cualquier endpoint protegido sin herramientas externas.

### Decisión 12: Suite de Pruebas Unitarias de Servicios (`t1_...` a `t6_...`)
* **Decisión:** Implementar pruebas unitarias estandarizadas para cada servicio de negocio siguiendo la nomenclatura secuencial de la guía:
  - `t1_crear()`
  - `t2_obtenerTodos()`
  - `t3_obtenerTodosPaginados()`
  - `t4_obtenerPorId()`
  - `t5_editar()`
  - `t6_eliminarPorId()`
* **Justificación:** Garantiza la verificación funcional de la lógica de negocio y proporciona evidencia clara de calidad y testing automatizado para la evaluación académica.

