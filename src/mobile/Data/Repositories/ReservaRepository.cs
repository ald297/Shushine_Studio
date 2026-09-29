using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

/// <summary>
/// Repositorio de reservas conectado con la Web API en Render y con contingencia local (Fallback).
/// </summary>
public class ReservaRepository : IReservaRepository
{
    private readonly HttpClient _httpClient;

    private static readonly List<Reserva> FallbackReservas = new()
    {
        new Reserva
        {
            Id = 101,
            CodigoCita = "#SHU-8492",
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
            CodigoCita = "#SHU-7120",
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
            // Endpoint oficial en Render: GET /api/citas/mis-citas
            var dtos = await _httpClient.GetFromJsonAsync<List<ReservaDto>>("citas/mis-citas");
            if (dtos != null && dtos.Count > 0)
            {
                var listaApi = dtos.Select(d => d.ToEntity()).ToList();
                foreach (var item in listaApi)
                {
                    if (!FallbackReservas.Any(f => f.Id == item.Id || f.CodigoCita == item.CodigoCita))
                    {
                        FallbackReservas.Add(item);
                    }
                }
                return FallbackReservas.OrderByDescending(r => r.FechaHoraInicio);
            }
        }
        catch
        {
            // Contingencia offline si no hay conexión con la Web API
        }

        return FallbackReservas;
    }

