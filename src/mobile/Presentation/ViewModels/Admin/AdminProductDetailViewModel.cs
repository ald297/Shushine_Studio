using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminProductDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Ficha de Producto";

    [ObservableProperty]
    private string avisoBackend = "El detalle y edición de productos físicos requiere soporte en el servidor para /api/productos. No se simulan ventas ni márgenes ficticios.";

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
