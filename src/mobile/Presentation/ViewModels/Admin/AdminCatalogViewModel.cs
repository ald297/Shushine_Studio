using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel de administración del catálogo de servicios (Wireframe Pág. 18).
/// Soporta CRUD completo persistente con la Web API en Render, edición in-card de tarifas
/// y creación modal con fotografía de servicios.
/// </summary>
public partial class AdminCatalogViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string filtroEstado = "Todos"; // "Todos", "Activos", "Inactivos"

    [ObservableProperty]
    private int totalRegistrados;

    [ObservableProperty]
    private int totalActivos;

    [ObservableProperty]
    private int totalInactivos;

    // Estado de Edición In-Card (Wireframe Pág. 18)
    [ObservableProperty]
    private long? servicioEnEdicionId;

    [ObservableProperty]
    private string precioEnEdicion = string.Empty;

    [ObservableProperty]
    private string imagenUrlEnEdicion = string.Empty;

    // Estado del Modal de Nuevo Servicio
    [ObservableProperty]
    private bool isCreandoNuevo;

    [ObservableProperty]
    private string nuevoNombre = string.Empty;

    [ObservableProperty]
    private string nuevoCategoria = "Cabello";

    [ObservableProperty]
    private string nuevoPrecio = "30.00";

    [ObservableProperty]
    private int nuevoDuracion = 45;

    [ObservableProperty]
    private string nuevoDescripcion = string.Empty;

    [ObservableProperty]
    private string nuevoImagenUrl = string.Empty;

    private List<Servicio> _todosLosServicios = new();

    public List<string> CategoriasDisponibles { get; } = new() { "Cabello", "Uñas", "Maquillaje", "Spa" };

    public AdminCatalogViewModel(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
        Title = "Control de Tarifas & Catálogo";
        _ = LoadCatalogAsync();
    }

    [RelayCommand]
    public async Task LoadCatalogAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var lista = await _servicioRepository.GetServiciosAsync();
            _todosLosServicios = lista.ToList();

            ActualizarContadores();
            AplicarFiltro();
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

    partial void OnTextoBusquedaChanged(string value) => AplicarFiltro();

    [RelayCommand]
    public void SeleccionarFiltroEstado(string estado)
    {
        FiltroEstado = estado;
        AplicarFiltro();
    }

    private void ActualizarContadores()
    {
        TotalRegistrados = _todosLosServicios.Count;
        TotalActivos = _todosLosServicios.Count(s => s.Activo);
        TotalInactivos = _todosLosServicios.Count(s => !s.Activo);
    }

    private void AplicarFiltro()
    {
        Servicios.Clear();

        IEnumerable<Servicio> filtrados = _todosLosServicios;

        if (FiltroEstado == "Activos")
        {
            filtrados = filtrados.Where(s => s.Activo);
        }
        else if (FiltroEstado == "Inactivos")
        {
            filtrados = filtrados.Where(s => !s.Activo);
        }

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            filtrados = filtrados.Where(s =>
                s.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                (s.CategoriaNombre ?? "").Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var s in filtrados)
        {
            Servicios.Add(s);
        }
    }

    // ========================================================
    // Edición In-Card de Tarifa (Wireframe Pág. 18)
    // ========================================================
    [RelayCommand]
    private void IniciarEdicion(Servicio servicio)
    {
        if (servicio == null) return;
        ServicioEnEdicionId = servicio.Id;
        PrecioEnEdicion = servicio.Precio.ToString("F2");
        ImagenUrlEnEdicion = servicio.ImagenUrl ?? string.Empty;
    }

    [RelayCommand]
    private void CancelarEdicion()
    {
        ServicioEnEdicionId = null;
        PrecioEnEdicion = string.Empty;
        ImagenUrlEnEdicion = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarEdicionAsync(Servicio servicio)
    {
        if (servicio == null) return;

        if (!decimal.TryParse(PrecioEnEdicion, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var nuevoPrecio) || nuevoPrecio <= 0)
        {
            await ShowAlertAsync("Tarifa Inválida", "Por favor ingresa un precio numérico mayor a 0.");
            return;
        }

        try
        {
            IsBusy = true;
            servicio.Precio = nuevoPrecio;
            if (!string.IsNullOrWhiteSpace(ImagenUrlEnEdicion))
            {
                servicio.ImagenUrl = ImagenUrlEnEdicion.Trim();
            }

            // Persistir cambio en la Web API de Render
            var exito = await _servicioRepository.ActualizarServicioAsync(servicio);
            
            ServicioEnEdicionId = null;
            ActualizarContadores();
            AplicarFiltro();

            await ShowAlertAsync(
                "Tarifa Actualizada",
                exito 
                    ? $"El precio de \"{servicio.Nombre}\" se actualizó a ${servicio.Precio:F2} en la base de datos."
                    : $"Tarifa actualizada localmente a ${servicio.Precio:F2}."
            );
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", $"No se pudo actualizar: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ========================================================
    // Switch Activar / Desactivar Servicio (Wireframe Pág. 18)
    // ========================================================
    [RelayCommand]
    private async Task ToggleActivoAsync(Servicio servicio)
    {
        if (servicio == null) return;

        try
        {
            servicio.Activo = !servicio.Activo;
            await _servicioRepository.ActualizarServicioAsync(servicio);
            ActualizarContadores();
            AplicarFiltro();
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
    }

    // ========================================================
    // Eliminar Servicio
    // ========================================================
    [RelayCommand]
    private async Task EliminarServicioAsync(Servicio servicio)
    {
        if (servicio == null || Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Eliminar Tratamiento",
            $"¿Confirmas que deseas eliminar permanentemente \"{servicio.Nombre}\" del catálogo oficial?",
            "Sí, Eliminar",
            "Cancelar"
        );

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _servicioRepository.EliminarServicioAsync(servicio.Id);

            _todosLosServicios.Remove(servicio);
            ActualizarContadores();
            AplicarFiltro();

            await ShowAlertAsync("Servicio Eliminado", $"\"{servicio.Nombre}\" ha sido retirado del catálogo.");
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ========================================================
    // Modal de Creación de Nuevo Servicio (con Foto y Tarifa)
    // ========================================================
    [RelayCommand]
    private void AbrirModalNuevo()
    {
        NuevoNombre = string.Empty;
        NuevoCategoria = "Cabello";
        NuevoPrecio = "35.00";
        NuevoDuracion = 45;
        NuevoDescripcion = string.Empty;
        NuevoImagenUrl = string.Empty;
        IsCreandoNuevo = true;
    }

    [RelayCommand]
    private void CerrarModalNuevo()
    {
        IsCreandoNuevo = false;
    }

    [RelayCommand]
    private async Task CrearNuevoServicioAsync()
    {
        if (string.IsNullOrWhiteSpace(NuevoNombre))
        {
            await ShowAlertAsync("Campo Requerido", "Por favor ingresa el nombre del tratamiento.");
            return;
        }

        if (!decimal.TryParse(NuevoPrecio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var precio) || precio <= 0)
        {
            await ShowAlertAsync("Tarifa Inválida", "Ingresa una tarifa numérica válida.");
            return;
        }

        try
        {
            IsBusy = true;

            int categoriaId = NuevoCategoria switch
            {
                "Cabello" => 1,
                "Uñas" => 2,
                "Maquillaje" => 3,
                "Spa" => 4,
                _ => 1
            };

            var nuevo = new Servicio
            {
                Nombre = NuevoNombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(NuevoDescripcion) 
                    ? $"Tratamiento profesional de {NuevoCategoria} en Shushine Studio." 
                    : NuevoDescripcion.Trim(),
                Precio = precio,
                DuracionMinutos = NuevoDuracion > 0 ? NuevoDuracion : 45,
                CategoriaId = categoriaId,
                CategoriaNombre = NuevoCategoria,
                ImagenUrl = !string.IsNullOrWhiteSpace(NuevoImagenUrl) ? NuevoImagenUrl.Trim() : null,
                Activo = true
            };

            var creado = await _servicioRepository.CrearServicioAsync(nuevo);

            _todosLosServicios.Insert(0, creado ?? nuevo);
            ActualizarContadores();
            AplicarFiltro();

            IsCreandoNuevo = false;

            await ShowAlertAsync(
                "¡Servicio Registrado!",
                $"\"{nuevo.Nombre}\" ha sido publicado en el catálogo oficial con tarifa de ${nuevo.Precio:F2}."
            );
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error al Guardar", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task ShowAlertAsync(string title, string message)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Aceptar");
        }
    }
}