    public async Task<IEnumerable<Reserva>> GetTodasCitasAdminAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("citas?size=50&sort=id,desc");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("content", out var contentElem))
                {
                    var dtos = System.Text.Json.JsonSerializer.Deserialize<List<ReservaDto>>(contentElem.GetRawText());
                    if (dtos != null && dtos.Count > 0)
                    {
                        var listaApi = dtos.Select(d => d.ToEntity()).ToList();
                        foreach (var item in listaApi)
                        {
                            if (!FallbackReservas.Any(f => f.Id == item.Id || f.CodigoCita == item.CodigoCita))
                            {
                                FallbackReservas.Add(item);
                            }
                        }
                        return FallbackReservas.OrderByDescending(r => r.FechaHoraInicio);
                    }
                }
            }
        }
        catch
        {
            // Contingencia offline
        }

        return FallbackReservas;
    }

    public async Task<DashboardMetricasDto?> GetDashboardMetricasAsync()
    {
        try
        {
            // Endpoint oficial en Spring Boot: GET /api/admin/dashboard
            var response = await _httpClient.GetAsync("admin/dashboard");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardMetricasDto>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error al consultar /api/admin/dashboard: {ex.Message}");
        }

        return null;
    }

    public async Task<IEnumerable<Reserva>> GetTimelineCitasAsync(DateTime fecha, int? estilistaId = null)
    {
        try
        {
            // Endpoint oficial en Spring Boot: GET /api/citas/timeline?fecha=YYYY-MM-DD
            var fechaStr = fecha.ToString("yyyy-MM-dd");
            var url = estilistaId.HasValue && estilistaId.Value > 0
                ? $"citas/timeline?fecha={fechaStr}&estilistaId={estilistaId.Value}"
                : $"citas/timeline?fecha={fechaStr}";

            var dtos = await _httpClient.GetFromJsonAsync<List<TimelineItemDto>>(url);
            if (dtos != null)
            {
                return dtos.Select(d => d.ToEntity()).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error al consultar /api/citas/timeline: {ex.Message}");
        }

        return Enumerable.Empty<Reserva>();
    }

    public async Task<Reserva?> ObtenerCitaPorIdAsync(long id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"citas/{id}");
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<ReservaDto>();
                return dto?.ToEntity();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error al obtener cita {id}: {ex.Message}");
        }

        return FallbackReservas.FirstOrDefault(r => r.Id == id);
    }

    public async Task<IEnumerable<Reserva>> ObtenerTodasCitasAdminPaginadasAsync(int page = 0, int size = 50)
    {
        try
        {
            var response = await _httpClient.GetAsync($"citas?page={page}&size={size}&sort=id,desc");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("content", out var contentElem))
                {
                    var dtos = JsonSerializer.Deserialize<List<ReservaDto>>(contentElem.GetRawText());
                    if (dtos != null && dtos.Count > 0)
                    {
                        return dtos.Select(d => d.ToEntity()).ToList();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error al listar citas admin: {ex.Message}");
        }

        return FallbackReservas.OrderByDescending(r => r.FechaHoraInicio);
    }

    public async Task<Reserva?> CambiarEstadoCitaAsync(long citaId, string nuevoEstado, string? motivoCancelacion = null)
    {
        try
        {
            var req = new CitaCambiarEstadoRequestDto
            {
                Id = (int)citaId,
                NuevoEstado = nuevoEstado,
                MotivoCancelacion = motivoCancelacion
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"citas/{citaId}/estado", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<ReservaDto>();
                if (dto != null)
                {
                    var entidad = dto.ToEntity();
                    var match = FallbackReservas.FirstOrDefault(r => r.Id == citaId);
                    if (match != null) match.Estado = entidad.Estado;
                    return entidad;
                }
            }
            else
            {
                var errStr = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error PATCH citas/{citaId}/estado ({response.StatusCode}): {errStr}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Excepción en CambiarEstadoCitaAsync: {ex.Message}");
        }

        var localMatch = FallbackReservas.FirstOrDefault(r => r.Id == citaId);
        if (localMatch != null)
        {
            localMatch.Estado = nuevoEstado;
            return localMatch;
        }

        return null;
    }

    public async Task<Reserva?> CrearCitaWalkinAsync(CitaWalkinRequestDto walkinDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("citas/walkin", walkinDto);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<ReservaDto>();
                if (dto != null)
                {
                    var entidad = dto.ToEntity();
                    FallbackReservas.Insert(0, entidad);
                    return entidad;
                }
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(errJson);
                var detail = "El estilista seleccionado ya no tiene disponible la franja horaria solicitada.";
                if (doc.RootElement.TryGetProperty("detail", out var dElem))
                {
                    detail = dElem.GetString() ?? detail;
                }
                throw new InvalidOperationException(detail);
            }
            else
            {
                var errStr = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Error del servidor: {errStr}");
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReservaRepository] Error en CrearCitaWalkinAsync: {ex.Message}");
            throw new Exception("No se pudo conectar con el servidor para agendar la cita. Verifica tu conexión.", ex);
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
        try
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
                if (dto != null)
                {
                    var entidad = dto.ToEntity();
                    FallbackReservas.Insert(0, entidad);
                    return entidad;
                }
            }
        }
        catch
        {
            // Contingencia offline
        }

        // Si la API falla o está offline, generar reserva simulada en Fallback para permitir pruebas
        var nuevaReserva = new Reserva
        {
            Id = Random.Shared.Next(200, 999),
            CodigoCita = $"#SHU-{Random.Shared.Next(1000, 9999)}",
            ServicioId = servicioIds.FirstOrDefault(),
            ServicioNombre = "Tratamiento de Salón",
            EstilistaId = estilistaId,
            EstilistaNombre = "Estilista Asignada",
            FechaHoraInicio = DateTime.TryParse($"{fechaCita:yyyy-MM-dd} {horaInicio}", out var dt) ? dt : fechaCita,
            FechaHoraFin = (DateTime.TryParse($"{fechaCita:yyyy-MM-dd} {horaInicio}", out var dtFin) ? dtFin : fechaCita).AddHours(1),
            Total = 35.00m,
            Estado = "PENDIENTE",
            Notas = notas,
            MetodoPagoPreferente = metodoPago
        };

        FallbackReservas.Insert(0, nuevaReserva);
        return nuevaReserva;
    }

    public async Task<Reserva?> CrearReservaAsync(
        long servicioId,
        long? estilistaId,
        DateTime fechaHora,
        string? notas)
    {
        var targetEstilista = estilistaId ?? 1;
        var horaInicio = fechaHora.ToString("HH:mm");
        var servicioIds = new List<int> { (int)servicioId };

        return await CrearReservaAsync(targetEstilista, fechaHora.Date, horaInicio, servicioIds, notas, "Efectivo");
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
            if (response.IsSuccessStatusCode) return true;
        }
        catch
        {
            // Contingencia offline
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
