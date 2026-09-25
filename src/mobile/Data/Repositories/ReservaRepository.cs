using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly HttpClient _httpClient;

    public ReservaRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Reserva>> GetMisCitasAsync()
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<ReservaDto>>("reservas/mis-citas");
        return dtos?.Select(d => d.ToEntity()) ?? Enumerable.Empty<Reserva>();
    }

    public async Task<Reserva?> CrearReservaAsync(long servicioId, long? estilistaId, DateTime fechaHora, string? notas)
    {
        var request = new CrearReservaRequestDto
        {
            ServicioId = servicioId,
            EstilistaId = estilistaId,
            FechaHoraInicio = fechaHora,
            Notas = notas
        };

        var response = await _httpClient.PostAsJsonAsync("reservas", request);
        if (response.IsSuccessStatusCode)
        {
            var dto = await response.Content.ReadFromJsonAsync<ReservaDto>();
            return dto?.ToEntity();
        }

        return null;
    }

    public async Task<bool> CancelarReservaAsync(long reservaId)
    {
        var response = await _httpClient.PutAsync($"reservas/{reservaId}/cancelar", null);
        return response.IsSuccessStatusCode;
    }
}
