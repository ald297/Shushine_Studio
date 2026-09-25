namespace ShushineStudio.Mobile.Domain.Entities;

public class Estilista
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string EspecialidadPrincipal { get; set; } = string.Empty;
    public string? Biografia { get; set; }
    public string? AvatarUrl { get; set; }
    public string ColorAgenda { get; set; } = "#E91E63";
    public bool Activo { get; set; } = true;

    // Ayudante para interfaz gráfica (avatares con iniciales si no hay foto)
    public string Iniciales => string.Concat(
        NombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(s => s[0])
    ).ToUpper();
}
