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
| **Pruebas de Estrés y Concurrencia (RNF02)** | Listo para ejecución de script | **0%** ⏳ (Pendiente hoy) |
| **Sincronización de Tablero Azure Boards** | Pendiente de token para marcar "Done" | **75%** ⏳ (En proceso) |
| **Conexión de Vistas en Frontend (.NET MAUI)** | Arquitectura base lista, vistas en progreso | **40%** 📱 (Siguiente fase) |

---

## 📋 2. Matriz de Tareas Pendientes por Prioridad

### 🔴 Prioridad Alta (Fase Inmediata — Hoy)

#### Tarea 1: Pruebas de Estrés y Concurrencia Transaccional (`TSK-6.02.1` / RNF02)
* **Objetivo:** Demostrar ante el docente que la API maneja transacciones ACID y bloquea la doble reserva (*overbooking*).
* **Acción:** Disparar simultáneamente 10 peticiones HTTP en el mismo milisegundo intentando reservar el mismo estilista y horario.
* **Resultado Esperado:**
  - `1` petición exitosa con código `201 Created`.
  - `9` peticiones rechazadas con código `409 Conflict` (formato RFC 7807 `application/problem+json`).
* **Evidencia:** Documentar la tabla de tiempos y códigos en `docs/REPORTE_CONCURRENCIA_RNF02.md`.

#### Tarea 2: Sincronización y Actualización de Azure Boards (`TSK-1.04.2`)
* **Objetivo:** Reflejar en el tablero Kanban oficial de Azure DevOps todas las tareas completadas por Alex para evaluación del docente.
* **Acción:**
  1. Generar un **Personal Access Token (PAT)** fresco en Azure DevOps con permisos de *Work Items (Read & Write)* y *Code (Read & Write)*.
  2. Ejecutar el script automatizado para actualizar el estado de las tareas de Alex a `Done`.
  3. Realizar `git push origin feature/refinamiento-logica-modelo-bd` hacia Azure Repos.

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

## 📌 3. Próximo Paso Inmediato

1. Proporcionar un **PAT fresco de Azure DevOps** para sincronizar Azure Repos y mover las tareas a `Done` en Azure Boards.
2. Ejecutar la **prueba de concurrencia y estrés RNF02** para certificar el bloqueo de doble reserva.
