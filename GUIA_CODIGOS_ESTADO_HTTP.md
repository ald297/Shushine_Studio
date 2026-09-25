# 🌐 Guía Técnica y Exhaustiva de Códigos de Estado HTTP y Manejo de Errores

> **Documento de Investigación y Estándar Académico**  
> **Proyecto:** Shushine Studio — Sistema Móvil de Gestión y Reservas para Salón de Belleza  
> **Institución:** Escuela Superior Franciscana Especializada – AGAPE (ESFE AGAPE / MEGATEC)  
> **Carrera:** Técnico en Ingeniería de Desarrollo de Software | **Módulo:** Construcción de APIs Web  
> **Equipo:** Alex Fernando Alfaro Diaz (Backend / Scrum Master) & Camila Antonia Calderon Cortez (Móvil / QA)  

---

## 1. Introducción y Fundamentos del Protocolo HTTP

En la arquitectura cliente-servidor de la Web y las APIs RESTful, el protocolo **HTTP (Hypertext Transfer Protocol)** define un conjunto de reglas para la transmisión de mensajes. Cada vez que una aplicación cliente (como nuestra app móvil en **.NET MAUI**) realiza una petición a un servidor backend (nuestra Web API en **Java 21 Spring Boot 3.3.3**), el servidor está obligado a responder con un **Código de Estado HTTP (*HTTP Status Code*)**.

Un código de estado HTTP es un número de **3 dígitos** estandarizado internacionalmente por el **IETF (Internet Engineering Task Force)** a través de las especificaciones **RFC 9110** (que actualiza las anteriores RFC 7231 y RFC 2616). 

### Anatomía de una Respuesta HTTP

```http
HTTP/1.1 409 Conflict                          <-- Status Line (Protocolo, Código, Frase)
Content-Type: application/problem+json         <-- Response Headers (Metadatos)
Date: Fri, 25 Sep 2026 10:00:00 GMT

{                                              <-- Response Body (Cuerpo de datos o error)
  "type": "https://httpstatuses.io/409",
  "title": "Conflict",
  "status": 409,
  "detail": "El estilista seleccionado ya no tiene disponibilidad en este horario.",
  "instance": "/api/reservas"
}
```

---

## 2. Las 5 Familias / Clases Oficiales de Códigos HTTP

El primer dígito del código de 3 dígitos define la **clase o familia** de la respuesta:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                      LAS 5 FAMILIAS DE CÓDIGOS HTTP                         │
├──────────┬─────────────────────────────┬────────────────────────────────────┤
│ Familia  │ Categoría Oficial           │ Significado y Responsabilidad      │
├──────────┼─────────────────────────────┼────────────────────────────────────┤
│  1xx     │ Respuestas Informativas     │ Petición recibida; el proceso      │
│          │ (Informational)             │ continúa en segundo plano.         │
├──────────┼─────────────────────────────┼────────────────────────────────────┤
│  2xx     │ Éxito                       │ La acción solicitada fue recibida, │
│          │ (Successful)                │ entendida y aceptada con éxito.    │
├──────────┼─────────────────────────────┼────────────────────────────────────┤
│  3xx     │ Redirección                 │ Se requiere acción adicional del   │
│          │ (Redirection)               │ cliente para completar la solicitud│
├──────────┼─────────────────────────────┼────────────────────────────────────┤
│  4xx     │ Errores del Cliente         │ La petición tiene errores o no     │
│          │ (Client Errors)             │ cumple con las reglas requeridas.  │
├──────────┼─────────────────────────────┼────────────────────────────────────┤
│  5xx     │ Errores del Servidor        │ La petición era válida, pero el    │
│          │ (Server Errors)             │ servidor falló internamente.       │
└──────────┴─────────────────────────────┴────────────────────────────────────┘
```

---

## 3. Códigos de Éxito Más Utilizados (Familia 2xx)

Son los códigos que confirman que la transacción se realizó sin inconvenientes:

| Código | Nombre Oficial | Descripción y Cuándo Ocurre | Ejemplo en Shushine Studio |
| :---: | :--- | :--- | :--- |
| **`200`** | **OK** | Petición estándar exitosa. El cuerpo contiene los datos solicitados. | `GET /api/servicios` devuelve la lista de tratamientos de belleza. |
| **`201`** | **Created** | La petición resultó en la creación exitosa de un nuevo recurso. Devuelve la entidad creada o su cabecera `Location`. | `POST /api/reservas` agenda una cita y retorna el comprobante `#SHU-1024`. |
| **`202`** | **Accepted** | La petición fue aceptada para procesamiento, pero este no ha concluido (tareas asíncronas / batch). | Envío de recordatorio masivo por correo en segundo plano. |
| **`204`** | **No Content** | La petición se procesó con éxito, pero la respuesta intencionalmente no incluye contenido en el cuerpo. | `DELETE /api/servicios/5` elimina un servicio; no hay nada que retornar. |

