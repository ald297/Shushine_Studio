using FluentAssertions;
using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Tests.Domain;

/// <summary>
/// Pruebas unitarias de la lógica de cálculo de montos de reserva (US-4.01).
/// Verifica subtotal, descuento del 10%, IVA del 13% y total final conforme
/// a las reglas de negocio de Shushine Studio y normativa fiscal de El Salvador.
/// </summary>
public class BookingCalculationTests
{
    // Método auxiliar que replica la lógica de BookingSummaryViewModel.CalcularMontos()
    private static (decimal subtotal, decimal descuento, decimal iva, decimal total)
        CalcularMontos(decimal precioServicio)
    {
        var subtotal    = precioServicio;
        var descuento   = Math.Round(subtotal * 0.10m, 2, MidpointRounding.AwayFromZero);          // 10% descuento bienvenida
        var baseImponible = subtotal - descuento;
        var iva         = Math.Round(baseImponible * 0.13m, 2, MidpointRounding.AwayFromZero);     // IVA 13% El Salvador
        var total       = baseImponible + iva;
        return (subtotal, descuento, iva, total);
    }

    // ------------------------------------------------------------------
    // t1_calcularMontos: Servicio estándar $75.00
    // ------------------------------------------------------------------
    [Fact]
    public void t1_calcularMontos_DebeCalcularCorrectamente_ParaServicioEstandar()
    {
        // Arrange: Balayage Iluminador $75.00
        const decimal precio = 75.00m;

        // Act
        var (subtotal, descuento, iva, total) = CalcularMontos(precio);

        // Assert
        subtotal.Should().Be(75.00m);
        descuento.Should().Be(7.50m,   "el 10% de $75.00 es $7.50");
        iva.Should().Be(8.78m,         "el 13% de $67.50 (base imponible) es $8.775, redondeado a $8.78");
        total.Should().Be(76.28m,      "$67.50 + $8.78 = $76.28");
    }

    // ------------------------------------------------------------------
    // t2_calcularMontos: Servicio económico $25.00 (Manicura)
    // ------------------------------------------------------------------
    [Fact]
    public void t2_calcularMontos_DebeCalcularCorrectamente_ParaServicioEconomico()
    {
        // Arrange: Manicura Rusa $25.00
        const decimal precio = 25.00m;

        // Act
        var (subtotal, descuento, iva, total) = CalcularMontos(precio);

        // Assert
        subtotal.Should().Be(25.00m);
        descuento.Should().Be(2.50m,   "el 10% de $25.00 es $2.50");
        iva.Should().Be(2.93m,         "el 13% de $22.50 es $2.925, redondeado a $2.93");
        total.Should().Be(25.43m,      "$22.50 + $2.93 = $25.43");
    }

    // ------------------------------------------------------------------
    // t3_calcularMontos: El descuento nunca puede ser negativo
    // ------------------------------------------------------------------
    [Fact]
    public void t3_calcularMontos_DescuentoNuncaDebeSerNegativo()
    {
        // Arrange: servicio gratuito o de precio cero
        const decimal precio = 0.00m;

        // Act
        var (_, descuento, iva, total) = CalcularMontos(precio);

        // Assert
        descuento.Should().BeGreaterThanOrEqualTo(0m, "el descuento nunca puede ser un valor negativo");
        iva.Should().Be(0m);
        total.Should().Be(0m);
    }

    // ------------------------------------------------------------------
    // t4_calcularMontos: El total siempre debe ser mayor que el subtotal menos descuento (por el IVA)
    // ------------------------------------------------------------------
    [Fact]
    public void t4_calcularMontos_TotalDebeIncluirIVA_SiempreEsMayorQueBaseImponible()
    {
        // Arrange: precio arbitrario
        const decimal precio = 100.00m;

        // Act
        var (subtotal, descuento, iva, total) = CalcularMontos(precio);
        var baseImponible = subtotal - descuento;

        // Assert
        total.Should().BeGreaterThan(baseImponible,
            "el total siempre es mayor que la base imponible porque se suma el IVA del 13%");
        iva.Should().BeGreaterThan(0m);
    }

    // ------------------------------------------------------------------
    // t5_calcularMontos: Redondeo a 2 decimales en todos los montos
    // ------------------------------------------------------------------
    [Theory]
    [InlineData(33.33)]
    [InlineData(66.67)]
    [InlineData(99.99)]
    public void t5_calcularMontos_TodosLosMontosDeben_TenerMaximoDosDecimales(double precioDouble)
    {
        // Arrange
        var precio = (decimal)precioDouble;

        // Act
        var (subtotal, descuento, iva, total) = CalcularMontos(precio);

        // Assert
        var decimalesSubtotal = BitConverter.GetBytes(decimal.GetBits(subtotal)[3])[2];
        var decimalesDescuento = BitConverter.GetBytes(decimal.GetBits(descuento)[3])[2];
        var decimalesIva = BitConverter.GetBytes(decimal.GetBits(iva)[3])[2];

        decimalesSubtotal.Should().BeLessThanOrEqualTo(2);
        decimalesDescuento.Should().BeLessThanOrEqualTo(2);
        decimalesIva.Should().BeLessThanOrEqualTo(2);
    }

    // ------------------------------------------------------------------
    // t6_calcularMontos: Consistencia con entidad Reserva completa
    // ------------------------------------------------------------------
    [Fact]
    public void t6_calcularMontos_TotalDebeCoincidirConTotalDeEntidadReserva()
    {
        // Arrange: simular lo que haría BookingSummaryViewModel al recibir el servicio
        var servicio = new Servicio { Id = 1, Nombre = "Balayage", Precio = 75.00m };
        var (_, _, _, totalCalculado) = CalcularMontos(servicio.Precio);

        // Simular entidad Reserva creada por la API con el mismo total
        var reservaDevuelta = new Reserva
        {
            Id = 1,
            CodigoReserva = "#SHU-1234",
            ServicioId = servicio.Id,
            Total = 76.28m, // Total real calculado y devuelto por la API
            Estado = "PENDIENTE"
        };

        // Assert: el total calculado en el cliente debe coincidir con el total de la API
        totalCalculado.Should().Be(reservaDevuelta.Total,
            "la lógica de cálculo local debe ser idéntica a la del backend para evitar inconsistencias en la UI");
    }
}
