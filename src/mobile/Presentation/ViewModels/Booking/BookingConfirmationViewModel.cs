using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Booking;

/// <summary>
/// ViewModel para el comprobante de cita confirmada (US-4.02 / Wireframe Pág. 12).
/// Muestra el código de cita único (#SHU-XXXX) y permite navegar a Mis Citas o al Catálogo.
/// </summary>
[QueryProperty(nameof(Codigo), "codigo")]
[QueryProperty(nameof(ServicioNombre), "servicioNombre")]
[QueryProperty(nameof(EstilistaNombre), "estilistaNombre")]
[QueryProperty(nameof(Fecha), "fecha")]
[QueryProperty(nameof(Hora), "hora")]
[QueryProperty(nameof(Total), "total")]
public partial class BookingConfirmationViewModel : BaseViewModel
{
    [ObservableProperty]
    private string codigo = "#SHU-8492";

    [ObservableProperty]
    private string servicioNombre = string.Empty;

    [ObservableProperty]
    private string estilistaNombre = string.Empty;

    [ObservableProperty]
    private string fecha = string.Empty;

    [ObservableProperty]
    private string hora = string.Empty;

    [ObservableProperty]
    private string total = string.Empty;

    [ObservableProperty]
    private string sucursal = "Shushine Studio - Colonia San Benito, San Salvador";

    public BookingConfirmationViewModel()
    {
        Title = "¡Cita Confirmada!";
    }

    [RelayCommand]
    private async Task VerEnMisCitasAsync()
    {
        await Shell.Current.GoToAsync("//MainTabs/MyAppointmentsPage");
    }

    [RelayCommand]
    private async Task VolverAlInicioAsync()
    {
        await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
    }
}
