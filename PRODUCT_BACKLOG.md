# 📋 Product Backlog Oficial Refinado — Shushine Studio

> **Proyecto:** Shushine Studio — Sistema Móvil de Gestión y Reservas para Salón de Belleza  
> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)  
> **Carrera:** Técnico en Ingeniería de Desarrollo de Software | **Módulo:** Construcción de APIs Web  
> **Equipo:** Alex Fernando Alfaro Diaz (Backend / Scrum Master) & Camila Antonia Calderon Cortez (Móvil / QA)  
> **Herramienta de Gestión:** Azure DevOps (Azure Boards - Proceso Scrum / Agile)  

---

## 1. Marco Metodológico y Definiciones del Equipo

### 1.1 Asignación de Roles y Responsabilidades
* **Alex Fernando Alfaro Diaz (Backend / Scrum Master):**
  * Responsable de la arquitectura y desarrollo de la Web API RESTful en Java 21 Spring Boot 3.3.3 (`com.shushinestudio`).
  * Persistencia en PostgreSQL con Spring Data JPA e Hibernate (conexión a Supabase).
  * Implementación del motor de cálculo de disponibilidad y transacciones ACID contra concurrencia.
  * Configuración de base de datos en Supabase, seguridad Spring Security 6 / JJWT, pipelines CI/CD y despliegue en Azure / Cloud.
