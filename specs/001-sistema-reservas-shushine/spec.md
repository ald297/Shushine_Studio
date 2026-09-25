# Feature Specification: Sistema Móvil de Gestión y Reservas Shushine Studio

**Feature Branch**: `001-sistema-reservas-shushine`  
**Created**: 2026-09-11  
**Status**: Ready for Planning  
**Input**: Contexto integral del proyecto Shushine Studio (Product Backlog, Diagrama de Base de Datos PostgreSQL / Supabase, Reglas de Arquitectura AGENTS.md, Wireframes del Salón)

---

## Alcance y Delimitación del MVP (Feature 001 vs Fases Futuras)

Para garantizar foco en la entrega y calidad técnica evaluable, el proyecto se organiza en fases claramente delimitadas:
* **Alcance de Feature 001 (MVP Core - Esta Entrega):**
  1. **US1:** Autenticación y Perfil de Usuario con JWT y roles oficiales (`ADMIN`, `CLIENTE`).
  2. **US2:** Catálogo Categorizado de Servicios con precios, tiempos y fotos de Supabase Storage.
  3. **US3:** Motor de Disponibilidad horaria en tiempo real por estilista.
  4. **US4:** Creación Transaccional de Citas con control de concurrencia y código correlativo (`SHU-YYYY-NNNN`).
  5. **US5:** Agenda Timeline para estilistas/administración y atención de clientes espontáneos (*Walk-in*).
  6. **US6:** Registro de Pagos (Efectivo/Tarjeta), Facturación con IVA (13%) y Dashboard gerencial de métricas.
* **Módulos Avanzados (Backlog Fases 2 y 3 - Documentados en `DOCUMENTO_EVOLUCION_SHUNSHINE_STUDIO.md`):**
  - Chat bidireccional en tiempo real por cita (`MensajesChatCita`).
  - Módulo de Cotización de Diseños Personalizados con subida de referencias a Storage (`SolicitudesDiseno` y `Cotizaciones`).
  - Acumulación y redención de puntos de fidelidad (`TransaccionesPuntos` / `NivelCliente`).
  - Módulo de Reseñas y Calificaciones post-servicio.

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Autenticación y Gestión de Perfil de Usuario (Priority: P1) 🎯 MVP Core
Como cliente o miembro del personal del salón, quiero autenticarme de manera segura mediante mi correo y contraseña o registro rápido, para acceder a mis citas, gestionar mi perfil y contar con una sesión protegida.

* **Why this priority**: Es el punto de entrada mandatorio del sistema. Sin autenticación y emisión de tokens seguros, ninguna consulta transaccional ni reservación puede ser atribuida de forma confiable al cliente.
* **Independent Test**: Puede probarse de forma autónoma registrando un nuevo usuario, iniciando sesión, validando la emisión del JWT de Supabase, verificando la creación automática del perfil en la base de datos y consultando el endpoint `/api/auth/me`.
* **Acceptance Scenarios**:
  1. **Given** un usuario no registrado con correo y contraseña válidos, **When** solicita el registro en la aplicación móvil, **Then** Supabase Auth crea el usuario, se dispara el trigger `on_auth_user_created` en PostgreSQL insertando el registro en `usuarios` y `clientes`, y la app inicia sesión automáticamente.
  2. **Given** un usuario registrado con credenciales correctas, **When** ingresa sus datos y pulsa "Iniciar Sesión", **Then** la app recibe un JWT válido y un refresh token, permitiendo el acceso a la pantalla principal.
  3. **Given** un usuario con sesión iniciada, **When** accede a su perfil, **Then** puede consultar y actualizar su nombre, teléfono y preferencias personales.
  4. **Given** un token expirado, **When** la aplicación realiza una petición a la API, **Then** el interceptor de red ejecuta el refresco automático de sesión sin interrumpir la experiencia del usuario.

---

### User Story 2 - Exploración de Catálogo Categorizado y Ficha de Servicios (Priority: P2)
Como cliente del salón, quiero explorar el catálogo de servicios organizados por categorías (Cabello, Uñas, Maquillaje, Cuidado Facial), ver detalles, precios, duración y promociones, para elegir los tratamientos que deseo reservar.

