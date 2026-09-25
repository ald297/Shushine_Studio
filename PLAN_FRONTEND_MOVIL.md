# 📱 Plan Maestro por Fases — Frontend Móvil (.NET MAUI)
## Shushine Studio — Sistema de Gestión y Reservas de Belleza

> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)  
> **Desarrolladora Frontend & QA:** Camila Antonia Calderon Cortez  
> **Backend & Scrum Master:** Alex Fernando Alfaro Diaz  
> **Arquitectura:** Clean Architecture + MVVM (`CommunityToolkit.Mvvm`)  
> **Documentos de Referencia:**  
> * [Registro_Requerimientos_Wireframes_SalonBelleza.pdf](./Registro_Requerimientos_Wireframes_SalonBelleza.pdf)  
> * [PRODUCT_BACKLOG.md](./PRODUCT_BACKLOG.md)  
> * [AGENTS.md](./AGENTS.md)  
> * [src/mobile/README.md](./src/mobile/README.md)  

---

## 🧭 1. Diagnóstico del Estado Actual (Lo que Alex dejó listo)

Alex dejó estructurado el núcleo del proyecto en `src/mobile/`, resolviendo la parte más compleja de infraestructura:
1. **Clean Architecture:** Separación estricta de carpetas en `Core`, `Domain`, `Data` y `Presentation`.
2. **Inyección de Dependencias:** Registrada en `MauiProgram.cs` para clientes HTTP, repositorios, casos de uso, ViewModels y páginas.
3. **Manejo de Red y Seguridad:** `ErrorDelegatingHandler` para tokens JWT Bearer y estándar de errores **RFC 7807** (`ProblemDetails`), además de `TokenStorageService` con `SecureStorage`.
4. **Vistas y Modelos Base:** Maquetación inicial de `LoginPage`, `RegisterPage`, `CatalogPage`, `MyAppointmentsPage` y `ProfilePage`.
5. **Identidad Visual:** Paleta cromática oficial configurada en `Resources/Styles/Colors.xaml` (`PrimaryColor: #D48B96`, `BackgroundColor: #FDF6F6`, etc.).

---

## 🎯 2. Metodología de Trabajo Híbrida (Antigravity ➔ Visual Studio)

Para maximizar la eficiencia y evitar bloqueos en el entorno local:
* **Fase de Programación (Aquí en Antigravity):**
  * Maquetación visual fiel a los wireframes en archivos XAML.
  * Programación de ViewModels con `CommunityToolkit.Mvvm` (propiedades observables, comandos `[RelayCommand]`, validaciones).
  * Conexión con contratos de datos y casos de uso.
  * Manejo de ramas en Git (`feature/movil-...`) y commits en español.
* **Fase de Validación y Pruebas (En Visual Studio 2022):**
  * Abrir `ShushineStudio.Mobile.csproj`.
  * Compilar y ejecutar con **F5** en el Emulador de Android (ej. *Pixel 5 - API 34*).
  * Probar interactividad táctil, navegación real y animaciones.

---

## 🚀 3. Plan de Implementación por Fases

