namespace ShushineStudio.Mobile.Domain.Entities;

public class Reserva
{
    public long Id { get; set; }
    public string CodigoReserva { get; set; } = string.Empty;
    public long ServicioId { get; set; }
    public string ServicioNombre { get; set; } = string.Empty;
    public long EstilistaId { get; set; }
    public string EstilistaNombre { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? Notas { get; set; }
}

public class Estilista
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public bool Disponible { get; set; } = true;
}

public class Usuario
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = "CLIENTE";
    public string? Telefono { get; set; }
}
