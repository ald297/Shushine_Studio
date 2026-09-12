# Implementation Plan: Sistema Móvil de Gestión y Reservas Shushine Studio

**Branch**: `001-sistema-reservas-shushine` | **Date**: 2026-09-11 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification de Shushine Studio, arquitectura [AGENTS.md](../../AGENTS.md) y constitución [.specify/memory/constitution.md](../../.specify/memory/constitution.md)

---

## Summary

Implementación integral del sistema móvil de reservas y gestión operativa para el salón de belleza Shushine Studio. Combina una aplicación móvil en Flutter bajo Clean Architecture y BLoC para la experiencia del cliente y estilistas, con una Web API RESTful en C# ASP.NET Core (.NET 8/9) conectada a PostgreSQL en Supabase como única autoridad transaccional de negocio, disponibilidad de horarios y facturación.

---

## Technical Context

* **Language/Version:** 
  * Backend: C# 12 / .NET 8.0 o 9.0 LTS.
  * Mobile: Dart 3.3+ / Flutter 3.19+.
* **Primary Dependencies:**
  * Backend: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore` (OpenAPI/Swagger).
  * Mobile: `flutter_bloc` (8.1+), `dio` (5.4+), `supabase_flutter` (2.3+), `get_it`, `intl`, `equatable`.
* **Storage & Cloud:** 
  * PostgreSQL 15+ alojado en Supabase (16 tablas relacionales normalizadas).
  * Supabase Storage (buckets públicos `servicios-imagenes`, `estilistas-avatares`).
  * Supabase Auth para emisión de tokens JWT seguros.
* **Testing Frameworks:**
  * Backend: `xUnit`, `Moq`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`.
  * Mobile: `flutter_test`, `bloc_test`, `mocktail`.
* **Target Platform:**
  * Backend: Contenedores Linux / Azure App Service.
  * Mobile: Multiplataforma nativo (Android SDK 24+ e iOS 13+).
* **Project Type:** Web API RESTful desacoplada + Aplicación Móvil Híbrida.
* **Performance Goals:** Cálculo de disponibilidad en `<300ms`, tiempo total de reserva en `<90s`, 0.0% de sobreventa de franjas horarias.
* **Constraints:** Estricto cumplimiento del estándar RFC 7807 (`application/problem+json`), cero acceso directo de la app móvil a tablas de Supabase, código en inglés y UI en español.

---

## Constitution Check

*GATE: Evaluación obligatoria según los principios de [.specify/memory/constitution.md](../../.specify/memory/constitution.md)*

| Principio Constitucional | Estado | Justificación |
| :--- | :---: | :--- |
| **I. Clean Architecture & Layer Decoupling** | ✅ PASS | Frontend dividido en `presentation`, `domain`, `data`; Backend en `Api`, `Application`, `Domain`, `Infrastructure`. |
| **II. Single Source of Truth (Backend)** | ✅ PASS | La API C# es la única autorizada para calcular slots de disponibilidad, precios e impuestos y gestionar concurrencia. |
| **III. Database Isolation & Security First** | ✅ PASS | Móvil usa Supabase SDK **únicamente para Auth**; todas las lecturas/escrituras de negocio van por la API C#. RLS activo. |
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
│   ├── ShushineStudio.Api/             # Controladores, Middleware RFC 7807, Program.cs
│   ├── ShushineStudio.Application/     # Casos de uso, Servicios, DTOs, Validadores
│   ├── ShushineStudio.Domain/          # Entidades de negocio, Enums, Interfaces de Repositorios
│   ├── ShushineStudio.Infrastructure/  # EF Core DbContext, Npgsql, Repositorios, Supabase Client
│   └── tests/
│       ├── ShushineStudio.UnitTests/
│       └── ShushineStudio.IntegrationTests/
│
└── mobile/
    ├── lib/
    │   ├── core/                       # Inyección de dependencias, Interceptores Dio, Constantes
    │   ├── presentation/               # Vistas (Screens), Widgets, BLoCs / Cubits
    │   ├── domain/                     # Entities, Use Cases, Repository Contracts (Dart puro)
    │   ├── data/                       # DTOs, DataSources (Remote/Local), Repository Implementations
    │   └── main.dart                   # Inicialización de Supabase, DI y MaterialApp
    └── test/                           # Pruebas unitarias de BLoCs y Use Cases
```

---

## Generated Artifacts References

* **Fase 0 (Investigación):** [research.md](./research.md)
* **Fase 1 (Modelo de Datos):** [data-model.md](./data-model.md)
* **Fase 1 (Contratos de Interfaz):** [contracts/openapi.yaml](./contracts/openapi.yaml)
* **Fase 1 (Guía de Validación):** [quickstart.md](./quickstart.md)
