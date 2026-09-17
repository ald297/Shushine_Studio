# Data Model Specification: Plataforma Móvil de Gestión y Reservas Shunshine Studio

**Feature**: `001-sistema-reservas-shushine`  
**Database**: PostgreSQL 15+ (Supabase AWS Pooler)  
**ORM / Persistencia**: Spring Data JPA / Hibernate (`org.postgresql:postgresql`)  
**Status**: Ready & Aligned with Database Schema (23 Tables + 6 Buckets)  

---

## 1. Diagrama de Relaciones y Entidades Principales

El modelo de datos comprende **23 tablas relacionales normalizadas** y **6 buckets de almacenamiento** para soportar el ciclo integral operativo y comercial de **Shunshine Studio**:

```
[auth.users] (Supabase Auth)
     │ (1:1 via trigger)
     ▼
[Usuarios] ◄────────────── (N:1) ─────────────── [Roles]
     │
     ├─ (1:1 / 0:1) ──► [Clientes] ◄─────────────────────────────────────────────┐
     │                                                                           │
     ├─ (1:N) ────────► [AuditoriaLogs]                                          │
     │                                                                           │
     └─ (1:N) ────────► [MensajesChatCita]                                       │
                              ▲                                                  │
                              │                                                  │
[Categorias]                  │                                                  │
     │                        │                                                  │
     ├─ (1:N) ──► [Servicios] │                                                  │
     │                 │      │                                                  │
     │                 ├──────┼────────── (N:M) ──────────► [Productos]          │
     │                 │      │                                                  │
     │                 │      │                                                  │
     │                 ├──────┼────────── (N:M) ──► [Estilistas]                 │
     │                 │      │                         │                        │
     │                 │      │                         ├─ (1:N) ──► [HorariosEstilistas]
     │                 ▼      │                         ├─ (1:N) ──► [BloqueosHorarios]
     │         [CitaServicios]│                         └─ (1:N) ──► [PersonalPortafolios]
     │                 ▲      │                                     │
     │                 │      │                                     │
     └─────────────────┴── [Citas] ─────────────────────────────────┤
                              │                                     │
                              ├─ (1:1) ──► [SolicitudesDiseno] ──► [Cotizaciones]
                              ├─ (1:N) ──► [Pagos]                  │
                              ├─ (1:1) ──► [Facturas]               │
                              ├─ (1:1) ──► [Resenas] ───────────────┘
                              ├─ (1:N) ──► [TransaccionesPuntos] ──► [Clientes]
                              └─ (1:N) ──► [Favoritos] ────────────► [Clientes]
```

---

## 2. Definición Detallada de Entidades (Schema Oficial)

### 2.1 Seguridad, Usuarios y Clientes

#### `roles`
* `id_rol` (int, PK, auto-increment)
* `nombre` (varchar(50), NOT NULL, UNIQUE) — `'Cliente'`, `'Administrador'`, `'Recepcionista'`
* `descripcion` (varchar(200), NULL)

#### `usuarios`
* `id_usuario` (uuid, PK) — Llave foránea directa hacia `auth.users(id)` ON DELETE CASCADE.
* `id_rol` (int, FK -> `roles.id_rol`, NOT NULL)
* `nombre_completo` (varchar(150), NOT NULL)
* `correo` (varchar(150), NOT NULL, UNIQUE)
* `telefono` (varchar(20), NOT NULL)
* `activo` (boolean, NOT NULL, DEFAULT true)
* `fecha_registro` (timestamptz, NOT NULL, DEFAULT now())

#### `clientes`
* `id_cliente` (int, PK, auto-increment)
* `id_usuario` (uuid, FK -> `usuarios.id_usuario`, NULL, UNIQUE) — Nullable para walk-in clients
* `nombre_walkin` (varchar(150), NULL)
* `telefono_walkin` (varchar(20), NULL)
* `fecha_nacimiento` (date, NULL)
* `nivel_fidelidad` (varchar(50), NOT NULL, DEFAULT `'Bronce'`) — Bronce, Plata, Oro
* `puntos_acumulados` (int, NOT NULL, DEFAULT 0)
* `tipo_cabello` (varchar(100), NULL)
* `notas_preferencias` (varchar(500), NULL)
* `es_walkin` (boolean, NOT NULL, DEFAULT false)
* `fecha_creacion` (timestamptz, NOT NULL, DEFAULT now())

