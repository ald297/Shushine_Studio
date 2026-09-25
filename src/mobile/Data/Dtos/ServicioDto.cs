using System.Text.Json.Serialization;
using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Data.Dtos;

public class ServicioDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("duracionMinutos")]
    public int DuracionMinutos { get; set; }

    [JsonPropertyName("categoriaNombre")]
    public string? CategoriaNombre { get; set; }

    [JsonPropertyName("imagenUrl")]
    public string? ImagenUrl { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }

    [JsonPropertyName("protocolo")]
    public string? Protocolo { get; set; }

    public Servicio ToEntity() => new()
    {
        Id = Id,
        Nombre = Nombre,
        Descripcion = Descripcion,
        Precio = Precio,
        DuracionMinutos = DuracionMinutos,
        CategoriaNombre = CategoriaNombre,
        ImagenUrl = ImagenUrl,
        Activo = Activo,
        Protocolo = Protocolo
    };
}
