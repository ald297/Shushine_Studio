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

        await IrACitaWalkinAsync();
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
