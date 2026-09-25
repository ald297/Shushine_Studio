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

    public async Task<Reserva?> ExecuteAsync(
        long estilistaId,
        DateTime fechaCita,
        string horaInicio,
        List<int> servicioIds,
        string? notas = null,
        string metodoPago = "Efectivo")
    {
        return await _repository.CrearReservaAsync(estilistaId, fechaCita, horaInicio, servicioIds, notas, metodoPago);
    }

    // Sobrecarga de conveniencia para un solo servicio
    public async Task<Reserva?> ExecuteAsync(
        long servicioId,
        long? estilistaId,
        DateTime fechaHora,
        string? notas)
    {
        var targetEstilista = estilistaId ?? 1;
        var horaInicio = fechaHora.ToString("HH:mm");
        var servicioIds = new List<int> { (int)servicioId };

        return await _repository.CrearReservaAsync(targetEstilista, fechaHora.Date, horaInicio, servicioIds, notas);
    }
}
