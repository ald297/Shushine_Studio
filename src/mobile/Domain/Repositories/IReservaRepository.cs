using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Domain.Repositories;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> GetMisCitasAsync();
    Task<Reserva?> CrearReservaAsync(long servicioId, long? estilistaId, DateTime fechaHora, string? notas);
    Task<bool> CancelarReservaAsync(long reservaId);
}

public interface IAuthRepository
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string nombre, string email, string password, string telefono);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<Usuario?> GetCurrentUserAsync();
}
