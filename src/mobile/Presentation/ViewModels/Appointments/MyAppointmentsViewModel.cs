using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

/// <summary>
/// ViewModel reactivo para la gestión e historial de citas del cliente (US-4.03 & US-4.04 / Wireframe Pág. 13).
/// Soporta pestañas para citas futuras y pasadas, y cancelación con validación de política de 2 horas.
/// </summary>
public partial class MyAppointmentsViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasProximas = new();

    [ObservableProperty]
    private ObservableCollection<Reserva> citasPasadas = new();

    [ObservableProperty]
    private bool mostrarFuturas = true;

    public MyAppointmentsViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Mis Citas";
        _ = LoadCitasAsync();
    }

    [RelayCommand]
    private void VerFuturas()
    {
        MostrarFuturas = true;
    }

    [RelayCommand]
    private void VerPasadas()
    {
        MostrarFuturas = false;
    }

    [RelayCommand]
    public async Task LoadCitasAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var citas = await _reservaRepository.GetMisCitasAsync();
            CitasProximas.Clear();
            CitasPasadas.Clear();

            var now = DateTime.Now;
            foreach (var cita in citas)
            {
                if (cita.Estado == "PENDIENTE" && cita.FechaHoraInicio >= now.AddHours(-1))
                {
                    CitasProximas.Add(cita);
                }
                else
                {
                    CitasPasadas.Add(cita);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelarCitaAsync(Reserva reserva)
    {
        if (reserva == null || Application.Current?.MainPage == null) return;

        // Validar política de cancelación (US-4.04: límite de 2 horas antes de la cita)
        var tiempoRestante = reserva.FechaHoraInicio - DateTime.Now;
        if (tiempoRestante.TotalHours < 2 && tiempoRestante.TotalSeconds > 0)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Política de Cancelación", 
                "No es posible cancelar la cita con menos de 2 horas de anticipación desde la app. Por favor contacta directamente a recepción al (+503) 2400-0000.", 
                "Entendido"
            );
            return;
        }

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Cancelar Reserva", 
            $"¿Estás segura de cancelar tu cita para {reserva.ServicioNombre} ({reserva.CodigoReserva})?", 
            "Sí, Cancelar", 
            "Mantener Cita"
        );

        if (confirm)
        {
            IsBusy = true;
            try
            {
                var success = await _reservaRepository.CancelarReservaAsync(reserva.Id);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Cita Cancelada", 
                        "Tu reserva ha sido cancelada y el espacio del estilista ha sido liberado.", 
                        "Aceptar"
                    );
                    await LoadCitasAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ReprogramarCitaAsync(Reserva reserva)
    {
        if (reserva == null) return;

        // Redirigir al catálogo para seleccionar nuevo horario
        await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
    }
}