---

### 2.2 Catálogo de Servicios y Productos

#### `categorias`
* `id_categoria` (int, PK, auto-increment)
* `nombre` (varchar(100), NOT NULL, UNIQUE) — ej. "Cabello", "Uñas", "Pestañas & Cejas", "Spa & Facial", "Cuidado Capilar"
* `descripcion` (varchar(255), NULL)
* `icono_url` (varchar(500), NULL)
* `tipo` (varchar(30), NOT NULL, DEFAULT `'Servicio'`) — `'Servicio'`, `'Producto'`
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `servicios`
* `id_servicio` (int, PK, auto-increment)
* `codigo_servicio` (varchar(20), NOT NULL, UNIQUE) — ej. `'SRV-CAP-01'`
* `id_categoria` (int, FK -> `categorias.id_categoria`, NOT NULL)
* `nombre` (varchar(150), NOT NULL)
* `descripcion` (text, NOT NULL)
* `precio_base` (numeric(10,2), NOT NULL, CHECK >= 0)
* `es_precio_variable` (boolean, NOT NULL, DEFAULT false)
* `duracion_minutos` (int, NOT NULL, CHECK > 0)
* `intervalo_seguimiento_dias` (int, NOT NULL, DEFAULT 21)
* `imagen_url` (varchar(500), NULL)
* `costo_insumos` (numeric(10,2), NOT NULL, DEFAULT 0.00)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `productos`
* `id_producto` (int, PK, auto-increment)
* `codigo_producto` (varchar(20), NOT NULL, UNIQUE) — ej. `'PRD-CAP-01'`
* `id_categoria` (int, FK -> `categorias.id_categoria`, NOT NULL)
* `nombre` (varchar(150), NOT NULL)
* `marca` (varchar(100), NOT NULL)
* `descripcion` (text, NULL)
* `precio` (numeric(10,2), NOT NULL, CHECK >= 0)
* `stock_actual` (int, NOT NULL, DEFAULT 0, CHECK >= 0)
* `stock_minimo` (int, NOT NULL, DEFAULT 5)
* `imagen_url` (varchar(500), NULL)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `servicio_productos_rec`
* `id_relacion` (int, PK, auto-increment)
* `id_servicio` (int, FK -> `servicios.id_servicio`, NOT NULL)
* `id_producto` (int, FK -> `productos.id_producto`, NOT NULL)
* `motivo_recomendacion` (varchar(255), NULL)
* UNIQUE (`id_servicio`, `id_producto`)

---

### 2.3 Estilistas y Gestión de Horarios

#### `estilistas`
* `id_estilista` (int, PK, auto-increment)
* `nombre_completo` (varchar(150), NOT NULL)
* `especialidad_principal` (varchar(100), NOT NULL)
* `biografia` (text, NULL)
* `avatar_url` (varchar(500), NULL)
* `color_agenda` (varchar(20), NOT NULL, DEFAULT `'#D81B60'`)
* `porcentaje_comision` (numeric(5,2), NOT NULL, DEFAULT 0.00)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `estilista_servicios`
* `id_estilista_servicio` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `id_servicio` (int, FK -> `servicios.id_servicio`, NOT NULL)
* UNIQUE (`id_estilista`, `id_servicio`)

#### `horarios_estilistas`
* `id_horario` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `dia_semana` (int, NOT NULL, CHECK 1 a 7)
* `hora_inicio` (time, NOT NULL)
* `hora_fin` (time, NOT NULL)
* `hora_inicio_almuerzo` (time, NULL)
* `hora_fin_almuerzo` (time, NULL)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `bloqueos_horarios`
* `id_bloqueo` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `fecha` (date, NOT NULL)
* `hora_inicio` (time, NOT NULL)
* `hora_fin` (time, NOT NULL)
* `motivo` (varchar(200), NOT NULL)

#### `personal_portafolios`
* `id_portafolio` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `titulo` (varchar(150), NOT NULL)
* `descripcion` (varchar(500), NULL)
* `imagen_url` (varchar(500), NOT NULL)
* `fecha_publicacion` (timestamptz, NOT NULL, DEFAULT now())

