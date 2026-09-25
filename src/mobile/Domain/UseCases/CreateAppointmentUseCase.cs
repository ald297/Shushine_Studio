using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Domain.UseCases;

public class CreateAppointmentUseCase
{
    private readonly IReservaRepository _repository;

    public CreateAppointmentUseCase(IReservaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Reserva?> ExecuteAsync(long servicioId, long? estilistaId, DateTime fechaHora, string? notas)
    {
        return await _repository.CrearReservaAsync(servicioId, estilistaId, fechaHora, notas);
    }
}
