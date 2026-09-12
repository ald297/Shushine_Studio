# Shushine Studio — Sistema Móvil de Gestión y Reservas para Salón de Belleza

> **Documento Oficial de Contexto, Requerimientos de Software y Wireframes**  
> **Documento fuente en PDF:** [Registro_Requerimientos_Wireframes_SalonBelleza.pdf](./Registro_Requerimientos_Wireframes_SalonBelleza.pdf)

---

## 1. Identificación del Proyecto e Institución

* **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)
* **Carrera:** Técnico en Ingeniería de Desarrollo de Software
* **Módulo:** Construcción de APIs Web
* **Título del Trabajo:** Registro de Requerimientos y Wireframes
* **Ciclo Lectivo:** 04/2026
* **Fecha de Presentación:** 08/09/2026

### Equipo de Desarrollo

| Integrante | Rol | Participación |
| :--- | :--- | :--- |
| **Alex Fernando Alfaro Diaz** | Scrum Master / Desarrollador Backend | 100% |
| **Camila Antonia Calderon Cortez** | Desarrolladora Móvil / QA-Tester | 100% |

---

## 2. Descripción General del Sistema

Sistema integral móvil y web para la administración y reserva de citas en salones de belleza (**Shushine Studio** / marcas prototipo *L'Élixir Salón* y *Atelier Admin*). El sistema está compuesto por dos módulos principales:

1. **Módulo del Cliente (App Móvil):**
   * Registro e inicio de sesión seguro.
   * Exploración de catálogo categorizado de servicios (cabello, uñas, maquillaje).
   * Selección de estilista o profesional de preferencia.
   * Consulta dinámica de disponibilidad en tiempo real por fecha y duración.
   * Flujo guiado de reserva en pasos claros con confirmación y generación de código de cita.
   * Consulta de historial de citas (activas y pasadas).
   * Gestión de perfil y preferencias estéticas.

2. **Módulo Administrativo / Recepción (App / Panel Móvil y Web):**
   * Dashboard con métricas del día (citas programadas, completadas, canceladas, ocupación e ingresos proyectados).
   * Agenda interactiva diaria en formato *Timeline* agrupada por estilista y horarios.
   * Registro rápido de clientes presenciales (*Walk-in Client*).
   * Gestión de ciclo de vida de la cita (estados: Pendiente, Completada, Cancelada, No Asistió).
   * Administración del catálogo de servicios (precios, duración, estado activo/inactivo).
   * Control de disponibilidad y estado de los estilistas.

### Stack Tecnológico Oficial

* **Frontend Móvil:** Flutter (Dart) — Clean Architecture con BLoC.
* **Backend API:** C# ASP.NET Core Web API (RESTful, .NET 8/9).
* **Base de Datos & BaaS:** Supabase (PostgreSQL relacional en la nube + Supabase Auth).
* **Seguridad y Autenticación:** Supabase Auth (JWT emitido en cliente) + Autorización RBAC en ASP.NET Core.
* **Control de Versiones:** Azure DevOps / Git.

---

## 3. Matriz de Requerimientos del Sistema

### 3.1 Requerimientos Funcionales (RF)

| Código | Requerimiento | Prioridad | Criterio de Aceptación | Verbo HTTP | Endpoint |
| :---: | :--- | :---: | :--- | :---: | :--- |
| **RF01** | Registrar una cuenta de cliente mediante datos personales. | **Alta** | El cliente completa el formulario de registro y su cuenta es creada en el sistema. | `POST` | `/api/auth/registro` |
| **RF02** | Iniciar sesión mediante autenticación JWT. | **Alta** | El usuario valida credenciales y recibe un token JWT firmado válido junto con sus roles. | `POST` | `/api/auth/login` |
| **RF03** | Consultar el catálogo de servicios activos con precio y duración. | **Alta** | Se obtienen los servicios activos agrupados o filtrados por categoría con precio y duración en minutos. | `GET` | `/api/servicios` |
| **RF04** | Consultar los estilistas disponibles para un servicio seleccionado. | **Alta** | Se listan los profesionales capacitados para realizar el servicio solicitado. | `GET` | `/api/estilistas` |
| **RF05** | Consultar la disponibilidad de un estilista para una fecha determinada. | **Alta** | El sistema retorna únicamente los bloques de horario libres considerando jornada laboral, duración del servicio y citas ya reservadas. | `GET` | `/api/disponibilidad` |
| **RF06** | Registrar una reserva seleccionando servicio, estilista, fecha y horario. | **Alta** | La reserva se crea únicamente si el slot de tiempo continúa desocupado al momento de la confirmación. | `POST` | `/api/reservas` |
| **RF07** | Consultar el historial de citas del cliente. | **Media** | El cliente autenticado visualiza sus citas agendadas futuras y su histórico previo. | `GET` | `/api/reservas/cliente/{id}` |
| **RF08** | Consultar las métricas diarias del salón desde el dashboard administrativo. | **Alta** | El administrador visualiza citas del día, citas completadas, cancelaciones e ingresos proyectados. | `GET` | `/api/admin/dashboard` |
| **RF09** | Consultar la agenda diaria agrupada por estilista. | **Alta** | El administrador visualiza las reservas del día distribuidas por empleado y línea horaria. | `GET` | `/api/admin/agenda` |
| **RF10** | Agregar un cliente que llega directamente al salón en un espacio disponible (*Walk-in Client*). | **Alta** | El administrador o recepcionista registra una cita presencial inmediata o en huecos libres de la agenda. | `POST` | `/api/reservas` |
| **RF11** | Cambiar el estado de una reserva. | **Alta** | El administrador puede actualizar el estado a `Pendiente`, `Completada`, `Cancelada` o `No Asistió`. | `PUT` | `/api/admin/reservas/{id}/estado` |
| **RF12** | Administrar los servicios y estilistas del salón. | **Media** | El administrador puede activar/desactivar servicios, actualizar precios y cambiar la disponibilidad de los estilistas. | `PUT` | `/api/servicios/{id}`<br>`/api/estilistas/{id}` |

### 3.2 Requerimientos No Funcionales (RNF)

> [!NOTE]
> Los Requerimientos No Funcionales definen atributos de calidad transversal (seguridad, integridad, concurrencia y rendimiento).

| Código | Requerimiento | Prioridad | Criterio de Aceptación | Ámbito / Endpoint |
| :---: | :--- | :---: | :--- | :--- |
| **RNF01** | **Seguridad y Autorización RBAC con JWT** | **Alta** | Los usuarios autenticados acceden estrictamente a los recursos y funciones correspondientes a su rol (Cliente o Administrador). | Transversal (`/api/*`) |
| **RNF02** | **Control de Concurrencia Transaccional** | **Alta** | Si dos clientes intentan reservar el mismo estilista y horario simultáneamente, el sistema mediante transacciones y bloqueos aprueba solo una y notifica a la otra. | Transversal (`/api/reservas`) |
| **RNF03** | **Cálculo Dinámico de Disponibilidad** | **Alta** | La API calcula en tiempo real los slots disponibles cruzando horarios del estilista, duración estimada del servicio y reservas existentes. | Transversal (`/api/disponibilidad`) |
| **RNF04** | **Protección de Datos y Privacidad** | **Alta** | Un cliente no puede ver, editar ni cancelar reservas o datos personales de otros clientes (control a nivel de Token/Claims). | Transversal (`/api/*`) |

---

## 4. Arquitectura de Navegación y Wireframes

### 4.1 Módulo Cliente

```
[Inicio de Sesión] ───► [Registro de Cuenta]
       │
       ▼
[Catálogo de Servicios] ───► [Detalle del Servicio] ───► [Selección de Estilista]
                                                                 │
                                                                 ▼
[Historial de Citas] ◄─── [Confirmación] ◄─── [Resumen] ◄─── [Selección de Horario]
       ▲
       │
[Perfil del Cliente]
```

#### Fichas Técnicas de Vistas del Cliente

1. **`RegistroClienteView` (Pág. 5)**
   * **Propósito:** Creación de cuenta para nuevos clientes.
   * **Componentes:** Inputs de Nombre Completo, Correo Electrónico, Teléfono, Contraseña, Confirmación de Contraseña, Botón "Registrarse", Enlace a Login.
   * **Flujo:** Login $\rightarrow$ Registro $\rightarrow$ Catálogo de Servicios.

2. **`LoginClienteView` (Pág. 6)**
   * **Propósito:** Autenticación con credenciales para acceder a la aplicación.
   * **Componentes:** Input de Email/Teléfono, Input de Contraseña (con toggle de visibilidad), "¿Olvidaste tu contraseña?", Botón "Iniciar Sesión", Enlace a Registro.
   * **Flujo:** Login $\rightarrow$ Catálogo de Servicios.

3. **`CatalogoServiciosView` (Pág. 7)**
   * **Propósito:** Mostrar los servicios disponibles clasificados por especialidad.
   * **Componentes:** Filtros de categoría (*Cabello*, *Uñas*, *Maquillaje*), Tarjetas con imagen/código, Nombre, Descripción corta, Precio, Duración en minutos, Botón "Seleccionar $\rightarrow$", Barra de navegación inferior (*Servicios*, *Mis Citas*, *Mi Perfil*).
   * **Flujo:** Catálogo $\rightarrow$ Categoría $\rightarrow$ Servicio.

4. **`DetalleServicioView` (Pág. 8)**
   * **Propósito:** Ficha técnica y descriptiva del tratamiento antes de proceder a agendar.
   * **Componentes:** Mockup/Foto principal del servicio, Categoría, Título, Precio estimado, Duración, Protocolo paso a paso (ej. Lavado dermocalmante, Mascarilla nutritiva), Botón "Continuar con la reserva $\rightarrow$".
   * **Flujo:** Catálogo $\rightarrow$ Servicio $\rightarrow$ Selección de Estilista.

5. **`SeleccionEstilistaView` (Pág. 9)**
   * **Propósito:** Escoger al profesional idóneo para la atención.
   * **Componentes:** Resumen del servicio en la cabecera, Lista de estilistas con badge de disponibilidad y especialidad, Opción especial *"Cualquier estilista disponible (Asignación automática más rápida)"*, Botón "Continuar a Fecha y Hora $\rightarrow$".
   * **Flujo:** Servicio $\rightarrow$ Estilista $\rightarrow$ Disponibilidad.

6. **`SeleccionHorarioView` (Pág. 10)**
   * **Propósito:** Elegir la fecha en calendario y el bloque de tiempo desocupado.
   * **Componentes:** Resumen de cita seleccionada, Selector de mes/año con vista de calendario mensual, Grilla de franjas horarias marcadas como *Seleccionado*, *Libre* u *Ocupado*, Botón continuar.
   * **Flujo:** Disponibilidad $\rightarrow$ Horario $\rightarrow$ Resumen.

7. **`ResumenReservaView` (Pág. 11)**
   * **Propósito:** Desglose final de detalles, costos y políticas antes de confirmar.
   * **Componentes:** Tarjeta con servicio, estilista asignado, fecha, hora (con aviso de presentarse 5 min antes), desglose de subtotal e impuestos, total a pagar en salón, política de cancelación, Botón "Confirmar Reserva $\rightarrow$".
   * **Flujo:** Horario $\rightarrow$ Resumen $\rightarrow$ Confirmación.

8. **`ConfirmacionReservaView` (Pág. 12)**
   * **Propósito:** Comprobante visual de confirmación de cita agendada exitosamente.
   * **Componentes:** Icono de éxito, código alfanumérico único de reserva (ej. `#ELX-8492`), detalles de servicio, profesional, fecha, horario, dirección de la sucursal, recordatorio de notificación, Botones "Ver en Mis Citas" y "Volver al Inicio".
   * **Flujo:** Resumen $\rightarrow$ Confirmación $\rightarrow$ Historial.

9. **`HistorialCitasView` (Pág. 13)**
   * **Propósito:** Gestión de citas pasadas y seguimiento de reservas en curso.
   * **Componentes:** Pestañas *Citas Futuras* y *Citas Pasadas*, Tarjetas con estado (*Próxima*, *Confirmada*, *Completada*), detalles de estilista, importe, fecha/hora, Botones "Ver Detalles" y "Reprogramar".
   * **Flujo:** Navegación principal $\rightarrow$ Historial de Citas.

10. **`PerfilClienteView` (Pág. 14)**
    * **Propósito:** Datos de cuenta, fidelidad y ficha técnica capilar/estética.
    * **Componentes:** Avatar con iniciales, insignia de nivel (*Nivel Oro / Frecuente*), datos personales (correo, teléfono, nacimiento), preferencias de belleza (estilista frecuente, tipo de cabello/diagnóstico).
    * **Flujo:** Navegación principal $\rightarrow$ Perfil.

---

### 4.2 Módulo Administrador

```
[Login Admin] ───► [Dashboard Administrativo]
                          │
         ┌────────────────┼────────────────┐
         ▼                ▼                ▼
  [Agenda Diaria]   [Catálogo Serv.]  [Equipo Estilistas]
         │
   ┌─────┴──────────────┐
   ▼                    ▼
[Walk-in Client]   [Estado Reserva]
```

#### Fichas Técnicas de Vistas Administrativas

1. **`DashboardAdminView` (Pág. 15)**
   * **Propósito:** Monitor central con KPIs operativos del día.
   * **Componentes:**
     * Contador de citas de hoy con % de capacidad y desglose por turno (Mañana, Tarde, Noche).
     * Métrica de completadas y porcentaje del día.
     * Métrica de canceladas y tasa de cancelación.
     * Proyección de ingresos (cobrado vs pendiente) y meta del día.
     * Monitor de ocupación de estaciones/cabinas de trabajo (ej. Corte, Color, Peinado, Uñas, Spa).
     * Navegación inferior: *Dashboard*, *Agenda*, *Catálogo*, *Estilistas*.

2. **`WalkInClientView` / Agenda Interactiva (Pág. 16)**
   * **Propósito:** Registrar clientes espontáneos presenciales ocupando slots libres en la agenda diaria.
   * **Componentes:** Selector de fecha, columnas de estilistas (con conteo de citas asignadas), cuadrícula horaria tipo Timeline con citas existentes y bloques libres marcados como `+ Walk-in Client (Libre - Disponible)`.

3. **`EstadoReservaView` (Pág. 17)**
   * **Propósito:** Control del ciclo de vida operativo de una cita específica.
   * **Componentes:** Identificador de cita, cliente, teléfono, servicio, estilista, horario y monto total. Selector de estado: `Pendiente`, `Completada`, `Cancelada` o `No Asistió`. Botón "Guardar Cambio de Estado" con registro de auditoría.

4. **`AdministracionCatalogoView` (Pág. 18)**
   * **Propósito:** Gestión de tarifas, tiempos y disponibilidad de servicios.
   * **Componentes:** Búsqueda rápida, filtros (*Todos*, *Activos*, *Inactivos*), tarjetas de servicio con interruptor de activación on/off, edición rápida de tarifa/precio en modal o inline, botón "+ Nuevo Servicio".

5. **`EstadoEstilistaView` (Pág. 19)**
   * **Propósito:** Gestión de disponibilidad del personal para turnos y agenda pública.
   * **Componentes:** Listado de profesionales con cargo y avatar, conteo de activos vs inactivos temporales, selectores para marcar disponibilidad activa o inactivo temporal (bloqueando automáticamente la asignación de nuevas citas).

---

## 5. Sistema de Diseño e Identidad Visual UI/UX

* **Enfoque de Experiencia:** Fluida, minimalista, orientada a reducir fricción tanto para el cliente final como para el recepcionista/administrador.
* **Paleta Cromática Oficial:**
  * **Rosa Principal / Terracota Elegante:** Acentos de marca, botones principales de acción (CTA).
  * **Rosa Claro / Soft Blush:** Fondos secundarios, badges de estado y contenedores suaves.
  * **Lavanda Sutil:** Elementos de relajación, citas de spa y tratamientos capilares.
  * **Blanco Puro y Tonos Neutros Cálidos:** Fondos limpios, tarjetas elevadas y excelente legibilidad.
* **Componentes Clave:**
  * Flujo tipo stepper (Paso 1 de 4, 2 de 4, etc.).
  * Calendario interactivo con microestados (*Libre*, *Ocupado*, *Seleccionado*).
  * Agenda tipo Timeline multi-columna para administración.
  * Badges de estado con codificación cromática (Verde: Completada, Amarillo/Rosa: Pendiente, Rojo/Gris: Cancelada/No Asistió).

---

## 6. Recursos y Archivos Incluidos

* **Archivo Maestro PDF:** [`Registro_Requerimientos_Wireframes_SalonBelleza.pdf`](./Registro_Requerimientos_Wireframes_SalonBelleza.pdf) (Ubicado en la raíz del proyecto).
* **Diagrama y Modelo de Base de Datos (DBML, Mermaid, SQL):** [`docs/DIAGRAMA_BASE_DE_DATOS.md`](./docs/DIAGRAMA_BASE_DE_DATOS.md) (Especificación técnica de entidades, relaciones y script para Supabase / PostgreSQL).
* **Vistas de Wireframes extraídas en alta resolución:** Disponibles en la carpeta [`docs/wireframes/`](./docs/wireframes/) para consulta e integración visual directa.
