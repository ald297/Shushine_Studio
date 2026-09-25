namespace ShushineStudio.Mobile.Domain.Entities;

public class Reserva
{
    public long Id { get; set; }
    public string CodigoCita { get; set; } = string.Empty;
    public string CodigoReserva => CodigoCita; // Alias de retrocompatibilidad

    public long ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string? ClienteTelefono { get; set; }

    public long EstilistaId { get; set; }
    public string EstilistaNombre { get; set; } = string.Empty;

    public string FechaCita { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string HoraInicio { get; set; } = string.Empty; // "HH:mm"
    public string HoraFin { get; set; } = string.Empty; // "HH:mm"

    public DateTime FechaHoraInicio
    {
        get
        {
            if (DateTime.TryParse($"{FechaCita} {HoraInicio}", out var dt))
                return dt;
            return DateTime.MinValue;
        }
    }

    public DateTime FechaHoraFin
    {
        get
        {
            if (DateTime.TryParse($"{FechaCita} {HoraFin}", out var dt))
                return dt;
            return DateTime.MinValue;
        }
    }

    public string Estado { get; set; } = "PENDIENTE";
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    public string MetodoPagoPreferente { get; set; } = "Efectivo";
    public string EstadoPago { get; set; } = "PENDIENTE";
    public string? Notas { get; set; }

    public List<string> ServiciosNombres { get; set; } = new();

    // Propiedades calculadas para presentación XAML
    public string ServiciosResumen => ServiciosNombres != null && ServiciosNombres.Count > 0
        ? string.Join(", ", ServiciosNombres)
        : "Servicio de Belleza";

    public string ServicioNombre => ServiciosResumen; // Alias para vistas previas

    public string TotalFormateado => $"${Total:N2}";
    public string SubtotalFormateado => $"${Subtotal:N2}";
    public string IvaFormateado => $"${Iva:N2}";

    public string HorarioTexto => $"{HoraInicio} - {HoraFin}";

    public string EstadoColor => Estado?.ToUpper() switch
    {
        "CONFIRMADA" => "#2E7D32", // Verde bosque
        "COMPLETADA" => "#1565C0", // Azul primario
        "CANCELADA" => "#C62828",  // Rojo alerta
        "EN_PROCESO" => "#E65100", // Naranja
        _ => "#F57C00"            // Ámbar pendiente
    };

    public bool PuedeCancelar => Estado?.ToUpper() is "PENDIENTE" or "CONFIRMADA";
}