---

## 4. Códigos de Redirección (Familia 3xx)

| Código | Nombre Oficial | Descripción y Cuándo Ocurre |
| :---: | :--- | :--- |
| **`301`** | **Moved Permanently** | El recurso fue movido permanentemente a una nueva URI. El cliente debe actualizar su referencia. |
| **`302`** | **Found (Temporary Redirect)** | El recurso reside temporalmente bajo una URI diferente. |
| **`304`** | **Not Modified** | El recurso no ha cambiado desde la última descarga (usado intensamente con cabeceras de caché `ETag` o `If-Modified-Since` para ahorrar datos en la app móvil). |

---

## 5. Desglose Exhaustivo de Errores del Cliente (Familia 4xx)

> **Regla de Diagnóstico:** Cuando ocurre un error **4xx**, la causa raíz está en la **solicitud enviada por el cliente** (.NET MAUI, Postman, etc.). El servidor rechazó la solicitud legítimamente debido a sintaxis inválida, falta de autenticación, permisos insuficientes o violación de reglas del negocio.

---

### 🔴 `400 Bad Request` (Solicitud Incorrecta)
* **Definición:** El servidor no puede procesar la solicitud debido a un error evidente del cliente.
* **Causas Comunes:**
  * Cuerpo JSON con sintaxis rota (falta una coma `,`, comilla o llave `}`).
  * Tipos de datos incompatibles (ej. enviar texto `"mucho"` en un campo numérico `precio`).
  * Parámetros obligatorios ausentes en la consulta (*Missing Query Param*).
* **Ejemplo en el Salón:** Enviar una petición a `POST /api/auth/registro` sin el campo obligatorio `email`.

---

### 🔒 `401 Unauthorized` (No Autenticado / Falta de Identidad)
* **Definición:** La petición requiere autenticación del usuario, pero el cliente **no suministró credenciales o las suministradas son inválidas**.
* **Causas Comunes:**
  * No se incluyó la cabecera `Authorization: Bearer <token>`.
  * El token JWT ha expirado (*Expired Token*).
  * La firma criptográfica del JWT es inválida o fue manipulada.
* **Ejemplo en el Salón:** La clienta abre la app tras 30 días; su token expiró e intenta consultar sus citas en `GET /api/reservas/cliente/5`. La API responde `401 Unauthorized`.

---

### 🚫 `403 Forbidden` (Prohibido / Sin Autorización)
* **Definición:** El servidor conoce la identidad del usuario (está autenticado), pero **su rol no posee los permisos requeridos** para ejecutar la acción.
* **Causas Comunes:**
  * Un usuario con rol `CLIENTE` intenta consumir endpoints administrativos protegidos con `@PreAuthorize("hasRole('ADMIN')")`.
* **Ejemplo en el Salón:** Una clienta descubre la URL `GET /api/admin/dashboard` e intenta ingresar; el backend Java valida su JWT, detecta que su rol es `CLIENTE` y responde `403 Forbidden`.

---

### 🔍 `404 Not Found` (Recurso No Encontrado)
* **Definición:** El servidor no encontró el recurso solicitado en la base de datos o la ruta URL del endpoint no existe.
* **Causas Comunes:**
  * ID inexistente en la tabla relacional.
  * Error tipográfico en la URL (ej. escribir `/api/servisios` en lugar de `/api/servicios`).
* **Ejemplo en el Salón:** Consultar `GET /api/servicios/999` cuando el ID más alto registrado en PostgreSQL es 15.

---

### 🛑 `405 Method Not Allowed` (Método HTTP No Permitido)
* **Definición:** El recurso existe, pero no admite el verbo HTTP empleado (`GET`, `POST`, `PUT`, `DELETE`).
* **Causas Comunes:**
  * Intentar hacer un `POST` sobre un endpoint configurado únicamente con `@GetMapping`.
