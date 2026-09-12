# DOCUMENTO DE EVOLUCIÓN, AMPLIACIÓN Y DEFINICIÓN FUNCIONAL — SHUNSHINE STUDIO

## Comparación entre la propuesta inicial y la versión ampliada del sistema

> **Proyecto:** Shunshine Studio
> **Tipo de documento:** Documentación oficial de evolución del sistema
> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)
> **Equipo:**
> - **Alex Fernando Alfaro Diaz** — Backend, Base de Datos, Scrum Master
> - **Camila Antonia Calderon Cortez** — Frontend Móvil, UI/UX, QA y Testing
>
> **Propósito:** Documentar con exactitud qué existía en la propuesta inicial y qué se agregó posteriormente, explicando en cada caso el problema que resuelve, cómo funciona y cómo se relaciona con el resto del sistema.

---

## Índice

1. Resumen Ejecutivo de la Evolución
2. Comparación General por Área
3. Evolución por Funcionalidad (21 módulos)
4. Flujo Completo del Cliente (versión actual)
5. Flujo Completo del Administrador (versión actual)
6. Relación entre Módulos del Sistema
7. Modelo Conceptual de Entidades
8. Arquitectura Técnica
9. Modelo de Roles y Actores
10. Supabase Storage
11. Seguridad y Reglas de Negocio Críticas
12. Lo que Cambió con Respecto a la Idea Original
13. Funcionalidades que Más Cambiaron el Proyecto
14. Principios que Deben Mantenerse
15. Matriz Final de Evolución
16. Objetivo Final del Sistema

---

## 1. Resumen Ejecutivo de la Evolución

### El punto de partida

Shunshine Studio comenzó como una aplicación móvil con un objetivo claro y acotado:

> **Permitir a las clientas de un salón de belleza reservar citas de manera digital.**

La propuesta original contemplaba: registro de cuenta, inicio de sesión, catálogo de servicios, selección de profesional, selección de fecha y hora, resumen de la reserva, confirmación, historial de citas, dashboard del día, agenda interactiva y mantenimiento básico del catálogo.

### El problema que dejaba sin resolver

A medida que se analizó el funcionamiento real de un salón, quedaron preguntas sin respuesta:

- ¿Qué ocurre cuando el servicio tiene precio variable según el diseño solicitado?
- ¿Cómo se comunica la cliente con el salón sin salir a otra aplicación?
- ¿Qué pasa después de que la cita se completa? ¿Termina la relación?
- ¿Cómo sabe el salón cuáles servicios son más populares o cuál profesional tiene mejor calificación?
- ¿Cómo se retiene a una clienta que visitó el salón hace un mes?

Ninguna de estas preguntas tenía respuesta en la propuesta original.

### La evolución

El sistema evolucionó para convertirse en:

> **Una plataforma móvil integral que gestiona la relación entre cliente y salón antes, durante y después de cada servicio.**

El recorrido completo es ahora:

`
CLIENTE
  → Descubre servicios y productos
  → Explora portafolios del personal
  → Reserva un horario
  → Envía diseño de referencia (si aplica)
  → Recibe valoración del precio de Admin
  → Acepta o rechaza el precio propuesto
  → Elige método de pago
  → Acude al salón
  → Se atiende y cierra la cita
  → Recibe solicitud de reseña (opcional)
  → Acumula puntos de fidelidad
  → Consulta su historial de belleza
  → Recibe recordatorio de retoque según el servicio
  → Recibe recomendaciones de productos relacionados
  → Vuelve a reservar
`

---

## 2. Comparación General por Área

| Área | Antes | Ahora | Motivo de ampliación |
|:---|:---|:---|:---|
| Reservas | Sí, flujo básico | Flujo ampliado con cotización y estado de pago | Mejor control del proceso completo |
| Catálogo de servicios | Parcialmente estático | 100% administrable en tiempo real | Evitar cambios en código para actualizar precios |
| Personal (Staff) | Selección básica | Entidad administrable, portafolio, asignación automática | Eliminar complejidad de roles de usuario |
| Inventario / Productos | NO EXISTÍA | Sí, catálogo completo con stock | El salón administra y puede vender productos |
| Solicitudes de diseño | NO EXISTÍA | Sí, flujo formal imagen + notas + cotización | Resolver servicios con precio variable |
| Cotizaciones / Valoración | NO EXISTÍA | Sí, precio propuesto por Admin con aceptación | Evitar conflictos de precio |
| Chat interno | NO EXISTÍA | Sí, hilo contextual dentro de la cita | Comunicación dentro del sistema sin WhatsApp |
| Notificaciones | Básicas (cita agendada) | Sistema segmentado y ampliado | Seguimiento inteligente de la relación |
| Reseñas | NO EXISTÍA | Sí, calificación post-servicio no obligatoria | Medir experiencia y reputación del personal |
| Historial | Lista de citas | Mi Historial de Belleza completo | Línea de tiempo de la relación clienta-salón |
| Seguimiento post-servicio | NO EXISTÍA | Sí, basado en intervalos configurables | Aumentar frecuencia de visitas |
| Recomendaciones | NO EXISTÍA | Sí, basadas en reglas configurables | Conectar servicios con productos |
| Favoritos | NO EXISTÍA | Sí, para servicios, productos y personal | Personalización y acceso rápido |
| Fidelización / Puntos | NO EXISTÍA | Sí, sistema de puntos por niveles | Retención de clientas frecuentes |
| Pagos | Referencia básica | Método + estado + monto + trazabilidad | Control financiero real del negocio |
| Walk-in clients | Sin flujo formal | Flujo formal en agenda del día | Gestionar atención presencial sin cita previa |
| Dashboard admin | Métricas básicas | KPIs completos: finanzas, calidad, ocupación | Soporte para decisiones de negocio |
| Reportes | Básicos | Ampliados por área, exportables PDF/Excel | Análisis profundo del negocio |
| Auditoría | NO EXISTÍA | Sí, registro de cambios críticos | Trazabilidad y transparencia |
| Asignación automática personal | NO EXISTÍA | Sí, con balanceo de carga | Distribución equitativa del trabajo |
| Portafolio del personal | NO EXISTÍA | Sí, galería de trabajos | Mostrar calidad antes de reservar |

---

## 3. Evolución por Funcionalidad

---

### 3.01 — Sistema de Reservas y Máquina de Estados

**Estado anterior:** El flujo de reserva ya existía. El cliente seleccionaba servicio, profesional, fecha y hora, veía un resumen y confirmaba de forma lineal con precio fijo asumido.

**Nueva incorporación:** El flujo se amplía para incluir:
- Envío de diseños de referencia con estado "Pendiente de Valoración" (`PendingQuote`).
- Selección de método de pago en recepción.
- Verificación doble de disponibilidad en backend (control de concurrencia transaccional con bloqueos ACID).
- Máquina formal de estados del ciclo de vida de la cita.
- Políticas estrictas de cancelación y reagendamiento.

**¿Por qué?** El flujo original asumía precio fijo siempre. En la práctica muchos servicios varían según la complejidad. Sin verificación de concurrencia, dos clientas podrían reservar el mismo slot simultáneamente. Además, sin una máquina de estados clara, no es posible auditar el progreso ni gestionar ausencias o cancelaciones.

#### Máquina Oficial de Estados de la Cita

