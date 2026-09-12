#!/usr/bin/env python3
"""
Script para sincronizar y subir todo el Product Backlog a Azure DevOps Boards
con jerarquía completa: Épicas -> Product Backlog Items -> Tasks.
"""

import sys
import os
import json
import base64
import urllib.request
import urllib.parse
import urllib.error

ORGANIZATION = "cc25003"
PROJECT_NAME = "Shunshine Studio"
PAT = "7ssaGsowBIx9Dmcp50xmZdMgfIto09s0D1PBiW1w7BIKjygSh1O8JQQJ99CIACAAAAAAAAAAAAASAZDO41gW"

ALEX = "Alex alfaro <lalafaro6@gmail.com>"
CAMILA = "cami calderon <cc25003@esfe.agape.edu.sv>"

AUTH_HEADER = "Basic " + base64.b64encode(f":{PAT}".encode("utf-8")).decode("utf-8")
ENCODED_PROJECT = urllib.parse.quote(PROJECT_NAME)

def post_work_item(item_type, patch_doc):
    encoded_type = urllib.parse.quote(f"${item_type}")
    url = f"https://dev.azure.com/{ORGANIZATION}/{ENCODED_PROJECT}/_apis/wit/workitems/{encoded_type}?api-version=7.0"
    data = json.dumps(patch_doc).encode("utf-8")
    req = urllib.request.Request(url, data=data, headers={
        "Content-Type": "application/json-patch+json",
        "Authorization": AUTH_HEADER
    }, method="POST")
    
    try:
        with urllib.request.urlopen(req) as resp:
            return True, json.loads(resp.read().decode("utf-8"))["id"], None
    except urllib.error.HTTPError as e:
        return False, None, e.read().decode("utf-8")
    except Exception as e:
        return False, None, str(e)

