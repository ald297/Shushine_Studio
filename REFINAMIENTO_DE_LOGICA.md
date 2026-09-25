# 💎 Refinamiento de Lógica de Negocio y Reglas Operativas — Shunshine Studio

> **Proyecto:** Shunshine Studio — Plataforma Móvil Integral de Gestión y Reservas para Salón de Belleza  
> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)  
> **Carrera:** Técnico en Ingeniería de Desarrollo de Software | **Módulo:** Construcción de APIs Web  
> **Equipo de Desarrollo:**  
> - **Alex Fernando Alfaro Diaz** (Backend / Base de Datos / Scrum Master)  
> - **Camila Antonia Calderon Cortez** (Frontend Móvil / UI/UX / QA & Testing)  
> **Fecha de Emisión:** Septiembre 2026  
> **Estado:** Documento Normativo Oficial de Lógica y Casos Borde  

---

## 1. Identidad Oficial del Proyecto

* **Nombre Oficial Unificado:** **Shunshine Studio** (con "n" intermedia).  
* Todos los encabezados, textos de interfaz de usuario (UI en español neutro), contratos de API, nombres de repositorios y documentación técnica adoptan **Shunshine Studio** como la denominación de marca definitiva.

---

## 2. Resumen Ejecutivo del Refinamiento

Este documento complementa el [DOCUMENTO_EVOLUCION_SHUNSHINE_STUDIO.md](./DOCUMENTO_EVOLUCION_SHUNSHINE_STUDIO.md) y el SDD (*Software Design Document*), estableciendo las definiciones técnicas exactas, reglas de transición, casos borde (*edge cases*) y ajustes de base de datos acordados para garantizar que el desarrollo en la **Web API** y la app móvil en **.NET MAUI** sea 100% robusto, predecible e interoperable.

---

