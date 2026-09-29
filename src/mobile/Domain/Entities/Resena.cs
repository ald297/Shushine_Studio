namespace ShushineStudio.Mobile.Domain.Entities;

/// <summary>
/// Entidad de Dominio que modela una Reseña / Calificación en Shushine Studio.
/// Alineada al esquema relacional de la tabla 'public.resenas'.
/// </summary>
public class Resena
{
    public long Id { get; set; }
    public long CitaId { get; set; }
    public string CodigoCita { get; set; } = string.Empty;
    public long ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public long EstilistaId { get; set; }
    public string EstilistaNombre { get; set; } = string.Empty;
    public string ServicioNombre { get; set; } = string.Empty;
    public int Estrellas { get; set; } = 5;
    public string Comentario { get; set; } = string.Empty;
    public bool VisiblePublica { get; set; } = true;
    public DateTime FechaEmision { get; set; } = DateTime.Now;

    public string EstrellasTexto => new string('★', Math.Clamp(Estrellas, 1, 5)) +
                                   new string('☆', 5 - Math.Clamp(Estrellas, 1, 5));

    public string FechaFormateada => FechaEmision.ToString("dd MMM yyyy");

    public string NivelTexto => Estrellas switch
    {
        1 => "Por Mejorar",
        2 => "Oportunidad de Mejora",
        3 => "Buena Experiencia",
        4 => "Gran Experiencia",
        5 => "Experiencia Excepcional",
        _ => "Sin Calificar"
    };
}
