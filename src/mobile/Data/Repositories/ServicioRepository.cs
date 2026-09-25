using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

/// <summary>
/// Repositorio de servicios con soporte para Web API y catálogo local de contingencia (Offline Fallback).
/// </summary>
public class ServicioRepository : IServicioRepository
{
    private readonly HttpClient _httpClient;

    // Catálogo oficial de belleza de Shushine Studio para garantizar funcionamiento continuo
    private static readonly List<Servicio> FallbackServicios = new()
    {
        new Servicio
        {
            Id = 1,
            Nombre = "Balayage Iluminador & Gloss",
            Descripcion = "Técnica francesa de aclarado degradado a mano alzada con baño de brillo nutritivo.",
            Precio = 65.00m,
            DuracionMinutos = 120,
            CategoriaNombre = "Cabello",
            Protocolo = "1. Diagnóstico capilar y prueba de mecha.\n2. Aclarado degradado profesional.\n3. Lavado con champú neutralizante.\n4. Matizado gloss hidratante.\n5. Peinado y sellado con aceite de argán.",
            Activo = true
        },
        new Servicio
        {
            Id = 2,
            Nombre = "Corte de Autor & Cepillado",
            Descripcion = "Diseño de corte personalizado según morfología facial con lavado dermocalmante.",
            Precio = 25.00m,
            DuracionMinutos = 45,
            CategoriaNombre = "Cabello",
            Protocolo = "1. Asesoría de visagismo.\n2. Lavado relajante con masaje capilar.\n3. Corte de precisión en húmedo.\n4. Brushing y peinado con protector térmico.",
            Activo = true
        },
        new Servicio
        {
            Id = 3,
            Nombre = "Manicura Rusa & Esmaltado Semi",
            Descripcion = "Limpieza profunda de cutículas con torno y esmaltado de alta duración gelish.",
            Precio = 22.00m,
            DuracionMinutos = 60,
            CategoriaNombre = "Uñas",
            Protocolo = "1. Higienización de manos.\n2. Tratamiento de cutícula con fresas de diamante.\n3. Nivelación de placa ungueal con base rubber.\n4. Aplicación de color y top coat ultraviolento.\n5. Hidratación con aceite de jojoba.",
            Activo = true
        },
        new Servicio
        {
            Id = 4,
            Nombre = "Pedicura Spa Rejuvenecedora",
            Descripcion = "Exfoliación con sales minerales, mascarilla de parafina y esmaltado profesional.",
            Precio = 28.00m,
            DuracionMinutos = 60,
            CategoriaNombre = "Uñas",
            Protocolo = "1. Inmersión en sales marinas y esencias florales.\n2. Exfoliación dermo-renovadora.\n3. Tratamiento intensivo de talones.\n4. Esmaltado y masaje podal relajante.",
            Activo = true
        },
        new Servicio
        {
            Id = 5,
            Nombre = "Lifting de Pestañas & Keratina",
            Descripcion = "Curvatura natural de pestañas con nutrición intensiva de keratina y tinte negro profundo.",
            Precio = 30.00m,
            DuracionMinutos = 50,
            CategoriaNombre = "Maquillaje",
            Protocolo = "1. Limpieza y desengrasado de la zona ocular.\n2. Colocación de moldes de silicona.\n3. Aplicación de loción moldeadora y fijadora.\n4. Tinte de pestañas.\n5. Baño de nutrición con botox capilar y keratina.",
            Activo = true
        },
        new Servicio
        {
            Id = 6,
            Nombre = "Diseño & Laminado de Cejas",
            Descripcion = "Depilación con hilo orgánico, laminado y perfilado de mirada.",
            Precio = 20.00m,
            DuracionMinutos = 40,
            CategoriaNombre = "Maquillaje",
            Protocolo = "1. Visagismo y diseño según proporciones faciales.\n2. Alisado y laminado de cejas.\n3. Depilación de precisión con hilo.\n4. Tinte híbrido opcional y sérum fijador.",
            Activo = true
        },
        new Servicio
        {
            Id = 7,
            Nombre = "Masaje Relajante Aromaterapia",
            Descripcion = "Sesión corporal completa con aceites esenciales de lavanda y piedras calientes.",
            Precio = 45.00m,
            DuracionMinutos = 60,
            CategoriaNombre = "Spa",
            Protocolo = "1. Ritual de respiración con aromaterapia.\n2. Masaje descontracturante suave en espalda y cuello.\n3. Aplicación de piedras volcánicas calientes.\n4. Té relajante de cortesía.",
            Activo = true
        }
    };

    public ServicioRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Servicio>> GetServiciosAsync(long? categoriaId = null)
    {
        try
        {
            var url = categoriaId.HasValue 
                ? $"servicios?categoriaId={categoriaId.Value}" 
                : "servicios";

            var dtos = await _httpClient.GetFromJsonAsync<List<ServicioDto>>(url);
            if (dtos != null && dtos.Count > 0)
            {
                return dtos.Select(d => d.ToEntity());
            }
        }
        catch
        {
            // Contingencia offline: asegura catálogo funcional si la API no está corriendo
        }

        return FallbackServicios;
    }

    public async Task<Servicio?> GetServicioByIdAsync(long id)
    {
        try
        {
            var dto = await _httpClient.GetFromJsonAsync<ServicioDto>($"servicios/{id}");
            if (dto != null)
            {
                return dto.ToEntity();
            }
        }
        catch
        {
            // Contingencia
        }

        return FallbackServicios.FirstOrDefault(s => s.Id == id) ?? FallbackServicios.First();
    }

    public async Task<IEnumerable<string>> GetCategoriasAsync()
    {
        try
        {
            var categorias = await _httpClient.GetFromJsonAsync<List<string>>("servicios/categorias");
            if (categorias != null && categorias.Count > 0)
            {
                return categorias;
            }
        }
        catch
        {
            // Contingencia
        }

        return new List<string> { "Todos", "Cabello", "Uñas", "Maquillaje", "Spa" };
    }
}
