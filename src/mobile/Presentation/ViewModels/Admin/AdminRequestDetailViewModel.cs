using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

[QueryProperty(nameof(SolicitudId), "id")]
public partial class AdminRequestDetailViewModel : ObservableObject
{
    private readonly ISolicitudRepository _solicitudRepository;

    [ObservableProperty]
    private string title = "Detalle de Solicitud";

    [ObservableProperty]
    private int solicitudId;

    [ObservableProperty]
    private SolicitudDiseno? solicitud;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isSaving = false;

    [ObservableProperty]
    private decimal precioPropuesto;

    [ObservableProperty]
    private string descripcionTrabajo = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool TieneSolicitud => Solicitud != null;

    public AdminRequestDetailViewModel(ISolicitudRepository solicitudRepository)
    {
        _solicitudRepository = solicitudRepository;
    }

    partial void OnSolicitudIdChanged(int value)
    {
        if (value > 0)
        {
            _ = CargarDetalleAsync(value);
        }
    }

    [RelayCommand]
    public async Task CargarDetalleAsync(int id)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Solicitud = await _solicitudRepository.GetSolicitudPorIdAdminAsync(id);

            if (Solicitud?.Cotizacion != null)
            {
                PrecioPropuesto = Solicitud.Cotizacion.PrecioPropuesto;
                DescripcionTrabajo = Solicitud.Cotizacion.DescripcionTrabajo;
            }

            OnPropertyChanged(nameof(TieneSolicitud));
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al cargar la solicitud: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task EnviarCotizacionAsync()
    {
        if (Solicitud == null) return;

        if (PrecioPropuesto <= 0)
        {
            await Shell.Current.DisplayAlert("Validación", "Por favor ingresa un precio propuesto válido mayor a 0.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(DescripcionTrabajo))
        {
            await Shell.Current.DisplayAlert("Validación", "Por favor ingresa las observaciones o descripción del trabajo.", "OK");
            return;
        }

        try
        {
            IsSaving = true;
            var actualizada = await _solicitudRepository.CotizarSolicitudAdminAsync(
                Solicitud.Id,
                PrecioPropuesto,
                DescripcionTrabajo.Trim());

            Solicitud = actualizada;
            OnPropertyChanged(nameof(TieneSolicitud));

            await Shell.Current.DisplayAlert("Cotización Enviada", "La cotización fue registrada en la base de datos y la clienta podrá visualizarla.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo registrar la cotización: " + ex.Message, "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