---

### 2.4 Citas, Diseños y Cotizaciones

#### `citas`
* `id_cita` (int, PK, auto-increment)
* `codigo_cita` (varchar(20), NOT NULL, UNIQUE) — ej. `'#SHU-8492'`
* `id_cliente` (int, FK -> `clientes.id_cliente`, NOT NULL)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `fecha_cita` (date, NOT NULL)
* `hora_inicio` (time, NOT NULL)
* `hora_fin` (time, NOT NULL)
* `estado` (varchar(30), NOT NULL, DEFAULT `'Confirmed'`) — `PendingQuote`, `QuoteProposed`, `Confirmed`, `InProgress`, `Completed`, `Cancelled`, `NoShow`
* `subtotal` (numeric(10,2), NOT NULL)
* `descuento_puntos` (numeric(10,2), NOT NULL, DEFAULT 0.00)
* `iva` (numeric(10,2), NOT NULL)
* `total` (numeric(10,2), NOT NULL)
* `metodo_pago_preferente` (varchar(50), NOT NULL, DEFAULT `'Efectivo'`)
* `estado_pago` (varchar(30), NOT NULL, DEFAULT `'Pending'`) — `Pending`, `Paid`, `PartiallyPaid`, `Refunded`
* `es_walkin` (boolean, NOT NULL, DEFAULT false)
* `motivo_cancelacion` (varchar(300), NULL)
* `fecha_creacion` (timestamptz, NOT NULL, DEFAULT now())

#### `cita_servicios`
* `id_cita_servicio` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL)
* `id_servicio` (int, FK -> `servicios.id_servicio`, NOT NULL)
* `precio_aplicado` (numeric(10,2), NOT NULL)
* `duracion_minutos` (int, NOT NULL)
* `notas` (varchar(255), NULL)

#### `solicitudes_diseno`
* `id_solicitud` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL, UNIQUE)
* `imagenes_referencia_urls` (jsonb, NOT NULL) — Array de 1 a 3 URLs de Supabase Storage
* `notas_cliente` (text, NULL)
* `estado` (varchar(30), NOT NULL, DEFAULT `'Pendiente'`)
* `fecha_solicitud` (timestamptz, NOT NULL, DEFAULT now())

#### `cotizaciones`
* `id_cotizacion` (int, PK, auto-increment)
* `id_solicitud` (int, FK -> `solicitudes_diseno.id_solicitud` ON DELETE CASCADE, NOT NULL, UNIQUE)
* `precio_propuesto` (numeric(10,2), NOT NULL)
* `descripcion_trabajo` (text, NOT NULL)
* `estado` (varchar(30), NOT NULL, DEFAULT `'Propuesta'`) — `Propuesta`, `Aceptada`, `Rechazada`, `Expirada`
* `fecha_cotizacion` (timestamptz, NOT NULL, DEFAULT now())
* `fecha_respuesta` (timestamptz, NULL)

#### `mensajes_chat_cita`
* `id_mensaje` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL)
* `id_emisor_usuario` (uuid, FK -> `usuarios.id_usuario`, NOT NULL)
* `remitente_tipo` (varchar(20), NOT NULL) — `'Cliente'`, `'Salon'`
* `mensaje` (text, NOT NULL)
* `imagenes_adjuntas` (jsonb, NULL)
* `es_mensaje_sistema` (boolean, NOT NULL, DEFAULT false)
* `leido` (boolean, NOT NULL, DEFAULT false)
* `fecha_envio` (timestamptz, NOT NULL, DEFAULT now())

---

### 2.5 Pagos, Facturación, Reseñas, Puntos y Auditoría

#### `pagos`
* `id_pago` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL)
* `monto` (numeric(10,2), NOT NULL)
* `metodo_pago` (varchar(50), NOT NULL) — `'Efectivo'`, `'Tarjeta'`
* `estado` (varchar(30), NOT NULL, DEFAULT `'Aprobado'`) — `Aprobado`, `Reembolsado`, `Anulado`
* `referencia_pos` (varchar(100), NULL)
* `motivo_ajuste_precio` (varchar(300), NULL)
* `fecha_pago` (timestamptz, NOT NULL, DEFAULT now())

