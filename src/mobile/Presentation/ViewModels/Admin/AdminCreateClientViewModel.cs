using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminCreateClientViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Nuevo Cliente";

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public AdminCreateClientViewModel()
    {
    }

    [RelayCommand]
    private async Task GuardarClienteAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ErrorMessage = "El nombre del cliente es obligatorio.";
            return;
        }

        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "El backend de Shushine Studio no dispone actualmente del endpoint 'POST /api/clientes'. Los clientes se registran mediante la app móvil o al agendar citas Walk-in en recepción.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task IrACitaWalkinAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateAppointmentPage", new Dictionary<string, object>
        {
            { "nombreClientePredefinido", $"{Nombre} {Apellido}".Trim() },
            { "telefonoClientePredefinido", Telefono }
        });
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
