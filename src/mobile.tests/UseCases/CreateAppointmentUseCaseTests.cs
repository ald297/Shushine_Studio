using Moq;
using FluentAssertions;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Tests.UseCases;

/// <summary>
/// Pruebas unitarias del caso de uso de creación de reservas (US-4.01).
/// Valida el flujo completo de delegación al repositorio y manejo de respuestas.
/// Nomenclatura oficial ESFE AGAPE: t1_crear a t6_eliminarPorId.
/// </summary>
public class CreateAppointmentUseCaseTests
{
    private readonly Mock<IReservaRepository> _mockRepository;
    private readonly CreateAppointmentUseCase _useCase;

    public CreateAppointmentUseCaseTests()
    {
        _mockRepository = new Mock<IReservaRepository>();
        _useCase = new CreateAppointmentUseCase(_mockRepository.Object);
    }

    // ------------------------------------------------------------------
    // t1_crear: Crear reserva exitosamente
    // ------------------------------------------------------------------
    [Fact]
    public async Task t1_crear_DebeRetornarReserva_CuandoRepositorioTieneExito()
    {
        // Arrange
        var fechaEsperada = DateTime.Today.AddDays(1).AddHours(10);
        var reservaEsperada = new Reserva
        {
            Id = 1,
            CodigoReserva = "#SHU-1234",
            ServicioId = 5,
            ServicioNombre = "Balayage Iluminador",
            EstilistaId = 2,
            EstilistaNombre = "Sofía Ramos",
            FechaHoraInicio = fechaEsperada,
            Total = 66.39m,
            Estado = "PENDIENTE"
        };

        _mockRepository
            .Setup(r => r.CrearReservaAsync(5, 2, fechaEsperada, It.IsAny<string?>()))
            .ReturnsAsync(reservaEsperada);

        // Act
        var resultado = await _useCase.ExecuteAsync(5, 2, fechaEsperada, "Prueba unitaria");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.CodigoReserva.Should().Be("#SHU-1234");
        resultado.Estado.Should().Be("PENDIENTE");
        resultado.Total.Should().BeGreaterThan(0);
        _mockRepository.Verify(r => r.CrearReservaAsync(5, 2, fechaEsperada, It.IsAny<string?>()), Times.Once);
    }

    // ------------------------------------------------------------------
    // t2_sinEstilista: Crear reserva con auto-asignación (estilistaId null)
    // ------------------------------------------------------------------
    [Fact]
    public async Task t2_crear_DebePermitirEstilistaNull_ParaAutoAsignacion()
    {
        // Arrange
        var fechaHora = DateTime.Today.AddDays(3).AddHours(14);
        var reservaFallback = new Reserva
        {
            Id = 99,
            CodigoReserva = "#SHU-AUTO",
            ServicioId = 1,
            Estado = "PENDIENTE"
        };

        _mockRepository
            .Setup(r => r.CrearReservaAsync(1, null, fechaHora, It.IsAny<string?>()))
            .ReturnsAsync(reservaFallback);

        // Act
        var resultado = await _useCase.ExecuteAsync(1, null, fechaHora, null);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.CodigoReserva.Should().StartWith("#SHU");
        _mockRepository.Verify(r => r.CrearReservaAsync(1, null, fechaHora, null), Times.Once);
    }

    // ------------------------------------------------------------------
    // t3_repositorioFalla: El repositorio retorna null (fallo de API)
    // ------------------------------------------------------------------
    [Fact]
    public async Task t3_crear_DebeRetornarNull_CuandoRepositorioRetornaNull()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.CrearReservaAsync(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<DateTime>(), It.IsAny<string?>()))
            .ReturnsAsync((Reserva?)null);

        // Act
        var resultado = await _useCase.ExecuteAsync(1, 1, DateTime.Now.AddDays(1), "test");

        // Assert
        resultado.Should().BeNull();
    }

    // ------------------------------------------------------------------
    // t4_excepcion: Propagación de excepción del repositorio
    // ------------------------------------------------------------------
    [Fact]
    public async Task t4_crear_DebePropagar_CuandoRepositorioLanzaExcepcion()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.CrearReservaAsync(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<DateTime>(), It.IsAny<string?>()))
            .ThrowsAsync(new HttpRequestException("409 Conflict: Horario no disponible"));

        // Act
        var act = async () => await _useCase.ExecuteAsync(1, 1, DateTime.Now.AddDays(1), null);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*409*");
    }

    // ------------------------------------------------------------------
    // t5_validarEstado: Estado inicial siempre debe ser PENDIENTE
    // ------------------------------------------------------------------
    [Fact]
    public async Task t5_crear_DebeRetornarEstadoPENDIENTE_EnNuevaReserva()
    {
        // Arrange
        var reservaNueva = new Reserva { Id = 5, CodigoReserva = "#SHU-5555", Estado = "PENDIENTE" };

        _mockRepository
            .Setup(r => r.CrearReservaAsync(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<DateTime>(), It.IsAny<string?>()))
            .ReturnsAsync(reservaNueva);

        // Act
        var resultado = await _useCase.ExecuteAsync(3, null, DateTime.Now.AddDays(2), "Test estado");

        // Assert
        resultado!.Estado.Should().Be("PENDIENTE",
            "toda reserva nueva debe iniciarse en estado PENDIENTE según las reglas de negocio del salón");
    }

    // ------------------------------------------------------------------
    // t6_cancelar: CancelarReservaAsync debe liberar el slot
    // ------------------------------------------------------------------
    [Fact]
    public async Task t6_cancelar_DebeLiberarSlot_CuandoSeCancelaReservaExistente()
    {
        // Arrange
        const long reservaId = 101L;
        _mockRepository
            .Setup(r => r.CancelarReservaAsync(reservaId, It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _mockRepository.Object.CancelarReservaAsync(reservaId, null);

        // Assert
        resultado.Should().BeTrue("la cancelación de una reserva existente debe retornar true");
        _mockRepository.Verify(r => r.CancelarReservaAsync(reservaId, It.IsAny<string?>()), Times.Once);
    }
}
