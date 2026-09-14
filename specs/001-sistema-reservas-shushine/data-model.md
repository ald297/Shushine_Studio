# Data Model Specification: Sistema Móvil de Gestión y Reservas Shushine Studio

**Feature**: `001-sistema-reservas-shushine`  
**Database**: PostgreSQL 15+ (Supabase)  
**ORM**: Entity Framework Core 8/9 (`Npgsql.EntityFrameworkCore.PostgreSQL`)  
**Status**: Ready & Aligned with Database Schema  

---

## 1. Diagrama de Relaciones y Entidades Principales

El modelo de datos comprende 16 tablas relacionales normalizadas para soportar la gestión operativa de un salón de belleza de alta gama:

```
[auth.users] (Supabase Auth)
     │ (1:1 via trigger)
     ▼
[Usuarios] ◄─── (N:1) ─── [Roles]
     │
     ├─ (1:1) ──► [Clientes] ◄──── (N:1) ────┐
     │                                       │
     └─ (1:N) ──► [AuditoriaLogs]            │
                                             │
[CategoriasServicio]                         │
     │ (1:N)                                 │
     ▼                                       │
[Servicios]                                  │
     │ (1:N)                                 │
     ▼                                       │
[CitaDetalles] ◄─── (N:1) ─── [Citas] ───────┤
                                 │           │
[Estilistas] ────────────────────┤           │
     ├─ (1:N) ──► [HorariosEstilistas]       │
     ├─ (1:N) ──► [BloqueosHorarios]         │
     └─ (1:N) ──► [EstilistaEspecialidades]  │
                                 │           │
                                 ├─ (1:N) ──► [TransaccionesPago]
                                 ├─ (1:1) ──► [Facturas]
                                 ├─ (1:1) ──► [Reseñas]
                                 └─ (1:N) ──► [Notificaciones]
```

---

## 2. Definición Detallada de Entidades (Schema)

### 2.1 Usuarios y Seguridad

#### `Roles`
* `id_rol` (int, PK, auto-increment)
* `nombre` (varchar(50), NOT NULL, UNIQUE) — Valores: `'Administrador'`, `'Recepcionista'`, `'Cliente'`
* `descripcion` (varchar(200), NULL)

#### `Usuarios`
* `id_usuario` (uuid, PK) — Llave foránea directa hacia `auth.users(id)`.
* `id_rol` (int, FK -> `Roles.id_rol`, NOT NULL)
* `nombre_completo` (varchar(150), NOT NULL)
* `correo` (varchar(150), NOT NULL, UNIQUE)
* `telefono` (varchar(20), NOT NULL)
* `activo` (boolean, NOT NULL, DEFAULT true)
* `fecha_registro` (timestamptz, NOT NULL, DEFAULT now())

#### `Clientes`
* `id_cliente` (int, PK, auto-increment)
* `id_usuario` (uuid, FK -> `Usuarios.id_usuario`, NOT NULL, UNIQUE)
* `fecha_nacimiento` (date, NULL)
* `nivel_fidelidad` (varchar(50), DEFAULT `'Nivel Oro'`) — Bronce, Plata, Oro
* `tipo_cabello` (varchar(100), NULL) — ej. "Ondulado Fino - Tratado"
* `id_estilista_preferido` (int, FK -> `Estilistas.id_estilista`, NULL)
* `notas_preferencias` (varchar(500), NULL)

---

### 2.2 Catálogo de Servicios

#### `CategoriasServicio`
* `id_categoria` (int, PK, auto-increment)
* `nombre` (varchar(100), NOT NULL, UNIQUE) — ej. "Cabello", "Uñas", "Maquillaje", "Spa & Facial"
* `descripcion` (varchar(255), NULL)
* `icono_url` (varchar(500), NULL)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `Servicios`
* `id_servicio` (int, PK, auto-increment)
* `codigo_servicio` (varchar(20), NOT NULL, UNIQUE) — ej. `'SRV-C01'`, `'SRV-U05'`
* `id_categoria` (int, FK -> `CategoriasServicio.id_categoria`, NOT NULL)
* `nombre` (varchar(150), NOT NULL)
* `descripcion` (text, NOT NULL)
* `precio` (decimal(10,2), NOT NULL, CHECK price > 0)
* `duracion_minutos` (int, NOT NULL, CHECK duracion > 0) — ej. 30, 45, 60, 90 min
* `imagen_url` (varchar(500), NULL) — Enlace público a Supabase Storage
* `activo` (boolean, NOT NULL, DEFAULT true)
* `costo_insumos` (decimal(10,2), DEFAULT 0.00)

---

### 2.3 Estilistas y Disponibilidad Horaria

#### `Estilistas`
* `id_estilista` (int, PK, auto-increment)
* `id_usuario` (uuid, FK -> `Usuarios.id_usuario`, NULL) — Permite vincular cuenta si tiene acceso al sistema
* `nombre` (varchar(100), NOT NULL)
* `apellido` (varchar(100), NOT NULL)
* `especialidad_principal` (varchar(100), NOT NULL)
* `biografia` (text, NULL)
* `foto_perfil_url` (varchar(500), NULL)
* `color_agenda` (varchar(20), NOT NULL, DEFAULT `'#E91E63'`) — Hexadecimal para timeline de UI
* `porcentaje_comision` (decimal(5,2), NOT NULL, DEFAULT 0.00)
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `HorariosEstilistas`
* `id_horario` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `Estilistas.id_estilista`, NOT NULL)
* `dia_semana` (int, NOT NULL, CHECK 1 a 7) — 1=Lunes, 7=Domingo
* `hora_inicio` (time, NOT NULL) — ej. `'09:00:00'`
* `hora_fin` (time, NOT NULL) — ej. `'18:00:00'`
* `hora_inicio_almuerzo` (time, NULL) — ej. `'13:00:00'`
* `hora_fin_almuerzo` (time, NULL) — ej. `'14:00:00'`
* `activo` (boolean, NOT NULL, DEFAULT true)