* **Why this priority**: Los clientes necesitan conocer con claridad la oferta del salón, tiempos estimados y costos antes de poder seleccionar fecha y estilista.
* **Independent Test**: Puede probarse de forma aislada consumiendo los endpoints de categorías y servicios (`/api/categories`, `/api/services`), verificando que los filtros por categoría y la búsqueda por texto funcionen correctamente y rendericen las imágenes públicas de Supabase Storage.
* **Acceptance Scenarios**:
  1. **Given** un cliente en la pantalla de inicio, **When** visualiza el catálogo, **Then** observa un carrusel de categorías activas y un listado de servicios destacados con imagen, nombre, duración estimada y precio en dólares.
  2. **Given** un cliente buscando un servicio específico, **When** escribe un término en el buscador o selecciona una categoría, **Then** la lista se filtra instantáneamente mostrando solo los servicios coincidentes.
  3. **Given** un cliente que pulsa sobre una tarjeta de servicio, **When** se abre el detalle, **Then** se muestran la descripción completa, insumos utilizados, recomendaciones previas y el botón de acción "Reservar Cita".

---

### User Story 3 - Motor de Disponibilidad y Selección de Estilistas (Priority: P3)
Como cliente, quiero seleccionar un estilista calificado (o elegir "Cualquier profesional disponible") y una fecha en el calendario, para ver en tiempo real las franjas horarias disponibles calculadas por la API.

* **Why this priority**: Es el núcleo inteligente del salón. La Web API en Spring Boot debe ser la autoridad única que cruce horarios laborales, descansos y citas previas para no permitir citas sobrepuestas.
* **Independent Test**: Puede probarse enviando una fecha y el ID de un estilista al endpoint `/api/stylists/{id}/availability`, verificando que devuelva únicamente bloques horarios libres de 30 o 45 minutos que no colisionen con citas existentes ni bloqueos.
* **Acceptance Scenarios**:
  1. **Given** un servicio seleccionado de 60 minutos, **When** el cliente elige una fecha y un estilista, **Then** la API calcula y retorna los slots horarios continuos donde el estilista tiene agenda libre según su horario laboral.
  2. **Given** un estilista con una cita confirmada de 10:00 AM a 11:00 AM, **When** otro cliente consulta la disponibilidad para esa fecha, **Then** el slot de las 10:00 AM no figura disponible para selección.
  3. **Given** la opción "Cualquier estilista disponible", **When** el cliente selecciona una hora, **Then** el sistema asigna automáticamente al profesional con menor carga de trabajo para esa franja.

---

### User Story 4 - Creación Transaccional de Reserva y Prevención de Doble Reserva (Priority: P4)
Como cliente, quiero confirmar el resumen de mi cita con el desglose de servicios, subtotales, impuestos y anticipo requerido, para asegurar mi franja horaria bajo una transacción segura que evite doble reserva.

* **Why this priority**: Garantiza la integridad financiera y de negocio del salón. Las reservas deben crearse bajo transacciones ACID en PostgreSQL para resolver colisiones concurrentes de manera atómica.
* **Independent Test**: Puede probarse simulando dos peticiones simultáneas con el mismo estilista y horario; la API debe confirmar la primera y responder con un error estructurado RFC 7807 (409 Conflict) a la segunda.
* **Acceptance Scenarios**:
  1. **Given** un cliente en la pantalla de resumen de reserva, **When** revisa el desglose y presiona "Confirmar Reserva", **Then** la API ejecuta una transacción que bloquea la franja, inserta la cita en estado `Pendiente` o `Confirmada` y genera el código único de cita.
  2. **Given** dos clientes intentando reservar simultáneamente el mismo slot horario con el mismo estilista, **When** ambas solicitudes llegan a la API con milisegundos de diferencia, **Then** la primera se aprueba y la segunda es rechazada con un código HTTP 409 Conflict y mensaje legible en español.
  3. **Given** una cita confirmada exitosamente, **When** el cliente revisa su sección "Mis Citas", **Then** puede visualizar el estado de la cita, fecha, hora, estilista asignado y opciones para reprogramar o cancelar con al menos 2 horas de anticipación.

---

### User Story 5 - Agenda Timeline Interactiva y Gestión de Citas para Estilistas / Admin (Priority: P5)
Como estilista o recepcionista/administrador del salón, quiero visualizar una agenda interactiva diaria y semanal con filtros por profesional, registrar clientes sin cita previa (walk-ins) y actualizar estados de las citas, para organizar el flujo de trabajo en cabina.

* **Why this priority**: Permite la operatividad física del negocio en el local, controlando llegadas, retrasos, servicios en proceso y atenciones directas sin reserva previa.
* **Independent Test**: Puede probarse consultando el endpoint `/api/admin/appointments/timeline?date=YYYY-MM-DD`, actualizando el estado de una cita a `En_Progreso` y registrando un cliente walk-in con asignación directa.
* **Acceptance Scenarios**:
  1. **Given** un estilista autenticado, **When** abre su agenda del día, **Then** visualiza las citas ordenadas cronológicamente con indicadores visuales por color según su estado (`Pendiente`, `Confirmada`, `En_Progreso`, `Completada`, `Cancelada`).
  2. **Given** un cliente que llega directamente al salón sin reserva previa, **When** el recepcionista registra un "Walk-in", **Then** el sistema asigna el servicio y estilista libre de inmediato sin requerir cuenta de correo previa del cliente.
  3. **Given** un cliente que concluye su servicio, **When** el estilista marca la cita como `Completada`, **Then** se habilita la generación del cobro y liberación inmediata de la cabina.

