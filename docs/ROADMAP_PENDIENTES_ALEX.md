# 🗺️ Roadmap de Tareas Pendientes — Alex Fernando Alfaro Diaz
> **Rol:** Backend Lead & Scrum Master  
> **Proyecto:** Shushine Studio — Sistema Móvil de Gestión y Reservas para Salón de Belleza  
> **Institución:** Escuela Superior Franciscana Especializada (ESFE AGAPE / MEGATEC)  
> **Fecha de Actualización:** 25 de Septiembre de 2026  

---

## 🎯 1. Resumen de Estado Actual

| Área de Trabajo | Estado Actual | Porcentaje de Cumplimiento |
| :--- | :---: | :---: |
| **Arquitectura Backend & Configuración Cloud** | Desplegado en Render y Supabase | **100%** ✅ |
| **Cobertura de Endpoints RESTful (33/33)** | Todos probados y funcionando en vivo | **100%** ✅ |
| **Seguridad JWT y Manejo RFC 7807** | Operativo con HMAC-SHA512 y ProblemDetails | **100%** ✅ |
| **Pruebas de Estrés y Concurrencia (RNF02)** | Certificado con 0% overbooking en Render | **100%** ✅ |
| **Sincronización de Tablero Azure Boards** | Sincronizado a Azure Repos y tareas en Done | **100%** ✅ |
| **Capa de Datos Móvil (.NET MAUI)** | DTOs, Repositorios y Handlers alineados con Render | **100%** ✅ |

---

## 📋 2. Matriz de Tareas Pendientes por Prioridad

### 🟢 Fase Inmediata — Completada al 100% ✅

#### Tarea 1: Pruebas de Estrés y Concurrencia Transaccional (`TSK-6.02.1` / RNF02) — COMPLETADA ✅
* **Resultado:** 10 peticiones simultáneas, 1 aprobada (201 Created), 9 bloqueadas (409 Conflict RFC 7807). Cero overbooking.
* **Evidencia oficial:** [docs/REPORTE_CONCURRENCIA_RNF02.md](file:///home/alex/Desktop/Shushine_Studio/docs/REPORTE_CONCURRENCIA_RNF02.md).

#### Tarea 2: Sincronización y Actualización de Azure Boards (`TSK-1.04.2`) — COMPLETADA ✅
* **Resultado:** Commits sincronizados en Azure Repos (`origin/feature/refinamiento-logica-modelo-bd`).
* **Azure Boards:** Todas las Épicas, Historias y Tareas de Alex actualizadas a `Done`.

---

### 🟡 Prioridad Media (Integración con Móvil .NET MAUI — Esta Semana)

#### Tarea 3: Apoyo en Conexión de Pantallas XAML en .NET MAUI (Scrum Master / Frontend)
* **Objetivo:** Conectar el frontend móvil con la API en la nube ya desplegada en Render (`https://shushine-studio.onrender.com/api`).
* **Pantallas y Flujos Clave:**
  1. **Autenticación:** `LoginClientePage` y `RegistroClientePage` consumiendo `POST /api/auth/login`.
  2. **Catálogo de Servicios:** `CatalogoServiciosPage` consumiendo `GET /api/servicios/lista`.
  3. **Selector de Horarios:** `SeleccionHorarioPage` consumiendo `GET /api/estilistas/{id}/disponibilidad`.
  4. **Confirmación de Reserva:** `ResumenReservaPage` enviando `POST /api/citas` y recibiendo código `#SHU-XXXX`.
  5. **Historial de Citas:** `HistorialCitasPage` consumiendo `GET /api/citas/mis-citas`.

#### Tarea 4: Compilación del Paquete Release Android APK (`TSK-6.03.2`)
* **Objetivo:** Generar el instalador `.apk` de la aplicación móvil para instalarlo en el dispositivo del docente o emulador.
* **Comando:**
  ```bash
  dotnet publish src/mobile/ShushineStudio.Mobile.csproj -f net8.0-android -c Release
  ```

---

### 🟢 Prioridad Baja (Cierre de Sprint y Documentación Final)

#### Tarea 5: Preparación de la Ficha de Demostración Docente (Demo Pitch) (`TSK-6.03.3`)
* **Objetivo:** Crear una hoja resumen con los accesos directos para la presentación académica:
  - Enlace oficial a Swagger UI: `https://shushine-studio.onrender.com/swagger-ui/index.html`.
  - Credenciales demo sembradas:
    - **Admin:** `admin` / `admin123`
    - **Cliente:** `cliente` / `cliente123`
  - Resumen de métricas de calidad y arquitectura pedagógica ESFE AGAPE.

---

## 📌 3. Próximos Pasos Inmediatos Disponibles

1. **Ficha de Demostración Docente (`docs/DEMO_PITCH_DOCENTE.md`):** Generar el documento guía oficial para la evaluación del proyecto ante los evaluadores de ESFE AGAPE con Swagger UI, credenciales y guion de defensa.
2. **ViewModels del Flujo de Reserva (`BookingSummaryViewModel` y `SeleccionHorarioViewModel`):** Dejar lista la lógica de presentación y navegación para que Camila solo aplique sus estilos XAML sin preocuparse por la lógica de negocio.
3. **Instalación de Workloads para APK Android:** Ejecutar `dotnet workload restore` si se desea generar el paquete instalador `.apk` de manera local.
