using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Domain.Repositories;

public interface IServicioRepository
{
    Task<IEnumerable<Servicio>> GetServiciosAsync(long? categoriaId = null);
    Task<Servicio?> GetServicioByIdAsync(long id);
    Task<IEnumerable<string>> GetCategoriasAsync();
}
