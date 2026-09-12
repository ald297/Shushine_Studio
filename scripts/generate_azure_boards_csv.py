import csv
import os

items = [
    # SPRINT 0 - EPICA 1
    {"type": "Epic", "title": "Épica 1: Arquitectura Base, Infraestructura y DevOps", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": 13, "hours": "", "priority": 1, "desc": "Establecer los cimientos desacoplados del sistema, entornos cloud y pipelines de Azure DevOps."},
    
    {"type": "User Story", "title": "US-1.01: Configuración de Base de Datos, Triggers y Seguridad en Supabase", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": 3, "hours": "", "priority": 1, "desc": "Desplegar el esquema relacional en PostgreSQL de Supabase con RLS, triggers y buckets de almacenamiento.", "criteria": "Dado el script DDL, cuando se ejecute, se crean las 12 tablas. Dado anon_key, RLS deniega acceso directo. Dado registro en auth.users, trigger crea usuarios y clientes. Buckets publicos creados."},
    {"type": "Task", "title": "TSK-1.01.1: Ejecutar script DDL en SQL Editor de Supabase y validar tablas", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Ejecutar DDL completo en Supabase y verificar claves foráneas e índices."},
    {"type": "Task", "title": "TSK-1.01.2: Implementar trigger on_auth_user_created para sincronización de usuarios", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Crear función handle_new_user() y trigger para poblar public.usuarios y public.clientes."},
    {"type": "Task", "title": "TSK-1.01.3: Configurar políticas Row Level Security (RLS) en todas las tablas", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 1, "desc": "Habilitar RLS en esquema public sin permisos anónimos para proteger la base de datos."},
    {"type": "Task", "title": "TSK-1.01.4: Crear buckets en Supabase Storage con políticas de lectura pública", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 1, "desc": "Crear buckets servicios-imagenes y estilistas-avatares con acceso público de lectura."},

    {"type": "User Story", "title": "US-1.02: Inicialización de la Web API en C# ASP.NET Core", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": 5, "hours": "", "priority": 1, "desc": "Crear la solución en .NET 8/9 con Clean Architecture y soporte para Npgsql, Swagger y RFC 7807.", "criteria": "Conexion Npgsql exitosa. Middleware RFC 7807 activo. Validacion JWT Supabase operativa. Swagger OpenAPI disponible."},
    {"type": "Task", "title": "TSK-1.02.1: Scaffolding de solución .NET 8/9 con Clean Architecture", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 4, "priority": 1, "desc": "Crear proyectos Api, Application, Domain, Infrastructure y referencias."},
    {"type": "Task", "title": "TSK-1.02.2: Configurar DbContext con Npgsql EF Core y conexión a Supabase", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 4, "priority": 1, "desc": "Instalar Npgsql.EntityFrameworkCore.PostgreSQL y mapear entidades."},
    {"type": "Task", "title": "TSK-1.02.3: Configurar autenticación JwtBearer en Program.cs", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Configurar JwtBearerOptions con URL de Supabase y JWT Secret."},
    {"type": "Task", "title": "TSK-1.02.4: Configurar middleware global de excepciones y formato RFC 7807", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Registrar AddProblemDetails() y ExceptionHandler global."},
    {"type": "Task", "title": "TSK-1.02.5: Configurar Swagger / OpenAPI con soporte para Bearer Token", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 1, "desc": "Habilitar interfaz interactiva Swagger para pruebas de endpoints protegidos."},

    {"type": "User Story", "title": "US-1.03: Inicialización de la App Móvil con Flutter y Clean Architecture", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": 3, "hours": "", "priority": 1, "desc": "Inicializar el proyecto Flutter configurando flutter_bloc, dio, get_it y temas visuales.", "criteria": "Tema oficial configurado. Dio con interceptor RFC 7807 e inyeccion de token. Inyector GetIt operativo."},
    {"type": "Task", "title": "TSK-1.03.1: Inicializar proyecto Flutter y estructura Clean Architecture", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Crear estructura core, features, data, domain, presentation."},
    {"type": "Task", "title": "TSK-1.03.2: Implementar AppTheme con paleta oficial de colores y fuentes", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": "", "hours": 3, "priority": 1, "desc": "Configurar colores Rosa, Blush, Lavanda y estilos de componentes."},
    {"type": "Task", "title": "TSK-1.03.3: Configurar cliente Dio con interceptor de autenticación y errores RFC 7807", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": "", "hours": 4, "priority": 1, "desc": "Crear ErrorInterceptor para deserializar ProblemDetails y manejar 401."},
    {"type": "Task", "title": "TSK-1.03.4: Configurar inyección de dependencias centralizada con GetIt", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 1, "desc": "Configurar injection_container.dart para DataSources y Repositorios."},

    {"type": "User Story", "title": "US-1.04: Configuración de Repositorio, GitFlow y Tableros en Azure DevOps", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": 2, "hours": "", "priority": 2, "desc": "Configurar el proyecto en Azure DevOps con ramas protegidas y políticas de PR.", "criteria": "Branch policies en main y develop. Sprints creados en Azure Boards."},
    {"type": "Task", "title": "TSK-1.04.1: Configurar políticas de rama en main y develop", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 2, "desc": "Bloquear push directo y exigir Pull Requests con revisión mínima."},
    {"type": "Task", "title": "TSK-1.04.2: Configurar Sprints 0 a 5 y columnas Kanban en Azure Boards", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 0", "points": "", "hours": 2, "priority": 2, "desc": "Crear iteraciones y configurar tableros para seguimiento de historias y tareas."},

    # SPRINT 1 - EPICA 2
    {"type": "Epic", "title": "Épica 2: Autenticación, Seguridad y Perfiles de Usuario", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 1", "points": 16, "hours": "", "priority": 1, "desc": "Permitir el registro, autenticación híbrida y gestión de ficha de belleza del cliente."},

    {"type": "User Story", "title": "US-2.01: Registro de Nuevos Clientes", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": 5, "hours": "", "priority": 1, "desc": "Crear cuenta de cliente ingresando nombre completo, correo, teléfono y contraseña.", "criteria": "Registro exitoso via Supabase Auth. Trigger crea registros en usuarios y clientes. Validación de correo duplicado."},
    {"type": "Task", "title": "TSK-2.01.1: Maquetar RegistroClienteView con validaciones de formulario", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 4, "priority": 1, "desc": "Crear interfaz declarativa de registro según Wireframe Pág. 5."},
    {"type": "Task", "title": "TSK-2.01.2: Implementar AuthBloc para registro con Supabase Auth", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 4, "priority": 1, "desc": "Conectar evento RegisterSubmitted con supabase.auth.signUp() y metadatos."},
    {"type": "Task", "title": "TSK-2.01.3: Validar en PostgreSQL inserción de usuario y perfil de cliente", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 1", "points": "", "hours": 3, "priority": 1, "desc": "Comprobar integridad referencial y asignación de rol Cliente por defecto."},
    {"type": "Task", "title": "TSK-2.01.4: Pruebas de QA de validación de campos y manejo de errores", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 2, "priority": 1, "desc": "Probar campos vacíos, correos duplicados y contraseñas débiles."},

    {"type": "User Story", "title": "US-2.02: Inicio de Sesión y Manejo Seguro de Sesión con JWT", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": 5, "hours": "", "priority": 1, "desc": "Iniciar sesión con credenciales, obtener JWT y mantener sesión activa de forma segura.", "criteria": "Login exitoso emite JWT con claims. Token persistido de forma segura. Refresco automático ante 401."},
    {"type": "Task", "title": "TSK-2.02.1: Maquetar LoginClienteView con toggles de visibilidad", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 3, "priority": 1, "desc": "Diseñar vista de login según Wireframe Pág. 6."},
    {"type": "Task", "title": "TSK-2.02.2: Implementar Custom Access Token Hook en Supabase", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 1", "points": "", "hours": 3, "priority": 1, "desc": "Inyectar claims role e internal_user_id en el JWT emitido por Supabase."},
    {"type": "Task", "title": "TSK-2.02.3: Implementar login en AuthBloc y persistencia segura de token", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 4, "priority": 1, "desc": "Guardar tokens en flutter_secure_storage y emitir estado Authenticated."},
    {"type": "Task", "title": "TSK-2.02.4: Implementar flujo de refresco automático ante 401 en Dio", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 3, "priority": 1, "desc": "Reintentar petición con refresh_token o redirigir a Login si expiró."},

    {"type": "User Story", "title": "US-2.03: Visualización y Edición del Perfil de Cliente y Ficha Estética", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": 6, "hours": "", "priority": 2, "desc": "Visualizar y editar datos personales, fidelidad y diagnóstico capilar del cliente.", "criteria": "GET /api/perfil devuelve ficha completa. PUT /api/perfil actualiza campos de contacto y preferencias."},
    {"type": "Task", "title": "TSK-2.03.1: Implementar endpoints GET y PUT /api/perfil en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 1", "points": "", "hours": 4, "priority": 2, "desc": "Crear PerfilController y lógica de actualización en Clientes."},
    {"type": "Task", "title": "TSK-2.03.2: Maquetar PerfilClienteView según Wireframe Pág. 14", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 4, "priority": 2, "desc": "Diseñar avatar, badge Nivel Oro y formulario de ficha capilar."},
    {"type": "Task", "title": "TSK-2.03.3: Implementar PerfilBloc conectando con la API", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 3, "priority": 2, "desc": "Gestionar estados de carga y guardado de perfil."},
    {"type": "Task", "title": "TSK-2.03.4: Pruebas de QA de persistencia de perfil y ficha estética", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 1", "points": "", "hours": 2, "priority": 2, "desc": "Verificar persistencia de cambios en base de datos."},

    # SPRINT 2 - EPICA 3
    {"type": "Epic", "title": "Épica 3: Catálogo de Servicios y Motor de Disponibilidad", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": 21, "hours": "", "priority": 1, "desc": "Explorar la oferta comercial del salón y calcular en tiempo real los bloques de tiempo libres por estilista."},

    {"type": "User Story", "title": "US-3.01: Catálogo de Servicios Categorizados y Filtros Rápidos", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": 5, "hours": "", "priority": 1, "desc": "Explorar catálogo clasificado por categorías (Cabello, Uñas, Maquillaje, Spa).", "criteria": "GET /api/servicios devuelve solo activos. Filtrado instantáneo por chip de categoría en app móvil."},
    {"type": "Task", "title": "TSK-3.01.1: Implementar GET /api/servicios con filtro por categoría en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 4, "priority": 1, "desc": "Crear ServiciosController con consultas optimizadas."},
    {"type": "Task", "title": "TSK-3.01.2: Maquetar CatalogoServiciosView según Wireframe Pág. 7", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 4, "priority": 1, "desc": "Diseñar chips de categorías y tarjetas de servicios."},
    {"type": "Task", "title": "TSK-3.01.3: Implementar CatalogoBloc con filtrado reactivo", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 3, "priority": 1, "desc": "Controlar estados Loading, Loaded y Filtered."},
    {"type": "Task", "title": "TSK-3.01.4: Pruebas de QA de carga de catálogo y renderizado de imágenes", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 2, "priority": 1, "desc": "Verificar carga rápida de imágenes desde Supabase Storage."},

    {"type": "User Story", "title": "US-3.02: Ficha Técnica y Detalle del Tratamiento", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": 3, "hours": "", "priority": 2, "desc": "Visualizar descripción detallada, tiempo estimado y protocolo paso a paso del servicio.", "criteria": "GET /api/servicios/{id} devuelve protocolo completo. Navegación fluida hacia reserva."},
    {"type": "Task", "title": "TSK-3.02.1: Implementar GET /api/servicios/{id} detallado en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 2, "priority": 2, "desc": "Retornar descripción completa, duración y protocolo."},
    {"type": "Task", "title": "TSK-3.02.2: Maquetar DetalleServicioView según Wireframe Pág. 8", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 3, "priority": 2, "desc": "Diseñar cabecera con foto, badge de precio y pasos de protocolo."},
    {"type": "Task", "title": "TSK-3.02.3: Conectar botón Continuar transmitiendo ID de servicio", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 2, "priority": 2, "desc": "Almacenar servicio seleccionado en ReservaBloc."},

    {"type": "User Story", "title": "US-3.03: Selección de Estilistas y Asignación Automática", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": 5, "hours": "", "priority": 1, "desc": "Ver estilistas capacitados para el servicio o elegir opción de asignación automática más rápida.", "criteria": "GET /api/estilistas filtra por habilidad. Opción 'Cualquiera disponible' pasa null a la API."},
    {"type": "Task", "title": "TSK-3.03.1: Implementar GET /api/estilistas?servicioId={id} en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 4, "priority": 1, "desc": "Filtrar estilistas activos vinculados en estilista_servicios."},
    {"type": "Task", "title": "TSK-3.03.2: Maquetar SeleccionEstilistaView según Wireframe Pág. 9", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 4, "priority": 1, "desc": "Diseñar tarjetas de estilistas y card destacada de autoasignación."},
    {"type": "Task", "title": "TSK-3.03.3: Manejar estado de selección en ReservaBloc", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 3, "priority": 1, "desc": "Manejar selección individual o asignación automática."},

    {"type": "User Story", "title": "US-3.04: Motor de Cálculo de Disponibilidad Dinámica de Horarios", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": 8, "hours": "", "priority": 1, "desc": "Calcular en tiempo real slots libres considerando turnos, citas existentes y duración del servicio.", "criteria": "GET /api/disponibilidad cruza jornada laboral y reservas. Bloques ocupados deshabilitados en UI."},
    {"type": "Task", "title": "TSK-3.04.1: Desarrollar AvailabilityService con algoritmo de slots en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 8, "priority": 1, "desc": "Cruzar horarios_estilista con reservas existentes y segmentar intervalos."},
    {"type": "Task", "title": "TSK-3.04.2: Implementar endpoint GET /api/disponibilidad en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 4, "priority": 1, "desc": "Exponer endpoint REST con validación de parámetros de fecha y servicio."},
    {"type": "Task", "title": "TSK-3.04.3: Maquetar SeleccionHorarioView según Wireframe Pág. 10", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 5, "priority": 1, "desc": "Diseñar calendario mensual interactivo y grilla de horarios."},
    {"type": "Task", "title": "TSK-3.04.4: Conectar DisponibilidadBloc con la grilla de slots", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 2", "points": "", "hours": 3, "priority": 1, "desc": "Pintar bloques libres en verde/rosa y deshabilitar ocupados."},
    {"type": "Task", "title": "TSK-3.04.5: Pruebas integradas de casos límite de disponibilidad", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 2", "points": "", "hours": 3, "priority": 1, "desc": "Probar solapamientos, límites de jornada y múltiples reservas consecutivas."},

    # SPRINT 3 - EPICA 4
    {"type": "Epic", "title": "Épica 4: Motor Transaccional de Reservas y Gestión Personal", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": 24, "hours": "", "priority": 1, "desc": "Transaccionar citas con control estricto de concurrencia (ACID), confirmación y gestión del historial de citas."},

    {"type": "User Story", "title": "US-4.01: Resumen y Creación de Reserva con Control de Concurrencia", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": 8, "hours": "", "priority": 1, "desc": "Confirmar reserva con transacción ACID garantizando prevención de doble reserva (Overbooking).", "criteria": "POST /api/reservas transaccional. Asignación de estación de trabajo. Retorno 409 Conflict si el slot se ocupó."},
    {"type": "Task", "title": "TSK-4.01.1: Implementar CreateAppointmentUseCase con transacción ACID en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": "", "hours": 6, "priority": 1, "desc": "Bloqueo pesimista/serializable en PostgreSQL para prevenir concurrencia."},
    {"type": "Task", "title": "TSK-4.01.2: Implementar asignación de estación de trabajo libre en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 1, "desc": "Asignar cabina/estación disponible según categoría del servicio."},
    {"type": "Task", "title": "TSK-4.01.3: Maquetar ResumenReservaView según Wireframe Pág. 11", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 4, "priority": 1, "desc": "Diseñar desglose de servicio, estilista, fecha, hora, subtotal e impuestos."},
    {"type": "Task", "title": "TSK-4.01.4: Conectar botón Confirmar con POST /api/reservas en Flutter", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 4, "priority": 1, "desc": "Manejar respuesta 201 Created y capturar 409 Conflict mostrando alerta amigable."},

    {"type": "User Story", "title": "US-4.02: Comprobante y Confirmación de Cita con Código Único", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": 3, "hours": "", "priority": 1, "desc": "Visualizar comprobante de confirmación con código alfanumérico único de reserva.", "criteria": "Generación de código único (#SHU-XXXX). Pantalla visual de comprobante y navegación."},
    {"type": "Task", "title": "TSK-4.02.1: Implementar generador de código de reserva único en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": "", "hours": 2, "priority": 1, "desc": "Generar código no repetible con prefijo de marca #SHU-XXXX."},
    {"type": "Task", "title": "TSK-4.02.2: Maquetar ConfirmacionReservaView según Wireframe Pág. 12", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 1, "desc": "Diseñar ticket de comprobante con código, dirección y avisos."},
    {"type": "Task", "title": "TSK-4.02.3: Configurar navegación hacia Mis Citas o Catálogo", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 2, "priority": 1, "desc": "Vincular botones de acción hacia pestañas correspondientes."},

    {"type": "User Story", "title": "US-4.03: Historial de Citas del Cliente (Próximas y Pasadas)", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": 5, "hours": "", "priority": 1, "desc": "Consultar citas agendadas futuras y citas completadas previamente.", "criteria": "GET /api/reservas/cliente/{id} validado por claims de usuario. Pestañas Próximas y Pasadas en UI."},
    {"type": "Task", "title": "TSK-4.03.1: Implementar GET /api/reservas/cliente/{id} con filtro de seguridad", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": "", "hours": 4, "priority": 1, "desc": "Validar que el id coincida con el claim internal_user_id (RNF04)."},
    {"type": "Task", "title": "TSK-4.03.2: Maquetar HistorialCitasView según Wireframe Pág. 13", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 4, "priority": 1, "desc": "Diseñar pestañas Citas Futuras y Citas Pasadas."},
    {"type": "Task", "title": "TSK-4.03.3: Conectar HistorialBloc y renderizado de tarjetas con estado", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 1, "desc": "Renderizar badges cromáticos (Verde: Confirmada, Amarillo: Pendiente)."},

    {"type": "User Story", "title": "US-4.04: Cancelación y Solicitud de Reprogramación de Citas", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": 5, "hours": "", "priority": 2, "desc": "Cancelar cita pendiente con anticipación o iniciar flujo de cambio de horario.", "criteria": "PUT /api/reservas/{id}/cancelar valida límite de 2h. Liberación inmediata de slot."},
    {"type": "Task", "title": "TSK-4.04.1: Implementar PUT /api/reservas/{id}/cancelar en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 2, "desc": "Validar límite de tiempo previo de cancelación y cambiar estado."},
    {"type": "Task", "title": "TSK-4.04.2: Implementar modal de confirmación de cancelación en Flutter", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 2, "desc": "Diseñar diálogo con advertencia de políticas del salón."},
    {"type": "Task", "title": "TSK-4.04.3: Conectar flujo de reprogramación hacia selector de horario", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 3, "priority": 2, "desc": "Reutilizar SeleccionHorarioView pasando la reserva a modificar."},
    {"type": "Task", "title": "TSK-4.04.4: Pruebas de QA de cancelación y liberación de disponibilidad", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 3", "points": "", "hours": 2, "priority": 2, "desc": "Verificar que el slot cancelado vuelve a aparecer libre para otros usuarios."},

    # SPRINT 4 - EPICA 5
    {"type": "Epic", "title": "Épica 5: Panel de Control Administrativo y Gestión del Salón", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": 29, "hours": "", "priority": 1, "desc": "Proveer herramientas exclusivas para recepción y gerencia (dashboard de KPIs, agenda timeline, walk-ins y mantenimiento)."},

    {"type": "User Story", "title": "US-5.01: Dashboard Operativo Diario y Métricas Clave", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 5, "hours": "", "priority": 1, "desc": "Visualizar panel con contadores del día, ocupación de estaciones e ingresos proyectados.", "criteria": "GET /api/admin/dashboard protegido por rol Administrador. Métricas de ingresos y estaciones."},
    {"type": "Task", "title": "TSK-5.01.1: Implementar consultas de métricas en AdminDashboardController en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 5, "priority": 1, "desc": "Calcular citas del día, % capacidad, ingresos proyectados y ocupación."},
    {"type": "Task", "title": "TSK-5.01.2: Proteger endpoints administrativos con [Authorize(Roles = 'Administrador')]", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 2, "priority": 1, "desc": "Verificar rechazo 403 Forbidden para usuarios con rol Cliente."},
    {"type": "Task", "title": "TSK-5.01.3: Maquetar DashboardAdminView según Wireframe Pág. 15", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 5, "priority": 1, "desc": "Diseñar tarjetas de KPIs, gráficos de ocupación y monitores de cabinas."},
    {"type": "Task", "title": "TSK-5.01.4: Conectar AdminDashboardBloc con la API", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 1, "desc": "Gestionar carga y refresco automático de métricas diarias."},

    {"type": "User Story", "title": "US-5.02: Agenda Diaria Interactiva en Formato Timeline Multi-Estilista", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 8, "hours": "", "priority": 1, "desc": "Visualizar agenda tipo Timeline agrupada por estilista para gestionar la jornada.", "criteria": "GET /api/admin/agenda devuelve citas estructuradas por estilista. Bloques libres interactivos."},
    {"type": "Task", "title": "TSK-5.02.1: Implementar endpoint GET /api/admin/agenda en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 6, "priority": 1, "desc": "Estructurar respuesta agrupando cronograma de citas y descansos por empleado."},
    {"type": "Task", "title": "TSK-5.02.2: Maquetar vista Timeline multi-columna según Wireframe Pág. 16", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 6, "priority": 1, "desc": "Diseñar grilla interactiva con bloques de citas proporcionales al tiempo."},
    {"type": "Task", "title": "TSK-5.02.3: Conectar selector de fecha y refresco dinámico", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 1, "desc": "Permitir navegar entre fechas para consultar agendas pasadas y futuras."},

    {"type": "User Story", "title": "US-5.03: Registro Rápido de Clientes Presenciales (Walk-in Clients)", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 5, "hours": "", "priority": 1, "desc": "Registrar citas espontáneas para clientes sin cuenta ocupando espacios libres de la agenda.", "criteria": "POST /api/reservas/walk-in registra es_walk_in = true. Refresco automático de Timeline."},
    {"type": "Task", "title": "TSK-5.03.1: Implementar endpoint POST /api/reservas/walk-in en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 4, "priority": 1, "desc": "Permitir id_cliente nulo y guardar nombre_walk_in y telefono_walk_in."},
    {"type": "Task", "title": "TSK-5.03.2: Maquetar formulario modal rápido de Walk-in en Flutter", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 4, "priority": 1, "desc": "Invocar modal al presionar bloque libre '+ Walk-in Client' en el Timeline."},
    {"type": "Task", "title": "TSK-5.03.3: Refresco inmediato de la agenda tras registro", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 1, "desc": "Ocupar visualmente el slot sin recargar toda la pantalla."},

    {"type": "User Story", "title": "US-5.04: Gestión del Ciclo de Vida y Auditoría de Estados de Citas", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 4, "hours": "", "priority": 1, "desc": "Actualizar estados de citas (Pendiente, Completada, Cancelada, No Asistió) con registro de auditoría.", "criteria": "PUT /api/admin/reservas/{id}/estado actualiza y guarda en historial_estado_reserva."},
    {"type": "Task", "title": "TSK-5.04.1: Implementar actualización y auditoría en historial_estado_reserva en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 1, "desc": "Guardar id_usuario_cambio, estado_anterior y estado_nuevo en transacción."},
    {"type": "Task", "title": "TSK-5.04.2: Maquetar EstadoReservaView según Wireframe Pág. 17", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 1, "desc": "Diseñar selector de estado y campo de texto para observaciones de auditoría."},
    {"type": "Task", "title": "TSK-5.04.3: Pruebas de QA de trazabilidad de cambios de estado", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 2, "priority": 1, "desc": "Verificar registro de auditoría en la base de datos tras cada cambio."},

    {"type": "User Story", "title": "US-5.05: Mantenimiento de Catálogo de Servicios y Precios", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 4, "hours": "", "priority": 2, "desc": "Modificar precios, duración y activar/desactivar servicios del salón.", "criteria": "POST/PUT /api/servicios/{id} operativo. Reflejo inmediato en la app pública."},
    {"type": "Task", "title": "TSK-5.05.1: Implementar endpoints de mantenimiento de catálogo en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 2, "desc": "Crear endpoints de creación, edición y toggle de activación de servicios."},
    {"type": "Task", "title": "TSK-5.05.2: Maquetar AdministracionCatalogoView según Wireframe Pág. 18", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 2, "desc": "Diseñar switches on/off y modal de edición de tarifas."},
    {"type": "Task", "title": "TSK-5.05.3: Pruebas de QA de reactividad en catálogo público", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 2, "priority": 2, "desc": "Comprobar que un servicio desactivado desaparece del catálogo del cliente."},

    {"type": "User Story", "title": "US-5.06: Control de Disponibilidad y Turnos de Estilistas", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": 3, "hours": "", "priority": 2, "desc": "Marcar estilistas como inactivos temporales por imprevistos para bloquear su agenda.", "criteria": "PUT /api/estilistas/{id}/estado actualiza disponibilidad. Exclusión automática del motor de cálculo."},
    {"type": "Task", "title": "TSK-5.06.1: Implementar endpoint PUT /api/estilistas/{id}/estado en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 2, "priority": 2, "desc": "Actualizar campo estado_disponibilidad en estilistas."},
    {"type": "Task", "title": "TSK-5.06.2: Maquetar EstadoEstilistaView según Wireframe Pág. 19", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 4", "points": "", "hours": 3, "priority": 2, "desc": "Diseñar lista de estilistas con toggles de disponibilidad."},
    {"type": "Task", "title": "TSK-5.06.3: Verificar bloqueo automático en motor de disponibilidad", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 4", "points": "", "hours": 2, "priority": 2, "desc": "Comprobar que el estilista inactivo no genera slots libres en fechas futuras."},

    # SPRINT 5 - EPICA 6
    {"type": "Epic", "title": "Épica 6: Pruebas Integrales, Rendimiento y Despliegue", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": 16, "hours": "", "priority": 1, "desc": "Asegurar la calidad técnica (QA), pruebas de carga concurrentes y despliegue para la evaluación académica."},

    {"type": "User Story", "title": "US-6.01: Pruebas Unitarias y de Integración Automatizadas", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 5", "points": 5, "hours": "", "priority": 1, "desc": "Desarrollar pruebas automatizadas con xUnit en backend y bloc_test en frontend.", "criteria": "Pruebas de algoritmo de disponibilidad en C#. Pruebas de estados de BLoC en Flutter."},
    {"type": "Task", "title": "TSK-6.01.1: Implementar pruebas unitarias con xUnit en C#", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": "", "hours": 5, "priority": 1, "desc": "Probar AvailabilityService, validaciones de entrada y manejo de errores."},
    {"type": "Task", "title": "TSK-6.01.2: Implementar pruebas de BLoC con bloc_test en Flutter", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 5", "points": "", "hours": 5, "priority": 1, "desc": "Probar transiciones de estado en AuthBloc, CatalogoBloc y ReservaBloc."},

    {"type": "User Story", "title": "US-6.02: Pruebas de Estrés y Concurrencia Transaccional (RNF02)", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": 5, "hours": "", "priority": 1, "desc": "Simular 50 peticiones simultáneas sobre el mismo slot para verificar bloqueo de doble reserva.", "criteria": "Exactamente 1 petición retorna 201 Created y 49 peticiones retornan 409 Conflict."},
    {"type": "Task", "title": "TSK-6.02.1: Crear script de prueba de carga con k6 simulando concurrencia", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": "", "hours": 4, "priority": 1, "desc": "Configurar 50 peticiones concurrentes en el mismo milisegundo para la misma cita."},
    {"type": "Task", "title": "TSK-6.02.2: Ejecutar prueba de concurrencia y documentar evidencias", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 5", "points": "", "hours": 4, "priority": 1, "desc": "Comprobar 1 éxito (201) y 49 rechazos controlados (409 Conflict RFC 7807)."},

    {"type": "User Story", "title": "US-6.03: Despliegue en la Nube y Generación de Entregables", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": 6, "hours": "", "priority": 1, "desc": "Publicar Web API en Azure App Service y compilar APK release de Flutter para entrega académica.", "criteria": "API accesible bajo HTTPS con Swagger activo. APK Flutter Release compilado y conectado."},
    {"type": "Task", "title": "TSK-6.03.1: Desplegar Web API C# en Azure App Service", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": "", "hours": 4, "priority": 1, "desc": "Configurar App Service, Application Insights y variables de entorno de producción."},
    {"type": "Task", "title": "TSK-6.03.2: Compilar APK Release de Flutter apuntando a Azure", "assigned": "Camila Antonia Calderon Cortez", "sprint": "Sprint 5", "points": "", "hours": 4, "priority": 1, "desc": "Compilar artefacto final con flutter build apk --release --dart-define-from-file."},
    {"type": "Task", "title": "TSK-6.03.3: Preparar reporte final y entorno de demostración académica", "assigned": "Alex Fernando Alfaro Diaz", "sprint": "Sprint 5", "points": "", "hours": 3, "priority": 1, "desc": "Documentar métricas, evidencias y preparar presentación para docentes de ESFE AGAPE."}
]

csv_file = "/home/alex/Desktop/Shushine_Studio/azure_boards_import.csv"

headers = [
    "ID",
    "Work Item Type",
    "Title",
    "Assigned To",
    "State",
    "Story Points",
    "Original Estimate",
    "Priority",
    "Iteration Path",
    "Area Path",
    "Description",
    "Acceptance Criteria"
]

with open(csv_file, mode="w", newline="", encoding="utf-8") as f:
    writer = csv.writer(f)
    writer.writerow(headers)
    
    for item in items:
        writer.writerow([
            "",  # ID (vacío para que Azure DevOps lo genere)
            item["type"],
            item["title"],
            item["assigned"],
            "New" if item["type"] != "Task" else "To Do",
            item.get("points", ""),
            item.get("hours", ""),
            item.get("priority", 2),
            f"Shunshine Studio\\{item['sprint']}",
            "Shunshine Studio",
            item.get("desc", ""),
            item.get("criteria", "")
        ])

print(f"Generado exitosamente: {csv_file} con {len(items)} elementos de trabajo.")
