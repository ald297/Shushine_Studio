using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminServiceDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly IServicioRepository _servicioRepository;

    [ObservableProperty]
    private string title = "Detalle de Servicio";

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string codigoServicio = string.Empty;

    [ObservableProperty]
    private string categoriaNombre = "General";

    [ObservableProperty]
    private string descripcion = string.Empty;

    [ObservableProperty]
    private string precio = "0.00";

    [ObservableProperty]
    private int duracionMinutos = 45;

    [ObservableProperty]
    private bool activo = true;

    [ObservableProperty]
    private bool isEditing = false;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public AdminServiceDetailViewModel(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("servicio", out var sObj) && sObj is Servicio s)
        {
            CargarServicio(s);
        }
        else if (query.TryGetValue("servicioId", out var idObj) && idObj is long id)
        {
            ServicioId = id;
            await CargarPorIdAsync(id);
        }
    }

    private async Task CargarPorIdAsync(long id)
    {
        try
        {
            IsBusy = true;
            var s = await _servicioRepository.GetServicioByIdAsync(id);
            if (s != null)
            {
                CargarServicio(s);
            }
            else
            {
                ErrorMessage = "No se encontró el servicio solicitado.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al consultar el servicio: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarServicio(Servicio s)
    {
        Servicio = s;
        ServicioId = s.Id;
        Nombre = s.Nombre;
        CodigoServicio = !string.IsNullOrWhiteSpace(s.CodigoServicio) ? s.CodigoServicio : $"SRV-{s.Id}";
        CategoriaNombre = !string.IsNullOrWhiteSpace(s.CategoriaNombre) ? s.CategoriaNombre : "General";
        Descripcion = s.Descripcion ?? "Sin descripción detallada.";
        Precio = s.Precio.ToString("F2");
        DuracionMinutos = s.DuracionMinutos;
        Activo = s.Activo;
    }

    [RelayCommand]
    private void HabilitarEdicion()
    {
        IsEditing = true;
    }

    [RelayCommand]
    private void CancelarEdicion()
    {
        if (Servicio != null)
        {
            CargarServicio(Servicio);
        }
        IsEditing = false;
    }

    [RelayCommand]
    private async Task GuardarCambiosAsync()
    {
        if (IsBusy || Servicio == null) return;

        if (string.IsNullOrWhiteSpace(Nombre))
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "El nombre del servicio no puede estar vacío.", "Aceptar");
            }
            return;
        }

        if (!decimal.TryParse(Precio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var precioDecimal) || precioDecimal <= 0)
        {
            if (!decimal.TryParse(Precio, out precioDecimal) || precioDecimal <= 0)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese una tarifa válida mayor a $0.00.", "Aceptar");
                }
                return;
            }
        }

        try
        {
            IsBusy = true;

            Servicio.Nombre = Nombre.Trim();
            Servicio.Descripcion = Descripcion?.Trim() ?? string.Empty;
            Servicio.Precio = precioDecimal;
            Servicio.DuracionMinutos = DuracionMinutos > 0 ? DuracionMinutos : 45;
            Servicio.Activo = Activo;

            var exito = await _servicioRepository.ActualizarServicioAsync(Servicio);
            if (exito)
            {
                IsEditing = false;
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "El servicio ha sido actualizado en el servidor.", "Aceptar");
                }
            }
            else
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el servicio en la API.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un fallo: {ex.Message}", "Aceptar");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AlternarEstadoAsync()
    {
        if (IsBusy || Servicio == null) return;

        try
        {
            IsBusy = true;
            Activo = !Activo;
            Servicio.Activo = Activo;

            var exito = await _servicioRepository.ActualizarServicioAsync(Servicio);
            if (!exito)
            {
                Activo = !Activo; // Revertir
                Servicio.Activo = Activo;
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cambiar el estado en el servidor.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            Activo = !Activo;
            if (Servicio != null) Servicio.Activo = Activo;
            System.Diagnostics.Debug.WriteLine($"[AdminServiceDetailViewModel] Error estado: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EliminarServicioAsync()
    {
        if (IsBusy || Servicio == null) return;

        if (Application.Current?.MainPage != null)
        {
            var confirma = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Está seguro de eliminar '{Nombre}' del catálogo? Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar");

            if (!confirma) return;
        }

        try
        {
            IsBusy = true;
            var exito = await _servicioRepository.EliminarServicioAsync(ServicioId);
            if (exito)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "El servicio ha sido eliminado del catálogo.", "Aceptar");
                }
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "No se pudo eliminar el servicio del servidor.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdminServiceDetailViewModel] Error eliminar: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