* **Camila Antonia Calderon Cortez (Frontend Móvil / QA - Tester):**
  * Responsable del diseño y maquetación visual de la app móvil en .NET MAUI (C# / XAML) conforme a los wireframes oficiales.
  * Gestión de estados reactivos con MVVM (`CommunityToolkit.Mvvm`) y consumo de API con `HttpClient`.
  * Diseño y ejecución del plan de aseguramiento de calidad (QA), pruebas automatizadas y pruebas funcionales.
  * Generación de entregables y compilación de artefactos y paquetes de aplicación (.NET MAUI Release).

### 1.2 Escala de Estimación y Priorización
* **Estimación:** Serie de Fibonacci para *Story Points* (1, 2, 3, 5, 8, 13).
* **Estimación de Tareas:** Horas reales de trabajo (1h a 8h por tarea técnica).
* **Priorización:** Método **MoSCoW** (Must have [Crítico], Should have [Importante], Could have [Deseable], Won't have [Fuera de alcance inicial]).

### 1.3 Definition of Ready (DoR) — Cuándo una Historia entra al Sprint
1. La Historia de Usuario sigue el formato formal *"Como [rol] quiero [acción] para [beneficio]"*.
2. Posee Criterios de Aceptación verificables redactados en formato BDD/Gherkin (*Dado... Cuando... Entonces...*).
3. Tiene desglosadas sus Tareas Técnicas (Tasks) con responsable asignado (Alex o Camila) y horas estimadas.
4. Tiene asociado el wireframe o vista de diseño correspondiente (Páginas 1 a 20 del prototipo).
5. El contrato de datos (Endpoint HTTP, DTO Request/Response y código de error RFC 7807) está definido.
6. Los Story Points han sido acordados entre Alex Alfaro y Camila Calderón.

### 1.4 Definition of Done (DoD) — Cuándo una Historia se considera Terminada
1. **Código:** Escrito en inglés para lógica y nomenclatura, textos de UI en español neutro ([AGENTS.md](./AGENTS.md)).
2. **Backend:** Endpoint documentado y probado en Swagger/OpenAPI, con DTOs especializados y manejo de errores RFC 7807.
3. **Frontend Móvil:** Implementado bajo Clean Architecture con MVVM (`CommunityToolkit.Mvvm`) y estados reactivos (`IsBusy`, `ErrorMessage`, `ObservableCollection`).
4. **Seguridad:** Tokens JWT validados; sin secretos ni URLs quemadas en duro en el repositorio.
5. **Control de Versiones:** Pull Request en Azure DevOps revisado y aprobado por el compañero de equipo en la rama `develop`.

---

## 2. Plan de Sprints y Distribución de Carga de Trabajo

| Sprint | Duración | Objetivo Principal | Historias | Puntos Totales | Horas Estimadas |
| :---: | :---: | :--- | :--- | :---: | :---: |
| **Sprint 0** | 1 semana | Setup de arquitectura, Supabase, Backend API, App .NET MAUI y CI/CD en Azure DevOps. | US-1.01 a US-1.04 | 13 pts | 34h |
| **Sprint 1** | 2 semanas | Autenticación híbrida con Supabase Auth, manejo de JWT y perfil de usuario. | US-2.01 a US-2.03 | 16 pts | 37h |
| **Sprint 2** | 2 semanas | Catálogo categorizado, fichas de servicios y motor de cálculo de disponibilidad. | US-3.01 a US-3.04 | 21 pts | 47h |
| **Sprint 3** | 2 semanas | Motor transaccional de reservas, control de concurrencia e historial de citas. | US-4.01 a US-4.04 | 24 pts | 48h |
| **Sprint 4** | 2 semanas | Dashboard de salón, agenda timeline interactiva, walk-ins y mantenimiento. | US-5.01 a US-5.06 | 29 pts | 56h |
| **Sprint 5** | 1 semana | Pruebas integradas de concurrencia, manuales y entrega académica final. | US-6.01 a US-6.03 | 16 pts | 32h |

---

## 3. Desglose Detallado del Product Backlog (Historias y Tareas Asignadas)

---

### 🏛️ ÉPICA 1: Arquitectura Base, Infraestructura y DevOps (Sprint 0)
**Objetivo:** Establecer los cimientos desacoplados del sistema, entornos cloud y pipelines de Azure DevOps.

#### US-1.01: Configuración de Base de Datos, Triggers y Seguridad en Supabase
* **Prioridad:** Must have | **Estimación:** 3 SP | **Asignado Principal:** Alex Alfaro
* **Trazabilidad:** [DIAGRAMA_BASE_DE_DATOS.md](./docs/DIAGRAMA_BASE_DE_DATOS.md), [RECORDATORIOS_SUPABASE.md](./RECORDATORIOS_SUPABASE.md)
* **Descripción:**  
  *Como* desarrollador,  
  *quiero* desplegar el esquema relacional en PostgreSQL de Supabase con RLS, triggers y buckets de almacenamiento,  
  *para* que la Web API en Java Spring Boot y la app móvil tengan un entorno de persistencia seguro y reactivo.
* **Criterios de Aceptación:**
  * **Dado** el script DDL de Supabase, **cuando** se ejecute en el SQL Editor, **entonces** deben crearse las 12 tablas con sus llaves foráneas e índices.
  * **Dado** que la app móvil usa `anon_key`, **cuando** un usuario intente consultar tablas directamente vía PostgREST, **entonces** RLS debe denegar el acceso.
  * **Dado** un registro en `auth.users`, **cuando** se inserte una cuenta, **entonces** el trigger `on_auth_user_created` debe crear la fila en `public.usuarios` y `public.clientes`.
  * **Dado** Supabase Storage, **cuando** se creen los buckets `servicios-imagenes` y `estilistas-avatares`, **entonces** deben quedar públicos para lectura.
* **Tareas Técnicas Desglosadas:**
  * `TSK-1.01.1` [Alex Alfaro / 3h]: Ejecutar script DDL en SQL Editor de Supabase y verificar creación íntegra de tablas, claves foráneas e índices.
  * `TSK-1.01.2` [Alex Alfaro / 3h]: Implementar y probar función `handle_new_user()` y trigger `on_auth_user_created` para sincronización automática de usuarios.
  * `TSK-1.01.3` [Alex Alfaro / 2h]: Configurar políticas de Row Level Security (RLS) en todas las tablas del esquema `public`.
  * `TSK-1.01.4` [Camila Calderón / 2h]: Crear buckets de almacenamiento en Supabase Storage (`servicios-imagenes`, `estilistas-avatares`) y verificar políticas de lectura pública.

#### US-1.02: Inicialización de la Web API en Java Spring Boot 3.3.3
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Alex Alfaro
* **Trazabilidad:** [AGENTS.md](./AGENTS.md) (Sección 2, 3.4, 4, 5.4)
* **Descripción:**  
  *Como* desarrollador backend,  
  *quiero* crear el proyecto Spring Boot 3.3.3 con arquitectura en capas (`com.shushinestudio`), Spring Data JPA, Springdoc OpenAPI y RFC 7807,  
  *para* exponer endpoints RESTful robustos y tipados para el consumo móvil.
* **Criterios de Aceptación:**
  * **Dado** el proyecto de API, **cuando** se inicie, **entonces** debe conectarse exitosamente a PostgreSQL en Supabase mediante Spring Data JPA e HikariCP Pooler.
  * **Dado** el estándar RFC 7807, **cuando** ocurra cualquier error no controlado o validación fallida, **entonces** la API debe responder con `ProblemDetail` (`application/problem+json`) vía `@RestControllerAdvice`.
  * **Dado** el filtro de seguridad `JwtAuthenticationFilter`, **cuando** reciba una petición con `Bearer <token>`, **entonces** debe validar la firma criptográfica del JWT emitido por Supabase / Auth.
  * **Dado** el entorno de desarrollo, **cuando** se ingrese a `/swagger-ui.html`, **entonces** debe visualizarse la documentación interactiva OpenAPI con soporte para Bearer Token.
* **Tareas Técnicas Desglosadas:**
  * `TSK-1.02.1` [Alex Alfaro / 4h]: Scaffolding de solución Java Spring Boot 3.3.3 con arquitectura en capas (`com.shushinestudio`: controladores, servicios, repositorios, modelos, DTOs).
  * `TSK-1.02.2` [Alex Alfaro / 4h]: Configurar DataSource con Spring Data JPA / PostgreSQL y cadena de conexión segura a Supabase.
  * `TSK-1.02.3` [Alex Alfaro / 3h]: Configurar Spring Security 6 y filtro `JwtAuthenticationFilter` validando tokens de Supabase y roles (`ADMIN`, `CLIENTE`).
  * `TSK-1.02.4` [Alex Alfaro / 3h]: Configurar controlador global de excepciones (`@RestControllerAdvice`) y formato estandarizado RFC 7807 (`ProblemDetail`).
  * `TSK-1.02.5` [Alex Alfaro / 2h]: Configurar Swagger/OpenAPI (`springdoc-openapi`) con esquema de seguridad `Bearer JWT` para pruebas interactivas.

#### US-1.03: Inicialización de la App Móvil con .NET MAUI y Clean Architecture
* **Prioridad:** Must have | **Estimación:** 3 SP | **Asignado Principal:** Camila Calderón
* **Trazabilidad:** [AGENTS.md](./AGENTS.md) (Sección 2, 5.4)
* **Descripción:**  
  *Como* desarrolladora móvil,  
  *quiero* inicializar el proyecto .NET MAUI configurando `CommunityToolkit.Mvvm`, `HttpClient`, `IServiceCollection` y el sistema de temas visuales XAML,  
  *para* maquetar las vistas del salón con navegación fluida y arquitectura desacoplada.
* **Criterios de Aceptación:**
  * **Dado** el proyecto .NET MAUI, **cuando** se ejecute en Android/iOS/Windows, **entonces** debe renderizar el Splash/Tema base con la paleta cromática oficial (Rosa principal, Soft Blush, Lavanda).
  * **Dado** el cliente HTTP (`HttpClient` / `DelegatingHandler`), **cuando** se configure, **entonces** debe inyectar automáticamente el token JWT en el header `Authorization` e incluir un `ErrorDelegatingHandler` para deserializar RFC 7807.
  * **Dado** el contenedor de inyección de dependencias en `MauiProgram.cs` (`IServiceCollection`), **cuando** se registren servicios, repositorios y ViewModels, **entonces** las páginas no deben instanciar clases directamente.
* **Tareas Técnicas Desglosadas:**
  * `TSK-1.03.1` [Camila Calderón / 3h]: Inicializar proyecto .NET MAUI y definir estructura de directorios por capas (`Presentation`, `Domain`, `Data`, `Core`).
  * `TSK-1.03.2` [Camila Calderón / 3h]: Implementar `Resources/Styles` en XAML con paleta oficial de colores, tipografía y estilos de controles.
  * `TSK-1.03.3` [Camila Calderón / 4h]: Configurar cliente HTTP con `HttpClient`, manejador de autenticación y manejo de errores RFC 7807.
  * `TSK-1.03.4` [Camila Calderón / 2h]: Configurar inyección de dependencias centralizada en `MauiProgram.cs` (`IServiceCollection`).

#### US-1.04: Configuración de Repositorio, GitFlow y Tableros en Azure DevOps
* **Prioridad:** Should have | **Estimación:** 2 SP | **Asignado Principal:** Alex Alfaro
* **Trazabilidad:** [AGENTS.md](./AGENTS.md) (Sección 6)
* **Descripción:**  
  *Como* Scrum Master,  
  *quiero* configurar el proyecto en Azure DevOps con ramas protegidas (`main`, `develop`) y políticas de PR,  
  *para* asegurar la trazabilidad del código y la revisión por pares entre Alex y Camila.
* **Criterios de Aceptación:**
  * **Dado** el repositorio en Azure Repos / Git, **cuando** se intente hacer push directo a `main` o `develop`, **entonces** el servidor debe rechazarlo exigiendo un Pull Request con al menos 1 voto de aprobación.
  * **Dado** Azure Boards, **cuando** se cree el backlog, **entonces** las historias deben contar con tags de Sprint y asignación formal de roles.
* **Tareas Técnicas Desglosadas:**
  * `TSK-1.04.1` [Alex Alfaro / 2h]: Configurar políticas de rama en `main` y `develop` (requerir Pull Request y aprobación mínima).
  * `TSK-1.04.2` [Alex Alfaro / 2h]: Configurar Sprints (0 a 5), capacidad de equipo y columnas Kanban en Azure Boards.

---

### 🔐 ÉPICA 2: Autenticación, Seguridad y Perfiles de Usuario (Sprint 1)
**Objetivo:** Permitir el registro, autenticación híbrida y gestión de ficha de belleza del cliente.

#### US-2.01: Registro de Nuevos Clientes
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF01, Wireframe Pág. 5 (`RegistroClienteView`)
* **Descripción:**  
  *Como* cliente nue  * **Criterios de Aceptación:**
  * **Dado** el formulario `RegistroClienteView`, **cuando** el usuario ingrese datos válidos y presione *"Registrarse"*, **entonces** la app en .NET MAUI invoca `supabase.auth.signUp()` con metadatos personales.
  * **Dado** que el usuario fue creado en `auth.users`, **cuando** se active el trigger de PostgreSQL, **entonces** se insertan automáticamente sus registros vinculados en `public.usuarios` y `public.clientes`.
  * **Dado** un correo electrónico previamente registrado, **cuando** se intente duplicar, **entonces** la app muestra un mensaje en español: *"Este correo ya está registrado"*.
* **Tareas Técnicas Desglosadas:**
  * `TSK-2.01.1` [Camila Calderón / 4h]: Maquetar `RegistroClienteView` con campos validados en XAML (nombre, correo, teléfono, contraseña).
  * `TSK-2.01.2` [Camila Calderón / 4h]: Implementar `RegisterViewModel` (comando `RegisterCommand`) llamando al servicio de autenticación con metadatos.
  * `TSK-2.01.3` [Alex Alfaro / 3h]: Validar en PostgreSQL que el trigger cree el usuario en `public.usuarios` con rol `Cliente` y su fila en `public.clientes`.
  * `TSK-2.01.4` [Camila Calderón / 2h]: Pruebas de QA de validación de entradas inválidas y notificación amigable de correo duplicado.

#### US-2.02: Inicio de Sesión y Manejo Seguro de Sesión con JWT
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF02, Wireframe Pág. 6 (`LoginClienteView`)
* **Descripción:**  
  *Como* usuario registrado,  
  *quiero* iniciar sesión con mi correo y contraseña y mantener mi sesión abierta,  
  *para* interactuar con la app sin tener que digitar mis credenciales en cada ingreso.
* **Criterios de Aceptación:**
  * **Dado** `LoginClienteView`, **cuando** las credenciales sean correctas, **entonces** Supabase Auth emite el `access_token` (JWT) con el rol inyectado mediante el Custom Access Token Hook.
  * **Dado** el token en el cliente móvil, **cuando** se almacene en almacenamiento seguro (`SecureStorage`), **entonces** la app navega al catálogo principal.
  * **Dado** que el token expira durante una petición a la Web API, **cuando** el manejador HTTP detecte `401 Unauthorized`, **entonces** debe ejecutar `refreshSession()` de Supabase; si falla, redirigir al login y limpiar sesión en `SecureStorage`.
* **Tareas Técnicas Desglosadas:**
  * `TSK-2.02.1` [Camila Calderón / 3h]: Maquetar `LoginClienteView` en XAML con toggles de visibilidad de password y validación reactiva.
  * `TSK-2.02.2` [Alex Alfaro / 3h]: Implementar función `custom_access_token_hook` en Supabase para inyectar claims (`role`, `internal_user_id`).
  * `TSK-2.02.3` [Camila Calderón / 4h]: Implementar flujo de login en `LoginViewModel` y persistencia segura de token con `SecureStorage`.
  * `TSK-2.02.4` [Camila Calderón / 3h]: Implementar lógica de refresco de token en `ErrorDelegatingHandler` ante respuesta 401.

#### US-2.03: Visualización y Edición del Perfil de Cliente y Ficha Estética
* **Prioridad:** Should have | **Estimación:** 6 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** Wireframe Pág. 14 (`PerfilClienteView`), Endpoint `GET/PUT /api/perfil`
* **Descripción:**  
  *Como* cliente,  
  *quiero* visualizar mi información personal, nivel de fidelidad y preferencias capilares,  
  *para* mantener actualizados mis datos de contacto y recordatorios de estilista.
* **Criterios de Aceptación:**
  * **Dado** un usuario autenticado, **cuando** la app llame a `GET /api/perfil`, **entonces** la API devuelve nombre, correo, teléfono, fecha de nacimiento, tipo de cabello y nivel de fidelidad (Oro/Plata/Bronce).
  * **Dado** el formulario de edición en `PerfilClienteView`, **cuando** el usuario actualice su teléfono o notas de preferencias, **entonces** `PUT /api/perfil` persiste los cambios en `public.clientes`.
* **Tareas Técnicas Desglosadas:**
  * `TSK-2.03.1` [Alex Alfaro / 4h]: Implementar controlador `PerfilController` con endpoints `GET /api/perfil` y `PUT /api/perfil`.
  * `TSK-2.03.2` [Camila Calderón / 4h]: Maquetar `PerfilClienteView` con avatar, insignia de fidelidad y formulario de preferencias.
  * `TSK-2.03.3` [Camila Calderón / 3h]: Implementar `ProfileViewModel` conectando DataSource y Repository con la API.
  * `TSK-2.03.4` [Camila Calderón / 2h]: Pruebas de QA de actualización de datos personales y verificación en base de datos.

---

### 💇 ÉPICA 3: Catálogo de Servicios y Motor de Disponibilidad (Sprint 2)
**Objetivo:** Explorar la oferta comercial del salón y calcular en tiempo real los bloques de tiempo libres por estilista.

#### US-3.01: Catálogo de Servicios Categorizados y Filtros Rápidos
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF03, Wireframe Pág. 7 (`CatalogoServiciosView`), Endpoint `GET /api/servicios`
* **Descripción:**  
  *Como* cliente,  
  *quiero* explorar el catálogo de tratamientos clasificados por categorías (*Cabello*, *Uñas*, *Maquillaje*, *Spa*),  
  *para* encontrar fácilmente el procedimiento que deseo realizarme.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/servicios?categoriaId={id}`, **cuando** se consulte, **entonces** retorna únicamente servicios con `activo = true` con su código (`SRV-C01`), nombre, precio, duración en minutos e URL de imagen.
  * **Dado** `CatalogoServiciosView`, **cuando** el cliente seleccione el chip de filtro "Uñas", **entonces** la lista se filtra de forma instantánea sin recargas completas.
* **Tareas Técnicas Desglosadas:**
  * `TSK-3.01.1` [Alex Alfaro / 4h]: Implementar `ServiciosController` con `GET /api/servicios` y filtrado por categoría.
  * `TSK-3.01.2` [Camila Calderón / 4h]: Maquetar `CatalogoServiciosView` en XAML con selector horizontal de categorías y tarjetas de servicio.
  * `TSK-3.01.3` [Camila Calderón / 3h]: Implementar `CatalogViewModel` con manejo de estados y colecciones observables filtradas.
  * `TSK-3.01.4` [Camila Calderón / 2h]: Pruebas de QA de carga de catálogo y renderizado de imágenes desde Supabase Storage.

#### US-3.02: Ficha Técnica y Detalle del Tratamiento
* **Prioridad:** Should have | **Estimación:** 3 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** Wireframe Pág. 8 (`DetalleServicioView`), Endpoint `GET /api/servicios/{id}`
* **Descripción:**  
  *Como* cliente,  
  *quiero* ingresar al detalle del servicio para ver su descripción paso a paso y tiempo estimado,  
  *para* conocer las condiciones y preparación antes de reservar.
* **Criterios de Aceptación:**
  * **Dado** el identificador del servicio, **cuando** se consulte `GET /api/servicios/{id}`, **entonces** la API devuelve la ficha completa incluyendo precio, tiempo y protocolo descriptivo.
  * **Dado** `DetalleServicioView`, **cuando** el cliente presione *"Continuar con la reserva $\rightarrow$"*, **entonces** la app transiciona al selector de estilistas pasando el servicio en el ViewModel de navegación.
* **Tareas Técnicas Desglosadas:**
  * `TSK-3.02.1` [Alex Alfaro / 2h]: Implementar endpoint `GET /api/servicios/{id}` retornando DTO detallado de servicio.
  * `TSK-3.02.2` [Camila Calderón / 3h]: Maquetar `DetalleServicioView` con foto destacada, precio, tiempo y protocolo paso a paso.
  * `TSK-3.02.3` [Camila Calderón / 2h]: Conectar transición hacia el flujo de reserva transmitiendo el ID del servicio en el ViewModel.

#### US-3.03: Selección de Estilistas y Asignación Automática ("Cualquiera Disponible")
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF04, Wireframe Pág. 9 (`SeleccionEstilistaView`), Endpoint `GET /api/estilistas?servicioId={id}`
* **Descripción:**  
  *Como* cliente,  
  *quiero* ver qué profesionales están capacitados para el servicio seleccionado o elegir la opción *"Cualquiera disponible"*,  
  *para* elegir a mi estilista preferido o conseguir el horario más próximo.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/estilistas?servicioId={id}`, **cuando** se consulte, **entonces** la API retorna únicamente estilistas activos vinculados en `estilista_servicios`.
  * **Dado** `SeleccionEstilistaView`, **cuando** el cliente elija la tarjeta destacada *"Cualquier estilista disponible (Asignación automática más rápida)"*, **entonces** el ViewModel almacena `idEstilista = null` para delegar la asignación algorítmica a la API.
* **Tareas Técnicas Desglosadas:**
  * `TSK-3.03.1` [Alex Alfaro / 4h]: Implementar endpoint `GET /api/estilistas?servicioId={id}` cruzando tabla `estilista_servicios`.
  * `TSK-3.03.2` [Camila Calderón / 4h]: Maquetar `SeleccionEstilistaView` con tarjetas de personal y opción especial destacada "Cualquier estilista disponible".
  * `TSK-3.03.3` [Camila Calderón / 3h]: Gestionar estado de selección en `BookingViewModel` (`SelectedStylist` vs `AutoAssignSelected`).

#### US-3.04: Motor de Cálculo de Disponibilidad Dinámica de Horarios
* **Prioridad:** Must have | **Estimación:** 8 SP | **Asignado Principal:** Alex Alfaro (Backend) & Camila Calderón (UI)
* **Trazabilidad:** RF05, RNF03, Wireframe Pág. 10 (`SeleccionHorarioView`), Endpoint `GET /api/disponibilidad`
* **Descripción:**  
  *Como* cliente,  
  *quiero* seleccionar una fecha en el calendario y ver los bloques de horas realmente disponibles,  
  *para* no seleccionar horarios en conflicto ni fuera de la jornada laboral.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/disponibilidad?estilistaId={id}&fecha={fecha}&servicioId={id}`, **cuando** la API se ejecute, **entonces** debe:
    1. Obtener la jornada del estilista en `horarios_estilista` para ese día de la semana.
    2. Obtener todas las reservas activas (`Pendiente` o `Completada`) en `reservas` para esa fecha.
    3. Segmentar el día en slots acordes a la duración del servicio (ej. 45 min) y restar los intervalos ocupados.
    4. Si se solicitó "Cualquier estilista", cruzar la unión de todos los estilistas disponibles para el servicio.
  * **Dado** `SeleccionHorarioView`, **cuando** el cliente pulse una fecha, **entonces** los bloques libres se colorean en verde menta/rosa y los ocupados quedan deshabilitados en gris.
* **Tareas Técnicas Desglosadas:**
  * `TSK-3.04.1` [Alex Alfaro / 8h]: Desarrollar `AvailabilityService` en Java Spring Boot implementando el algoritmo de segmentación de slots temporales y cruce de reservas.
  * `TSK-3.04.2` [Alex Alfaro / 4h]: Implementar endpoint `GET /api/disponibilidad` con consultas optimizadas para fechas consultadas frecuentemente.
  * `TSK-3.04.3` [Camila Calderón / 5h]: Maquetar `SeleccionHorarioView` en XAML con calendario interactivo y selector de bloques horarios.
  * `TSK-3.04.4` [Camila Calderón / 3h]: Conectar `AvailabilityViewModel` deshabilitando visualmente los slots no disponibles.
  * `TSK-3.04.5` [Alex & Camila / 3h]: Pruebas integradas de casos límite (horarios de almuerzo, citas continuas y días no laborables).

---

### 📅 ÉPICA 4: Motor Transaccional de Reservas y Gestión Personal (Sprint 3)
**Objetivo:** Transaccionar citas con control estricto de concurrencia (ACID), confirmación y gestión del historial de citas.

#### US-4.01: Resumen y Creación de Reserva con Control de Concurrencia
* **Prioridad:** Must have | **Estimación:** 8 SP | **Asignado Principal:** Alex Alfaro (ACID) & Camila Calderón (UI)
* **Trazabilidad:** RF06, RNF02, Wireframe Pág. 11 (`ResumenReservaView`), Endpoint `POST /api/reservas`
* **Descripción:**  
  *Como* cliente,  
  *quiero* revisar el resumen de mi cita con desglose de precios y políticas y confirmar mi reserva,  
  *para* garantizar mi cupo sin riesgo de que otra persona me gane el espacio en el mismo segundo.
* **Criterios de Aceptación:**
  * **Dado** `ResumenReservaView`, **cuando** se presente la pantalla, **entonces** muestra servicio, profesional, fecha, hora (con advertencia de presentarse 5 min antes) y desglose total calculado por la API.
  * **Dado** `POST /api/reservas`, **cuando** la API procese la solicitud, **entonces** debe ejecutar una transacción con nivel de aislamiento serializable o bloqueo de fila:
    * Si el slot sigue desocupado: aprueba la reserva, asigna estación de trabajo libre y devuelve `201 Created` con el código generado.
    * Si otro usuario confirmó milisegundos antes el mismo slot: hace *Rollback* y devuelve `409 Conflict` (RFC 7807) con el mensaje: *"El estilista ya no tiene disponible la franja horaria solicitada"*.
* **Tareas Técnicas Desglosadas:**
  * `TSK-4.01.1` [Alex Alfaro / 6h]: Implementar `CreateAppointmentUseCase` con transacción ACID en backend y bloqueo contra solapamientos.
  * `TSK-4.01.2` [Alex Alfaro / 3h]: Implementar asignación automática de estación de trabajo libre según categoría del servicio.
  * `TSK-4.01.3` [Camila Calderón / 4h]: Maquetar `ResumenReservaView` en XAML con ficha descriptiva de costos, impuestos y botón de confirmación.
  * `TSK-4.01.4` [Camila Calderón / 4h]: Conectar `BookingViewModel` con `POST /api/reservas` y manejar respuestas de éxito y error `409 Conflict`.

#### US-4.02: Comprobante y Confirmación de Cita con Código Único
* **Prioridad:** Must have | **Estimación:** 3 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** Wireframe Pág. 12 (`ConfirmacionReservaView`)
* **Descripción:**  
  *Como* cliente,  
  *quiero* ver una pantalla de confirmación exitosa con mi código alfanumérico único de cita,  
  *para* presentarme al salón o guardarlo como comprobante de atención.
* **Criterios de Aceptación:**
  * **Dado** una reserva exitosa, **cuando** la app transicione a `ConfirmacionReservaView`, **entonces** renderiza el código alfanumérico generado (ej. `#SHU-8492`), fecha, hora, estilista y dirección de la sucursal.
  * **Dado** los botones de acción, **cuando** el cliente pulse *"Ver en Mis Citas"*, **entonces** navega a la pestaña de historial; si pulsa *"Volver al Inicio"*, regresa al catálogo.
* **Tareas Técnicas Desglosadas:**
  * `TSK-4.02.1` [Alex Alfaro / 2h]: Desarrollar generador de código de reserva alfanumérico único (`#SHU-XXXX`) garantizando no duplicidad.
  * `TSK-4.02.2` [Camila Calderón / 3h]: Maquetar `ConfirmacionReservaView` en XAML con tarjeta tipo comprobante de cita y detalles de la sucursal.
  * `TSK-4.02.3` [Camila Calderón / 2h]: Configurar redirecciones hacia *"Mis Citas"* o pantalla principal.

#### US-4.03: Historial de Citas del Cliente (Próximas y Pasadas)
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF07, RNF04, Wireframe Pág. 13 (`HistorialCitasView`), Endpoint `GET /api/reservas/cliente/{id}`
* **Descripción:**  
  *Como* cliente autenticado,  
  *quiero* consultar mis citas activas agendadas y el histórico de servicios recibidos previamente,  
  *para* llevar el control de mis visitas y estar al tanto de mis citas futuras.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/reservas/cliente/{id}`, **cuando** el usuario autenticado consulte, **entonces** la API valida en sus claims que el `{id}` coincida con su identidad (RNF04 - Protección de Privacidad).
  * **Dado** `HistorialCitasView`, **cuando** el cliente cambie entre pestañas, **entonces** *Citas Futuras* muestra reservas en estado `Pendiente`, y *Citas Pasadas* muestra citas `Completadas` o `Canceladas`.
* **Tareas Técnicas Desglosadas:**
  * `TSK-4.03.1` [Alex Alfaro / 4h]: Implementar endpoint `GET /api/reservas/cliente/{id}` con validación de claims contra accesos no autorizados.
  * `TSK-4.03.2` [Camila Calderón / 4h]: Maquetar `HistorialCitasView` en XAML con pestañas *"Citas Futuras"* y *"Citas Pasadas"*.
  * `TSK-4.03.3` [Camila Calderón / 3h]: Conectar `AppointmentsHistoryViewModel` y renderizar tarjetas con badges de estado.

#### US-4.04: Cancelación y Solicitud de Reprogramación de Citas
* **Prioridad:** Should have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** Wireframe Pág. 13 (`HistorialCitasView`), Endpoint `PUT /api/reservas/{id}/cancelar`
* **Descripción:**  
  *Como* cliente,  
  *quiero* cancelar una cita pendiente con anticipación si no puedo asistir,  
  *para* liberar el espacio del estilista y permitir que otro cliente reserve.
* **Criterios de Aceptación:**
  * **Dado** una cita en estado `Pendiente`, **cuando** el cliente pulse *"Cancelar Reserva"* y confirme en el diálogo, **entonces** `PUT /api/reservas/{id}/cancelar` actualiza el estado a `Cancelada`.
  * **Dado** que la cita está a menos de 2 horas de su inicio, **cuando** el cliente intente cancelarla, **entonces** la API rechaza la acción con `400 Bad Request` indicando la política de cancelación del salón.
* **Tareas Técnicas Desglosadas:**
  * `TSK-4.04.1` [Alex Alfaro / 3h]: Implementar endpoint `PUT /api/reservas/{id}/cancelar` con regla de validación de límite de 2 horas.
  * `TSK-4.04.2` [Camila Calderón / 3h]: Implementar modal de confirmación de cancelación en .NET MAUI.
  * `TSK-4.04.3` [Camila Calderón / 3h]: Conectar flujo de reprogramación redirigiendo al usuario al selector de horario.
  * `TSK-4.04.4` [Camila Calderón / 2h]: Pruebas de QA de cancelación y liberación inmediata de slots en la agenda.

---

### 👑 ÉPICA 5: Panel de Control Administrativo y Gestión del Salón (Sprint 4)
**Objetivo:** Proveer herramientas exclusivas para recepción y gerencia (dashboard de KPIs, agenda timeline, walk-ins y mantenimiento).

#### US-5.01: Dashboard Operativo Diario y Métricas Clave
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF08, Wireframe Pág. 15 (`DashboardAdminView`), Endpoint `GET /api/admin/dashboard`
* **Descripción:**  
  *Como* administrador del salón,  
  *quiero* visualizar un panel diario con citas programadas, porcentaje de ocupación e ingresos proyectados,  
  *para* monitorear la salud financiera y operativa de la sucursal en tiempo real.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/admin/dashboard?fecha={fecha}`, **cuando** un usuario con rol `Administrador` lo consulte, **entonces** devuelve:
    1. Total de citas del día y % de capacidad ocupada (dividido en Mañana, Tarde, Noche).
    2. Citas completadas vs canceladas con tasa porcentual.
    3. Ingresos proyectados (cobrado vs por cobrar).
    4. Ocupación de estaciones de trabajo (`Corte`, `Color`, `Peinado`, `Uñas`, `Spa`).
  * **Dado** un usuario con rol `Cliente`, **cuando** intente consultar este endpoint, **entonces** la API responde de inmediato con `403 Forbidden`.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.01.1` [Alex Alfaro / 5h]: Implementar `AdminDashboardController` con consultas para métricas e ingresos.
  * `TSK-5.01.2` [Alex Alfaro / 2h]: Configurar filtro de autorización de rol de Administrador.
  * `TSK-5.01.3` [Camila Calderón / 5h]: Maquetar `DashboardAdminView` en XAML con tarjetas de KPI y monitores de capacidad.
  * `TSK-5.01.4` [Camila Calderón / 3h]: Conectar `AdminDashboardViewModel` con la API.

#### US-5.02: Agenda Diaria Interactiva en Formato Timeline Multi-Estilista
* **Prioridad:** Must have | **Estimación:** 8 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF09, Wireframe Pág. 16 (`WalkInClientView` / Agenda), Endpoint `GET /api/admin/agenda`
* **Descripción:**  
  *Como* recepcionista o administrador,  
  *quiero* ver una grilla interactiva tipo Timeline con columnas de estilistas y franjas horarias,  
  *para* coordinar las estaciones de trabajo y detectar espacios libres en la jornada.
* **Criterios de Aceptación:**
  * **Dado** el endpoint `GET /api/admin/agenda?fecha={fecha}`, **cuando** se consulte, **entonces** la API devuelve una estructura agrupada por cada estilista con la lista cronológica de sus citas y sus bloques de descanso.
  * **Dado** la vista en la app, **cuando** el recepcionista visualice la pantalla, **entonces** los bloques ocupados muestran nombre de cliente y servicio, y los bloques libres muestran un botón interactivo `+ Walk-in Client (Libre)`.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.02.1` [Alex Alfaro / 6h]: Implementar endpoint `GET /api/admin/agenda` agrupando reservas cronológicas por estilista.
  * `TSK-5.02.2` [Camila Calderón / 6h]: Maquetar vista Timeline multi-columna en XAML con bloques visuales de citas y espacios libres interactivos.
  * `TSK-5.02.3` [Camila Calderón / 3h]: Conectar selector de fecha para cambiar el día de visualización de la agenda.

#### US-5.03: Registro Rápido de Clientes Presenciales ("Walk-in Clients")
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF10, Wireframe Pág. 16 (`WalkInClientView`), Endpoint `POST /api/reservas/walk-in`
* **Descripción:**  
  *Como* recepcionista,  
  *quiero* registrar una cita inmediata para un cliente presencial sin cuenta en la app,  
  *para* agendar clientes espontáneos ocupando espacios muertos en la agenda.
* **Criterios de Aceptación:**
  * **Dado** el formulario modal de Walk-in, **cuando** la recepcionista ingrese Nombre, Teléfono temporal, estilista y servicio, **entonces** la API crea la reserva con `es_walk_in = true` y `id_cliente = null`.
  * **Dado** el registro exitoso, **cuando** se refresque la agenda Timeline, **entonces** el slot libre pasa a estar ocupado por el cliente presencial.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.03.1` [Alex Alfaro / 4h]: Implementar endpoint `POST /api/reservas/walk-in` guardando datos en campos `nombre_walk_in` y `telefono_walk_in`.
  * `TSK-5.03.2` [Camila Calderón / 4h]: Maquetar formulario modal rápido en XAML invocado desde los bloques libres de la agenda.
  * `TSK-5.03.3` [Camila Calderón / 3h]: Refrescar automáticamente la vista Timeline tras registrar una cita presencial.

#### US-5.04: Gestión del Ciclo de Vida y Auditoría de Estados de Citas
* **Prioridad:** Must have | **Estimación:** 4 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF11, Wireframe Pág. 17 (`EstadoReservaView`), Endpoint `PUT /api/admin/reservas/{id}/estado`
* **Descripción:**  
  *Como* administrador,  
  *quiero* cambiar el estado de una cita a `Pendiente`, `Completada`, `Cancelada` o `No Asistió` con observaciones,  
  *para* reflejar la atención real en el salón y mantener trazabilidad operativa.
* **Criterios de Aceptación:**
  * **Dado** `EstadoReservaView`, **cuando** se seleccione un nuevo estado y se guarde, **entonces** la API actualiza `reservas.estado`.
  * **Dado** el cambio de estado, **cuando** se ejecute la actualización, **entonces** la API inserta automáticamente una fila en `public.historial_estado_reserva` con `id_usuario_cambio`, fecha y estado anterior/nuevo.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.04.1` [Alex Alfaro / 3h]: Implementar `PUT /api/admin/reservas/{id}/estado` con guardado automático en `historial_estado_reserva`.
  * `TSK-5.04.2` [Camila Calderón / 3h]: Maquetar `EstadoReservaView` en XAML con selector de estados operativos y campo de observaciones.
  * `TSK-5.04.3` [Camila Calderón / 2h]: Pruebas de QA de cambio de estado y verificación de auditoría en base de datos.

#### US-5.05: Mantenimiento de Catálogo de Servicios y Precios
* **Prioridad:** Should have | **Estimación:** 4 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF12, Wireframe Pág. 18 (`AdministracionCatalogoView`), Endpoint `POST/PUT /api/servicios/{id}`
* **Descripción:**  
  *Como* administrador,  
  *quiero* modificar tarifas, tiempos de atención y activar o desactivar servicios,  
  *para* adaptar la oferta del salón a temporadas y promociones.
* **Criterios de Aceptación:**
  * **Dado** `AdministracionCatalogoView`, **cuando** el administrador use el toggle on/off de un servicio, **entonces** `PUT /api/servicios/{id}/toggle` actualiza el campo `activo` en la BD.
  * **Dado** un servicio desactivado, **cuando** un cliente consulte el catálogo móvil, **entonces** este servicio no se renderiza.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.05.1` [Alex Alfaro / 3h]: Implementar endpoints `POST /api/servicios` y `PUT /api/servicios/{id}` para creación y edición de tarifas.
  * `TSK-5.05.2` [Camila Calderón / 3h]: Maquetar `AdministracionCatalogoView` en XAML con switches on/off y modal de edición de precios.
  * `TSK-5.05.3` [Camila Calderón / 2h]: Pruebas de actualización y reflejo inmediato en el catálogo del cliente.

#### US-5.06: Control de Disponibilidad y Turnos de Estilistas
* **Prioridad:** Should have | **Estimación:** 3 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Trazabilidad:** RF12, Wireframe Pág. 19 (`EstadoEstilistaView`), Endpoint `PUT /api/estilistas/{id}/estado`
* **Descripción:**  
  *Como* administrador,  
  *quiero* marcar a un estilista como *"Inactivo Temporal"* si presenta una emergencia médica o permiso,  
  *para* que el motor de disponibilidad bloquee automáticamente la asignación de nuevas citas para su turno.
* **Criterios de Aceptación:**
  * **Dado** `EstadoEstilistaView`, **cuando** se marque un estilista como inactivo temporal, **entonces** el motor de cálculo de disponibilidad de la US-3.04 omite sus slots para citas futuras.
* **Tareas Técnicas Desglosadas:**
  * `TSK-5.06.1` [Alex Alfaro / 2h]: Implementar endpoint `PUT /api/estilistas/{id}/estado` actualizando `estado_disponibilidad`.
  * `TSK-5.06.2` [Camila Calderón / 3h]: Maquetar `EstadoEstilistaView` en XAML con lista de profesionales y selectores de disponibilidad.
  * `TSK-5.06.3` [Alex & Camila / 2h]: Verificar que estilistas inactivos queden automáticamente excluidos del cálculo de disponibilidad de citas.

---

### 🧪 ÉPICA 6: Pruebas Integrales, Rendimiento y Despliegue (Sprint 5)
**Objetivo:** Asegurar la calidad técnica (QA), pruebas de carga concurrentes y despliegue para la evaluación académica.

#### US-6.01: Pruebas Unitarias y de Integración Automatizadas
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón (QA) & Alex Alfaro (Backend)
* **Descripción:**  
  *Como* equipo de desarrollo,  
  *quiero* implementar pruebas automatizadas en backend con JUnit 5 / Mockito y pruebas unitarias con xUnit / Moq para ViewModels en .NET MAUI,  
  *para* asegurar que los casos de uso principales funcionen sin regresiones.
* **Criterios de Aceptación:**
  * **Backend:** Pruebas unitarias con JUnit 5 para el ciclo CRUD de servicios (`t1_crear` a `t6_eliminar`) y el algoritmo de disponibilidad horaria.
  * **Frontend:** Pruebas de ViewModels para el flujo completo de reserva (`ReservaViewModel`: estados `IsBusy` $\rightarrow$ `AppointmentCreated`).
* **Tareas Técnicas Desglosadas:**
  * `TSK-6.01.1` [Alex Alfaro / 5h]: Implementar pruebas unitarias de backend con JUnit 5 y Mockito para `AvailabilityService` y validaciones de negocio.
  * `TSK-6.01.2` [Camila Calderón / 5h]: Implementar pruebas automatizadas con xUnit/Moq para `AuthViewModel` y `ReservaViewModel` en .NET MAUI.

#### US-6.02: Pruebas de Estrés y Concurrencia Transaccional (RNF02)
* **Prioridad:** Must have | **Estimación:** 5 SP | **Asignado Principal:** Camila Calderón & Alex Alfaro
* **Descripción:**  
  *Como* evaluador de calidad,  
  *quiero* simular peticiones simultáneas de reserva sobre el mismo slot de estilista con k6 o Apache JMeter,  
  *para* certificar que el sistema jamás produzca una doble reserva (*Overbooking*).
* **Criterios de Aceptación:**
  * **Dado** 50 peticiones concurrentes enviadas en el mismo segundo para el mismo estilista, fecha y hora, **cuando** la API las procese, **entonces** exactamente 1 petición debe retornar `201 Created` y las 49 restantes deben retornar `409 Conflict` bajo el formato RFC 7807 (`ProblemDetail`).
* **Tareas Técnicas Desglosadas:**
  * `TSK-6.02.1` [Alex Alfaro / 4h]: Crear script de prueba de carga con k6 simulando 50 peticiones concurrentes en el mismo milisegundo.
  * `TSK-6.02.2` [Camila Calderón & Alex Alfaro / 4h]: Ejecutar prueba de carga, capturar evidencias de logs y verificar 1 respuesta 201 y 49 respuestas 409 Conflict.

#### US-6.03: Despliegue en la Nube y Generación de Entregables
* **Prioridad:** Must have | **Estimación:** 6 SP | **Asignado Principal:** Alex Alfaro & Camila Calderón
* **Descripción:**  
  *Como* equipo de desarrollo,  
  *quiero* desplegar la Web API Java Spring Boot y compilar el paquete Release de .NET MAUI con variables de entorno protegidas,  
  *para* presentar la solución funcional ante los docentes de ESFE AGAPE / MEGATEC.
* **Criterios de Aceptación:**
  * La Web API se encuentra accesible bajo HTTPS con Swagger activo en producción/staging.
  * La app móvil en .NET MAUI se conecta de forma transparente con Supabase y la API.
* **Tareas Técnicas Desglosadas:**
  * `TSK-6.03.1` [Alex Alfaro / 4h]: Publicar Web API en el entorno cloud / App Service configurando variables de entorno y conexión segura a Supabase.
  * `TSK-6.03.2` [Camila Calderón / 4h]: Compilar paquete Release de .NET MAUI apuntando a la URL pública de la API.
  * `TSK-6.03.3` [Alex & Camila / 3h]: Redactar reporte final y preparar entorno para demostración académica en vivo.

---

## 4. Matriz de Trazabilidad Cruzada

| Código RF / RNF | Historia de Usuario (US) | Vista / Wireframe | Endpoint API (Spring Boot) | Tabla BD Principal | Responsables |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **RF01** | US-2.01 (Registro) | `RegistroClienteView` (Pág. 5) | `POST /api/auth/registro` | `usuarios`, `clientes` | Camila (UI) / Alex (BD) |
| **RF02** | US-2.02 (Login JWT) | `LoginClienteView` (Pág. 6) | Supabase Auth + Spring Security JWT | `usuarios`, `roles` | Camila (UI) / Alex (Hook) |
| **RF03** | US-3.01 (Catálogo) | `CatalogoServiciosView` (Pág. 7) | `GET /api/servicios` | `servicios`, `categorias_servicio` | Alex (API) / Camila (UI) |
| **RF04** | US-3.03 (Estilistas) | `SeleccionEstilistaView` (Pág. 9) | `GET /api/estilistas` | `estilistas`, `estilista_servicios` | Alex (API) / Camila (UI) |
| **RF05** | US-3.04 (Disponibilidad) | `SeleccionHorarioView` (Pág. 10) | `GET /api/disponibilidad` | `horarios_estilista`, `reservas` | Alex (Reglas) / Camila (UI) |
| **RF06** | US-4.01 (Reserva ACID) | `ResumenReservaView` (Pág. 11) | `POST /api/reservas` | `reservas`, `estaciones_trabajo` | Alex (Transac.) / Camila (UI) |
| **RF07** | US-4.03 (Historial) | `HistorialCitasView` (Pág. 13) | `GET /api/reservas/cliente/{id}` | `reservas`, `servicios`, `estilistas` | Alex (API) / Camila (UI) |
| **RF08** | US-5.01 (Dashboard) | `DashboardAdminView` (Pág. 15) | `GET /api/admin/dashboard` | `reservas`, `estaciones_trabajo` | Alex (Métricas) / Camila (UI) |
| **RF09** | US-5.02 (Agenda Timeline) | `WalkInClientView` (Pág. 16) | `GET /api/admin/agenda` | `reservas`, `estilistas` | Alex (API) / Camila (UI) |
| **RF10** | US-5.03 (Walk-in Clients) | `WalkInClientView` (Pág. 16) | `POST /api/reservas/walk-in` | `reservas` | Alex (API) / Camila (UI) |
| **RF11** | US-5.04 (Estado Reserva) | `EstadoReservaView` (Pág. 17) | `PUT /api/admin/reservas/{id}/estado` | `reservas`, `historial_estado_reserva` | Alex (API) / Camila (UI) |
| **RF12** | US-5.05 / US-5.06 (Mantenimiento) | `AdministracionCatalogoView` (Pág. 18-19) | `PUT /api/servicios/{id}` | `servicios`, `estilistas` | Alex (API) / Camila (UI) |
| **RNF01** | US-1.02 / US-2.02 (RBAC) | Todas las vistas protegidas | `@PreAuthorize("hasRole('...')")` | `roles`, JWT Claims | Alex (Backend) |
| **RNF02** | US-4.01 / US-6.02 (Concurrencia) | `ResumenReservaView` (Pág. 11) | `POST /api/reservas` (Transacción ACID) | `reservas` (`ix_reservas_disponibilidad`) | Alex (Backend) / Camila (QA) |
| **RNF03** | US-3.04 (Disponibilidad Dinámica) | `SeleccionHorarioView` (Pág. 10) | `GET /api/disponibilidad` | `horarios_estilista`, `reservas` | Alex (Backend) |
| **RNF04** | US-4.03 (Privacidad de Datos) | `HistorialCitasView` (Pág. 13) | `GET /api/reservas/cliente/{id}` | Token Claims `internal_user_id` | Alex (Backend) |