# Estructura de datos completa y jerárquica
backlog_tree = [
    # ==========================================
    # SPRINT 1 - ÉPICA 1
    # ==========================================
    {
        "title": "Épica 1: Arquitectura Base, Infraestructura y DevOps",
        "sprint": "Sprint 1",
        "assigned": ALEX,
        "desc": "Establecer los cimientos desacoplados del sistema, entornos cloud y pipelines de Azure DevOps.",
        "pbis": [
            {
                "title": "US-1.01: Configuración de Base de Datos, Triggers y Seguridad en Supabase",
                "assigned": ALEX,
                "points": 3,
                "desc": "Desplegar el esquema relacional en PostgreSQL de Supabase con RLS, triggers y buckets de almacenamiento.",
                "criteria": "Dado el script DDL de Supabase, cuando se ejecute, se crean las 12 tablas. Dado anon_key, RLS deniega acceso directo. Dado registro en auth.users, trigger crea usuarios y clientes. Buckets publicos creados.",
                "tasks": [
                    {"title": "TSK-1.01.1: Ejecutar script DDL en SQL Editor de Supabase y validar tablas", "assigned": ALEX, "hours": 3, "desc": "Ejecutar DDL completo en Supabase y verificar claves foráneas e índices."},
                    {"title": "TSK-1.01.2: Implementar trigger on_auth_user_created para sincronización de usuarios", "assigned": ALEX, "hours": 3, "desc": "Crear función handle_new_user() y trigger para poblar public.usuarios y public.clientes."},
                    {"title": "TSK-1.01.3: Configurar políticas Row Level Security (RLS) en todas las tablas", "assigned": ALEX, "hours": 2, "desc": "Habilitar RLS en esquema public sin permisos anónimos para proteger la base de datos."},
                    {"title": "TSK-1.01.4: Crear buckets en Supabase Storage con políticas de lectura pública", "assigned": CAMILA, "hours": 2, "desc": "Crear buckets servicios-imagenes y estilistas-avatares con acceso público de lectura."}
                ]
            },
            {
                "title": "US-1.02: Inicialización de la Web API en C# ASP.NET Core",
                "assigned": ALEX,
                "points": 5,
                "desc": "Crear la solución en .NET 8/9 con Clean Architecture y soporte para Npgsql, Swagger y RFC 7807.",
                "criteria": "Conexion Npgsql exitosa. Middleware RFC 7807 activo. Validacion JWT Supabase operativa. Swagger OpenAPI disponible.",
                "tasks": [
                    {"title": "TSK-1.02.1: Scaffolding de solución .NET 8/9 con Clean Architecture", "assigned": ALEX, "hours": 4, "desc": "Crear proyectos Api, Application, Domain, Infrastructure y referencias."},
                    {"title": "TSK-1.02.2: Configurar DbContext con Npgsql EF Core y conexión a Supabase", "assigned": ALEX, "hours": 4, "desc": "Instalar Npgsql.EntityFrameworkCore.PostgreSQL y mapear entidades."},
                    {"title": "TSK-1.02.3: Configurar autenticación JwtBearer en Program.cs", "assigned": ALEX, "hours": 3, "desc": "Configurar JwtBearerOptions con URL de Supabase y JWT Secret."},
                    {"title": "TSK-1.02.4: Configurar middleware global de excepciones y formato RFC 7807", "assigned": ALEX, "hours": 3, "desc": "Registrar AddProblemDetails() y ExceptionHandler global."},
                    {"title": "TSK-1.02.5: Configurar Swagger / OpenAPI con soporte para Bearer Token", "assigned": ALEX, "hours": 2, "desc": "Habilitar interfaz interactiva Swagger para pruebas de endpoints protegidos."}
                ]
            },
            {
                "title": "US-1.03: Inicialización de la App Móvil con Flutter y Clean Architecture",
                "assigned": CAMILA,
                "points": 3,
                "desc": "Inicializar el proyecto Flutter configurando flutter_bloc, dio, get_it y temas visuales.",
                "criteria": "Tema oficial configurado. Dio con interceptor RFC 7807 e inyeccion de token. Inyector GetIt operativo.",
                "tasks": [
                    {"title": "TSK-1.03.1: Inicializar proyecto Flutter y estructura Clean Architecture", "assigned": CAMILA, "hours": 3, "desc": "Crear estructura core, features, data, domain, presentation."},
                    {"title": "TSK-1.03.2: Implementar AppTheme con paleta oficial de colores y fuentes", "assigned": CAMILA, "hours": 3, "desc": "Configurar colores Rosa, Blush, Lavanda y estilos de componentes."},
                    {"title": "TSK-1.03.3: Configurar cliente Dio con interceptor de autenticación y errores RFC 7807", "assigned": CAMILA, "hours": 4, "desc": "Crear ErrorInterceptor para deserializar ProblemDetails y manejar 401."},
                    {"title": "TSK-1.03.4: Configurar inyección de dependencias centralizada con GetIt", "assigned": CAMILA, "hours": 2, "desc": "Configurar injection_container.dart para DataSources y Repositorios."}
                ]
            },
            {
                "title": "US-1.04: Configuración de Repositorio, GitFlow y Tableros en Azure DevOps",
                "assigned": ALEX,
                "points": 2,
                "desc": "Configurar el proyecto en Azure DevOps con ramas protegidas y políticas de PR.",
                "criteria": "Branch policies en main y develop. Sprints creados en Azure Boards.",
                "tasks": [
                    {"title": "TSK-1.04.1: Configurar políticas de rama en main y develop", "assigned": ALEX, "hours": 2, "desc": "Bloquear push directo y exigir Pull Requests con revisión mínima."},
                    {"title": "TSK-1.04.2: Configurar Sprints 1 a 6 y columnas Kanban en Azure Boards", "assigned": ALEX, "hours": 2, "desc": "Crear iteraciones y configurar tableros para seguimiento de historias y tareas."}
                ]
            }
        ]
    },

    # ==========================================
    # SPRINT 2 - ÉPICA 2
    # ==========================================
    {
        "title": "Épica 2: Autenticación, Seguridad y Perfiles de Usuario",
        "sprint": "Sprint 2",
        "assigned": ALEX,
        "desc": "Permitir el registro, autenticación híbrida y gestión de ficha de belleza del cliente.",
        "pbis": [
            {
                "title": "US-2.01: Registro de Nuevos Clientes",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Crear cuenta de cliente ingresando nombre completo, correo, teléfono y contraseña.",
                "criteria": "Registro exitoso via Supabase Auth. Trigger crea registros en usuarios y clientes. Validación de correo duplicado.",
                "tasks": [
                    {"title": "TSK-2.01.1: Maquetar RegistroClienteView con validaciones de formulario", "assigned": CAMILA, "hours": 4, "desc": "Crear interfaz declarativa de registro según Wireframe Pág. 5."},
                    {"title": "TSK-2.01.2: Implementar AuthBloc para registro con Supabase Auth", "assigned": CAMILA, "hours": 4, "desc": "Conectar evento RegisterSubmitted con supabase.auth.signUp() y metadatos."},
                    {"title": "TSK-2.01.3: Validar en PostgreSQL inserción de usuario y perfil de cliente", "assigned": ALEX, "hours": 3, "desc": "Comprobar integridad referencial y asignación de rol Cliente por defecto."},
                    {"title": "TSK-2.01.4: Pruebas de QA de validación de campos y manejo de errores", "assigned": CAMILA, "hours": 2, "desc": "Probar campos vacíos, correos duplicados y contraseñas débiles."}
                ]
            },
            {
                "title": "US-2.02: Inicio de Sesión y Manejo Seguro de Sesión con JWT",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Iniciar sesión con credenciales, obtener JWT y mantener sesión activa de forma segura.",
                "criteria": "Login exitoso emite JWT con claims. Token persistido de forma segura. Refresco automático ante 401.",
                "tasks": [
                    {"title": "TSK-2.02.1: Maquetar LoginClienteView con toggles de visibilidad", "assigned": CAMILA, "hours": 3, "desc": "Diseñar vista de login según Wireframe Pág. 6."},
                    {"title": "TSK-2.02.2: Implementar Custom Access Token Hook en Supabase", "assigned": ALEX, "hours": 3, "desc": "Inyectar claims role e internal_user_id en el JWT emitido por Supabase."},
                    {"title": "TSK-2.02.3: Implementar login en AuthBloc y persistencia segura de token", "assigned": CAMILA, "hours": 4, "desc": "Guardar tokens en flutter_secure_storage y emitir estado Authenticated."},
                    {"title": "TSK-2.02.4: Implementar flujo de refresco automático ante 401 en Dio", "assigned": CAMILA, "hours": 3, "desc": "Reintentar petición con refresh_token o redirigir a Login si expiró."}
                ]
            },
            {
                "title": "US-2.03: Visualización y Edición del Perfil de Cliente y Ficha Estética",
                "assigned": CAMILA,
                "points": 6,
                "desc": "Visualizar y editar datos personales, fidelidad y diagnóstico capilar del cliente.",
                "criteria": "GET /api/perfil devuelve ficha completa. PUT /api/perfil actualiza campos de contacto y preferencias.",
                "tasks": [
                    {"title": "TSK-2.03.1: Implementar endpoints GET y PUT /api/perfil en C#", "assigned": ALEX, "hours": 4, "desc": "Crear PerfilController y lógica de actualización en Clientes."},
                    {"title": "TSK-2.03.2: Maquetar PerfilClienteView según Wireframe Pág. 14", "assigned": CAMILA, "hours": 4, "desc": "Diseñar avatar, badge Nivel Oro y formulario de ficha capilar."},
                    {"title": "TSK-2.03.3: Implementar PerfilBloc conectando con la API", "assigned": CAMILA, "hours": 3, "desc": "Gestionar estados de carga y guardado de perfil."},
                    {"title": "TSK-2.03.4: Pruebas de QA de persistencia de perfil y ficha estética", "assigned": CAMILA, "hours": 2, "desc": "Verificar persistencia de cambios en base de datos."}
                ]
            }
        ]
    },

    # ==========================================
    # SPRINT 3 - ÉPICA 3
    # ==========================================
    {
        "title": "Épica 3: Catálogo de Servicios y Motor de Disponibilidad",
        "sprint": "Sprint 3",
        "assigned": ALEX,
        "desc": "Explorar la oferta comercial del salón y calcular en tiempo real los bloques de tiempo libres por estilista.",
        "pbis": [
            {
                "title": "US-3.01: Catálogo de Servicios Categorizados y Filtros Rápidos",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Explorar catálogo clasificado por categorías (Cabello, Uñas, Maquillaje, Spa).",
                "criteria": "GET /api/servicios devuelve solo activos. Filtrado instantáneo por chip de categoría en app móvil.",
                "tasks": [
                    {"title": "TSK-3.01.1: Implementar GET /api/servicios con filtro por categoría en C#", "assigned": ALEX, "hours": 4, "desc": "Crear ServiciosController con consultas optimizadas."},
                    {"title": "TSK-3.01.2: Maquetar CatalogoServiciosView según Wireframe Pág. 7", "assigned": CAMILA, "hours": 4, "desc": "Diseñar chips de categorías y tarjetas de servicios."},
                    {"title": "TSK-3.01.3: Implementar CatalogoBloc con filtrado reactivo", "assigned": CAMILA, "hours": 3, "desc": "Controlar estados Loading, Loaded y Filtered."},
                    {"title": "TSK-3.01.4: Pruebas de QA de carga de catálogo y renderizado de imágenes", "assigned": CAMILA, "hours": 2, "desc": "Verificar carga rápida de imágenes desde Supabase Storage."}
                ]
            },
            {
                "title": "US-3.02: Ficha Técnica y Detalle del Tratamiento",
                "assigned": CAMILA,
                "points": 3,
                "desc": "Visualizar descripción detallada, tiempo estimado y protocolo paso a paso del servicio.",
                "criteria": "GET /api/servicios/{id} devuelve protocolo completo. Navegación fluida hacia reserva.",
                "tasks": [
                    {"title": "TSK-3.02.1: Implementar GET /api/servicios/{id} detallado en C#", "assigned": ALEX, "hours": 2, "desc": "Retornar descripción completa, duración y protocolo."},
                    {"title": "TSK-3.02.2: Maquetar DetalleServicioView según Wireframe Pág. 8", "assigned": CAMILA, "hours": 3, "desc": "Diseñar cabecera con foto, badge de precio y pasos de protocolo."},
                    {"title": "TSK-3.02.3: Conectar botón Continuar transmitiendo ID de servicio", "assigned": CAMILA, "hours": 2, "desc": "Almacenar servicio seleccionado en ReservaBloc."}
                ]
            },
            {
                "title": "US-3.03: Selección de Estilistas y Asignación Automática",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Ver estilistas capacitados para el servicio o elegir opción de asignación automática más rápida.",
                "criteria": "GET /api/estilistas filtra por habilidad. Opción 'Cualquiera disponible' pasa null a la API.",
                "tasks": [
                    {"title": "TSK-3.03.1: Implementar GET /api/estilistas?servicioId={id} en C#", "assigned": ALEX, "hours": 4, "desc": "Filtrar estilistas activos vinculados en estilista_servicios."},
                    {"title": "TSK-3.03.2: Maquetar SeleccionEstilistaView según Wireframe Pág. 9", "assigned": CAMILA, "hours": 4, "desc": "Diseñar tarjetas de estilistas y card destacada de autoasignación."},
                    {"title": "TSK-3.03.3: Manejar estado de selección en ReservaBloc", "assigned": CAMILA, "hours": 3, "desc": "Manejar selección individual o asignación automática."}
                ]
            },
            {
                "title": "US-3.04: Motor de Cálculo de Disponibilidad Dinámica de Horarios",
                "assigned": ALEX,
                "points": 8,
                "desc": "Calcular en tiempo real slots libres considerando turnos, citas existentes y duración del servicio.",
                "criteria": "GET /api/disponibilidad cruza jornada laboral y reservas. Bloques ocupados deshabilitados en UI.",
                "tasks": [
                    {"title": "TSK-3.04.1: Desarrollar AvailabilityService con algoritmo de slots en C#", "assigned": ALEX, "hours": 8, "desc": "Cruzar horarios_estilista con reservas existentes y segmentar intervalos."},
                    {"title": "TSK-3.04.2: Implementar endpoint GET /api/disponibilidad en C#", "assigned": ALEX, "hours": 4, "desc": "Exponer endpoint REST con validación de parámetros de fecha y servicio."},
                    {"title": "TSK-3.04.3: Maquetar SeleccionHorarioView según Wireframe Pág. 10", "assigned": CAMILA, "hours": 5, "desc": "Diseñar calendario mensual interactivo y grilla de horarios."},
                    {"title": "TSK-3.04.4: Conectar DisponibilidadBloc con la grilla de slots", "assigned": CAMILA, "hours": 3, "desc": "Pintar bloques libres en verde/rosa y deshabilitar ocupados."},
                    {"title": "TSK-3.04.5: Pruebas integradas de casos límite de disponibilidad", "assigned": ALEX, "hours": 3, "desc": "Probar solapamientos, límites de jornada y múltiples reservas consecutivas."}
                ]
            }
        ]
    },

    # ==========================================
    # SPRINT 4 - ÉPICA 4
    # ==========================================
    {
        "title": "Épica 4: Motor Transaccional de Reservas y Gestión Personal",
        "sprint": "Sprint 4",
        "assigned": ALEX,
        "desc": "Transaccionar citas con control estricto de concurrencia (ACID), confirmación y gestión del historial de citas.",
        "pbis": [
            {
                "title": "US-4.01: Resumen y Creación de Reserva con Control de Concurrencia",
                "assigned": ALEX,
                "points": 8,
                "desc": "Confirmar reserva con transacción ACID garantizando prevención de doble reserva (Overbooking).",
                "criteria": "POST /api/reservas transaccional. Asignación de estación de trabajo. Retorno 409 Conflict si el slot se ocupó.",
                "tasks": [
                    {"title": "TSK-4.01.1: Implementar CreateAppointmentUseCase con transacción ACID en C#", "assigned": ALEX, "hours": 6, "desc": "Bloqueo pesimista/serializable en PostgreSQL para prevenir concurrencia."},
                    {"title": "TSK-4.01.2: Implementar asignación de estación de trabajo libre en C#", "assigned": ALEX, "hours": 3, "desc": "Asignar cabina/estación disponible según categoría del servicio."},
                    {"title": "TSK-4.01.3: Maquetar ResumenReservaView según Wireframe Pág. 11", "assigned": CAMILA, "hours": 4, "desc": "Diseñar desglose de servicio, estilista, fecha, hora, subtotal e impuestos."},
                    {"title": "TSK-4.01.4: Conectar botón Confirmar con POST /api/reservas en Flutter", "assigned": CAMILA, "hours": 4, "desc": "Manejar respuesta 201 Created y capturar 409 Conflict mostrando alerta amigable."}
                ]
            },
            {
                "title": "US-4.02: Comprobante y Confirmación de Cita con Código Único",
                "assigned": CAMILA,
                "points": 3,
                "desc": "Visualizar comprobante de confirmación con código alfanumérico único de reserva.",
                "criteria": "Generación de código único (#SHU-XXXX). Pantalla visual de comprobante y navegación.",
                "tasks": [
                    {"title": "TSK-4.02.1: Implementar generador de código de reserva único en C#", "assigned": ALEX, "hours": 2, "desc": "Generar código no repetible con prefijo de marca #SHU-XXXX."},
                    {"title": "TSK-4.02.2: Maquetar ConfirmacionReservaView según Wireframe Pág. 12", "assigned": CAMILA, "hours": 3, "desc": "Diseñar ticket de comprobante con código, dirección y avisos."},
                    {"title": "TSK-4.02.3: Configurar navegación hacia Mis Citas o Catálogo", "assigned": CAMILA, "hours": 2, "desc": "Vincular botones de acción hacia pestañas correspondientes."}
                ]
            },
            {
                "title": "US-4.03: Historial de Citas del Cliente (Próximas y Pasadas)",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Consultar citas agendadas futuras y citas completadas previamente.",
                "criteria": "GET /api/reservas/cliente/{id} validado por claims de usuario. Pestañas Próximas y Pasadas en UI.",
                "tasks": [
                    {"title": "TSK-4.03.1: Implementar GET /api/reservas/cliente/{id} con filtro de seguridad", "assigned": ALEX, "hours": 4, "desc": "Validar que el id coincida con el claim internal_user_id (RNF04)."},
                    {"title": "TSK-4.03.2: Maquetar HistorialCitasView según Wireframe Pág. 13", "assigned": CAMILA, "hours": 4, "desc": "Diseñar pestañas Citas Futuras y Citas Pasadas."},
                    {"title": "TSK-4.03.3: Conectar HistorialBloc y renderizado de tarjetas con estado", "assigned": CAMILA, "hours": 3, "desc": "Renderizar badges cromáticos (Verde: Confirmada, Amarillo: Pendiente)."}
                ]
            },
            {
                "title": "US-4.04: Cancelación y Solicitud de Reprogramación de Citas",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Cancelar cita pendiente con anticipación o iniciar flujo de cambio de horario.",
                "criteria": "PUT /api/reservas/{id}/cancelar valida límite de 2h. Liberación inmediata de slot.",
                "tasks": [
                    {"title": "TSK-4.04.1: Implementar PUT /api/reservas/{id}/cancelar en C#", "assigned": ALEX, "hours": 3, "desc": "Validar límite de tiempo previo de cancelación y cambiar estado."},
                    {"title": "TSK-4.04.2: Implementar modal de confirmación de cancelación en Flutter", "assigned": CAMILA, "hours": 3, "desc": "Diseñar diálogo con advertencia de políticas del salón."},
                    {"title": "TSK-4.04.3: Conectar flujo de reprogramación hacia selector de horario", "assigned": CAMILA, "hours": 3, "desc": "Reutilizar SeleccionHorarioView pasando la reserva a modificar."},
                    {"title": "TSK-4.04.4: Pruebas de QA de cancelación y liberación de disponibilidad", "assigned": CAMILA, "hours": 2, "desc": "Verificar que el slot cancelado vuelve a aparecer libre para otros usuarios."}
                ]
            }
        ]
    },

    # ==========================================
    # SPRINT 5 - ÉPICA 5
    # ==========================================
    {
        "title": "Épica 5: Panel de Control Administrativo y Gestión del Salón",
        "sprint": "Sprint 5",
        "assigned": ALEX,
        "desc": "Proveer herramientas exclusivas para recepción y gerencia (dashboard de KPIs, agenda timeline, walk-ins y mantenimiento).",
        "pbis": [
            {
                "title": "US-5.01: Dashboard Operativo Diario y Métricas Clave",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Visualizar panel con contadores del día, ocupación de estaciones e ingresos proyectados.",
                "criteria": "GET /api/admin/dashboard protegido por rol Administrador. Métricas de ingresos y estaciones.",
                "tasks": [
                    {"title": "TSK-5.01.1: Implementar consultas de métricas en AdminDashboardController en C#", "assigned": ALEX, "hours": 5, "desc": "Calcular citas del día, % capacidad, ingresos proyectados y ocupación."},
                    {"title": "TSK-5.01.2: Proteger endpoints administrativos con [Authorize(Roles = 'Administrador')]", "assigned": ALEX, "hours": 2, "desc": "Verificar rechazo 403 Forbidden para usuarios con rol Cliente."},
                    {"title": "TSK-5.01.3: Maquetar DashboardAdminView según Wireframe Pág. 15", "assigned": CAMILA, "hours": 5, "desc": "Diseñar tarjetas de KPIs, gráficos de ocupación y monitores de cabinas."},
                    {"title": "TSK-5.01.4: Conectar AdminDashboardBloc con la API", "assigned": CAMILA, "hours": 3, "desc": "Gestionar carga y refresco automático de métricas diarias."}
                ]
            },
            {
                "title": "US-5.02: Agenda Diaria Interactiva en Formato Timeline Multi-Estilista",
                "assigned": CAMILA,
                "points": 8,
                "desc": "Visualizar agenda tipo Timeline agrupada por estilista para gestionar la jornada.",
                "criteria": "GET /api/admin/agenda devuelve citas estructuradas por estilista. Bloques libres interactivos.",
                "tasks": [
                    {"title": "TSK-5.02.1: Implementar endpoint GET /api/admin/agenda en C#", "assigned": ALEX, "hours": 6, "desc": "Estructurar respuesta agrupando cronograma de citas y descansos por empleado."},
                    {"title": "TSK-5.02.2: Maquetar vista Timeline multi-columna según Wireframe Pág. 16", "assigned": CAMILA, "hours": 6, "desc": "Diseñar grilla interactiva con bloques de citas proporcionales al tiempo."},
                    {"title": "TSK-5.02.3: Conectar selector de fecha y refresco dinámico", "assigned": CAMILA, "hours": 3, "desc": "Permitir navegar entre fechas para consultar agendas pasadas y futuras."}
                ]
            },
            {
                "title": "US-5.03: Registro Rápido de Clientes Presenciales (Walk-in Clients)",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Registrar citas espontáneas para clientes sin cuenta ocupando espacios libres de la agenda.",
                "criteria": "POST /api/reservas/walk-in registra es_walk_in = true. Refresco automático de Timeline.",
                "tasks": [
                    {"title": "TSK-5.03.1: Implementar endpoint POST /api/reservas/walk-in en C#", "assigned": ALEX, "hours": 4, "desc": "Permitir id_cliente nulo y guardar nombre_walk_in y telefono_walk_in."},
                    {"title": "TSK-5.03.2: Maquetar formulario modal rápido de Walk-in en Flutter", "assigned": CAMILA, "hours": 4, "desc": "Invocar modal al presionar bloque libre '+ Walk-in Client' en el Timeline."},
                    {"title": "TSK-5.03.3: Refresco inmediato de la agenda tras registro", "assigned": CAMILA, "hours": 3, "desc": "Ocupar visualmente el slot sin recargar toda la pantalla."}
                ]
            },
            {
                "title": "US-5.04: Gestión del Ciclo de Vida y Auditoría de Estados de Citas",
                "assigned": CAMILA,
                "points": 4,
                "desc": "Actualizar estados de citas (Pendiente, Completada, Cancelada, No Asistió) con registro de auditoría.",
                "criteria": "PUT /api/admin/reservas/{id}/estado actualiza y guarda en historial_estado_reserva.",
                "tasks": [
                    {"title": "TSK-5.04.1: Implementar actualización y auditoría en historial_estado_reserva en C#", "assigned": ALEX, "hours": 3, "desc": "Guardar id_usuario_cambio, estado_anterior y estado_nuevo en transacción."},
                    {"title": "TSK-5.04.2: Maquetar EstadoReservaView según Wireframe Pág. 17", "assigned": CAMILA, "hours": 3, "desc": "Diseñar selector de estado y campo de texto para observaciones de auditoría."},
                    {"title": "TSK-5.04.3: Pruebas de QA de trazabilidad de cambios de estado", "assigned": CAMILA, "hours": 2, "desc": "Verificar registro de auditoría en la base de datos tras cada cambio."}
                ]
            },
            {
                "title": "US-5.05: Mantenimiento de Catálogo de Servicios y Precios",
                "assigned": CAMILA,
                "points": 4,
                "desc": "Modificar precios, duración y activar/desactivar servicios del salón.",
                "criteria": "POST/PUT /api/servicios/{id} operativo. Reflejo inmediato en la app pública.",
                "tasks": [
                    {"title": "TSK-5.05.1: Implementar endpoints de mantenimiento de catálogo en C#", "assigned": ALEX, "hours": 3, "desc": "Crear endpoints de creación, edición y toggle de activación de servicios."},
                    {"title": "TSK-5.05.2: Maquetar AdministracionCatalogoView según Wireframe Pág. 18", "assigned": CAMILA, "hours": 3, "desc": "Diseñar switches on/off y modal de edición de tarifas."},
                    {"title": "TSK-5.05.3: Pruebas de QA de reactividad en catálogo público", "assigned": CAMILA, "hours": 2, "desc": "Comprobar que un servicio desactivado desaparece del catálogo del cliente."}
                ]
            },
            {
                "title": "US-5.06: Control de Disponibilidad y Turnos de Estilistas",
                "assigned": CAMILA,
                "points": 3,
                "desc": "Marcar estilistas como inactivos temporales por imprevistos para bloquear su agenda.",
                "criteria": "PUT /api/estilistas/{id}/estado actualiza disponibilidad. Exclusión automática del motor de cálculo.",
                "tasks": [
                    {"title": "TSK-5.06.1: Implementar endpoint PUT /api/estilistas/{id}/estado en C#", "assigned": ALEX, "hours": 2, "desc": "Actualizar campo estado_disponibilidad en estilistas."},
                    {"title": "TSK-5.06.2: Maquetar EstadoEstilistaView según Wireframe Pág. 19", "assigned": CAMILA, "hours": 3, "desc": "Diseñar lista de estilistas con toggles de disponibilidad."},
                    {"title": "TSK-5.06.3: Verificar bloqueo automático en motor de disponibilidad", "assigned": ALEX, "hours": 2, "desc": "Comprobar que el estilista inactivo no genera slots libres en fechas futuras."}
                ]
            }
        ]
    },

    # ==========================================
    # SPRINT 6 - ÉPICA 6
    # ==========================================
    {
        "title": "Épica 6: Pruebas Integrales, Rendimiento y Despliegue",
        "sprint": "Sprint 6",
        "assigned": ALEX,
        "desc": "Asegurar la calidad técnica (QA), pruebas de carga concurrentes y despliegue para la evaluación académica.",
        "pbis": [
            {
                "title": "US-6.01: Pruebas Unitarias y de Integración Automatizadas",
                "assigned": CAMILA,
                "points": 5,
                "desc": "Desarrollar pruebas automatizadas con xUnit en backend y bloc_test en frontend.",
                "criteria": "Pruebas de algoritmo de disponibilidad en C#. Pruebas de estados de BLoC en Flutter.",
                "tasks": [
                    {"title": "TSK-6.01.1: Implementar pruebas unitarias con xUnit en C#", "assigned": ALEX, "hours": 5, "desc": "Probar AvailabilityService, validaciones de entrada y manejo de errores."},
                    {"title": "TSK-6.01.2: Implementar pruebas de BLoC con bloc_test en Flutter", "assigned": CAMILA, "hours": 5, "desc": "Probar transiciones de estado en AuthBloc, CatalogoBloc y ReservaBloc."}
                ]
            },
            {
                "title": "US-6.02: Pruebas de Estrés y Concurrencia Transaccional (RNF02)",
                "assigned": ALEX,
                "points": 5,
                "desc": "Simular 50 peticiones simultáneas sobre el mismo slot para verificar bloqueo de doble reserva.",
                "criteria": "Exactamente 1 petición retorna 201 Created y 49 peticiones retornan 409 Conflict.",
                "tasks": [
                    {"title": "TSK-6.02.1: Crear script de prueba de carga con k6 simulando concurrencia", "assigned": ALEX, "hours": 4, "desc": "Configurar 50 peticiones concurrentes en el mismo milisegundo para la misma cita."},
                    {"title": "TSK-6.02.2: Ejecutar prueba de concurrencia y documentar evidencias", "assigned": CAMILA, "hours": 4, "desc": "Comprobar 1 éxito (201) y 49 rechazos controlados (409 Conflict RFC 7807)."}
                ]
            },
            {
                "title": "US-6.03: Despliegue en la Nube y Generación de Entregables",
                "assigned": ALEX,
                "points": 6,
                "desc": "Publicar Web API en Azure App Service y compilar APK release de Flutter para entrega académica.",
                "criteria": "API accesible bajo HTTPS con Swagger activo. APK Flutter Release compilado y conectado.",
                "tasks": [
                    {"title": "TSK-6.03.1: Desplegar Web API C# en Azure App Service", "assigned": ALEX, "hours": 4, "desc": "Configurar App Service, Application Insights y variables de entorno de producción."},
                    {"title": "TSK-6.03.2: Compilar APK Release de Flutter apuntando a Azure", "assigned": CAMILA, "hours": 4, "desc": "Compilar artefacto final con flutter build apk --release --dart-define-from-file."},
                    {"title": "TSK-6.03.3: Preparar reporte final y entorno de demostración académica", "assigned": ALEX, "hours": 3, "desc": "Documentar métricas, evidencias y preparar presentación para docentes de ESFE AGAPE."}
                ]
            }
        ]
    }
]

