using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

public class ServicioRepository : IServicioRepository
{
    private readonly HttpClient _httpClient;

    public ServicioRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Servicio>> GetServiciosAsync(long? categoriaId = null)
    {
        // En Render / Spring Boot, el endpoint de listado rápido sin paginación para selectores móviles es 'servicios/lista'
        var url = categoriaId.HasValue 
            ? $"servicios/lista?categoriaId={categoriaId.Value}" 
            : "servicios/lista";

        try
        {
            var dtos = await _httpClient.GetFromJsonAsync<List<ServicioDto>>(url);
            if (dtos != null && dtos.Count > 0)
            {
                return dtos.Select(d => d.ToEntity());
            }
        }
        catch (Exception)
        {
            // Fallback a endpoint paginado si fuera requerido
            try
            {
                var page = await _httpClient.GetFromJsonAsync<PageResponseDto<ServicioDto>>("servicios");
                if (page?.Content != null)
                {
                    return page.Content.Select(d => d.ToEntity());
                }
            }
            catch
            {
                // Silencioso para fallback controlado
            }
        }

        return Enumerable.Empty<Servicio>();
    }

    public async Task<Servicio?> GetServicioByIdAsync(long id)
    {
        try
        {
            var dto = await _httpClient.GetFromJsonAsync<ServicioDto>($"servicios/{id}");
            return dto?.ToEntity();
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetCategoriasAsync()
    {
        try
        {
            var categorias = await _httpClient.GetFromJsonAsync<List<string>>("categorias/nombres");
            if (categorias != null && categorias.Count > 0)
            {
                return categorias;
            }
        }
        catch
        {
            // Fallback elegante a categorías por defecto del salón
        }

        return new List<string> { "Todos", "Corte y Peinado", "Coloración", "Tratamientos", "Uñas", "Maquillaje" };
    }
}
