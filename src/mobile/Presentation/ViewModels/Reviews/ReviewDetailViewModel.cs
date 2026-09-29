using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Reviews;

/// <summary>
/// ViewModel para la visualización y edición preparada de una reseña.
/// Muestra los detalles de la reseña y aclara que las operaciones de edición
/// o eliminación quedan sujetas a soporte de endpoints en el backend.
/// </summary>
public partial class ReviewDetailViewModel : BaseViewModel, IQueryAttributable
{
    [ObservableProperty]
    private Resena? resena;

    [ObservableProperty]
    private int estrellas = 5;

    [ObservableProperty]
    private string comentario = string.Empty;

    public bool BackendModificacionDisponible => false;

    public ReviewDetailViewModel()
    {
        Title = "Detalle de Reseña";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("resena", out var obj) && obj is Resena r)
        {
            CargarResena(r);
        }
    }

    private void CargarResena(Resena r)
    {
        Resena = r;
        Estrellas = r.Estrellas;
        Comentario = r.Comentario;
    }

    [RelayCommand]
    private async Task GuardarEdicionAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La edición de reseñas requiere del endpoint PUT /api/resenas/{id} en el servidor. La función estará activa cuando el backend lo incorpore.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task EliminarResenaAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La eliminación de reseñas requiere del endpoint DELETE /api/resenas/{id} en el servidor.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
