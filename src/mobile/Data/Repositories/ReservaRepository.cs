using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

/// <summary>
/// Repositorio de reservas conectado con la Web API y con fallback para pruebas locales continuas.
/// </summary>
public class ReservaRepository : IReservaRepository
{
    private readonly HttpClient _httpClient;

    private static readonly List<Reserva> FallbackReservas = new()
    {
        new Reserva
        {
            Id = 101,
            CodigoReserva = "#SHU-8492",
            ServicioId = 1,
            ServicioNombre = "Balayage Iluminador & Gloss",
            EstilistaId = 1,
            EstilistaNombre = "Sofía Ramos",
            FechaHoraInicio = DateTime.Today.AddDays(2).AddHours(14),
            FechaHoraFin = DateTime.Today.AddDays(2).AddHours(16),
            Total = 66.39m,
            Estado = "PENDIENTE",
            Notas = "Cliente frecuente Nivel Oro"
        },
        new Reserva
        {
            Id = 102,
            CodigoReserva = "#SHU-7120",
            ServicioId = 3,
            ServicioNombre = "Manicura Rusa & Esmaltado Semi",
            EstilistaId = 2,
            EstilistaNombre = "Valentina Gómez",
            FechaHoraInicio = DateTime.Today.AddDays(-5).AddHours(11),
            FechaHoraFin = DateTime.Today.AddDays(-5).AddHours(12),
            Total = 22.46m,
            Estado = "COMPLETADA",
            Notas = "Atención finalizada con éxito"
        }
    };

    public ReservaRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Reserva>> GetMisCitasAsync()
    {
        try
        {
            var dtos = await _httpClient.GetFromJsonAsync<List<ReservaDto>>("reservas/mis-citas");
            if (dtos != null && dtos.Count > 0)
            {
                return dtos.Select(d => d.ToEntity());
            }
        }
        catch
        {
            // Contingencia offline
        }

        return FallbackReservas;
    }

    public async Task<Reserva?> CrearReservaAsync(long servicioId, long? estilistaId, DateTime fechaHora, string? notas)
    {
        try
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
                if (dto != null) return dto.ToEntity();
            }
        }
        catch
        {
            // Contingencia
        }

        var nuevaReserva = new Reserva
        {
            Id = Random.Shared.Next(200, 999),
            CodigoReserva = $"#SHU-{Random.Shared.Next(1000, 9999)}",
            ServicioId = servicioId,
            ServicioNombre = "Tratamiento de Salón",
            EstilistaId = estilistaId ?? 1,
            EstilistaNombre = "Estilista Asignada",
            FechaHoraInicio = fechaHora,
            FechaHoraFin = fechaHora.AddHours(1),
            Total = 35.00m,
            Estado = "PENDIENTE",
            Notas = notas
        };

        FallbackReservas.Insert(0, nuevaReserva);
        return nuevaReserva;
    }

    public async Task<bool> CancelarReservaAsync(long reservaId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"reservas/{reservaId}/cancelar", null);
            if (response.IsSuccessStatusCode) return true;
        }
        catch
        {
            // Contingencia
        }

        var match = FallbackReservas.FirstOrDefault(r => r.Id == reservaId);
        if (match != null)
        {
            match.Estado = "CANCELADA";
            return true;
        }

        return false;
    }
}
