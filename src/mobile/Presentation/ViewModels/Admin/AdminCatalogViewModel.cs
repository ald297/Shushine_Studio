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

    // Toast Boutique Flotante (Reemplaza los alertas nativos grises)
    [ObservableProperty]
    private bool isToastVisible;

    [ObservableProperty]
    private string toastTitulo = string.Empty;

    [ObservableProperty]
    private string toastMensaje = string.Empty;

    public void MostrarToast(string titulo, string mensaje)
    {
        ToastTitulo = titulo;
        ToastMensaje = mensaje;
        IsToastVisible = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(3500);
            MainThread.BeginInvokeOnMainThread(() => IsToastVisible = false);
        });
    }

    [RelayCommand]
    private void CerrarToast()
    {
        IsToastVisible = false;
    }

    // Estado del Modal de Edición Completa
    [ObservableProperty]
    private bool isEditando;

    [ObservableProperty]
    private long editandoId;

    [ObservableProperty]
    private string editandoNombre = string.Empty;

    [ObservableProperty]
    private string editandoCategoria = "Cabello";

    [ObservableProperty]
    private string editandoPrecio = "0.00";

    [ObservableProperty]
    private int editandoDuracion = 45;

    [ObservableProperty]
    private string editandoDescripcion = string.Empty;

    [ObservableProperty]
    private string editandoImagenUrl = string.Empty;

    [ObservableProperty]
    private bool editandoActivo = true;

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
    // Modal de Edición Completa de Servicio (Wireframe Pág. 18)
    // ========================================================
    [RelayCommand]
    private void AbrirEditar(Servicio servicio)
    {
        if (servicio == null) return;
        EditandoId = servicio.Id;
        EditandoNombre = servicio.Nombre;
        EditandoCategoria = servicio.CategoriaNombre ?? "Cabello";
        EditandoPrecio = servicio.Precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        EditandoDuracion = servicio.DuracionMinutos > 0 ? servicio.DuracionMinutos : 45;
        EditandoDescripcion = servicio.Descripcion ?? string.Empty;
        EditandoImagenUrl = servicio.ImagenUrl ?? string.Empty;
        EditandoActivo = servicio.Activo;
        IsEditando = true;
    }

    [RelayCommand]
    private void CerrarEditar()
    {
        IsEditando = false;
    }

    [RelayCommand]
    private async Task SeleccionarImagenEditarAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Seleccionar Imagen del Servicio"
            });
            if (result != null)
            {
                EditandoImagenUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Imagen", $"No se pudo abrir la galería: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SeleccionarImagenNuevoAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Seleccionar Imagen del Servicio"
            });
            if (result != null)
            {
                NuevoImagenUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Imagen", $"No se pudo abrir la galería: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GuardarEdicionCompletaAsync()
    {
        if (string.IsNullOrWhiteSpace(EditandoNombre))
        {
            await ShowAlertAsync("Campo Requerido", "Por favor ingresa el nombre del tratamiento.");
            return;
        }

        if (!decimal.TryParse(EditandoPrecio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var precio) || precio <= 0)
        {
            await ShowAlertAsync("Tarifa Inválida", "Ingresa una tarifa numérica válida mayor a 0.");
            return;
        }

        try
        {
            IsBusy = true;

            var servicio = _todosLosServicios.FirstOrDefault(s => s.Id == EditandoId);
            if (servicio != null)
            {
                int categoriaId = EditandoCategoria switch
                {
                    "Cabello" => 1,
                    "Uñas" => 2,
                    "Maquillaje" => 3,
                    "Spa" => 4,
                    _ => 1
                };

                servicio.Nombre = EditandoNombre.Trim();
                servicio.CategoriaId = categoriaId;
                servicio.CategoriaNombre = EditandoCategoria;
                servicio.Precio = precio;
                servicio.DuracionMinutos = EditandoDuracion > 0 ? EditandoDuracion : 45;
                servicio.Descripcion = !string.IsNullOrWhiteSpace(EditandoDescripcion) 
                    ? EditandoDescripcion.Trim() 
                    : $"Tratamiento profesional de {EditandoCategoria} en Shushine Studio.";
                servicio.ImagenUrl = !string.IsNullOrWhiteSpace(EditandoImagenUrl) ? EditandoImagenUrl.Trim() : null;
                servicio.Activo = EditandoActivo;

                // Persistir cambio completo en la Web API de Render
                var exito = await _servicioRepository.ActualizarServicioAsync(servicio);

                IsEditando = false;
                ActualizarContadores();
                AplicarFiltro();

                MostrarToast(
                    "Tratamiento Actualizado",
                    $"La tarifa y detalles de \"{servicio.Nombre}\" se guardaron correctamente."
                );
            }
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

            MostrarToast("Tratamiento Eliminado", $"\"{servicio.Nombre}\" ha sido retirado del catálogo.");
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

            MostrarToast(
                "¡Servicio Registrado!",
                $"\"{nuevo.Nombre}\" ha sido publicado en el catálogo oficial."
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

    private Task ShowAlertAsync(string title, string message)
    {
        MostrarToast(title, message);
        return Task.CompletedTask;
    }
}