## 3. Matriz de Refinamientos Técnicos y Reglas de Negocio

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                   SHUNSHINE STUDIO — ARQUITECTURA DE REGLAS                      │
├──────────────────────────────────────────────────────────────────────────────────┤
│ 1. Cotizaciones       ► Expiración auto (2h antes) / Rechazo con confirmación    │
│ 2. Walk-in Clients    ► Clientes sin Auth (id_usuario NULL) + Vinculación retro  │
│ 3. Fidelización       ► 1 pt/$1, 100 pts=$5. Reembolso en cancelaciones válidas  │
│ 4. Reseñas            ► Ventana de 14 días post-servicio, inmutables y públicas  │
│ 5. Chat por Cita      ► Emisor "Shunshine Studio", 24h post-completed a ReadOnly │
│ 6. Storage (6 Buckets)► Nomenclatura unificada + aislamiento RLS en referencias  │
│ 7. Cancelaciones      ► Ventana autónoma de 4 horas / Concurrencia transaccional │
└──────────────────────────────────────────────────────────────────────────────────┘
```

---

### 3.1 Refinamiento 1: Ciclo de Vida de Cotizaciones y Diseños Personalizados

#### Flujo de Solicitud de Diseño
1. Al agendar un servicio con precio base variable (ej. *Uñas Acrílicas — Desde $25.00*), la clienta puede activar la opción **"Tengo un diseño en mente"**.
2. Sube entre **1 y 3 imágenes** de referencia (alojadas en el bucket privado `disenos-referencias/{uid}/`) y redacta notas complementarias.
3. La cita se registra inmediatamente en estado **`PendingQuote` (Pendiente de Valoración)**, apartando provisionalmente el slot en la agenda.
4. El Administrador recibe la alerta urgente en su Dashboard, evalúa la complejidad, asigna el precio formal y redacta el detalle del trabajo incluido, pasando la cita a **`QuoteProposed` (Cotización Propuesta)**.
5. La clienta recibe una notificación push y visualiza una tarjeta interactiva en la app con las opciones: **"Aceptar Cotización"**, **"Continuar en Chat"** o **"Rechazar"**.

#### Casos Borde y Reglas Críticas:
* **Aceptación:** Al pulsar "Aceptar", la cita pasa de inmediato a **`Confirmed` (Confirmada)**, fijando el precio pactado en el resumen de cobro.
* **Rechazo de Cotización:** Si la clienta pulsa "Rechazar", la app despliega un diálogo de confirmación:
  * *Opción A:* "Deseo negociar en el chat" $\rightarrow$ La cita permanece en `QuoteProposed` y la redirige al chat de la cita para acordar ajustes con recepción.
  * *Opción B:* "Confirmar rechazo" $\rightarrow$ La cita pasa automáticamente a **`Cancelled` (Cancelada)** con motivo *"Cotización no aceptada por el cliente"*, liberando el slot horario de inmediato en la agenda.
* **Expiración Automática de Cotización por Tiempo Límite:**
  * Si faltan menos de **2 horas** para la hora pactada de la cita y la cotización sigue en estado `PendingQuote` o `QuoteProposed` sin haber sido aceptada por la clienta:
  * El backend de C# (mediante un job en background o al evaluar disponibilidad) transiciona la cita a **`Cancelled`** por expiración de tiempo.
  * Se libera la franja horaria para no bloquear la estación ni el turno del profesional.
  * Se notifica a la clienta y al administrador de la cancelación automática.

---

### 3.2 Refinamiento 2: Base de Datos y Manejo de Clientes Espontáneos (*Walk-in*)

#### Regla de Aislamiento y Flexibilidad en PostgreSQL
Para registrar clientes que llegan directamente al salón sin reserva previa, **NO se les obliga a crear una cuenta en Supabase Auth**.

#### Ajustes en el Esquema de Base de Datos:
1. **Tabla `Clientes`:**
   * La columna `id_usuario` debe ser **`NULLABLE` (`uuid NULL`)**.
   * Se agrega la columna `es_walkin` (`boolean NOT NULL DEFAULT false`).
   * Para un walk-in, se inserta una fila en `Clientes` con `id_usuario = NULL`, `nombre_completo` y `telefono`.
2. **Vinculación Retrospectiva Inteligente:**
   * Si en el futuro esa persona descarga la app móvil y se registra en Supabase Auth con el mismo número telefónico o correo:
   * El trigger de autenticación o el servicio de registro en la Web API busca coincidencias previas en `Clientes` donde `id_usuario IS NULL`.
   * En caso de coincidencia, asocia el nuevo `id_usuario` (`auth.uid()`) a ese registro de `Clientes`, unificando su historial previo de servicios y notas sin duplicar entidades.

---

### 3.3 Refinamiento 3: Gestión Integral de Puntos de Fidelidad (*Loyalty System*)

#### Reglas de Acumulación y Canje
* **Acumulación:** La clienta gana **1 punto por cada $1.00 USD** efectivamente pagado. La acreditación de puntos ocurre de forma automática y transaccional en el backend en el instante en que la cita pasa al estado **`Completed`**.
* **Canje / Redención:** **100 puntos = $5.00 USD de descuento** aplicable directamente sobre el subtotal de servicios al momento de agendar o liquidar el servicio.
* **Vigencia:** Los puntos tienen una caducidad de **12 meses** desde su acreditación si no se registran nuevas citas completadas en el salón.

#### Manejo de Reversiones y Cancelaciones:
* **Cancelación Válida (>= 4 horas):** Si una cita confirmada donde se aplicó un descuento de puntos es cancelada dentro del tiempo permitido, el backend **reembolsa automáticamente el 100% de los puntos canjeados** al saldo del cliente, registrando el movimiento en `transacciones_puntos` con tipo `ReembolsoCancelacion`.
* **Inasistencia (`NoShow`):** Si la clienta no asiste a su cita, los puntos utilizados como descuento **NO se devuelven**, funcionando como penalización operativa por bloqueo de agenda.
* **Cálculo Financiero:** El descuento por puntos se deduce del subtotal antes de aplicar el IVA final (13% en comprobante formal):
  $$\text{Subtotal Neto} = \text{Subtotal Servicios} - \text{Descuento Puntos}$$
  $$\text{Total Final} = \text{Subtotal Neto} + \text{IVA (13\%)}$$

---

### 3.4 Refinamiento 4: Reseñas y Reputación Post-Servicio (*Reviews*)

#### Reglas de Calificación Multicriterio
* **Disparador:** Al cambiar el estado de la cita a `Completed`, el sistema genera una invitación no obligatoria para evaluar la experiencia.
* **Criterios (1 a 5 estrellas):**
  1. Experiencia General
  2. Calidad del Trabajo
  3. Atención del Personal
  4. Ambiente e Instalaciones del Salón
  * *Comentario abierto:* Opcional (hasta 500 caracteres).

#### Casos Borde y Vigencia:
* **Ventana de Calificación (14 días):** La clienta tiene un plazo máximo de **14 días naturales** posteriores a la finalización de la cita para emitir su reseña. Transcurrido ese lapso, la solicitud caduca y desaparece de pendientes para evitar reseñas descontextualizadas.
* **Inmutabilidad:** Una vez enviada la reseña, es de **solo lectura** (no editable ni eliminable por el cliente) para preservar la integridad de las métricas.
* **Visibilidad Pública:** Los promedios calculados y las opiniones verificadas se muestran públicamente en:
  * La ficha de cada servicio en el catálogo.
  * El portafolio/perfil de la estilista que realizó el servicio.

---

### 3.5 Refinamiento 5: Chat Interno Contextual por Cita e Identidad del Salón

#### Identidad y Reglas de Envío
* **Remitente Oficial del Salón:** Dado que el personal (*Staff*) no posee credenciales individuales en el sistema, todos los mensajes enviados por el personal administrativo o recepción se presentan en la interfaz de .NET MAUI bajo la insignia comercial:  
  🏷️ **"Shunshine Studio (Recepción)"** con el isotipo oficial del salón.
* **Ciclo de Vida y Modo Solo Lectura (Read-Only):**
  * **Citas Activas (`PendingQuote`, `QuoteProposed`, `Confirmed`, `InProgress`):** Chat bidireccional completamente habilitado.
  * **Cita `Completed`:** El chat permanece activo durante **24 horas** para coordinar cuidados inmediatos o dudas. Cumplidas las 24h, pasa a modo **Solo Lectura** permanente.
  * **Cita `Cancelled` o `NoShow`:** El chat pasa a modo **Solo Lectura de forma inmediata** al cambiar el estado.
* **Soporte Multimedia:** Se permite adjuntar hasta 3 fotos por mensaje con un límite de **5 MB** por archivo en formato comprimido (JPG, PNG, WebP).

---

### 3.6 Refinamiento 6: Estandarización de Supabase Storage (6 Buckets Oficiales)

Se fija la nomenclatura definitiva y las directivas de seguridad para los **6 buckets** de almacenamiento en Supabase:

| Bucket | Finalidad y Contenido | Tipo de Acceso | Política de Escritura (Upload/Delete) | Formatos y Límite |
| :--- | :--- | :--- | :--- | :--- |
| **`servicios-imagenes`** | Fotografías de tratamientos del catálogo | Público (`anon` lectura) | Solo Administrador (`role = Admin`) | JPG/PNG/WebP (5 MB) |
| **`categorias-imagenes`** | Íconos y banners de categorías | Público (`anon` lectura) | Solo Administrador (`role = Admin`) | JPG/PNG/WebP (2 MB) |
| **`personal-avatares`** | Fotos de perfil de estilistas/manicuristas | Público (`anon` lectura) | Solo Administrador (`role = Admin`) | JPG/PNG/WebP (2 MB) |
| **`personal-portafolio`** | Galería de trabajos realizados por el staff | Público (`anon` lectura) | Solo Administrador (`role = Admin`) | JPG/PNG/WebP (5 MB) |
| **`productos-imagenes`** | Fotos de productos del inventario del salón | Público (`anon` lectura) | Solo Administrador (`role = Admin`) | JPG/PNG/WebP (5 MB) |
| **`disenos-referencias`** | Referencias enviadas por clientas para cotizar | Privado (Propietaria + Admin) | Solo Cliente dueña (`/uid/*`) y Admin | JPG/PNG/WebP (5 MB) |

---

### 3.7 Refinamiento 7: Políticas de Cancelación y Concurrencia Transaccional

* **Ventana Autónoma de Cancelación:** La clienta puede cancelar o reprogramar su cita desde la aplicación móvil con un mínimo de **4 horas de anticipación** a la hora de inicio pactada.
* **Cancelaciones Tardías (< 4 horas):** La app deshabilita el botón de cancelación directa y despliega un aviso informando que debe comunicarse vía telefónica o chat con recepción.
* **Algoritmo de Asignación Automática ("No tengo preferencia"):**
  * La Web API de C# filtra a los miembros del personal activos y capacitados para el servicio solicitado.
  * Cruza las agendas para descartar quienes tengan citas solapadas o bloqueos de horario en esa franja.
  * Como criterio de balanceo de carga, asigna al profesional que tenga la **menor cantidad de citas agendadas en ese día**.
  * Toda la operación se ejecuta bajo una transacción con aislamiento en base de datos para prevenir colisiones concurrentes.

---

## 4. Resumen de Entidades Afectadas en Base de Datos

| Entidad / Tabla | Naturaleza del Cambio | Campos / Relaciones Nuevas |
| :--- | :--- | :--- |
| **`usuarios`** | Restricción de Roles | Solo admite roles `Administrador` y `Cliente`. Se elimina rol Estilista como usuario autenticado. |
| **`clientes`** | Soporte Walk-in | `id_usuario` pasa a ser **NULLABLE**, `es_walkin` (boolean), `puntos_acumulados` (int). |
| **`estilistas`** | Entidad de Catálogo | Se desvincula de `usuarios`. Administrada 100% por Admin. |
| **`servicios`** | Extensión Funcional | `precio_es_variable` (bool), `intervalo_seguimiento_dias` (int, default 21). |
| **`productos`** | Nueva Tabla | `id_producto`, `nombre`, `marca`, `precio`, `stock`, `imagen_url`, `activo`. |
| **`citas`** | Nuevos Estados | Estados: `PendingQuote`, `QuoteProposed`, `Confirmed`, `InProgress`, `Completed`, `Cancelled`, `NoShow`. |
| **`solicitudes_diseno`** | Nueva Tabla | `id_solicitud`, `id_cita`, `imagenes_url` (jsonb), `notas_instruccion`, `estado`. |
| **`cotizaciones`** | Nueva Tabla | `id_cotizacion`, `id_solicitud`, `precio_propuesto`, `descripcion_trabajo`, `estado`. |
| **`mensajes_chat_cita`** | Nueva Tabla | `id_mensaje`, `id_cita`, `id_emisor`, `mensaje`, `imagenes_adjuntas`, `fecha_envio`. |
| **`resenas`** | Nueva Tabla | `id_resena`, `id_cita`, `id_cliente`, `id_estilista`, `estrellas_general`, `estrellas_calidad`, `estrellas_atencion`, `estrellas_ambiente`, `comentario`. |
| **`transacciones_puntos`** | Nueva Tabla | `id_transaccion_puntos`, `id_cliente`, `id_cita`, `puntos`, `tipo_movimiento` (Acumulacion, Canje, Reembolso), `fecha`. |
| **`servicio_productos_rec`** | Nueva Tabla | Relación N:M para vincular servicio con productos recomendados post-tratamiento. |
| **`personal_portafolios`** | Nueva Tabla | `id_portafolio`, `id_estilista`, `imagen_url`, `descripcion_trabajo`. |
| **`favoritos`** | Nueva Tabla | `id_favorito`, `id_cliente`, `tipo_entidad` (Servicio, Producto, Estilista), `id_referencia`. |
| **`auditoria_logs`** | Estructuración | `id_auditoria`, `id_usuario`, `entidad_afectada`, `accion`, `valor_anterior`, `valor_nuevo`, `motivo`, `fecha_hora`. |

---

## 5. Próximos Pasos para la Fase de Desarrollo

1. **Actualización de Especificación SDD (`spec.md`, `data-model.md`, `openapi.yaml`):** Aplicar estos 7 refinamientos y los contratos REST correspondientes.
2. **Actualización de Base de Datos (`DIAGRAMA_BASE_DE_DATOS.md` y `MODELO_BASE_DE_DATOS.sql`):** Generar los scripts DDL en PostgreSQL con las tablas, llaves foráneas, triggers y los 6 buckets con sus políticas RLS.
3. **Refinamiento de Product Backlog y Sprints (`PRODUCT_BACKLOG.md`):** Reorganizar las historias y tareas para Alex Alfaro y Camila Calderón en Azure Boards.
