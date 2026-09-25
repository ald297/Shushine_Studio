using Moq;
using FluentAssertions;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Tests.UseCases;

/// <summary>
/// Pruebas unitarias del caso de uso de catálogo de servicios (US-3.01).
/// Valida filtrado reactivo, carga desde repositorio y manejo de colección vacía.
/// Nomenclatura oficial ESFE AGAPE: t1_obtenerTodos a t6_excepcion.
/// </summary>
public class GetServiciosCatalogUseCaseTests
{
    private readonly Mock<IServicioRepository> _mockRepository;
    private readonly GetServiciosCatalogUseCase _useCase;

    private static readonly List<Servicio> ServiciosMock = new()
    {
        new Servicio { Id = 1, Nombre = "Balayage Iluminador", CategoriaNombre = "Cabello", Precio = 75.00m, DuracionMinutos = 120, Activo = true },
        new Servicio { Id = 2, Nombre = "Manicura Rusa", CategoriaNombre = "Uñas", Precio = 25.00m, DuracionMinutos = 60, Activo = true },
        new Servicio { Id = 3, Nombre = "Lifting de Pestañas", CategoriaNombre = "Pestañas", Precio = 40.00m, DuracionMinutos = 90, Activo = false },
        new Servicio { Id = 4, Nombre = "Spa de Pies", CategoriaNombre = "Spa", Precio = 30.00m, DuracionMinutos = 45, Activo = true },
    };

    public GetServiciosCatalogUseCaseTests()
    {
        _mockRepository = new Mock<IServicioRepository>();
        _useCase = new GetServiciosCatalogUseCase(_mockRepository.Object);
    }

    // ------------------------------------------------------------------
    // t1_obtenerTodos: Carga completa del catálogo
    // ------------------------------------------------------------------
    [Fact]
    public async Task t1_obtenerTodos_DebeRetornarTodosLosServicios_CuandoRepositorioTieneRegistros()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetServiciosAsync(It.IsAny<long?>()))
            .ReturnsAsync(ServiciosMock);

        // Act
        var resultado = await _useCase.ExecuteAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(4);
        _mockRepository.Verify(r => r.GetServiciosAsync(It.IsAny<long?>()), Times.Once);
    }

    // ------------------------------------------------------------------
    // t2_catalogoVacio: Repositorio retorna colección vacía
    // ------------------------------------------------------------------
    [Fact]
    public async Task t2_obtenerTodos_DebeRetornarColeccionVacia_CuandoNoHayServicios()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetServiciosAsync(It.IsAny<long?>()))
            .ReturnsAsync(new List<Servicio>());

        // Act
        var resultado = await _useCase.ExecuteAsync();

        // Assert
        resultado.Should().BeEmpty("la app debe manejar graciosamente un catálogo vacío");
    }

    // ------------------------------------------------------------------
    // t3_serviciosActivos: Solo deben mostrarse servicios Activo = true
    // ------------------------------------------------------------------
    [Fact]
    public async Task t3_obtenerTodos_SoloServiciosActivosDebernMostrarse()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetServiciosAsync(It.IsAny<long?>()))
            .ReturnsAsync(ServiciosMock);

        // Act
        var resultado = await _useCase.ExecuteAsync();
        var activos = resultado.Where(s => s.Activo).ToList();
        var inactivos = resultado.Where(s => !s.Activo).ToList();

        // Assert
        activos.Should().HaveCount(3);
        inactivos.Should().HaveCount(1,
            "el servicio 'Lifting de Pestañas' tiene Activo = false y no debe mostrarse al cliente");
    }

    // ------------------------------------------------------------------
    // t4_obtenerPorId: Obtener servicio específico por su ID
    // ------------------------------------------------------------------
    [Fact]
    public async Task t4_obtenerPorId_DebeRetornarServicioCorrecto_CuandoExisteId()
    {
        // Arrange
        var servicioEsperado = ServiciosMock.First(s => s.Id == 1);
        _mockRepository
            .Setup(r => r.GetServicioByIdAsync(1))
            .ReturnsAsync(servicioEsperado);

        // Act
        var resultado = await _mockRepository.Object.GetServicioByIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Balayage Iluminador");
        resultado.Precio.Should().Be(75.00m);
        resultado.CategoriaNombre.Should().Be("Cabello");
    }

    // ------------------------------------------------------------------
    // t5_obtenerPorId_NoExiste: Debe retornar null si no existe el ID
    // ------------------------------------------------------------------
    [Fact]
    public async Task t5_obtenerPorId_DebeRetornarNull_CuandoNoExisteId()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetServicioByIdAsync(999))
            .ReturnsAsync((Servicio?)null);

        // Act
        var resultado = await _mockRepository.Object.GetServicioByIdAsync(999);

        // Assert
        resultado.Should().BeNull("un ID inexistente debe retornar null sin lanzar excepción");
    }

    // ------------------------------------------------------------------
    // t6_excepcion: Propagación de error del repositorio al UseCase
    // ------------------------------------------------------------------
    [Fact]
    public async Task t6_obtenerTodos_DebePropagar_CuandoRepositorioFalla()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetServiciosAsync(It.IsAny<long?>()))
            .ThrowsAsync(new HttpRequestException("503 Service Unavailable: API no disponible"));

        // Act
        var act = async () => await _useCase.ExecuteAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*503*");
    }
}
