# Implementation Plan: Sistema Móvil de Gestión y Reservas Shushine Studio

**Branch**: `001-sistema-reservas-shushine` | **Date**: 2026-09-11 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification de Shushine Studio, arquitectura [AGENTS.md](../../AGENTS.md) y constitución [.specify/memory/constitution.md](../../.specify/memory/constitution.md)

---

## Summary

Implementación integral del sistema móvil de reservas y gestión operativa para el salón de belleza Shushine Studio. Combina una aplicación móvil en Flutter bajo Clean Architecture y BLoC para la experiencia del cliente y estilistas, con una Web API RESTful construida con la arquitectura de capas y patrones de la guía oficial de ESFE AGAPE (`JavaControlProyectosAPI`): DTOs estandarizados por acción (`Guardar`, `Modificar`, `Salida`, `CambiarEstado`), semillero automático (`DataInitializer`), endpoints paginados (`Pageable`) y listas rápidas, documentación OpenAPI/Swagger con Bearer JWT interactivo, y pruebas unitarias de servicios.

---

## Technical Context

* **Language/Version:** 
  * Backend: Java 21 LTS (Spring Boot 3.3.3, paquete propio `com.shushinestudio`).
  * Mobile: Dart 3.3+ / Flutter 3.19+.
* **Primary Dependencies:**
  * Backend:
    * Java Spring Boot: `spring-boot-starter-web`, `spring-boot-starter-data-jpa`, `spring-boot-starter-security`, `org.postgresql:postgresql`, `io.jsonwebtoken:jjwt-api:0.12.6`, `org.modelmapper:modelmapper:3.2.1`, `org.springdoc:springdoc-openapi-starter-webmvc-ui:2.6.0`, `org.projectlombok:lombok`.
  * Mobile: `flutter_bloc` (8.1+), `dio` (5.4+), `supabase_flutter` (2.3+), `flutter_secure_storage` (9.0+), `get_it`, `intl`, `equatable`.
* **Storage & Cloud:** 
  * Base de Datos Única: PostgreSQL 15+ alojado en Supabase (AWS Pooler, tablas relacionales normalizadas).
  * Supabase Storage (buckets públicos `servicios-imagenes`, `estilistas-avatares`, `categorias-imagenes`).
  * Autenticación JWT con roles estipulados de Shushine Studio: `ADMIN` (Administrador) y `CLIENTE` (Cliente), con soporte opcional para `RECEPCIONISTA`.
* **Testing Frameworks:**
  * Backend: `JUnit 5`, `SpringBootTest`, `Mockito`.
  * Mobile: `flutter_test`, `bloc_test`, `mocktail`.
* **Target Platform:**
  * Backend: Contenedores Linux / Azure App Service / Localhost (Puerto 8080).
  * Mobile: Multiplataforma nativo (Android SDK 24+ e iOS 13+).
* **Project Type:** Web API RESTful desacoplada + Aplicación Móvil Híbrida.
* **Performance Goals:** Cálculo de disponibilidad en `<300ms`, tiempo total de reserva en `<90s`, 0.0% de sobreventa de franjas horarias.
* **Constraints:** Semillero de datos (`DataInitializer`) directo a PostgreSQL en Supabase, endpoints con soporte `Pageable` y `/lista`, estricto cumplimiento del estándar RFC 7807 (`application/problem+json`), código en inglés y UI en español.

---

## Constitution Check

*GATE: Evaluación obligatoria según los principios de [.specify/memory/constitution.md](../../.specify/memory/constitution.md)*

| Principio Constitucional | Estado | Justificación |
| :--- | :---: | :--- |
| **I. Clean Architecture & Layer Decoupling** | ✅ PASS | Frontend dividido en `presentation`, `domain`, `data`; Backend en `Api`, `Application`, `Domain`, `Infrastructure`. |
| **II. Single Source of Truth (Backend)** | ✅ PASS | La Web API de Spring Boot es la única autorizada para calcular slots de disponibilidad, precios e impuestos y gestionar concurrencia. |
| **III. Database Isolation & Security First** | ✅ PASS | Móvil no consulta la base de datos directamente; todas las lecturas/escrituras de negocio van por la Web API. RLS activo. |
| **IV. Standardized Contracts & RFC 7807** | ✅ PASS | OpenAPI 3.0 documentado en `contracts/openapi.yaml`. Respuestas de error estandarizadas en `ProblemDetails`. |
| **V. Bilingual Coding Standards** | ✅ PASS | Código, clases y endpoints en inglés; interfaz de usuario y commits de Git en español. |
| **VI. Authorship & Commit Attribution** | ✅ PASS | Cero mención a asistentes de IA; todo commit y tarea atribuido a Alex Alfaro o Camila Calderón. |
| **VII. Git Workflow & Branch per Feature** | ✅ PASS | Trabajo en ramas `feature/*`, prohibición de push directo a `develop` y `main`, PRs con revisión obligatoria. |

---

## Project Structure

### Documentation (this feature)

```text
specs/001-sistema-reservas-shushine/
├── spec.md                  # Especificación completa de requerimientos y casos de uso
├── plan.md                  # Este plan de implementación
├── research.md              # Decisiones técnicas y justificaciones arquitectónicas
├── data-model.md            # Modelo relacional de 16 tablas y máquina de estados
├── quickstart.md            # Guía de validación y pruebas de ejecución rápida
├── contracts/
│   └── openapi.yaml         # Contratos OpenAPI 3.0 para todos los endpoints
├── checklists/
│   └── requirements.md      # Checklist de validación de calidad del spec
└── tasks.md                 # Tareas ejecutables y ordenadas por dependencias
```

### Source Code (repository root)

```text
src/
├── backend/
│   │   # Estructura de Capas (Java 21 Spring Boot 3.3.3 - com.shushinestudio):
│   ├── config/                         # SecurityConfig, SwaggerConfig, DataInitializer, ModelMapperConfig
│   ├── controladores/                  # AuthController, CategoriaController, ServicioController, etc.
│   ├── servicios/                      # Interfaces e Implementaciones con lógica de negocio y mapeo DTO
│   ├── repositorios/                   # Interfaces de persistencia (JpaRepository / IRepository)
│   ├── dtos/                           # DTOs segregados (*Guardar, *Modificar, *Salida, *CambiarEstado)
│   ├── modelos/                        # Entidades relacionales del dominio
│   ├── seguridad/                      # JwtService, JwtAuthenticationFilter, Usuario, Rol, UsuarioService
│   └── tests/                          # Pruebas unitarias de servicios (t1_crear a t6_eliminar)
│
└── mobile/
    ├── lib/
    │   ├── core/                       # Inyección de dependencias, Interceptores Dio, Constantes
    │   ├── presentation/               # Vistas (Screens), Widgets, BLoCs / Cubits
    │   ├── domain/                     # Entities, Use Cases, Repository Contracts (Dart puro)
    │   ├── data/                       # DTOs, DataSources (Remote/Local), Repository Implementations
    │   └── main.dart                   # Inicialización de servicios, DI y MaterialApp
    └── test/                           # Pruebas unitarias de BLoCs y Use Cases
```

---

## Generated Artifacts References

* **Fase 0 (Investigación):** [research.md](./research.md)
* **Fase 1 (Modelo de Datos):** [data-model.md](./data-model.md)
* **Fase 1 (Contratos de Interfaz):** [contracts/openapi.yaml](./contracts/openapi.yaml)
* **Fase 1 (Guía de Validación):** [quickstart.md](./quickstart.md)