```
┌────────────────────────────────────────────────────────────────────────┐
│ FASE 1 (Sprint 1): Autenticación, Registro y Perfil de Usuario        │
│ ├── US-2.01: Registro de Nuevos Clientes (Wireframe Pág. 5)            │
│ ├── US-2.02: Inicio de Sesión y Manejo de JWT (Wireframe Pág. 6)       │
│ └── US-2.03: Perfil de Cliente y Ficha de Belleza (Wireframe Pág. 14)  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
┌───────────────────────────────────▼────────────────────────────────────┐
│ FASE 2 (Sprint 2): Catálogo de Servicios y Selección de Citas         │
│ ├── US-3.01: Catálogo Categorizado y Filtros Rápidos (Pág. 7)          │
│ ├── US-3.02: Ficha Técnica y Detalle del Servicio (Pág. 8)             │
│ ├── US-3.03: Selección de Estilistas / Auto-Asignación (Pág. 9)        │
│ └── US-3.04: Selector de Fecha y Horarios Dinámicos (Pág. 10)          │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
┌───────────────────────────────────▼────────────────────────────────────┐
│ FASE 3 (Sprint 3): Transacción de Reservas y Gestión Personal          │
│ ├── US-4.01: Resumen de Reserva con Políticas e Impuestos (Pág. 11)    │
│ ├── US-4.02: Comprobante de Confirmación y Código Único (Pág. 12)      │
│ ├── US-4.03: Historial de Citas (Próximas y Pasadas) (Pág. 13)         │
│ └── US-4.04: Cancelación y Reprogramación con Regla de 2 Horas         │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
┌───────────────────────────────────▼────────────────────────────────────┐
│ FASE 4 (Sprint 4): Vistas Administrativas y Gestión del Salón          │
│ ├── US-5.01: Dashboard de KPIs Operativos (Pág. 15)                    │
│ ├── US-5.02 / US-5.03: Agenda Timeline y Clientes Walk-in (Pág. 16)    │
│ └── US-5.04 / US-5.05: Estados de Citas y Mantenimiento de Catálogo   │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
┌───────────────────────────────────▼────────────────────────────────────┐
│ FASE 5 (Sprint 5): Control de Calidad (QA), Pruebas Unitarias y Release │
│ ├── Pruebas Unitarias de ViewModels con xUnit y Moq                    │
│ ├── Auditoría de Accesibilidad, Fuentes y Contraste de Colores         │
│ └── Verificación Integral con Backend y Demostración Final             │
└────────────────────────────────────────────────────────────────────────┘
```

---

### 📍 FASE 1: Autenticación, Registro y Perfil de Usuario (Sprint 1)
**Meta:** Garantizar el flujo de entrada del usuario y la persistencia de su sesión.

