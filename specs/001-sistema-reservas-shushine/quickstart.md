# Guía Rápida de Pruebas y Validación: Shushine Studio API & Móvil

> **Propósito:** Guía paso a paso para que el docente de ESFE AGAPE o cualquier evaluador pueda ejecutar el backend, validar los datos del semillero automático (`DataInitializer`), probar los endpoints interactivos con JWT en Swagger UI, ejecutar las pruebas unitarias y conectar la aplicación móvil en .NET MAUI.

---

## 1. Prerrequisitos de Entorno

* **Java SDK:** Java 21 LTS (`java -version`) con Maven o Wrapper (`mvnw`), y/o **.NET SDK:** .NET 8.0/9.0 (`dotnet --version`).
* **.NET MAUI Workload:** Workload de .NET MAUI instalado (`dotnet workload install maui`).
* **Navegador Web:** Chrome / Brave / Edge para Swagger UI y consola H2.
* **Dispositivo / Emulador:** Emulador Android / iOS, Windows o dispositivo físico para la app móvil.

---

## 2. Puesta en Marcha del Backend y Semillero Automático

Al iniciar el backend, el componente **`DataInitializer`** detecta automáticamente si las tablas en **PostgreSQL Supabase** están vacías e inserta:
* **Roles estipulados de Shushine Studio:** `ADMIN` y `CLIENTE` (con soporte para `RECEPCIONISTA`).
* **Usuarios de prueba:**
  * **Administrador del Salón:** Usuario: `admin` | Contraseña: `admin123` (Rol: `ADMIN`)
  * **Cliente del Salón:** Usuario: `cliente` | Contraseña: `cliente123` (Rol: `CLIENTE`)
* **Datos de prueba del salón:** Categorías de belleza (*"Corte y Peinado"*, *"Colorimetría"*, *"Uñas y Manicura"*, *"Cuidado Facial"*), servicios base con duración y tarifas, estilistas y citas de demostración.

### 2.1 Ejecutar el Backend

```bash
# En el directorio del backend (Java 21 / Spring Boot 3.3.3):
./mvnw spring-boot:run
```

* **Puerto por defecto:** `http://localhost:8080`
* **Conexión a Base de Datos:** PostgreSQL en Supabase (AWS Pooler directo).

---

## 3. Pruebas Interactivas de Endpoints con Swagger UI

El proyecto cuenta con **OpenAPI / Swagger UI con soporte Bearer Token** idéntico a la guía de referencia de ESFE AGAPE.

### Paso 1: Abrir Swagger UI
Abrir en el navegador:
```
http://localhost:8080/swagger-ui/index.html
```

### Paso 2: Autenticación y Obtención de Token JWT
1. En Swagger, buscar la sección **`Auth`** y desplegar `POST /api/auth/login`.
2. Presionar **"Try it out"** y enviar las credenciales del Administrador:
   ```json
   {
     "login": "admin",
     "clave": "admin123"
   }
   ```
3. Presionar **"Execute"**. En la respuesta (código `200 OK`) se recibirá:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }
   ```
4. Copiar únicamente el valor del token.

### Paso 3: Autorizar la Sesión en Swagger
1. Subir al inicio de la página de Swagger y hacer clic en el botón verde **"Authorize"** (con el candado).
2. Pegar el token copiado en el campo `Value` y hacer clic en **"Authorize"**.
3. Cerrar el modal. Ahora todas las peticiones incluirán automáticamente la cabecera:
   `Authorization: Bearer <token>`

### Paso 4: Probar Endpoints CRUD y Paginados
* **Consultar Categorías (Paginado):** Desplegar `GET /api/categorias`, indicar `page = 0`, `size = 10` y presionar **"Execute"**.
* **Consultar Categorías (Lista Rápida):** Desplegar `GET /api/categorias/lista` y presionar **"Execute"**.
* **Crear Nuevo Servicio / Categoría:** Desplegar `POST /api/categorias` con el DTO `CategoriaGuardar`:
   ```json
   {
     "nombre": "Tratamientos Capilares Avanzados"
   }
   ```
* **Verificar Control de Roles (RBAC):**
  * Si te autenticas con el usuario `user` (`user123`), las peticiones `GET` responderán `200 OK`, mientras que peticiones `POST`, `PUT` o `DELETE` responderán `403 Forbidden`.

---

## 4. Ejecución de la Suite de Pruebas Unitarias

Siguiendo el estándar de la guía de ESFE AGAPE, la lógica de negocio en la capa de servicios cuenta con pruebas unitarias secuenciales (`t1_crear` a `t6_eliminarPorId`):

```bash
# Ejecutar todas las pruebas unitarias:
./mvnw test

# O en .NET:
dotnet test
```

### Verificación de Pruebas:
* `t1_crear()`: Valida que el servicio cree la entidad a partir del DTO `Guardar` y no retorne nulo.
* `t2_obtenerTodos()`: Valida que la lista general contenga registros.
* `t3_obtenerTodosPaginados()`: Valida que el objeto `Page<T>` retorne elementos con metadatos.
* `t4_obtenerPorId()`: Valida la búsqueda individual por identificador.
* `t5_editar()`: Valida la modificación de datos mediante el DTO `Modificar`.
* `t6_eliminarPorId()`: Valida que la eliminación no lance excepciones.

---

## 5. Conexión de la Aplicación Móvil (.NET MAUI)

La app móvil en .NET MAUI consume los mismos endpoints que Swagger mediante su cliente HTTP con **`HttpClient`** y delegados:

```bash
# Ejecutar la app móvil en .NET MAUI:
dotnet build -t:Run -f net8.0-android
# (O ejecutar en Windows / iOS mediante Visual Studio / CLI)
```

1. La página de **Login** de .NET MAUI consume `POST /api/auth/login`.
2. El token JWT retornado es almacenado de forma segura en `Microsoft.Maui.Storage.SecureStorage`.
3. El manejador HTTP `ErrorDelegatingHandler` adjunta automáticamente el token en cada llamada subsecuente (`GET /api/servicios`, `POST /api/citas`, etc.).
4. Si el token expira (error 401), el interceptor redirige al usuario a la pantalla de Login de forma transparente.ige al usuario a la pantalla de Login de forma transparente.
