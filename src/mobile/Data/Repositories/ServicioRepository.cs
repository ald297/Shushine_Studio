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
        var url = categoriaId.HasValue 
            ? $"servicios?categoriaId={categoriaId.Value}" 
            : "servicios";

        var dtos = await _httpClient.GetFromJsonAsync<List<ServicioDto>>(url);
        return dtos?.Select(d => d.ToEntity()) ?? Enumerable.Empty<Servicio>();
    }

    public async Task<Servicio?> GetServicioByIdAsync(long id)
    {
        var dto = await _httpClient.GetFromJsonAsync<ServicioDto>($"servicios/{id}");
        return dto?.ToEntity();
    }

    public async Task<IEnumerable<string>> GetCategoriasAsync()
    {
        var categorias = await _httpClient.GetFromJsonAsync<List<string>>("servicios/categorias");
        return categorias ?? new List<string> { "Todos", "Cabello", "Uñas", "Maquillaje", "Spa" };
    }
}