#### `facturas`
* `id_factura` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL, UNIQUE)
* `numero_factura` (varchar(50), NOT NULL, UNIQUE) — ej. `'FAC-2026-00123'`
* `fecha_emision` (timestamptz, NOT NULL, DEFAULT now())
* `subtotal` (numeric(10,2), NOT NULL)
* `iva` (numeric(10,2), NOT NULL)
* `total` (numeric(10,2), NOT NULL)
* `datos_emisor_receptor` (jsonb, NULL)

#### `resenas`
* `id_resena` (int, PK, auto-increment)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE CASCADE, NOT NULL, UNIQUE)
* `id_cliente` (int, FK -> `clientes.id_cliente`, NOT NULL)
* `id_estilista` (int, FK -> `estilistas.id_estilista`, NOT NULL)
* `estrellas_general` (int, NOT NULL, CHECK 1 a 5)
* `estrellas_calidad` (int, NOT NULL, CHECK 1 a 5)
* `estrellas_atencion` (int, NOT NULL, CHECK 1 a 5)
* `estrellas_ambiente` (int, NOT NULL, CHECK 1 a 5)
* `comentario` (text, NULL)
* `visible_publica` (boolean, NOT NULL, DEFAULT true)
* `fecha_emision` (timestamptz, NOT NULL, DEFAULT now())

#### `transacciones_puntos`
* `id_transaccion_puntos` (int, PK, auto-increment)
* `id_cliente` (int, FK -> `clientes.id_cliente`, NOT NULL)
* `id_cita` (int, FK -> `citas.id_cita` ON DELETE SET NULL, NULL)
* `puntos` (int, NOT NULL) — Positivo (acumulación) o Negativo (canje)
* `tipo_movimiento` (varchar(30), NOT NULL) — `'Acumulacion'`, `'Canje'`, `'Reembolso'`, `'AjusteManual'`
* `descripcion` (varchar(200), NULL)
* `fecha_registro` (timestamptz, NOT NULL, DEFAULT now())

#### `favoritos`
* `id_favorito` (int, PK, auto-increment)
* `id_cliente` (int, FK -> `clientes.id_cliente` ON DELETE CASCADE, NOT NULL)
* `tipo_entidad` (varchar(30), NOT NULL) — `'Servicio'`, `'Producto'`, `'Estilista'`
* `id_referencia` (int, NOT NULL)
* `fecha_guardado` (timestamptz, NOT NULL, DEFAULT now())
* UNIQUE (`id_cliente`, `tipo_entidad`, `id_referencia`)

#### `auditoria_logs`
* `id_auditoria` (int, PK, auto-increment)
* `id_usuario` (uuid, FK -> `usuarios.id_usuario` ON DELETE SET NULL, NULL)
* `entidad_afectada` (varchar(50), NOT NULL) — `'Citas'`, `'Precios'`, `'Catalogo'`, `'Pagos'`
* `accion` (varchar(20), NOT NULL) — `'INSERT'`, `'UPDATE'`, `'DELETE'`
* `valor_anterior` (text, NULL)
* `valor_nuevo` (text, NULL)
* `motivo` (varchar(300), NOT NULL)
* `fecha_hora` (timestamptz, NOT NULL, DEFAULT now())

---

## 3. Buckets de Almacenamiento (Supabase Storage)

| Bucket | Nivel de Acceso | Límite por Archivo | Formatos Permitidos |
| :--- | :--- | :--- | :--- |
| `servicios-imagenes` | Público (`anon` / `authenticated`) | 5 MB | JPG, PNG, WebP |
| `categorias-imagenes` | Público (`anon` / `authenticated`) | 2 MB | JPG, PNG, WebP |
| `personal-avatares` | Público (`anon` / `authenticated`) | 2 MB | JPG, PNG, WebP |
| `personal-portafolio` | Público (`anon` / `authenticated`) | 5 MB | JPG, PNG, WebP |
| `productos-imagenes` | Público (`anon` / `authenticated`) | 5 MB | JPG, PNG, WebP |
| `disenos-referencias` | Privado (`/uid/*` propietario + Admin) | 5 MB | JPG, PNG, WebP |