def main():
    print(f"🚀 Iniciando carga jerárquica a Azure DevOps Boards ({PROJECT_NAME})...\n")
    
    total_created = 0

    for epic_data in backlog_tree:
        sprint_path = f"{PROJECT_NAME}\\{epic_data['sprint']}"
        
        # 1. Crear Épica
        epic_patch = [
            {"op": "add", "path": "/fields/System.Title", "value": epic_data["title"]},
            {"op": "add", "path": "/fields/System.AreaPath", "value": PROJECT_NAME},
            {"op": "add", "path": "/fields/System.IterationPath", "value": sprint_path},
            {"op": "add", "path": "/fields/System.AssignedTo", "value": epic_data["assigned"]},
            {"op": "add", "path": "/fields/System.Description", "value": epic_data["desc"]},
            {"op": "add", "path": "/fields/Microsoft.VSTS.Common.Priority", "value": 1}
        ]
        
        ok, epic_id, err = post_work_item("Epic", epic_patch)
        if not ok:
            print(f"❌ Error al crear Épica '{epic_data['title']}': {err}")
            continue
            
        print(f"👑 [Epic] {epic_data['title']} (ID: {epic_id})")
        total_created += 1
        
        # 2. Crear Product Backlog Items vinculados a la Épica
        for pbi_data in epic_data["pbis"]:
            pbi_patch = [
                {"op": "add", "path": "/fields/System.Title", "value": pbi_data["title"]},
                {"op": "add", "path": "/fields/System.AreaPath", "value": PROJECT_NAME},
                {"op": "add", "path": "/fields/System.IterationPath", "value": sprint_path},
                {"op": "add", "path": "/fields/System.AssignedTo", "value": pbi_data["assigned"]},
                {"op": "add", "path": "/fields/System.Description", "value": pbi_data["desc"]},
                {"op": "add", "path": "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", "value": pbi_data["criteria"]},
                {"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.Effort", "value": float(pbi_data["points"])},
                {"op": "add", "path": "/fields/Microsoft.VSTS.Common.Priority", "value": 1},
                {
                    "op": "add", 
                    "path": "/relations/-", 
                    "value": {
                        "rel": "System.LinkTypes.Hierarchy-Reverse",
                        "url": f"https://dev.azure.com/{ORGANIZATION}/_apis/wit/workItems/{epic_id}",
                        "attributes": {"comment": "Parent Epic"}
                    }
                }
            ]
            
            ok, pbi_id, err = post_work_item("Product Backlog Item", pbi_patch)
            if not ok:
                print(f"  ❌ Error al crear PBI '{pbi_data['title']}': {err}")
                continue
                
            print(f"  📦 [PBI] {pbi_data['title']} (ID: {pbi_id}) -> Parent Epic {epic_id}")
            total_created += 1
            
            # 3. Crear Tasks vinculadas al PBI
            for task_data in pbi_data["tasks"]:
                task_patch = [
                    {"op": "add", "path": "/fields/System.Title", "value": task_data["title"]},
                    {"op": "add", "path": "/fields/System.AreaPath", "value": PROJECT_NAME},
                    {"op": "add", "path": "/fields/System.IterationPath", "value": sprint_path},
                    {"op": "add", "path": "/fields/System.AssignedTo", "value": task_data["assigned"]},
                    {"op": "add", "path": "/fields/System.Description", "value": task_data["desc"]},
                    {"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.OriginalEstimate", "value": float(task_data["hours"])},
                    {"op": "add", "path": "/fields/Microsoft.VSTS.Scheduling.RemainingWork", "value": float(task_data["hours"])},
                    {"op": "add", "path": "/fields/Microsoft.VSTS.Common.Priority", "value": 1},
                    {
                        "op": "add", 
                        "path": "/relations/-", 
                        "value": {
                            "rel": "System.LinkTypes.Hierarchy-Reverse",
                            "url": f"https://dev.azure.com/{ORGANIZATION}/_apis/wit/workItems/{pbi_id}",
                            "attributes": {"comment": "Parent PBI"}
                        }
                    }
                ]
                
                ok, task_id, err = post_work_item("Task", task_patch)
                if not ok:
                    print(f"    ❌ Error al crear Task '{task_data['title']}': {err}")
                    continue
                    
                print(f"    🔧 [Task] {task_data['title']} (ID: {task_id}) [{task_data['assigned'].split()[0]} | {task_data['hours']}h] -> Parent PBI {pbi_id}")
                total_created += 1

    print(f"\n✨ Sincronización exitosa total: {total_created} Work Items creados y vinculados en Azure Boards!")

if __name__ == "__main__":
    main()
