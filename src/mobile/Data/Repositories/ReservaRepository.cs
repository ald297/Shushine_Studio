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
        try
        {
            // Endpoint oficial en Render: GET /api/citas/mis-citas
            var dtos = await _httpClient.GetFromJsonAsync<List<ReservaDto>>("citas/mis-citas");
            return dtos?.Select(d => d.ToEntity()) ?? Enumerable.Empty<Reserva>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Reserva>();
        }
    }

    public async Task<Reserva?> CrearReservaAsync(
        long estilistaId,
        DateTime fechaCita,
        string horaInicio,
        List<int> servicioIds,
        string? notas,
        string metodoPago = "Efectivo")
    {
        var request = new CrearReservaRequestDto
        {
            EstilistaId = estilistaId,
            FechaCita = fechaCita.ToString("yyyy-MM-dd"),
            HoraInicio = horaInicio,
            ServicioIds = servicioIds,
            NotasCliente = notas,
            MetodoPagoPreferente = metodoPago
        };

        // Endpoint oficial en Render: POST /api/citas
        var response = await _httpClient.PostAsJsonAsync("citas", request);
        if (response.IsSuccessStatusCode)
        {
            var dto = await response.Content.ReadFromJsonAsync<ReservaDto>();
            return dto?.ToEntity();
        }

        // Si la respuesta fue un error (ej. 409 Conflict o 400 Bad Request),
        // ErrorDelegatingHandler ya habrá capturado el ProblemDetails para la UI
        return null;
    }

    public async Task<bool> CancelarReservaAsync(long reservaId, string? motivo = null)
    {
        try
        {
            // Endpoint oficial en Render: PUT /api/citas/{id}/cancelar
            var url = string.IsNullOrEmpty(motivo)
                ? $"citas/{reservaId}/cancelar"
                : $"citas/{reservaId}/cancelar?motivo={Uri.EscapeDataString(motivo)}";

            var response = await _httpClient.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
