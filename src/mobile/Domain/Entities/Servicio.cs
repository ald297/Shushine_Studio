namespace ShushineStudio.Mobile.Domain.Entities;

public class Servicio
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
    public string? CategoriaNombre { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
    public string? Protocolo { get; set; }
}