| Estado Backend (C# / BD) | Estado Visible (UI Flutter) | Descripción y Transición | Actor que la ejecuta |
|:---|:---|:---|:---|
| `PendingQuote` | Pendiente de Valoración | Cita creada con diseño de referencia. En espera de propuesta de precio. | Cliente al agendar |
| `QuoteProposed` | Cotización Propuesta | Admin evaluó las referencias y propuso un precio formal. En espera de respuesta. | Admin |
| `Confirmed` | Confirmada | Cita agendada formalmente con slot reservado y precio pactado (directo o tras aceptar cotización). | Sistema / Cliente |
| `InProgress` | En Atención | La clienta se presentó al salón y el servicio está siendo realizado. | Admin |
| `Completed` | Completada | Servicio finalizado con éxito. Habilita reseñas, puntos de fidelidad y recomendaciones. | Admin |
| `Cancelled` | Cancelada | Cita anulada con motivo obligatorio. Libera el horario de inmediato. | Cliente o Admin |
| `NoShow` | No Asistió | La clienta no se presentó en su horario reservado. | Admin |

#### Política de Cancelaciones y Reagendamientos
- **Ventana autónoma:** La clienta puede cancelar o solicitar reagendamiento desde la app móvil con un mínimo de **4 horas de anticipación** a la hora pactada.
- **Cancelación tardía (< 4 horas):** La aplicación bloquea la cancelación directa y solicita contactar al salón vía chat interno o llamada para coordinar la contingencia.
- **Liberación inmediata de slot:** Tan pronto como una cita pasa al estado `Cancelled`, el backend libera el slot horario en la agenda de forma automática, dejándolo visible para otras reservas o para clientes espontáneos (*walk-in*).

**¿Cómo funciona el flujo de reserva?**

```
Selección de servicio
  ↓
¿El cliente tiene referencia/diseño?
  NO → Precio base definido → Flujo estándar
  SÍ → Cita: PendingQuote (Pendiente de Valoración)
         → Admin evalúa y cotiza (QuoteProposed)
         → Cliente acepta o rechaza
         → Si acepta → Cita: Confirmed (Confirmada)
```

**¿Quién usa?** Cliente (inicia), Admin (cotiza, inicia atención, completa o cancela), Sistema (control de concurrencia y liberación de slots).

**Relación:** Solicitudes de Diseño, Cotizaciones, Chat, Pagos, Auditoría.

---

### 3.02 — Catálogo de Servicios

**Estado anterior:** Existía. Categorías fijas con servicios predefinidos.

**Nueva incorporación:** 100% administrable en tiempo real. Se agrega:
- Precio base con sufijo "desde " para servicios con precio variable.
- Intervalo de seguimiento sugerido por servicio (en días), usado para recordatorios de retoque.
- Relación configurable: servicio → productos recomendados.

**¿Por qué?** Si cambiar un precio requiere modificar código fuente, el sistema no es práctico para un negocio real. El catálogo debe adaptarse al ritmo del salón, no al de los desarrolladores.

**¿Cómo funciona?** Admin gestiona el catálogo sin restricciones. El campo duracion_minutos es usado por el backend para calcular disponibilidad real. El intervalo de seguimiento activa recordatorios post-servicio.

**¿Quién usa?** Admin (gestiona), Cliente (consulta), Sistema (usa duración e intervalo para cálculos automáticos).

**Relación:** Personal, Reservas, Solicitudes de Diseño, Seguimiento Post-Servicio, Recomendaciones.

---

### 3.03 — Personal del Salón (Staff)

**Estado anterior:** La selección de estilista existía. Su naturaleza dentro del sistema no estaba bien definida.

**DECISIÓN ARQUITECTÓNICA FUNDAMENTAL:**

> El personal NO son usuarios autenticados. Son registros de catálogo administrados por Admin.

Esto significa que el personal NO tiene cuenta, contraseña, login, dashboard ni permisos. Admin puede agregar o desactivar miembros del personal sin modificar la arquitectura de autenticación.

Cada miembro puede tener: nombre, cargo, fotografía, portafolio (galería de trabajos), especialidades, servicios asignados, horarios semanales, bloqueos excepcionales y estado (Activo / Inactivo temporal).

**¿Por qué?** Incorporar al personal como usuarios autenticados generaría complejidad innecesaria: contraseñas, recuperación de acceso, sesiones paralelas y gestión de roles adicionales. Operacionalmente son una entidad de catálogo y de agenda.

**¿Quién usa?** Admin (gestiona el equipo), Cliente (ve perfiles y portafolio durante la reserva), Sistema (consulta horarios para calcular disponibilidad).

**Relación:** Reservas, Disponibilidad, Reseñas, Reportes, Asignación Automática.

---

### 3.04 — Asignación Automática de Personal

**ANTES NO EXISTÍA. El cliente debía seleccionar obligatoriamente un profesional.**

**Nueva incorporación — Tres modalidades:**

**A. Cliente elige:** Selecciona a su profesional de confianza. El sistema consulta esa agenda específica.

**B. "No tengo preferencia" (opción por defecto):**
El sistema busca automáticamente un profesional que cumpla:
- Capacitado para el servicio seleccionado.
- Con el bloque horario libre disponible.
- Criterio de desempate: el profesional con menor cantidad de citas asignadas en el día (balanceo de carga).

**C. Asignación manual por Admin:** Admin crea la cita directamente y asigna al profesional de su elección.

**¿Por qué?** Muchas clientas priorizan el horario sobre la persona. Obligarlas a elegir agrega fricción innecesaria. La asignación automática también distribuye el trabajo de manera más equitativa entre el equipo.

**¿Cómo funciona?** La API C# filtra profesionales habilitados para el servicio, cruza horarios semanales, descuenta bloqueos y reservas existentes, y retorna el primero disponible según carga del día.

**¿Quién usa?** Cliente (elige modalidad), Admin (puede hacer asignación manual), Sistema (ejecuta el algoritmo).

**Relación:** Reservas, Horarios del Personal, Disponibilidad, Walk-in Clients.

---

### 3.05 — Inventario y Catálogo de Productos

**ANTES NO EXISTÍA UN MÓDULO FORMAL DE INVENTARIO NI CATÁLOGO DE PRODUCTOS.**

La propuesta original estaba exclusivamente centrada en servicios.

**Nueva incorporación:** Módulo de productos diferenciado del catálogo de servicios.

Ejemplos de productos: shampoos y acondicionadores profesionales, mascarillas capilares, tratamientos reconstructores, aceites y serums, esmaltes y productos para uñas, kits de cuidado personal, accesorios, cosméticos.

Cada producto puede tener: nombre, descripción, marca, fotografías, categoría, precio, stock, estado de disponibilidad (Disponible / Agotado) y estado de catálogo (Activo / Inactivo).

**¿Por qué?** Un salón no solo presta servicios. También puede vender o administrar productos. La relación entre "servicio prestado" y "productos recomendados para cuidado en casa" es natural en la industria de belleza.

**Fases de implementación:**
- Fase 1 (inmediata): Catálogo informativo conectado a favoritos y recomendaciones post-servicio.
- Fase 2 (arquitectura preparada, no implementada inicialmente): Carrito, pedido y pago con recogida en salón.

**¿Quién usa?** Admin (gestiona catálogo y stock), Cliente (consulta y marca favoritos), Sistema (genera recomendaciones basadas en el servicio recibido).

**Relación:** Catálogo de Servicios, Recomendaciones, Favoritos, Historial de Belleza, Reportes.

---

### 3.06 — Solicitudes de Diseño y Personalización

**ANTES NO EXISTÍA UN FLUJO FORMAL DE SOLICITUD DE DISEÑOS PERSONALIZADOS.**

**Nueva incorporación:** Módulo completo para servicios con precio variable según complejidad.

La clienta puede:
1. Seleccionar un servicio con precio base variable (ej. Uñas Acrílicas — Desde ).
2. Indicar que tiene un diseño en mente.
3. Subir entre 1 y 3 fotos de referencia a Supabase Storage.
4. Escribir notas de instrucción (ej. "El mismo diseño pero en tonos pastel con uñas más cortas").

**¿Por qué?** El mismo servicio puede costar  o  según complejidad. Sin un mecanismo formal, el salón resuelve cada caso por WhatsApp o llamada, generando conflictos y pérdidas sin trazabilidad.

**¿Cómo funciona?**

`
Cliente activa "Tengo un diseño en mente"
  ↓
Sube fotos + escribe instrucciones
  ↓
Cita: PENDIENTE DE VALORACIÓN
  ↓
Alerta urgente en Dashboard de Admin
  ↓
Admin revisa imágenes y analiza complejidad
  ↓
Admin envía cotización: precio propuesto + descripción incluida
  ↓
Cliente recibe notificación push
  ↓
Cliente ACEPTA o RECHAZA dentro de la app
`

**¿Quién usa?** Cliente (envía diseño), Admin (cotiza), Sistema (gestiona estados y genera notificaciones).

**Relación:** Reservas, Cotización Dinámica, Chat Interno, Pagos, Notificaciones, Auditoría.

---

### 3.07 — Valoración y Cotización Dinámica de Precio

**ANTES NO EXISTÍA EL CONCEPTO FORMAL DE VALORACIÓN DEL SERVICIO PERSONALIZADO.**

**Nueva incorporación:** Flujo formal de cotización donde Admin propone un precio específico y la clienta lo acepta o rechaza antes de confirmar la cita.

**¿Por qué?** Un salón que no puede cotizar correctamente pierde dinero en trabajos complejos o pierde clientas al cobrar más de lo que anunciaron. La cotización deja todo registrado y acordado formalmente.

**¿Cómo funciona?**

Admin recibe la solicitud y registra:
- Precio propuesto.
- Descripción del trabajo incluido (ej. "Diseño degradado, pedrería en 3 dedos y acabado cromado").

La clienta recibe notificación y encuentra una tarjeta interactiva. Puede: Aceptar precio, Rechazar, o continuar conversando en el chat de la cita.

**Regla fundamental:** El sistema NO cobra automáticamente porque se subió una imagen. La imagen es una solicitud de valoración. El precio queda definido SOLO cuando Admin cotiza y la clienta acepta.

**¿Quién usa?** Admin (cotiza y propone), Cliente (acepta o rechaza), Sistema (gestiona estado y notifica).

**Relación:** Solicitudes de Diseño, Chat Interno, Reservas, Pagos, Notificaciones, Auditoría.

---

### 3.08 — Cambios de Precio con Auditoría

**ANTES NO EXISTÍA UN MECANISMO FORMAL PARA REGISTRAR CAMBIOS DE PRECIO EN UNA CITA.**

**Nueva incorporación:** Si durante la atención la clienta solicita trabajo adicional, Admin puede modificar el precio final con condiciones obligatorias:
- Motivo del cambio es requerido.
- El sistema registra automáticamente: precio anterior, precio nuevo, usuario, fecha y hora.

Ejemplo:
- Precio acordado: .00
- Cambio solicitado: encapsulado adicional en 3 dedos
- Precio final: .00
- Motivo registrado: "Encapsulado adicional solicitado por la clienta durante la atención"

**¿Por qué?** Los cambios de precio en salón son inevitables. Sin registro formal se pierde trazabilidad y se generan conflictos sobre lo que se acordó.

**¿Quién usa?** Admin (realiza el ajuste con motivo), Sistema (registra en auditoría), Cliente (ve el precio final en su historial de la cita).

**Relación:** Auditoría, Pagos, Reservas.

---

### 3.09 — Chat Interno Contextual por Cita

**ANTES NO EXISTÍA UN SISTEMA DE CHAT INTERNO.**

Cualquier comunicación entre la clienta y el salón ocurría fuera de la aplicación (WhatsApp, llamada o en persona).

**Nueva incorporación:** Canal de comunicación directa entre Cliente y Admin ubicado dentro del detalle de cada cita. No es una red social ni mensajería abierta. Es un hilo contextual específico de cada reserva.

Capacidades:
- Mensajes de texto.
- Fotos adicionales de referencia.
- Mensajes automáticos del sistema sobre cambios de estado.
- Tarjeta interactiva de cotización incrustada (aceptar o rechazar sin salir del chat).

#### Ciclo de Vida y Reglas del Chat
- **Apertura automática:** Se habilita inmediatamente cuando se crea la cita (crucial para `PendingQuote`).
- **Fase Activa:** Permite envío bidireccional de mensajes y fotos entre Cliente y Admin mientras la cita esté en estado `PendingQuote`, `QuoteProposed`, `Confirmed` o `InProgress`.
- **Cierre y Modo Solo Lectura (Read-Only):**
  - Al marcarse la cita como `Completed`, el chat permanece activo durante **24 horas** para resolver dudas inmediatas posteriores al servicio. Cumplido ese plazo, el chat pasa automáticamente a modo **Solo Lectura** (auditoría e histórico inmutable).
  - Si la cita pasa a `Cancelled` o `NoShow`, el chat se cierra a modo Solo Lectura de forma inmediata para evitar sobrecarga operativa y desvíos fuera de citas vigentes.

**¿Por qué?** Sin canal interno, la clienta con dudas sobre su diseño o precio debe salir de la app y resolver en otra plataforma. Esto convierte a Shunshine Studio en una herramienta incompleta que depende de WhatsApp para funcionar. Además, limitar su ciclo de vida evita que el chat se use como canal de soporte indefinido y centraliza la atención en citas activas.

**¿Cómo funciona?**

Acceso desde: Mis Citas → Cita #SHU-XXXX → Conversación sobre mi cita

Ejemplo de conversación:

| Remitente | Mensaje |
|:---|:---|
| Cliente | "Quiero este diseño en tonos pastel" + imagen |
| Sistema | "Tu referencia fue recibida. Nuestro equipo la revisará." |
| Admin | "El precio propuesto es $35.00. Incluye pedrería y acabado crema." |
| Cliente | "¿Las piedras son Swarovski?" |
| Admin | "Sí, en los dedos índice y anular." |
| Cliente | (Acepta cotización desde tarjeta interactiva) |
| Sistema | "Precio aceptado. Tu cita ha sido confirmada." |

**¿Quién usa?** Cliente (plantea dudas, acepta cotizaciones), Admin (responde y gestiona cotizaciones), Sistema (mensajes automáticos y cierre a solo lectura).

**Relación:** Solicitudes de Diseño, Cotizaciones, Notificaciones, Reservas.

---

### 3.10 — Centro de Notificaciones Ampliado

**Estado anterior:** Básicas (confirmación de cita, recordatorio cercano).

**Nueva incorporación:** Sistema segmentado por categoría:

| Categoría | Notificaciones |
|:---|:---|
| Citas | Confirmación, recordatorio 24h, recordatorio 2h, reprogramación, cancelación |
| Diseños y Cotizaciones | Aviso cuando Admin valora el precio, respuestas en el chat de la cita |
| Post-servicio | Solicitud de reseña al completar, recordatorio de reseña pendiente |
| Seguimiento y Belleza | Recordatorio de retoque según intervalo configurado del servicio |
| Catálogo y Promociones | Nuevos servicios, productos, promociones del salón |

**Distinción fundamental:**

| Concepto | Qué es | Quién lo genera |
|:---|:---|:---|
| Notificación | Mensaje automático del sistema sobre un evento | El sistema automáticamente |
| Chat | Conversación activa entre personas | Cliente o Admin directamente |

**¿Por qué?** Las notificaciones son el principal mecanismo para mantener la relación con la clienta entre visita y visita. Sin ellas, la aplicación es pasiva y la clienta puede olvidarse del salón.

**¿Quién usa?** Sistema (genera la mayoría automáticamente), Admin (puede enviar sobre catálogo y promociones), Cliente (recibe y puede configurar preferencias).

---

### 3.11 — Reseñas y Calificaciones

**ANTES NO EXISTÍA EL MÓDULO DE RESEÑAS.**

**Nueva incorporación:** Cuando una cita pasa a estado Completed, el sistema genera automáticamente una solicitud de reseña. La evaluación NO es obligatoria.

Opciones para la clienta:
- Evaluar ahora.
- Recordar más tarde (queda pendiente con recordatorio).
- Omitir (no se vuelve a preguntar).

Información evaluable:

| Aspecto | Tipo |
|:---|:---|
| Experiencia general | 1 a 5 estrellas |
| Calidad del trabajo | 1 a 5 estrellas |
| Atención del personal | 1 a 5 estrellas |
| Ambiente del salón | 1 a 5 estrellas |
| Comentario abierto | Texto opcional |

Cada reseña queda vinculada a: la clienta, la cita específica, el servicio recibido y el miembro del personal que realizó la atención.

**¿Por qué?** Completar una cita no debe ser el final de la relación. El sistema necesita conocer la experiencia para identificar fortalezas del equipo e informar a futuras clientas.

**¿Quién usa?** Cliente (califica voluntariamente), Admin (consulta en reportes y panel del personal), Sistema (genera la solicitud y programa el recordatorio si corresponde).

**Relación:** Ciclo de Vida de la Cita, Historial de Belleza, Reportes, Notificaciones.

---

### 3.12 — Historial de Belleza

**Estado anterior:** Lista de citas anteriores con detalles básicos (servicio, fecha, estado).

**Nueva incorporación:** Evoluciona hacia "Mi Historial de Belleza": una línea de tiempo completa de la relación de la clienta con el salón.

Incluye:
- Tratamientos: servicio recibido, fecha, profesional que atendió, precio final pagado, diagnóstico o notas del servicio, reseña dejada.
- Productos: adquiridos en salón y recomendados después del servicio.
- Seguimientos: próximo retoque recomendado según intervalo del servicio.

**¿Por qué?** Una lista de citas pasadas tiene poco valor. Un historial detallado permite recordar qué se hizo, cuándo, con quién y qué resultó bien, convirtiendo al sistema en una herramienta de referencia personal.

**¿Quién usa?** Cliente (consulta su propio historial), Admin (puede consultarlo para brindar atención personalizada).

**Relación:** Reseñas, Seguimiento Post-Servicio, Recomendaciones, Fidelización.

---

### 3.13 — Seguimiento Post-Servicio

**ANTES NO EXISTÍA UN SISTEMA DE SEGUIMIENTO POSTERIOR PERSONALIZADO.**

La relación entre el sistema y la clienta terminaba cuando la cita se completaba.

**Nueva incorporación:** El sistema envía recordatorios de retoque basados en intervalos configurables por Admin según el servicio recibido.

Ejemplos de intervalos sugeridos:

| Servicio | Intervalo de retoque |
|:---|:---|
| Manicure semipermanente | 21 días |
| Uñas acrílicas (retoque) | 25-28 días |
| Corte de cabello | 35-45 días |
| Retoque de raíz o tinte | 28-35 días |
| Balayage o mechas | 45-60 días |
| Lifting de pestañas | 45-60 días |
| Tratamiento de hidratación | 21-30 días |

**Regla fundamental:** Los intervalos son CONFIGURABLES POR ADMIN. No están escritos directamente en el código de la aplicación.

**¿Cómo funciona?**

`
Cita pasa a COMPLETADA
  ↓
Sistema registra fecha y servicio realizado
  ↓
Calcula: fecha_completada + dias_seguimiento_del_servicio
  ↓
En esa fecha → notificación al cliente:
"Han pasado 21 días desde tu manicure.
 ¿Quieres agendar tu retoque? 💅"
`

**¿Por qué?** Una clienta satisfecha que no recibe recordatorio puede simplemente olvidar volver. El recordatorio oportuno es la forma más efectiva de aumentar la frecuencia de visitas sin inversión en publicidad adicional.

**¿Quién usa?** Sistema (genera el recordatorio), Cliente (lo recibe y puede reservar desde ahí), Admin (configura los intervalos de cada servicio).

**Relación:** Catálogo de Servicios, Notificaciones, Historial de Belleza, Reservas.

---

### 3.14 — Recomendaciones Basadas en Reglas

**ANTES NO EXISTÍA UN SISTEMA FORMAL DE RECOMENDACIONES.**

**Nueva incorporación:** Después de un servicio, el sistema recomienda productos y servicios complementarios mediante reglas configuradas por Admin.

Ejemplos:

| Servicio recibido | Productos recomendados |
|:---|:---|
| Hidratación capilar | Shampoo hidratante, Mascarilla nutritiva, Sérum reconstructor |
| Coloración / Tinte | Shampoo para cabello teñido, Protector del color |
| Manicure acrílico | Cuticle oil, Guantes de protección |
| Lifting de pestañas | Sérum para pestañas |

**Regla importante:** Las recomendaciones funcionan mediante reglas configurables. NO se requiere inteligencia artificial. Admin configura las relaciones: Servicio → Productos relacionados y Servicio → Servicios relacionados.

**¿Por qué?** La conexión entre servicio y productos de cuidado en casa es natural en la industria de belleza. El sistema puede capitalizar ese momento para la clienta (recomendación útil) y para el salón (oportunidad de venta).

**¿Quién usa?** Admin (configura relaciones entre servicios y productos), Sistema (ejecuta la recomendación al completar la cita), Cliente (recibe las sugerencias).

**Relación:** Catálogo de Servicios, Inventario de Productos, Historial de Belleza, Seguimiento, Notificaciones.

---

### 3.15 — Favoritos

**ANTES NO EXISTÍA EL MÓDULO DE FAVORITOS.**

**Nueva incorporación:** La clienta puede marcar con ❤️: servicios del catálogo, miembros del personal y productos.

El sistema los utiliza para:
- Mostrar atajos en la sección de perfil de la clienta.
- Personalizar el orden de resultados en el catálogo.
- Enriquecer las recomendaciones futuras.

**¿Por qué?** Facilita las futuras reservas de clientas frecuentes. Si siempre va con la misma manicurista y siempre pide el mismo servicio, el acceso rápido mejora la experiencia.

**¿Quién usa?** Cliente (gestiona favoritos), Sistema (los utiliza para personalizar la experiencia).

**Relación:** Catálogo de Servicios, Inventario de Productos, Personal, Recomendaciones.

---

### 3.16 — Fidelización y Puntos

**ANTES NO EXISTÍA UN SISTEMA DE FIDELIZACIÓN.**

**Nueva incorporación:** Sistema de acumulación de puntos, canje y niveles para incentivar la lealtad y retención de clientas.

#### Mecánica de Acumulación y Redención
- **Acumulación:** La clienta acumula **1 punto por cada $1.00 USD** pagado en servicios con estado `Completed`.
- **Canje / Redención:** **100 puntos equivalen a $5.00 USD de descuento directo** aplicable en el costo de servicios de estética o capilares al momento de reservar o pagar.
- **Vigencia:** Los puntos tienen una caducidad de **12 meses** desde su acreditación si no se registran nuevas citas en el salón.

#### Niveles y Beneficios

| Nivel | Puntos acumulados | Beneficios |
|:---|:---|:---|
| **Bronce** | 0 - 149 pts | Acceso a catálogo estándar y acumulación base (1 pt / $1). |
| **Plata** | 150 - 349 pts | 5% de descuento en tratamientos capilares seleccionados y recordatorios preferenciales. |
| **Oro** | 350+ pts | 10% de descuento en servicios + atención prioritaria + obsequio especial en el mes de cumpleaños. |

**¿Por qué?** Retener a una clienta existente es mucho más rentable que adquirir una nueva. El sistema de fidelización premia económicamente la recurrencia y consolida una relación a largo plazo.

**¿Quién usa?** Cliente (acumula, visualiza saldo y aplica descuentos), Admin (consulta nivel y ajusta parámetros), Sistema (acredita puntos automáticamente al marcar la cita como `Completed`).

**Relación:** Ciclo de Vida de la Cita, Pagos, Historial de Belleza, Reportes.

---

### 3.17 — Gestión de Pagos y Estados Financieros

**Estado anterior:** Solo referencia genérica al cobro en salón, sin módulo formal de control con estados y trazabilidad.

**Nueva incorporación:** Módulo financiero completo con:
- **Métodos de pago en salón:** Efectivo o Tarjeta (POS físico en recepción).
- **Monto de cobro:** Precio base o precio final cotizado y aceptado.
- **Trazabilidad con auditoría:** Si el monto cobrado al finalizar la atención difiere del precio cotizado previamente (por trabajos extra solicitados), Admin debe ingresar obligatoriamente un motivo justificado que queda registrado en `AuditLog`.

#### Estados Financieros del Pago

| Estado Backend | Estado UI | Significado |
|:---|:---|:---|
| `Pending` | Pendiente de Pago | La cita está confirmada o en curso, pendiente de cobro en recepción. |
| `Paid` | Pagado | Pago liquidado y recibido por Admin en recepción (Efectivo / POS). |
| `PartiallyPaid` | Parcialmente Pagado | Pago fraccionado o anticipo registrado en el salón. |
| `Refunded` | Reembolsado | Devolución registrada ante anulación de cobros previos. |

**Sobre pagos en línea:** En esta versión inicial NO se procesan pagos mediante pasarela web/móvil directa para mantener la simplicidad operativa del salón. La arquitectura queda 100% desacoplada y lista para conectar pasarelas como Stripe o Wompi en fases posteriores sin romper el modelo.

**¿Por qué?** Sin registro formal de pagos, el dashboard financiero no puede reflejar ingresos reales, no se detectan diferencias entre lo cotizado y lo cobrado, y los reportes contables carecen de validez.

**¿Quién usa?** Cliente (selecciona método preferente), Admin (cobra y valida en recepción), Sistema (actualiza estados y consolida reportes).

**Relación:** Reservas, Auditoría, Dashboard Administrativo, Reportes, Fidelización.

---

### 3.18 — Walk-in Clients (Clientes Espontáneos sin Cuenta)

**ANTES NO EXISTÍA COMO FLUJO FORMAL.**

**Nueva incorporación:** Proceso ágil en la agenda de Admin para registrar clientas que llegan físicamente al salón sin reserva previa.

#### Regla Arquitectónica de Walk-in (Sin Supabase Auth obligatorio)
Para una atención presencial inmediata, **NO se exige que la persona cree una cuenta en Supabase Auth ni que descargue la aplicación móvil**.
- Admin genera un registro express de `Client` en base de datos ingresando únicamente su **Nombre** y **Teléfono** (opcional).
- Esto permite registrar el servicio en menos de 30 segundos, ocupar el slot en la agenda y registrar el cobro formalmente.
- **Vinculación futura:** Si en el futuro esa clienta decide descargar la app móvil y registrarse con el mismo teléfono o correo, el backend asocia automáticamente su historial previo a su nueva cuenta de Supabase Auth.

```
Admin abre la agenda del día
  ↓
Identifica bloque horario libre (marcado como "+ Walk-in")
  ↓
Ingresa datos express de la clienta (Nombre y Teléfono)
  ↓
Selecciona servicio y personal disponible
  ↓
Registra método de pago y confirma cita (Confirmed / InProgress)
  ↓
Servicio realizado → Se cobra (Paid) y se completa (Completed)
```

**¿Por qué?** Un salón real recibe constantemente clientas espontáneas. Forzarlas a descargar la app en la puerta del salón crearía fricción y retrasos. Con este flujo express, el 100% de los ingresos y horas de trabajo quedan registrados en el sistema sin excepciones.

**¿Quién usa?** Admin (ejecuta el registro rápido), Sistema (bloquea el horario para evitar colisiones).

**Relación:** Agenda Interactiva, Personal, Disponibilidad, Pagos.

---

### 3.19 — Dashboard Administrativo Ampliado

**Estado anterior:** Métricas básicas del día (número de citas, estado simple).

**Nueva incorporación:** Panel de inteligencia operativa del negocio con:

- Citas: total del día por turno, desglose completadas / canceladas / no-show, porcentaje de cumplimiento.
- Finanzas: ingresos proyectados vs cobrados, desglose efectivo vs tarjeta, pagos pendientes.
- Calidad: calificación promedio del día/semana, solicitudes de diseño pendientes de cotización (alerta urgente).
- Operación: ocupación de estaciones de trabajo, profesional con más citas del día.

**¿Por qué?** Un dashboard que solo muestra el número de citas no le permite a Admin tomar decisiones. Para gestionar un negocio se necesita información financiera, de calidad y operativa en un solo lugar.

**¿Quién usa?** Admin exclusivamente.

**Relación:** Reservas, Pagos, Reseñas, Personal, Reportes.

---

### 3.20 — Reportes Ampliados

**Estado anterior:** Reportes básicos limitados al conteo de citas.

**Nueva incorporación:** Reportes estructurados por área:

| Área | Contenido |
|:---|:---|
| Citas | Por día, semana, mes o rango. Total, completadas, canceladas, no-show. |
| Finanzas | Ingresos totales, desglose por método, pagos pendientes, por servicio y profesional. |
| Servicios | Más solicitados (ranking), ingresos por servicio. |
| Personal | Citas atendidas, ingresos generados, calificación promedio. |
| Productos | Stock disponible, productos más recomendados o solicitados. |
| Reseñas | Promedio general, tendencia semanal/mensual, por servicio y por profesional. |

Exportación disponible en: PDF y Excel.

**¿Por qué?** Sin reportes detallados, Admin no puede saber qué servicios son rentables, qué profesional genera más valor, cuánto ingresa el salón en un mes ni cuál es la tasa de ausentismo.

**¿Quién usa?** Admin exclusivamente.

---

### 3.21 — Auditoría de Cambios

**ANTES NO EXISTÍA UN SISTEMA FORMAL DE AUDITORÍA.**

**Nueva incorporación:** Módulo que registra automáticamente cualquier acción crítica sobre las entidades del sistema.

Cada registro de auditoría contiene:

| Campo | Descripción |
|:---|:---|
| usuario | Quién realizó el cambio (Admin) |
| entidad_afectada | Qué tabla o entidad fue modificada |
| accion | Tipo de cambio realizado |
| valor_anterior | El valor que existía antes del cambio |
| valor_nuevo | El valor resultante del cambio |
| motivo | Texto obligatorio explicando la razón |
| fecha_hora | Timestamp exacto del cambio |

Acciones auditadas: cambio de estado de una cita, modificación de precio de una cita, respuesta o modificación de cotización, registro de pago, activación o desactivación de servicio, modificación de precio de servicio, desactivación de personal.

**¿Por qué?** Sin auditoría, cualquier inconsistencia es imposible de investigar. Con auditoría, cada cambio importante tiene responsable, fecha y motivo documentado.

**¿Quién usa?** Sistema (registra automáticamente), Admin (consulta cuando necesita investigar un cambio).

**Relación:** Pagos, Cambios de Precio, Reservas, Catálogo, Personal.

---

## 4. Flujo Completo del Cliente (Versión Actual)

`
CLIENTE
  ↓
Descubre catálogo de servicios y productos
  ↓
Explora portafolio y especialidades del personal
  ↓
Selecciona el servicio
  ↓
Selecciona profesional de preferencia
    O elige "No tengo preferencia" → sistema asigna automáticamente
  ↓
Selecciona fecha disponible
  ↓
Selecciona horario disponible
  ↓
¿Tiene referencia/diseño?
  NO → Precio calculado automáticamente → continúa
  SÍ → Sube fotos + escribe notas
         → Cita: PENDIENTE DE VALORACIÓN
         → Chat con Admin sobre el diseño
         → Admin cotiza precio
         → Cliente ACEPTA o RECHAZA
         → Si acepta → Precio definido → continúa
  ↓
Resumen de cita (servicio, profesional, fecha, hora, precio final)
  ↓
Selección de método de pago (Efectivo / Tarjeta en recepción)
  ↓
Confirmación y generación de código de reserva #SHU-XXXX
  ↓
Notificaciones de recordatorio (24h antes y 2h antes)
  ↓
SERVICIO EN SALÓN
  ↓
Cita marcada como COMPLETADA por Admin
  ↓
CICLO POST-SERVICIO:
  → Solicitud de reseña (opcional, no obligatoria)
  → Acumulación de puntos de fidelidad
  → Registro en Mi Historial de Belleza
  → Recomendaciones de productos post-tratamiento
  → Programación de recordatorio de retoque
  → Notificación en la fecha de retoque sugerida
  → Acceso rápido desde favoritos para re-reservar
`

---

## 5. Flujo Completo del Administrador (Versión Actual)

`
ADMIN
  ↓
Dashboard
(KPIs del día: citas, ingresos, ocupación, calidad, alertas urgentes)
  ↓
GESTIÓN OPERATIVA:
  ├── Agenda interactiva (Timeline por profesional)
  │     ├── Ver citas programadas del día
  │     ├── Detectar slots libres disponibles
  │     └── Registrar Walk-in clientes espontáneos
  │
  ├── Solicitudes de diseño pendientes de cotización
  │     ├── Revisar imágenes y notas enviadas por el cliente
  │     ├── Proponer precio y descripción del trabajo incluido
  │     └── Responder preguntas en el chat interno de la cita
  │
  └── Gestión del ciclo de vida de la cita
        ├── Cambiar estado (Confirmada / Completada / Cancelada / No Asistió)
        ├── Registrar el pago cuando se cobra en recepción
        └── Ajustar precio con motivo obligatorio si aplica
  ↓
GESTIÓN DE CATÁLOGOS:
  ├── Servicios: crear, editar, activar/desactivar, asignar
  │             productos recomendados e intervalos de retoque
  ├── Productos: crear, editar, actualizar stock, activar/desactivar
  └── Personal: perfiles, especialidades, horarios, bloqueos,
                portafolios y estado activo/inactivo temporal
  ↓
ANÁLISIS Y CONTROL:
  ├── Reportes: citas, finanzas, servicios, personal,
  │             productos, reseñas (exportación PDF y Excel)
  ├── Reseñas recibidas: calificaciones, comentarios y promedios
  ├── Auditoría: historial de cambios críticos en el sistema
  └── Configuración: intervalos de seguimiento, reglas de puntos,
                     configuraciones generales del salón
`

---

## 6. Relación entre Módulos del Sistema

Los módulos de Shunshine Studio no son funcionalidades independientes. Están profundamente interconectados.

### La Cita como entidad central

`
                        ┌──────────┐
                        │   CITA   │
                        └────┬─────┘
                             │
           ┌─────────────────┼─────────────────────┐
           ▼                 ▼                      ▼
        CLIENTE          SERVICIO              PERSONAL
           │                 │                      │
     Historial           Precio/Duración        Horarios
     Reseñas             Categoría              Bloqueos
     Puntos              Recomendaciones        Portafolio
     Favoritos           Intervalo retoque      Reseñas
           └─────────────────┼──────────────────────┘
                             │
              ┌──────────────┼──────────────────┐
              ▼              ▼                   ▼
           PAGO         SOLICITUD            RESEÑA
                        DE DISEÑO
                             │
                           CHAT
`

### Relaciones por entidad principal

| Entidad | Se relaciona con |
|:---|:---|
| Cita | Cliente, Servicio, Personal, Pago, Solicitud de Diseño, Chat, Reseña, Historial, Auditoría |
| Servicio | Categoría, Personal, Precio, Solicitudes de Diseño, Productos Recomendados, Seguimiento |
| Producto | Categoría, Stock, Recomendaciones, Favoritos |
| Cliente | Citas, Chat, Reseñas, Favoritos, Historial de Belleza, Puntos, Notificaciones |
| Personal | Servicios asignados, Horarios, Reservas, Reseñas, Portafolio |
| Auditoría | Citas, Pagos, Cambios de precio, Cotizaciones, Catálogo |

---

## 7. Modelo Conceptual de Entidades

| Entidad | Naturaleza | Descripción |
|:---|:---|:---|
| User | Autenticable en Supabase Auth | Cuenta de acceso al sistema |
| Role | Catálogo | Admin o Cliente únicamente |
| Client | Perfil de negocio | Nivel de fidelidad, puntos, preferencias |
| Staff | Entidad operativa NO autenticada | Personal del salón administrado por Admin |
| Category | Catálogo | Agrupa servicios y productos |
| Service | Catálogo | Tratamiento con precio, duración e intervalo de seguimiento |
| Product | Inventario | Artículo con stock, precio y estado |
| Appointment | Transacción principal | Cita con estados, profesional, precio y pago |
| AppointmentDetail | Detalle de la cita | Servicio y precio congelado al momento de la reserva |
| DesignRequest | Solicitud de diseño | Imágenes de referencia, notas y estado de cotización |
| Quote | Cotización | Precio propuesto por Admin y estado de aceptación |
| Message | Chat contextual | Mensaje entre cliente y Admin en el hilo de la cita |
| Review | Calificación | Evaluación post-servicio vinculada a cita, servicio y personal |
| Notification | Comunicación automática | Alertas segmentadas por categoría |
| Payment | Control financiero | Método, monto y estado del pago de la cita |
| LoyaltyAccount | Fidelización | Puntos acumulados y nivel del cliente |
| Recommendation | Regla de negocio | Relación entre servicio y productos o servicios sugeridos |
| Favorite | Preferencia del cliente | Servicios, productos y personal guardados |
| Schedule | Disponibilidad del personal | Jornadas semanales por profesional |
| ScheduleBlock | Bloqueo excepcional | Vacaciones, permisos, cierres del salón |
| AuditLog | Trazabilidad | Registro de quién cambió qué, cuándo y por qué |

> REGLA ARQUITECTÓNICA FUNDAMENTAL: Staff no debe modelarse como usuario autenticado. Es una entidad de catálogo. Solo User con Role igual a Admin o Cliente son autenticables en Supabase Auth.

---

## 8. Arquitectura Técnica

### Stack oficial y flujo de datos

`
            APLICACIÓN MÓVIL
            (Flutter + Dart + BLoC)
                    │
                    │ 1. Login o Registro
                    ▼
             SUPABASE AUTH
              (Emite JWT con claim de rol)
                    │
                    │ 2. JWT en header: Authorization: Bearer <token>
                    ▼
        ASP.NET CORE WEB API (.NET 8 o 9)
        (Única fuente de verdad del negocio)
        Valida JWT, ejecuta lógica, controla
        disponibilidad, reservas, cotizaciones,
        pagos, permisos, validaciones, reportes
                    │
                    │ 3. Consultas y transacciones ACID
                    ▼
        SUPABASE POSTGRESQL
        (Row Level Security activo en todas las tablas)

        Imágenes: Flutter → Supabase Storage
        (La URL de la imagen se guarda en PostgreSQL)
`

### Responsabilidades de cada capa

| Capa | Tecnología | Responsabilidad principal |
|:---|:---|:---|
| Frontend Móvil | Flutter + Dart + BLoC | UI, estados de pantalla, consumo de API, autenticación con Supabase Auth |
| Backend API | ASP.NET Core Web API | Disponibilidad, reservas, cotizaciones, pagos, validaciones, concurrencia, reportes |
| Base de Datos | Supabase PostgreSQL | Persistencia con Row Level Security activo |
| Autenticación | Supabase Auth | Emisión y verificación de JWT con claims de rol |
| Almacenamiento | Supabase Storage | Imágenes de servicios, productos, personal, portafolios y referencias de diseños |
| Documentación | Swagger / OpenAPI | Contratos de endpoints tipados y verificables |
| Errores | RFC 7807 ProblemDetails | Respuestas de error estructuradas en application/problem+json |

### Regla crítica de aislamiento

La aplicación Flutter tiene PROHIBIDO consultar directamente las tablas de PostgreSQL de negocio mediante el cliente de base de datos o PostgREST. Todo acceso a datos de negocio pasa exclusivamente por la Web API de C#. El único uso del SDK de Supabase en Flutter es la autenticación.

### Clean Architecture en Flutter

`
presentation/   UI, widgets, BLoC y Cubit
domain/         Entidades, casos de uso, interfaces de repositorio
data/           DTOs, data sources HTTP (Dio), implementaciones de repositorio
core/           Networking, manejo de errores, constantes, tema, utilidades
`

---

## 9. Modelo de Roles y Actores

`
      USUARIOS AUTENTICADOS EN SUPABASE AUTH
      ─────────────────────────────────────
      ┌──────────────┐     ┌──────────────┐
      │   CLIENTE    │     │    ADMIN     │
      └──────────────┘     └──────────────┘
      Reserva, personaliza,  Controla todo el
      chatea, evalúa,        negocio sin
      acumula puntos y       restricciones.
      consulta historial.

      ENTIDAD NO AUTENTICADA
      ──────────────────────
      ┌──────────────┐
      │   PERSONAL   │
      │   (STAFF)    │
      └──────────────┘
      Registros administrables.
      Sin login, sin cuenta,
      sin dashboard, sin permisos.
      Gestionados por Admin.
`

Esta decisión simplifica radicalmente la arquitectura de seguridad sin limitar la capacidad operativa del sistema.

---

## 10. Supabase Storage y Políticas de Almacenamiento

Todos los archivos multimedia e imágenes del sistema se gestionan en Supabase Storage. La base de datos relacional almacena únicamente URLs relativas/públicas de referencia, **nunca datos binarios (BLOBs)**.

#### Especificación de Buckets y Políticas de Acceso (RLS)

| Bucket | Contenido | Acceso Lectura | Acceso Escritura (Subida/Borrado) | Peso Máx. | Formatos Permitidos |
|:---|:---|:---|:---|:---|:---|
| `servicios-imagenes` | Fotos ilustrativas del catálogo de servicios | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| `categorias-imagenes` | Banners e íconos de categorías | Público (`anon`) | Solo Admin (`role = Admin`) | 2 MB | JPG, PNG, WebP |
| `personal-avatares` | Fotos de perfil de las estilistas/manicuristas | Público (`anon`) | Solo Admin (`role = Admin`) | 2 MB | JPG, PNG, WebP |
| `personal-portafolio` | Galería de trabajos realizados por el personal | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| `productos-imagenes` | Fotos de productos del inventario | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| `disenos-referencias` | Fotos enviadas por clientes al solicitar cotización | Privado (Propietaria + Admin) | Solo Cliente propietaria (subcarpeta `/uid/`) y Admin | 5 MB | JPG, PNG, WebP |

#### Reglas de Almacenamiento
1. **Aislamiento en `disenos-referencias`:** Ninguna clienta puede acceder ni listar imágenes de diseño subidas por otra persona. La política de Storage valida que el prefijo de la ruta coincida estrictamente con `auth.uid()`.
2. **Validación de tipos MIME:** El backend y el Storage descartan archivos ejecutables, PDFs o scripts; únicamente se admiten formatos gráficos comprimidos.
3. **Optimización de carga:** Flutter utiliza almacenamiento en caché de imágenes (`cached_network_image`) para evitar peticiones repetitivas a Supabase Storage y optimizar consumo de datos móviles.

---

## 11. Seguridad y Reglas de Negocio Críticas

Estas validaciones ocurren **exclusivamente en la Web API de C#**, la cual es la única autoridad de negocio:

1. **Verificación de disponibilidad antes de confirmar:** El backend recalcula y valida que el slot siga 100% libre al momento exacto de la confirmación para evitar colisiones.
2. **Control de concurrencia transaccional (ACID):** Bloqueo a nivel de base de datos durante la reserva de un horario para garantizar que dos peticiones simultáneas nunca reserven el mismo bloque con el mismo profesional.
3. **Autorización estricta por rol (JWT Claims):** Todos los endpoints bajo `/api/admin/*` validan criptográficamente la firma del JWT de Supabase y exigen el claim `role = Admin`.
4. **Aislamiento total de datos entre clientes:** Una clienta solo puede consultar sus propias citas, historial, favoritos, cotizaciones y mensajes mediante el claim `sub` (User ID).
5. **Máquina de transiciones válida:** El sistema rechaza cualquier transición de estado ilegal (por ejemplo, transicionar una cita de `Cancelled` a `Completed`, o de `PendingQuote` a `InProgress` sin cotización aceptada).
6. **Ventana de cancelación autónoma:** El backend rechaza peticiones de cancelación originadas desde la app cliente si la hora de la cita está a menos de **4 horas** del momento actual.
7. **Inmutabilidad y cierre del chat:** El backend rechaza la inserción de nuevos mensajes de texto o fotos en chats de citas cuyo estado sea `Completed` con más de 24 horas transcurridas, o citas en estado `Cancelled` / `NoShow`.
8. **Auditoría obligatoria de cambios críticos:** No se procesan modificaciones manuales de precio, anulaciones de cita o bajas de catálogo si la petición no incluye el parámetro `motivo` justificado.
9. **Control de stock atómico:** Al agregar productos a una orden de cobro, el backend descuenta el inventario de forma atómica y rechaza la transacción si el stock disponible es inferior a la cantidad solicitada.

---

## 12. Lo que Cambió con Respecto a la Idea Original

### ANTES

Shunshine Studio era principalmente:

> Una aplicación móvil para reservar citas en un salón de belleza.

Recorrido lineal:

`
Registro → Login → Ver servicios → Elegir estilista → Elegir fecha/hora → Confirmar → Ver mis citas
`

El sistema resolvía la necesidad de digitalizar la agenda. Pero no resolvía lo que ocurre antes del servicio (comunicación y personalización), durante el servicio (ajustes de precio) ni después del servicio (reseñas, fidelización, retención).

### AHORA

Shunshine Studio es:

> Una plataforma móvil integral para gestionar la relación entre clientas y salón antes, durante y después de cada servicio.

Recorrido como ciclo continuo:

`
Descubrir → Reservar → Personalizar → Valorar precio → Confirmar →
Atenderse → Completar → Evaluar → Acumular puntos →
Recibir seguimiento → Recibir recomendaciones → Volver a reservar
`

### La diferencia fundamental

| Aspecto | Antes | Ahora |
|:---|:---|:---|
| Objetivo | Digitalizar la agenda | Gestionar la relación completa clienta-salón |
| Valor para la clienta | Reservar con facilidad | Reservar + personalizar + comunicarse + fidelidad |
| Valor para el salón | Agenda digital | Control financiero + reputación + retención |
| Duración de la interacción | Solo al momento de reservar | Antes, durante y después del servicio |
| Precio | Siempre fijo | Precio base + cotización + confirmación + ajuste |

---

## 13. Funcionalidades que Más Cambiaron el Proyecto

Las siguientes siete funcionalidades representan los cambios más significativos porque cada una resuelve un problema real que una agenda digital no puede resolver:

1. Solicitudes de Diseño + Valoración de Precio: resuelve el problema más concreto del salón: manejar servicios con precio variable. Convierte un proceso que ocurría por WhatsApp en un flujo formal, trazable y documentado.

2. Chat Interno Contextual: mantiene toda la comunicación dentro de la aplicación. Elimina la dependencia de WhatsApp para resolver dudas sobre diseños, precios o detalles del servicio.

3. Reseñas Post-Servicio: transforma la relación de una sola dirección en bidireccional. Permite al salón medir su calidad de manera sistemática y a la clienta sentir que su opinión importa.

4. Inventario y Catálogo de Productos: reconoce que el salón administra y puede comercializar productos además de servicios. Sin este módulo el sistema es incompleto para la realidad del negocio.

5. Seguimiento Post-Servicio Personalizado: transforma cada cita completada en el inicio de la siguiente relación. Sin seguimiento el sistema es pasivo. Con seguimiento trabaja activamente para traer de vuelta a la clienta en el momento correcto.

6. Historial de Belleza: convierte el historial de citas en un registro de valor real para la clienta, haciendo que quiera usar la app incluso cuando no está reservando.

7. Auditoría de Cambios: hace que el sistema sea confiable para un negocio real. Sin trazabilidad, cualquier cambio genera dudas. Con auditoría, todo está documentado con responsable, fecha y motivo.

---

## 14. Principios que Deben Mantenerse

Decisiones de diseño y arquitectura que no deben modificarse sin revisión explícita del equipo:

1. Solo existen dos tipos de usuario autenticado: CLIENTE y ADMIN.
2. El personal del salón es una entidad de catálogo administrable, no un usuario autenticado.
3. Los precios personalizados requieren valoración manual por Admin y aceptación por la clienta.
4. Las imágenes de diseños son solicitudes de valoración. No implican confirmación de precio automática.
5. El chat permanece dentro de Shunshine Studio. No se envía a la clienta a WhatsApp para resolver procesos del sistema.
6. Las reseñas ocurren después de marcar una cita como Completed. No son obligatorias.
7. Los productos y los servicios son entidades distintas y separadas en el modelo de datos.
8. Las recomendaciones funcionan mediante reglas configurables. No requieren inteligencia artificial.
9. Los intervalos de seguimiento deben ser configurables por Admin. No escritos directamente en el código.
10. Los cambios de precio y modificaciones críticas deben quedar auditadas con motivo obligatorio.
11. El backend verifica disponibilidad antes de confirmar cualquier reserva, independientemente del frontend.
12. Las nuevas categorías, servicios, productos y miembros del personal pueden agregarse sin modificar el código.
13. La arquitectura de pagos está preparada para incorporar pasarela online en el futuro.
14. La aplicación Flutter no consulta directamente las tablas de PostgreSQL. Todo pasa por la Web API de C#.

---

## 15. Matriz Final de Evolución

| Funcionalidad | Existía antes | Se agrega o amplía ahora | Motivo | Prioridad |
|:---|:---:|:---:|:---|:---:|
| Registro de cliente | Sí | — | Flujo principal | Alta |
| Login / Autenticación | Sí | — | Flujo principal | Alta |
| Catálogo de servicios | Sí | Ampliado (administrable, precio base, intervalo) | Catálogo dinámico sin cambios de código | Alta |
| Detalle de servicio | Sí | Ampliado (personal sugerido, productos relacionados) | Enriquecer la decisión de reserva | Alta |
| Selección de personal | Sí | Ampliado (portafolio visible, asignación automática) | Flexibilidad y distribución de carga | Alta |
| Selección de fecha y hora | Sí | Ampliado (cálculo real de disponibilidad en backend) | Control de concurrencia transaccional | Alta |
| Resumen de reserva | Sí | Ampliado (precio cotizado, método de pago) | Control financiero desde el inicio | Alta |
| Confirmación de cita | Sí | Ampliado (estado de cotización cuando aplica) | Flujo completo con valoración de precio | Alta |
| Mis citas | Sí | Ampliado (estados adicionales, acceso al chat) | Ciclo de vida completo de la cita | Alta |
| Perfil del cliente | Sí | Ampliado (puntos y nivel de fidelidad) | Personalización de la experiencia | Media |
| Dashboard admin | Sí | Ampliado (métricas financieras, calidad, alertas) | Decisiones de negocio reales | Alta |
| Agenda admin | Sí | Ampliado (walk-ins, cotizaciones, cambios de estado) | Control operativo completo | Alta |
| Administración servicios | Sí | Ampliado (intervalo de retoque, productos vinculados) | Catálogo dinámico administrable | Alta |
| Inventario / Productos | No | Sí | El salón administra y puede vender productos | Alta |
| Solicitudes de diseño | No | Sí | Resolver servicios con precio variable | Alta |
| Valoración / Cotización | No | Sí | Evitar conflictos de precio cliente-salón | Alta |
| Cambio de precio con auditoría | No | Sí | Flexibilidad con trazabilidad completa | Alta |
| Chat interno por cita | No | Sí | Comunicación dentro del sistema | Alta |
| Notificaciones ampliadas | Parcial | Ampliado | Seguimiento inteligente de la relación | Alta |
| Reseñas y calificaciones | No | Sí | Medir experiencia post-servicio | Alta |
| Historial de belleza | Parcial | Ampliado | Línea de tiempo completa de la relación | Media |
| Seguimiento post-servicio | No | Sí | Aumentar frecuencia de visitas | Media |
| Recomendaciones | No | Sí | Conectar servicios con productos | Media |
| Favoritos | No | Sí | Personalización y acceso rápido | Media |
| Fidelización y puntos | No | Sí | Retención de clientas frecuentes | Media |
| Gestión de pagos con estados | Parcial | Ampliado | Control financiero real del negocio | Alta |
| Walk-in clients (flujo formal) | Sin flujo | Sí | Gestionar atención presencial sin cita previa | Media |
| Reportes ampliados | Básico | Ampliados (por área, exportables) | Soporte para decisiones administrativas | Media |
| Auditoría de cambios | No | Sí | Trazabilidad y transparencia del negocio | Alta |
| Asignación automática personal | No | Sí | Distribución equitativa del trabajo | Alta |
| Portafolio del personal | No | Sí | Mostrar calidad del trabajo antes de reservar | Media |

---

## 16. Objetivo Final del Sistema

El objetivo de Shunshine Studio ya no se limita a resolver la reserva de una cita.

El objetivo es:

> Centralizar en una sola plataforma la experiencia completa del cliente y la operación integral del salón de belleza: desde el descubrimiento y reserva del servicio, pasando por la personalización, valoración del precio, comunicación, atención, pago, cierre, reseña, historial, seguimiento, recomendaciones y fidelización, hasta la nueva reserva.

El sistema debe ser lo suficientemente flexible para crecer con el negocio sin requerir modificaciones de código para agregar nuevos servicios, productos, miembros del personal o categorías.

La aplicación debe hacer sentir a cada clienta que el salón la conoce, que recuerda lo que se hizo, que se preocupa por el resultado de su tratamiento y que merece una atención personalizada.

---

Documento oficial del proyecto Shunshine Studio.
No modifica código fuente, arquitectura, base de datos, dependencias ni funcionalidades existentes.