* [x] **Paso 1.1 — US-2.01: Registro de Clientes (`RegisterPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 5 (`RegistroClienteView`).
  * **Acciones en UI/XAML:**
    * Incorporar campos estipulados: Nombre Completo, Correo Electrónico, Teléfono (formato El Salvador `####-####`), Contraseña y Confirmación de Contraseña.
    * Agregar checkbox de aceptación de "Términos y Condiciones del Salón".
    * Botón primario: *"Crear mi cuenta"*.
    * Enlace hacia Login: *"¿Ya tienes cuenta? Inicia sesión aquí"*.
  * **Acciones en ViewModel (`RegisterViewModel.cs`):**
    * Validar sintaxis de correo con Regex.
    * Validar longitud mínima de contraseña (mínimo 6 caracteres) y coincidencia con la confirmación.
    * Mostrar spinner `ActivityIndicator` reactivo mediante `IsBusy`.
    * Invocar `RegisterAsync()` y notificar errores legibles del backend (RFC 7807, ej. correo duplicado).

* [x] **Paso 1.2 — US-2.02: Inicio de Sesión (`LoginPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 6 (`LoginClienteView`).
  * **Acciones en UI/XAML:**
    * Logotipo y título con estilo Shushine Studio.
    * Campos: Correo y Contraseña con botón de mostrar/ocultar contraseña (icono de ojo).
    * Enlace: *"¿Olvidaste tu contraseña?"*.
    * Botón de inicio de sesión con animación de pulsación.
  * **Acciones en ViewModel (`LoginViewModel.cs`):**
    * Manejo de autenticación contra la API.
    * Persistencia del token JWT en `SecureStorage` con rol asignado.
    * Redirección automática al catálogo principal (`//MainTabs/CatalogPage`).

* [x] **Paso 1.3 — US-2.03: Perfil del Cliente (`ProfilePage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 14 (`PerfilClienteView`).
  * **Acciones en UI/XAML:**
    * Avatar circular con iniciales o foto de perfil.
    * Insignia de fidelidad (Bronce / Plata / Oro).
    * Formulario de datos editables: Teléfono, preferencias capilares y alergias.
    * Botón de *"Cerrar Sesión"* (Logout) con diálogo de confirmación.
  * **Acciones en ViewModel (`ProfileViewModel.cs`):**
    * Carga reactiva de datos mediante `IAuthRepository.GetCurrentUserAsync()`.
    * Limpieza de `SecureStorage` y navegación de retorno a `//LoginPage`.

---

### 📍 FASE 2: Catálogo de Servicios y Selección de Citas (Sprint 2)
**Meta:** Guiar al cliente en la selección de tratamientos, profesionales y horarios disponibles.

* [x] **Paso 2.1 — US-3.01: Catálogo de Servicios (`CatalogPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 7 (`CatalogoServiciosView`).
  * **Acciones en UI/XAML:**
    * Barra superior de búsqueda rápida por nombre de tratamiento.
    * Barra horizontal de categorías (*Cabello*, *Uñas*, *Pestañas*, *Spa* / *Todos*) usando chips interactivos.
    * Lista de tarjetas de servicio (`CollectionView`) con foto, nombre, duración estimada en minutos y precio con formato de moneda (`$0.00`).
  * **Acciones en ViewModel (`CatalogViewModel.cs`):**
    * Invocación a `GetServiciosCatalogUseCase`.
    * Filtro en memoria reactivo por categoría seleccionada.

* [x] **Paso 2.2 — US-3.02: Detalle del Servicio (`ServiceDetailPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 8 (`DetalleServicioView`).
  * **Acciones en UI/XAML:**
    * Imagen de cabecera en alta resolución.
    * Badge de duración y costo.
    * Sección de descripción, beneficios y recomendaciones previas.
    * Botón flotante inferior: *"Continuar con la reserva ➔"*.

* [x] **Paso 2.3 — US-3.03: Selección de Estilista (`StylistSelectionPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 9 (`SeleccionEstilistaView`).
  * **Acciones en UI/XAML:**
    * Tarjeta destacada superior: *"Cualquier estilista disponible (Asignación automática más rápida)"*.
    * Listado de tarjetas de estilistas calificados con foto, nombre, especialidad y estrellas de calificación.

* [x] **Paso 2.4 — US-3.04: Selección de Fecha y Franja Horaria (`SlotSelectionPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 10 (`SeleccionHorarioView`).
  * **Acciones en UI/XAML:**
    * Selector interactivo de fecha (calendario horizontal de días).
    * Grilla de bloques horarios matutinos y vespertinos.
    * Estados visuales claros:
      * **Verde/Rosa:** Slot libre y seleccionable.
      * **Gris deshabilitado:** Slot ocupado o fuera de jornada laboral.
      * **Rosa resaltado:** Slot actualmente seleccionado por el cliente.

---

### 📍 FASE 3: Transacción de Reservas y Gestión Personal (Sprint 3)
**Meta:** Cerrar el ciclo de reserva con protección contra duplicidades y consulta de citas.

* [x] **Paso 3.1 — US-4.01: Resumen y Creación de Reserva (`BookingSummaryPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 11 (`ResumenReservaView`).
  * **Acciones en UI/XAML:**
    * Tarjeta de resumen: Servicio, Estilista asignado, Fecha y Hora seleccionada.
    * Desglose financiero: Subtotal, Descuentos aplicables, Impuestos y Monto Total.
    * Alerta amigable: *"Por favor preséntate 5 minutos antes de tu cita"*.
    * Botón de confirmación: *"Confirmar Cita"*.
  * **Acciones en ViewModel (`BookingSummaryViewModel.cs`):**
    * Invocación a `CreateAppointmentUseCase`.
    * Manejo de error concurrente **409 Conflict** (si otro usuario tomó el cupo) con reintento amigable.

* [x] **Paso 3.2 — US-4.02: Comprobante de Confirmación (`BookingConfirmationPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 12 (`ConfirmacionReservaView`).
  * **Acciones en UI/XAML:**
    * Icono de éxito con micro-animación.
    * Tarjeta tipo ticket con código de cita alfanumérico único (`#SHU-XXXX`).
    * Botones de acción: *"Ver en Mis Citas"* y *"Volver al Inicio"*.

* [x] **Paso 3.3 — US-4.03 & US-4.04: Historial de Citas y Cancelaciones (`MyAppointmentsPage.xaml`)** [COMPLETADO]
  * **Wireframe:** Pág. 13 (`HistorialCitasView`).
  * **Acciones en UI/XAML:**
    * Pestañas superiores: *"Próximas Citas"* y *"Historial Pasado"*.
    * Tarjetas de citas con distintivos de estado (`Pendiente`, `Completada`, `Cancelada`).
    * Botón *"Cancelar Cita"* en citas pendientes con diálogo modal advirtiendo la política de anticipación de 2 horas.

---

### 📍 FASE 4: Módulo Administrativo y Recepción (Sprint 4)
**Meta:** Permitir a recepción y administración monitorear la operación del salón.

* [ ] **Paso 4.1 — US-5.01: Dashboard Operativo (`AdminDashboardPage.xaml`)**
  * **Wireframe:** Pág. 15 (`DashboardAdminView`).
  * Tarjetas de métricas: Total de citas del día, % de capacidad ocupada, ingresos proyectados.
* [ ] **Paso 4.2 — US-5.02 & US-5.03: Agenda Timeline y Clientes Walk-in (`TimelineAgendaPage.xaml`)**
  * **Wireframe:** Pág. 16 (`WalkInClientView`).
  * Grilla horaria por columnas de estilistas y modal rápido para agregar citas presenciales espontáneas.
* [ ] **Paso 4.3 — US-5.04 a US-5.06: Estados de Reserva y Catálogo**
  * Modales de cambio de estado operativo y switches on/off para activar/desactivar servicios del catálogo.

---

### 📍 FASE 5: Calidad (QA), Pruebas Unitarias y Despliegue (Sprint 5)
**Meta:** Asegurar la estabilidad para la entrega final y evaluación académica.

* [ ] **Paso 5.1 — Pruebas Unitarias de ViewModels (`xUnit` / `Moq`):**
  * Pruebas de validación en `RegisterViewModel` (correo inválido, contraseñas no coincidentes).
  * Pruebas del ciclo de reserva en `BookingSummaryViewModel` (estados `IsBusy`, llamada al repositorio y manejo de errores 409).
* [ ] **Paso 5.2 — Revisión de Calidad UI/UX:**
  * Comparación lado a lado de cada pantalla maquetada contra el PDF de wireframes.
  * Verificación de contrastes de color, tamaños táctiles de botones (mínimo 44x44 dp) y comportamiento en modo oscuro/claro.
* [ ] **Paso 5.3 — Preparación de Release:**
  * Compilación en modo `Release` en Visual Studio 2022.
  * Verificación de conexión con la Web API en producción/staging y Supabase.

---

## 🛠️ 4. Guía de Ejecución Rápida para Cada Sesión de Trabajo

1. **Abrir sesión en Antigravity:** Seleccionar la tarea de la fase actual.
2. **Crear o cambiar de rama en Git:**
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/movil-<nombre-de-la-pantalla>
   ```
3. **Programar XAML y C#:** Diseñar y enlazar datos reactivos con MVVM.
4. **Hacer commit en español:**
   ```bash
   git commit -m "feat(movil): maquetar formulario de registro con validaciones de campos"
   ```
5. **Probar en Visual Studio 2022:** Abrir la solución, presionar **F5**, verificar visualmente en el emulador Android y marcar el paso como completado en este documento.
