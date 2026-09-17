# 📋 Recordatorios y Guía de Configuración en Supabase — Shunshine Studio

> **Ubicación:** Raíz del proyecto (`RECORDATORIOS_SUPABASE.md`)  
> **Objetivo:** Documentar todas las acciones manuales, configuraciones del panel y scripts SQL que deben ejecutarse en la consola de **Supabase** para garantizar la integración fluida con la **Web API C#** y la **App Móvil Flutter**.

---

## 📌 Índice de Tareas en Supabase

1. [Ejecución del Script DDL de Base de Datos](#1-ejecución-del-script-ddl-de-base-de-datos)
2. [Sincronización Automática de Usuarios (Trigger PostgreSQL)](#2-sincronización-automática-de-usuarios-trigger-postgresql)
3. [Inyección de Roles en el JWT (Supabase Auth Hook)](#3-inyección-de-roles-en-el-jwt-supabase-auth-hook)
4. [Protección RLS (Row Level Security)](#4-protección-rls-row-level-security)
5. [Ajustes de Autenticación en el Dashboard](#5-ajustes-de-autenticación-en-el-dashboard)
6. [Almacenamiento de Archivos (Supabase Storage - 6 Buckets)](#6-almacenamiento-de-archivos-supabase-storage---6-buckets)
7. [Extracción de Secretos para Variables de Entorno](#7-extracción-de-secretos-para-variables-de-entorno)

---

## 1. Ejecución del Script DDL de Base de Datos

* **Dónde:** Panel de Supabase $\rightarrow$ Menú lateral **SQL Editor** $\rightarrow$ **+ New query**.
* **Qué hacer:** Copiar y ejecutar todo el script DDL documentado en [`docs/DIAGRAMA_BASE_DE_DATOS.md`](./docs/DIAGRAMA_BASE_DE_DATOS.md#4-script-ddl-para-supabase-postgresql-15) o [`docs/migration_23_tables_shunshine.sql`](./docs/migration_23_tables_shunshine.sql).
* **Verificación:** Ir a **Table Editor** y comprobar que existan las **23 tablas**:
  * `roles`, `usuarios`, `clientes`, `categorias`, `servicios`, `productos`, `estilistas`, `estilista_servicios`, `horarios_estilistas`, `bloqueos_horarios`, `personal_portafolios`, `citas`, `cita_servicios`, `solicitudes_diseno`, `cotizaciones`, `mensajes_chat_cita`, `resenas`, `transacciones_puntos`, `servicio_productos_rec`, `favoritos`, `pagos`, `facturas`, `auditoria_logs`.

---

## 2. Sincronización Automática de Usuarios (Trigger PostgreSQL)

### ⚠️ El Problema
Cuando un cliente se registra en Flutter mediante:
```dart
await supabase.auth.signUp(
  email: email,
  password: password,
  data: {
    'nombre_completo': 'María López',
    'telefono': '7012-3456',
  },
);
```
Supabase crea el usuario en la tabla interna `auth.users`, pero **no** en `public.usuarios` ni en `public.clientes`. Sin un trigger, el usuario existirá para iniciar sesión pero no tendrá perfil de cliente en la base de datos del negocio.

### ✅ La Solución (Ejecutada automáticamente en la migración oficial)

```sql
-- 1. Función que inserta automáticamente en public.usuarios y public.clientes
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER 
SECURITY DEFINER
SET search_path = public
LANGUAGE plpgsql
AS $$
DECLARE
    v_id_rol_cliente INT;
    v_nombre_completo VARCHAR(150);
    v_telefono VARCHAR(20);
    v_cliente_existente_id INT;
BEGIN
    -- Obtener el ID del rol 'Cliente'
    SELECT id_rol INTO v_id_rol_cliente FROM public.roles WHERE nombre = 'Cliente' LIMIT 1;
    
    -- Extraer metadatos de auth.users
    v_nombre_completo := COALESCE(NEW.raw_user_meta_data->>'nombre_completo', split_part(NEW.email, '@', 1));
    v_telefono := COALESCE(NEW.raw_user_meta_data->>'telefono', '');

    -- Insertar en public.usuarios con id_usuario = NEW.id (UUID)
    INSERT INTO public.usuarios (
        id_usuario,
        id_rol,
        nombre_completo,
        correo,
        telefono,
        activo,
        fecha_registro
    )
    VALUES (
        NEW.id,
        v_id_rol_cliente,
        v_nombre_completo,
        NEW.email,
        v_telefono,
        TRUE,
        NOW()
    );

    -- Verificar si ya existía un cliente walk-in con el mismo teléfono para asociarlo retrospectivamente
    IF v_telefono <> '' THEN
        SELECT id_cliente INTO v_cliente_existente_id 
        FROM public.clientes 
        WHERE telefono_walkin = v_telefono AND id_usuario IS NULL 
        LIMIT 1;
    END IF;

    IF v_cliente_existente_id IS NOT NULL THEN
        -- Vincular cliente walk-in existente a la nueva cuenta
        UPDATE public.clientes 
        SET id_usuario = NEW.id,
            es_walkin = FALSE
        WHERE id_cliente = v_cliente_existente_id;
    ELSE
        -- Crear nuevo perfil de cliente
        INSERT INTO public.clientes (
            id_usuario,
            nivel_fidelidad,
            puntos_acumulados,
            es_walkin,
            fecha_creacion
        )
        VALUES (
            NEW.id,
            'Bronce',
            0,
            FALSE,
            NOW()
        );
    END IF;

    RETURN NEW;
END;
$$;

-- 2. Trigger que se activa al registrar un usuario en auth.users
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;

CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();
```

---

## 3. Inyección de Roles en el JWT (Supabase Auth Hook)

### ⚠️ El Problema
Por defecto, el token JWT de Supabase contiene claims como `sub`, `email` y `aud`, pero **no incluye el claim `"role"` con el rol del negocio** (`Cliente`, `Administrador`, `Recepcionista`). Esto complica la autorización en ASP.NET Core con atributos como `[Authorize(Roles = "Administrador")]`.

### ✅ La Solución (Auth Hook en PostgreSQL)

Supabase permite usar un **Custom Access Token Hook** para inyectar claims en el JWT antes de emitirlo.

```sql
-- Función Hook para agregar rol y id_usuario a los claims del JWT
CREATE OR REPLACE FUNCTION public.custom_access_token_hook(event jsonb)
RETURNS jsonb
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    claims jsonb;
    user_role text;
BEGIN
    SELECT r.nombre
    INTO user_role
    FROM public.usuarios u
    JOIN public.roles r ON u.id_rol = r.id_rol
    WHERE u.id_usuario = (event->>'user_id')::uuid;

    claims := event->'claims';

    IF user_role IS NOT NULL THEN
        claims := jsonb_set(claims, '{user_role}', to_jsonb(user_role));
        claims := jsonb_set(claims, '{role}', to_jsonb(user_role));
    END IF;

    event := jsonb_set(event, '{claims}', claims);
    RETURN event;
END;
$$;

-- Otorgar permisos al usuario de auth de Supabase
GRANT USAGE ON SCHEMA public TO supabase_auth_admin;
GRANT EXECUTE ON FUNCTION public.custom_access_token_hook TO supabase_auth_admin;
REVOKE EXECUTE ON FUNCTION public.custom_access_token_hook FROM authenticated, anon, public;
```

* **Paso en el Dashboard de Supabase:**
  1. Ir a **Authentication** $\rightarrow$ **Hooks**.
  2. En **Custom Access Token**, habilitar el hook seleccionando la función `public.custom_access_token_hook`.

---

## 4. Protección RLS (Row Level Security)

* La app móvil posee la clave pública `anon_key` para conectarse a Supabase Auth.
* Para evitar que un usuario haga llamadas HTTP directas a la API REST de Supabase (`https://<id>.supabase.co/rest/v1/...`) y se salte la lógica de negocio de la API en C#:
  * **Verificación:** Todas las 23 tablas del esquema `public` tienen la etiqueta **"RLS Enabled"** y **0 políticas anónimas directas de escritura**.
  * La **Web API en C# se conecta por PostgreSQL Connection String** (rol `postgres`), por lo cual tiene acceso total para transacciones y validaciones de negocio, ignorando las restricciones que bloquean al cliente móvil directo.

---

## 5. Ajustes de Autenticación en el Dashboard

Ir a **Authentication** $\rightarrow$ **Providers** $\rightarrow$ **Email**:
1. **Confirm email:** 
   * Para **Desarrollo/Testing**: Desactivar temporalmente el toggle *"Confirm email"* para que Alex y Camila puedan registrar clientes y probar el login inmediatamente sin esperar correos de activación.
   * Para **Producción**: Activar *"Confirm email"*.
2. **Secure password requirements:**
   * Longitud mínima recomendada: 6 u 8 caracteres.
3. **URL Configuration:**
   * Site URL: `http://localhost` (o el scheme de Flutter para deep linking si se usa en el futuro).

---

## 6. Almacenamiento de Archivos (Supabase Storage - 6 Buckets)

Se encuentran configurados los 6 buckets en Supabase Storage:

| Bucket | Finalidad | Acceso Lectura | Acceso Escritura | Tamaño Máx. | Formatos |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`servicios-imagenes`** | Fichas ilustrativas del catálogo | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| **`categorias-imagenes`** | Banners e íconos de categorías | Público (`anon`) | Solo Admin (`role = Admin`) | 2 MB | JPG, PNG, WebP |
| **`personal-avatares`** | Avatares y fotos del personal | Público (`anon`) | Solo Admin (`role = Admin`) | 2 MB | JPG, PNG, WebP |
| **`personal-portafolio`** | Galería de trabajos realizados | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| **`productos-imagenes`** | Fotos de productos de inventario | Público (`anon`) | Solo Admin (`role = Admin`) | 5 MB | JPG, PNG, WebP |
| **`disenos-referencias`** | Referencias privadas para cotizar | Privado (Propietaria + Admin) | Solo Cliente (`/uid/*`) y Admin | 5 MB | JPG, PNG, WebP |

---

## 7. Extracción de Secretos para Variables de Entorno

Copiar los valores desde **Project Settings $\rightarrow$ Database (Connection String)** y colocarlos en los archivos locales (protegidos por `.gitignore`):

### Para el Backend C# (`appsettings.Development.json` o `dotnet user-secrets`):
```json
{
  "ConnectionStrings": {
    "SupabasePostgres": "Host=aws-0-us-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.acikahicfjtojuvqcvxv;Password=<TU_PASSWORD_DB>;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Supabase": {
    "Url": "https://acikahicfjtojuvqcvxv.supabase.co",
    "AnonKey": "eyJhbGciOi..."
  },
  "Jwt": {
    "Authority": "https://acikahicfjtojuvqcvxv.supabase.co/auth/v1",
    "Audience": "authenticated"
  }
}
```

### Para el Frontend Flutter (`--dart-define` o `.env.local`):
```env
SUPABASE_URL=https://acikahicfjtojuvqcvxv.supabase.co
SUPABASE_ANON_KEY=eyJhbGciOi...
API_BASE_URL=http://localhost:5000/api
```

### Formato URI para Scripts de Migración y Herramientas:
```bash
postgresql://postgres.acikahicfjtojuvqcvxv:<TU_PASSWORD_DB>@aws-0-us-east-1.pooler.supabase.com:5432/postgres
```
