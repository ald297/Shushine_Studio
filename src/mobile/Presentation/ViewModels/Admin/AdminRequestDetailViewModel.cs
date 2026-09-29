using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminRequestDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Detalle de Solicitud";

    [ObservableProperty]
    private string avisoBackend = "El análisis de referencias, cotizaciones personalizadas y conversión a citas requiere soporte backend específico (/api/solicitudes). No se simulan cotizaciones ni presupuestos no persistidos.";

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
