using System.Text.Json.Serialization;
using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Data.Dtos;

public class ReservaDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("codigoReserva")]
    public string CodigoReserva { get; set; } = string.Empty;

    [JsonPropertyName("servicioId")]
    public long ServicioId { get; set; }

    [JsonPropertyName("servicioNombre")]
    public string ServicioNombre { get; set; } = string.Empty;

    [JsonPropertyName("estilistaId")]
    public long EstilistaId { get; set; }

    [JsonPropertyName("estilistaNombre")]
    public string EstilistaNombre { get; set; } = string.Empty;

    [JsonPropertyName("fechaHoraInicio")]
    public DateTime FechaHoraInicio { get; set; }

    [JsonPropertyName("fechaHoraFin")]
    public DateTime FechaHoraFin { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = "PENDIENTE";

    [JsonPropertyName("notas")]
    public string? Notas { get; set; }

    public Reserva ToEntity() => new()
    {
        Id = Id,
        CodigoReserva = CodigoReserva,
        ServicioId = ServicioId,
        ServicioNombre = ServicioNombre,
        EstilistaId = EstilistaId,
        EstilistaNombre = EstilistaNombre,
        FechaHoraInicio = FechaHoraInicio,
        FechaHoraFin = FechaHoraFin,
        Total = Total,
        Estado = Estado,
        Notas = Notas
    };
}

public class CrearReservaRequestDto
{
    [JsonPropertyName("servicioId")]
    public long ServicioId { get; set; }

    [JsonPropertyName("estilistaId")]
    public long? EstilistaId { get; set; }

    [JsonPropertyName("fechaHoraInicio")]
    public DateTime FechaHoraInicio { get; set; }

    [JsonPropertyName("notas")]
    public string? Notas { get; set; }
}
