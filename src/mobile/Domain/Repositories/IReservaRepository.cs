using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Domain.Repositories;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> GetMisCitasAsync();
    Task<Reserva?> CrearReservaAsync(long estilistaId, DateTime fechaCita, string horaInicio, List<int> servicioIds, string? notas, string metodoPago = "Efectivo");
    Task<Reserva?> CrearReservaAsync(long servicioId, long? estilistaId, DateTime fechaHora, string? notas);
    Task<bool> CancelarReservaAsync(long reservaId, string? motivo = null);
}