---

### User Story 6 - Registro de Pagos, Facturación y Métricas del Salón (Priority: P6)
Como administrador del salón, quiero registrar pagos en múltiples métodos (Efectivo, Tarjeta, Transferencia), emitir comprobantes de factura con IVA desglosado y consultar métricas diarias de ingresos, para mantener el control contable del negocio.

* **Why this priority**: Cierra el ciclo financiero y operativo del salón, garantizando trazabilidad contable y reportes gerenciales para toma de decisiones.
* **Independent Test**: Puede probarse registrando un pago para una cita completada en `/api/payments`, verificando la inserción en `transacciones_pago` y `facturas`, y consultando el endpoint del dashboard resumen `/api/admin/dashboard`.
* **Acceptance Scenarios**:
  1. **Given** una cita completada con total de $35.00, **When** el administrador registra el pago en efectivo o tarjeta, **Then** se genera una transacción de pago y se emite la factura con número correlativo fiscal e IVA (13%) calculado.
  2. **Given** el administrador en el Dashboard de Gestión, **When** consulta las métricas del día o mes, **Then** el sistema muestra el total recaudado, cantidad de citas atendidas, ticket promedio y servicios más demandados.

---

## Edge Cases

- **Colisión Concurrente de Slots:** Dos clientes presionan "Confirmar" exactamente al mismo tiempo sobre el mismo estilista y horario. *Solución:* Nivel de aislamiento serializable o bloqueo pesimista en base de datos (`SELECT FOR UPDATE`), devolviendo error RFC 7807 (409 Conflict) al segundo usuario.
- **Expiración de Sesión en Flujo de Pago:** El token JWT expira mientras el usuario está en la pantalla de confirmación. *Solución:* DelegatingHandler en HttpClient intercepta el 401, solicita refresco a Supabase Auth en segundo plano y reintenta la petición original transparentemente.
- **Cancelación Tardía:** Un cliente intenta cancelar su cita faltando menos de 2 horas para la hora pactada. *Solución:* La API valida la política de tiempo mínimo y rechaza la cancelación con mensaje indicando que debe comunicarse por vía telefónica con el salón.
- **Indisponibilidad Imprevista del Estilista:** Un estilista reporta incapacidad médica. *Solución:* El administrador registra un `bloqueo_horario` sobre el estilista, notificando a las citas afectadas para reasignación o reprogramación.
- **Pérdida de Conectividad Móvil:** La app pierde señal durante el envío de una reserva. *Solución:* Manejo de timeouts (10s), reintentos controlados con idempotency key y persistencia de estado local en el ViewModel para no duplicar reservas.

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE autenticar usuarios mediante correo/login y contraseña (cifrada con BCrypt) con emisión de tokens JWT seguros en la API (Spring Boot), soportando los roles oficiales de Shushine Studio (`ADMIN`, `CLIENTE`, `RECEPCIONISTA`).
- **FR-002**: El sistema DEBE sincronizar y poblar automáticamente mediante un semillero (`DataInitializer`) los roles base, usuarios de demostración (`admin` y `cliente`) y catálogo inicial si la base de datos PostgreSQL en Supabase está vacía.
- **FR-003**: La Web API DEBE ser la única autoridad de negocio responsable de calcular disponibilidad horaria, montos, impuestos y transacciones.
- **FR-004**: La aplicación móvil NO DEBE realizar consultas directas a las tablas de PostgreSQL de negocio; todas las operaciones deben cursarse exclusivamente a través de la Web API.
- **FR-005**: El sistema DEBE categorizar los servicios del salón y permitir su consulta filtrada por categoría, rango de precio y búsqueda textual.
- **FR-006**: El sistema DEBE calcular franjas horarias disponibles considerando: horario laboral del estilista, pausas/almuerzo, citas previas agendadas y bloqueos de calendario.
- **FR-007**: El sistema DEBE garantizar atomicidad transaccional (ACID con `@Transactional`) al crear una cita, impidiendo sobreventa o doble reserva de un mismo bloque horario.
- **FR-008**: Toda respuesta de error emitida por la API DEBE estructurarse bajo el estándar RFC 7807 Problem Details (`application/problem+json`).
- **FR-009**: El sistema DEBE permitir citas con múltiples servicios secuenciales, calculando la duración total acumulada y asignando el tiempo de cabina correspondiente.
- **FR-010**: El sistema DEBE soportar estados de cita: `Pendiente`, `Confirmada`, `En_Progreso`, `Completada`, `Cancelada`, `No_Asistio`.
- **FR-011**: El sistema DEBE validar que las cancelaciones autogestionadas por el cliente se realicen con un mínimo de 2 horas de anticipación.
- **FR-012**: El sistema DEBE permitir a los administradores registrar clientes presenciales ("walk-in") sin requerir registro previo de correo electrónico.
- **FR-013**: El sistema DEBE registrar transacciones de pago soportando Efectivo, Tarjeta de Crédito/Débito y Transferencia Bancaria.
- **FR-014**: El sistema DEBE emitir facturas con número correlativo único, fecha de emisión, subtotal, IVA (13%) y total desglosado.
- **FR-015**: El sistema DEBE proveer un endpoint de Dashboard gerencial con métricas de citas del día, ingresos totales, estilistas activos y ticket promedio.
- **FR-016**: La app móvil DEBE gestionar sus estados reactivos mediante el patrón MVVM (`CommunityToolkit.Mvvm`) desacoplado de las vistas XAML, almacenando el JWT de forma segura en almacenamiento protegido vía `Microsoft.Maui.Storage.SecureStorage`.
- **FR-017**: Las imágenes de servicios y avatares de estilistas DEBEN almacenarse en buckets públicos de Supabase Storage (`servicios-imagenes`, `estilistas-avatares`, `categorias-imagenes`).