* **Ejemplo en el Salón:** Enviar una petición `DELETE` a `/api/disponibilidad`.

---

### ⏱️ `408 Request Timeout` (Tiempo de Espera del Cliente Agotado)
* **Definición:** El servidor estuvo esperando la transmisión de la petición por parte del cliente, pero se cumplió el tiempo límite sin que el cliente terminara de enviarla.
* **Causas Comunes:**
  * Conexión a internet inestable o extremadamente lenta del dispositivo móvil durante el envío de un payload grande.

---

### ⚔️ `409 Conflict` (Conflicto de Estado / Concurrencia)
* **Definición:** La solicitud es sintáctica y semánticamente válida, pero no puede procesarse porque entra en **conflicto directo con el estado actual de los datos**.
* **Causas Comunes:**
  * **Concurrencia / Overbooking:** Dos clientes intentan reservar exactamente la misma franja horaria con el mismo estilista en el mismo segundo.
  * Violación de restricción de unicidad (`UNIQUE` constraint) en base de datos.
* **Ejemplo en el Salón:** El motor de transacciones ACID de Java detecta que la cabina estética ya fue ocupada por otra transacción confirmada milisegundos antes; aborta la segunda reserva y devuelve `409 Conflict`.

---

### 📄 `415 Unsupported Media Type` (Tipo de Contenido No Soportado)
* **Definición:** El formato de datos enviado en la cabecera `Content-Type` no es aceptado por el endpoint.
* **Causas Comunes:**
  * Enviar un cuerpo como `text/plain` o `application/xml` a un controlador que requiere `application/json` (`consumes = MediaType.APPLICATION_JSON_VALUE`).

---

### 🧩 `422 Unprocessable Entity` (Entidad No Procesable / Error de Negocio)
* **Definición:** El JSON tiene una sintaxis perfecta, pero contiene **errores semánticos o infringe reglas de negocio del dominio**.
* **Causas Comunes:**
  * Enviar una fecha de reserva en el pasado (ej. `2020-01-01`).
  * Enviar una hora de cita fuera de la jornada laboral del estilista (ej. 03:00 AM).
  * El teléfono no cumple con el formato internacional requerido (`+503...`).

---

### 🚦 `429 Too Many Requests` (Demasiadas Peticiones / Rate Limiting)
* **Definición:** El cliente superó el límite de peticiones permitidas en una ventana de tiempo estipulada.
* **Causas Comunes:**
  * Ataques de fuerza bruta en el login o escaneos automáticos de endpoints.

---

## 6. Desglose Exhaustivo de Errores del Servidor (Familia 5xx)

> **Regla de Diagnóstico:** Cuando ocurre un error **5xx**, la responsabilidad recae en el **servidor, la base de datos o la infraestructura de red**. La solicitud del cliente era totalmente válida, pero el backend sufrió una falla imprevista al procesarla.

---

### 💥 `500 Internal Server Error` (Error Interno del Servidor)
* **Definición:** El servidor encontró una condición inesperada que le impidió completar la solicitud.
* **Causas Comunes:**
  * Excepciones no controladas en el código Java (`NullPointerException`, `ArrayIndexOutOfBoundsException`).
  * Fallo en la lógica de persistencia de Hibernate / Spring Data JPA no capturado.
* **Ejemplo en el Salón:** Un servicio intenta leer `.getNombre()` sobre un objeto `Cliente` que vino nulo sin haber hecho la validación previa.

---

### 🚪 `502 Bad Gateway` (Puerta de Enlace Incorrecta)
* **Definición:** Un servidor intermedio (Proxy inverso, Nginx, Azure API Gateway o Cloudflare) recibió una respuesta inválida o nula del servidor backend de destino.
* **Causas Comunes:**
  * La aplicación Java Spring Boot colapsó, se apagó o está reiniciándose justo cuando el Gateway le envió la petición.
  * El puerto interno de la API no está escuchando peticiones.

---

### 🚧 `503 Service Unavailable` (Servicio No Disponible)
* **Definición:** El servidor no está en condiciones de atender la solicitud en este momento por encontrarse sobrecargado o en mantenimiento.
* **Causas Comunes:**
  * El servidor alcanzó el límite máximo de hilos (*Thread Pool Exhaustion*).
  * La base de datos PostgreSQL de Supabase agotó sus conexiones concurrentes (*Connection Pool Exhaustion*).
  * Mantenimiento programado de la infraestructura.