#### `BloqueosHorarios`
* `id_bloqueo` (int, PK, auto-increment)
* `id_estilista` (int, FK -> `Estilistas.id_estilista`, NOT NULL)
* `fecha` (date, NOT NULL)
* `hora_inicio` (time, NOT NULL)
* `hora_fin` (time, NOT NULL)
* `motivo` (varchar(200), NOT NULL) — "Vacaciones", "Cita Médica", "Mantenimiento Cabina"

---

### 2.4 Citas y Transaccionalidad

#### `Citas`
* `id_cita` (int, PK, auto-increment)
* `codigo_cita` (varchar(20), NOT NULL, UNIQUE) — ej. `'SHU-2026-0001'`
* `id_cliente` (int, FK -> `Clientes.id_cliente`, NOT NULL)
* `id_estilista` (int, FK -> `Estilistas.id_estilista`, NOT NULL)
* `fecha_cita` (date, NOT NULL)
* `hora_inicio` (time, NOT NULL)
* `hora_fin` (time, NOT NULL)
* `estado` (varchar(30), NOT NULL, DEFAULT `'Pendiente'`)
* `subtotal` (decimal(10,2), NOT NULL)
* `descuento` (decimal(10,2), NOT NULL, DEFAULT 0.00)
* `iva` (decimal(10,2), NOT NULL) — 13% IVA
* `total` (decimal(10,2), NOT NULL)
* `anticipo_pagado` (decimal(10,2), NOT NULL, DEFAULT 0.00)
* `saldo_pendiente` (decimal(10,2), NOT NULL)
* `es_walkin` (boolean, NOT NULL, DEFAULT false)
* `notas` (text, NULL)
* `fecha_creacion` (timestamptz, NOT NULL, DEFAULT now())

#### `CitaDetalles`
* `id_detalle` (int, PK, auto-increment)
* `id_cita` (int, FK -> `Citas.id_cita` ON DELETE CASCADE, NOT NULL)
* `id_servicio` (int, FK -> `Servicios.id_servicio`, NOT NULL)
* `precio_unitario` (decimal(10,2), NOT NULL)
* `duracion_minutos` (int, NOT NULL)
* `notas_servicio` (varchar(200), NULL)

---

### 2.5 Pagos y Facturación

#### `TransaccionesPago`
* `id_transaccion` (int, PK, auto-increment)
* `id_cita` (int, FK -> `Citas.id_cita`, NOT NULL)
* `monto` (decimal(10,2), NOT NULL)
* `metodo_pago` (varchar(50), NOT NULL) — `'Efectivo'`, `'Tarjeta'`, `'Transferencia'`
* `referencia_transaccion` (varchar(100), NULL)
* `estado_pago` (varchar(30), NOT NULL, DEFAULT `'Aprobado'`)
* `fecha_pago` (timestamptz, NOT NULL, DEFAULT now())

#### `Facturas`
* `id_factura` (int, PK, auto-increment)
* `id_cita` (int, FK -> `Citas.id_cita`, NOT NULL, UNIQUE)
* `numero_factura` (varchar(50), NOT NULL, UNIQUE) — ej. `'FAC-2026-00123'`
* `fecha_emision` (timestamptz, NOT NULL, DEFAULT now())
* `subtotal` (decimal(10,2), NOT NULL)
* `iva` (decimal(10,2), NOT NULL)
* `total` (decimal(10,2), NOT NULL)
* `datos_cliente_fiscal` (jsonb, NULL)

---

## 3. Máquina de Estados de Citas (State Transitions)

```
       [Creación Cita]
             │
             ▼
        [Pendiente]
             │
             ├──────────────────────► [Cancelada] (por cliente con >= 2h o por admin)
             │
             ▼ (Anticipo / Confirmación)
       [Confirmada]
             │
             ├──────────────────────► [No_Asistio] (cliente no se presentó)
             ├──────────────────────► [Cancelada]
             │
             ▼ (Cliente ingresa a cabina)
       [En_Progreso]
             │
             ▼ (Servicio terminado)
       [Completada]
             │
             ▼ (Cobro y Facturación)
          [Pagada]
```

### Reglas de Transición:
1. Solo citas en `Pendiente` o `Confirmada` pueden pasar a `Cancelada`.
2. Una cita en `En_Progreso` no puede ser cancelada; debe concluir como `Completada`.
3. Solo citas en `Completada` permiten la emisión de la factura final y el cobro del saldo pendiente.

---

## 4. Disparadores (Triggers) de Base de Datos

### Trigger `on_auth_user_created`
* **Tabla Origen:** `auth.users`
* **Operación:** `AFTER INSERT`
* **Acción:** Ejecuta la función `handle_new_user()` que extrae `id`, `email`, y metadata (`full_name`, `phone`) para insertar automáticamente en `public.usuarios` con rol `Cliente` y en `public.clientes`.