---

## Key Entities & Data Relationships

1. **`usuarios`**: Almacena credenciales base y perfil general, enlazado 1:1 con `auth.users(id)`. Atributos: `id`, `email`, `nombre`, `apellido`, `telefono`, `rol` (Cliente, Estilista, Admin), `estado`.
2. **`clientes`**: Extensión de perfil para clientes con preferencias personales, notas alérgicas y fecha de nacimiento.
3. **`estilistas`**: Información profesional, especialidades, biografía, foto de perfil, color identificador en agenda y comisión.
4. **`categorias_servicio`**: Taxonomía de servicios (Cabello, Uñas, Rostro, Spa) con nombre, slug, icono y orden.
5. **`servicios`**: Catálogo de tratamientos ofrecidos con duración en minutos, precio base, costo insumos y estado activo.
6. **`horarios_estilistas`**: Jornadas laborales por día de la semana (`dia_semana`, `hora_inicio`, `hora_fin`, `hora_inicio_almuerzo`, `hora_fin_almuerzo`).
7. **`bloqueos_horarios`**: Períodos no laborables específicos por vacaciones, permisos o emergencias.
8. **`citas`**: Registro principal de la reserva (`codigo_cita`, `cliente_id`, `estilista_id`, `fecha_cita`, `hora_inicio`, `hora_fin`, `estado`, `subtotal`, `descuento`, `iva`, `total`).
9. **`cita_detalles`**: Relación 1:N entre la cita y los servicios contratados con precio unitario y duración aplicada.
10. **`transacciones_pago`**: Detalle del pago procesado (`monto`, `metodo_pago`, `referencia`, `estado_pago`).
11. **`facturas`**: Documento fiscal con número de factura, datos del emisor, cliente y desglose de impuestos.

---

## Success Criteria *(mandatory)*

### Quantitative Metrics
- **Tiempo de Reserva:** Un cliente puede completar una reserva desde la selección del servicio hasta la confirmación en menos de **90 segundos**.
- **Tiempo de Respuesta de Disponibilidad:** El cálculo de franjas horarias disponibles para cualquier fecha responde en menos de **300 milisegundos**.
- **Cero Doble Reserva:** Tasa de colisión de reservas concurrentes del **0.0%** bajo pruebas de estrés concurrentes.
- **Disponibilidad del Sistema:** API operativa y accesible al menos el **99.5%** del tiempo de pruebas académicas.
- **Tolerancia a Fallos de Red:** El 100% de los errores 401 por expiración de token se resuelven mediante refresco transparente sin desloguear al usuario de forma prematura.

### Qualitative Metrics
- **Claridad Visual:** Interfaz móvil intuitiva y agradable que refleja la elegancia de un salón de belleza según los wireframes oficiales.
- **Confianza del Cliente:** Confirmación inmediata con desglose transparente de costos, tiempo de servicio y código único de reserva.
- **Facilidad Operativa para Estilistas:** Visualización instantánea de la carga de trabajo diaria en una agenda tipo timeline fácil de interpretar.