---

### ⏳ `504 Gateway Timeout` (Tiempo de Espera de la Puerta de Enlace Agotado)
* **Definición:** El servidor proxy o puerta de enlace no recibió una respuesta a tiempo de un servidor secundario (backend o base de datos) al que consultó para procesar la petición.
* **Causas Comunes:**
  * Una consulta SQL en PostgreSQL es tan pesada o está bloqueada por un cerrojo (*deadlock*) que tarda más de 30 segundos, provocando que el Gateway corte la conexión.
  * Un servicio externo (como pasarela de pagos o API de SMS) no responde a tiempo.

---

## 7. Diferencias Críticas para Evaluaciones Técnicas

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    COMPARATIVAS CLAVE PARA EXPOSICIONES                     │
├──────────────────────────┬──────────────────────────────────────────────────┤
│ Códigos en Conflicto     │ Diferencia Conceptual Fundamental                │
├──────────────────────────┼──────────────────────────────────────────────────┤
│ 401 vs 403               │ • 401 Unauthorized: "No sé quién eres".         │
│                          │   (Falta autenticación / Token JWT expirado).    │
│                          │ • 403 Forbidden: "Sé quién eres, pero no puedes".│
│                          │   (Autenticado, pero rol sin privilegios).       │
├──────────────────────────┼──────────────────────────────────────────────────┤
│ 400 vs 422               │ • 400 Bad Request: Error sintáctico en el JSON.  │
│                          │ • 422 Unprocessable: JSON válido, pero datos     │
│                          │   violan reglas de negocio (ej. fecha pasada).   │
├──────────────────────────┼──────────────────────────────────────────────────┤
│ 408 vs 504               │ • 408 Timeout: El cliente móvil tardó en enviar. │
│                          │ • 504 Timeout: El backend/base de datos tardó    │
│                          │   demasiado en responderle al Gateway.           │
├──────────────────────────┼──────────────────────────────────────────────────┤
│ 500 vs 502               │ • 500 Error: El código Java lanzó una excepción. │
│                          │ • 502 Bad Gateway: La API está apagada o caída   │
│                          │   detrás del proxy/servidor web.                 │
└──────────────────────────┴──────────────────────────────────────────────────┘
```

---

## 8. El Estándar Moderno: RFC 7807 (Problem Details for HTTP APIs)

En el desarrollo profesional de APIs RESTful, las respuestas de error **nunca deben devolverse como texto plano o estructuras desordenadas**. Se debe implementar el estándar **RFC 7807 / RFC 9457 (`application/problem+json`)**.

### Estructura JSON Oficial del Estándar RFC 7807

```json
{
  "type": "https://httpstatuses.io/409",
  "title": "Conflict",
  "status": 409,
  "detail": "El estilista seleccionado ya no tiene disponibilidad en la franja horaria solicitada.",
  "instance": "/api/reservas",
  "errors": {
    "horaInicio": ["El bloque de 14:00 a 14:45 ya fue confirmado por otra cita."]
  }
}
```

* **`type` (URI):** Identificador universal de la categoría del error.
* **`title` (String):** Resumen corto y legible del código HTTP.
* **`status` (Number):** El código numérico HTTP oficial (400, 404, 409, etc.).
* **`detail` (String):** Explicación clara y detallada en español para el usuario o desarrollador.
* **`instance` (URI):** Endpoint específico donde ocurrió el incidente.
* **`errors` (Object):** Mapa detallado de validaciones fallidas por campo.

---

## 9. Implementación en Shushine Studio (Java Spring Boot + .NET MAUI)

### 9.1 Manejador Global en Backend (Java Spring Boot 3)

Se implementa una clase centralizada con la anotación `@RestControllerAdvice` que captura las excepciones y devuelve un `ProblemDetail` nativo de Spring Boot 3:

```java
package com.shushinestudio.config;

import org.springframework.http.HttpStatus;
import org.springframework.http.ProblemDetail;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import java.net.URI;

@RestControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(CitaConflictoException.class)
    public ProblemDetail manejarConflictoCita(CitaConflictoException ex) {
        ProblemDetail problem = ProblemDetail.forStatusAndDetail(
            HttpStatus.CONFLICT, 
            ex.getMessage()
        );
        problem.setTitle("Conflicto de Disponibilidad");
        problem.setType(URI.create("https://httpstatuses.io/409"));
        problem.setProperty("instance", "/api/reservas");
        return problem;
    }

    @ExceptionHandler(RecursoNoEncontradoException.class)
    public ProblemDetail manejarNoEncontrado(RecursoNoEncontradoException ex) {
        ProblemDetail problem = ProblemDetail.forStatusAndDetail(
            HttpStatus.NOT_FOUND, 
            ex.getMessage()
        );
        problem.setTitle("Recurso No Encontrado");
        problem.setType(URI.create("https://httpstatuses.io/404"));
        return problem;
    }
}
```

---

### 9.2 Interceptor de Errores en Frontend (.NET MAUI con `DelegatingHandler`)

En la app móvil, el cliente HTTP cuenta con un `ErrorDelegatingHandler` que deserializa automáticamente las respuestas RFC 7807 y reacciona según el código:

```csharp
public class ErrorDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // Si el código es 401: Token expirado -> limpiar sesión y redirigir a Login
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.RemoveAll();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            // Si el código es 403: Rol sin permisos
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Acceso Denegado", 
                    "No posees los privilegios de administrador para esta acción.", 
                    "Aceptar"
                );
            }
            // Errores de negocio (400, 409, 422): Extraer ProblemDetails.detail
            else if (response.StatusCode == HttpStatusCode.Conflict || 
                     response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problem = JsonSerializer.Deserialize<ProblemDetailsDto>(content);
                await Application.Current.MainPage.DisplayAlert(
                    "Aviso del Salón", 
                    problem?.Detail ?? "Operación no permitida.", 
                    "Entendido"
                );
            }
            // Errores de Servidor (500, 502, 503, 504)
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de Conexión", 
                    "El servidor del salón no se encuentra disponible. Intente más tarde.", 
                    "Reintentar"
                );
            }
        }

        return response;
    }
}
```

---

## 10. Matriz Resumen de Consulta Rápida (Cheat Sheet)

| Código | Nombre | Familia | ¿Quién tiene la culpa? | Causa Típica | Acción Recomendada |
| :---: | :--- | :---: | :---: | :--- | :--- |
| **`200`** | **OK** | 2xx | Ninguno | Consulta exitosa. | Renderizar lista en pantalla. |
| **`201`** | **Created** | 2xx | Ninguno | Registro creado con éxito. | Navegar a confirmación o ticket. |
| **`204`** | **No Content** | 2xx | Ninguno | Eliminación o acción sin cuerpo. | Refrescar vista actual. |
| **`400`** | **Bad Request** | 4xx | Cliente | JSON mal formado o tipos inválidos. | Revisar serialización del modelo. |
| **`401`** | **Unauthorized** | 4xx | Cliente | Token no enviado o vencido. | Redirigir a pantalla de Login. |
| **`403`** | **Forbidden** | 4xx | Cliente | Rol sin privilegios suficientes. | Bloquear vista o mostrar alerta. |
| **`404`** | **Not Found** | 4xx | Cliente | ID o Endpoint inexistente. | Mostrar mensaje "Elemento no hallado". |
| **`405`** | **Method Not Allowed** | 4xx | Cliente | Método HTTP equivocado (POST vs GET).| Corregir el método en el cliente. |
| **`409`** | **Conflict** | 4xx | Cliente / Negocio | Concurrencia o dato duplicado. | Pedir al usuario seleccionar otro horario. |
| **`422`** | **Unprocessable Entity** | 4xx | Cliente | Regla de negocio violada. | Mostrar mensaje de validación. |
| **`429`** | **Too Many Requests** | 4xx | Cliente | Límite de peticiones superado. | Implementar espera exponencial (*Backoff*). |
| **`500`** | **Internal Server Error** | 5xx | Servidor | Excepción no capturada en Java. | Revisar logs del backend en consola/Azure. |
| **`502`** | **Bad Gateway** | 5xx | Servidor / Red | Backend caído detrás del Gateway. | Verificar que el servicio Spring Boot corra. |
| **`503`** | **Service Unavailable** | 5xx | Servidor | Sobrecarga o mantenimiento. | Mostrar pantalla de mantenimiento temporal. |
| **`504`** | **Gateway Timeout** | 5xx | Servidor / DB | Base de datos lenta o bloqueada. | Optimizar consultas e índices en PostgreSQL. |
