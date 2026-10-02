# SHUNSHINE STUDIO — ESPECIFICACIÓN VISUAL DEL SISTEMA DE DISEÑO (STITCH)

> **Documento Oficial de Identidad Visual y Estándares de Diseño**  
> **Proyecto:** Shunshine Studio — Aplicación Móvil de Belleza y Bienestar (.NET MAUI)  
> **Tema:** *Luxe Lotus & Warm Gold (Warm Editorial Luxury)*  

---

## 1. Identidad de Marca y Concepto Visual

La identidad visual de **Shunshine Studio** se inspira directamente en el emblema de la flor de loto dorada con degradados de rosa loto y vino royal, enmarcada con trazos dorados finos y la estrella superior.

El sistema visual implementa una estética de **Warm Editorial Luxury**:
- **Elegancia y Alto Valor Percibido:** Composiciones limpias, espaciado generoso, líneas sutiles y bordes suaves de inspiración estética de alta gama.
- **Calidez y Serenidad:** Fondos cálidos en porcelana y alabastro suave que sustituyen blancos clínicos planos.
- **Claridad Operativa:** Tipografía moderna con jerarquía definida para agendamiento fluido de citas y control administrativo nítido.

---

## 2. Paleta Cromática Oficial

| Token Semántico | Nombre de Color | Hexadecimal | Uso y Rol en la UI |
| :--- | :--- | :--- | :--- |
| `Primary` | **Primary Wine** (Royal Lotus Wine) | `#9B2D47` | Acciones principales, botones primarios (CTAs), acento activo de navegación y encabezados de alta jerarquía. |
| `Secondary` | **Champagne Gold** (Metallic Gold) | `#C5A059` | Acentos dorados, estrellas de calificación, insignias premium, bordes selectos y botones dorados. |
| `Tertiary` | **Lotus Rose** (Rose Blush) | `#E87A90` | Acentos secundarios interactivos, detalles sutiles, microinteracciones y halos suaves. |
| `NeutralCore` / `TextPrimary` | **Espresso Noir** | `#1A1412` | Texto principal de alto contraste, títulos editoriales y etiquetas clave. |
| `Background` | **Warm Background** | `#FFF8F6` | Fondo cálido general de las pantallas y lienzos principales. |
| `BackgroundSecondary` | **Secondary Background** | `#FBF9F8` | Fondos de contenedores secundarios y tarjetas alternadas. |
| `Card` / `Surface` | **Pure Porcelain** | `#FFFFFF` | Superficies de tarjetas, modales y hojas flotantes elevadas. |
| `MutedText` / `TextSecondary` | **Muted Text** | `#7D7571` | Subtítulos, metadatos, textos secundarios y leyendas explicativas. |
| `Border` | **Warm Border** | `#EFEAE6` | Bordes sutiles de tarjetas, divisores y campos de entrada. |
| `InputBackground` | **Warm Silk** | `#F5F3EF` | Fondo suave de campos de texto (Entry/Editor) y chips inactivos. |
| `Success` | **Botanical Green** | `#2E7D32` | Confirmaciones, éxito y citas confirmadas. |
| `SuccessBg` | **Success Background** | `#E8F5E9` | Fondo de badges y alertas de éxito. |
| `Warning` | **Refined Amber** | `#ED6C02` | Pendiente de aprobación, advertencias y confirmaciones previas. |
| `WarningBg` | **Warning Background** | `#FFF3E0` | Fondo de badges y alertas de advertencia o estado pendiente. |
| `Error` | **Muted Scarlet** | `#D32F2F` | Errores de validación, cancelaciones y botones de peligro. |
| `ErrorBg` | **Error Background** | `#FFEBEE` | Fondo de badges y alertas de error o cancelación. |

---

## 3. Tipografía

| Familia Tipográfica | Rol en la Aplicación | Estilos y Pesos |
| :--- | :--- | :--- |
| **Playfair Display** | **Títulos y Encabezados Editoriales** | Bold / SemiBold / Regular (20px - 32px) para nombres de secciones, títulos principales de servicios y pantallas heroicas. |
| **Plus Jakarta Sans** | **Cuerpo, Formularios, Botones y Operativa** | Regular (400), Medium (500), SemiBold (600) y Bold (700) para lectura óptima en móviles, inputs, botones y tablas. |

---

## 4. Radios de Curvatura (Shapes) y Espaciados

- **Elementos Estándar (8px):** Tags pequeños, chips sutiles, esquinas de inputs.
- **Botones y Tarjetas Medianas (14px - 16px):** Tarjetas de servicios, tarjetas de citas, botones principales.
- **Superficies Grandes (20px - 24px):** Tarjetas de resumen, drawers, cabeceras contenedoras.
- **Píldora (9999px):** Badges de estado, selectores de categorías, chips de estilistas.

---

## 5. Componentes y Botones

- **Botón Primario:** Fondo `#9B2D47`, texto blanco, altura 50px, CornerRadius 16, Plus Jakarta Sans Bold.
- **Botón Secundario (Outline):** Fondo transparente, borde 1.5px `#9B2D47`, texto `#9B2D47`, altura 50px, CornerRadius 16.
- **Botón Dorado (Luxury):** Fondo `#C5A059`, texto blanco, altura 50px, CornerRadius 16.
- **Botón de Peligro:** Fondo `#D32F2F`, texto blanco, altura 50px, CornerRadius 16.
- **Cards:** Fondo `#FFFFFF`, borde sutil 1px `#EFEAE6`, CornerRadius 16, padding 16px.

---

## 6. Estados de Citas y Reservas

| Estado | Color de Texto | Color de Fondo | Borde Sutil |
| :--- | :--- | :--- | :--- |
| **Confirmada** | `#2E7D32` | `#E8F5E9` | `#C8E6C9` |
| **Pendiente** | `#ED6C02` | `#FFF3E0` | `#FFE0B2` |
| **En proceso** | `#9B2D47` | `#FCECEF` | `#F7CAD3` |
| **Terminada** | `#594E46` | `#F5F3EF` | `#E2DCD5` |
| **Cancelada** | `#D32F2F` | `#FFEBEE` | `#FFCDD2` |
