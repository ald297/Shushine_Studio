using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

public partial class MyAppointmentsViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasProximas = new();

    [ObservableProperty]
    private ObservableCollection<Reserva> citasPasadas = new();

    public MyAppointmentsViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Mis Citas";
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
                if (cita.FechaHoraInicio >= now && cita.Estado != "CANCELADA")
                    CitasProximas.Add(cita);
                else
                    CitasPasadas.Add(cita);
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

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Cancelar Cita", 
            $"¿Estás segura de cancelar tu cita para {reserva.ServicioNombre}?", 
            "Sí, Cancelar", 
            "Volver"
        );

        if (confirm)
        {
            IsBusy = true;
            try
            {
                var success = await _reservaRepository.CancelarReservaAsync(reserva.Id);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Cita Cancelada", "Tu cita ha sido cancelada exitosamente.", "Aceptar");
                    await LoadCitasAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
