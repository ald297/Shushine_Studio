using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Domain.Repositories;

public interface IEstilistaRepository
{
    Task<IEnumerable<Estilista>> GetEstilistasAsync();
    Task<IEnumerable<Estilista>> GetTodosEstilistasAsync();
    Task<Estilista?> GetEstilistaByIdAsync(long id);
    Task<DisponibilidadEstilista?> GetDisponibilidadAsync(long estilistaId, DateTime fecha, long servicioId);
    Task<bool> ActualizarEstadoEstilistaAsync(long id, bool activo);
}

