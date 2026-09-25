using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

public class EstilistaRepository : IEstilistaRepository
{
    private readonly HttpClient _httpClient;

    public EstilistaRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Estilista>> GetEstilistasAsync()
    {
        try
        {
            var dtos = await _httpClient.GetFromJsonAsync<List<EstilistaDto>>("estilistas");
            return dtos?.Where(e => e.Activo).Select(e => e.ToEntity()) ?? Enumerable.Empty<Estilista>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Estilista>();
        }
    }

    public async Task<Estilista?> GetEstilistaByIdAsync(long id)
    {
        try
        {
            var dto = await _httpClient.GetFromJsonAsync<EstilistaDto>($"estilistas/{id}");
            return dto?.ToEntity();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<DisponibilidadEstilista?> GetDisponibilidadAsync(long estilistaId, DateTime fecha, long servicioId)
    {
        try
        {
            var fechaStr = fecha.ToString("yyyy-MM-dd");
            var url = $"estilistas/{estilistaId}/disponibilidad?fecha={fechaStr}&servicioId={servicioId}";
            var dto = await _httpClient.GetFromJsonAsync<DisponibilidadSalidaDto>(url);
            return dto?.ToEntity();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
