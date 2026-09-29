using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminCreateServiceViewModel : ObservableObject
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Nuevo Servicio";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string codigoServicio = string.Empty;

    [ObservableProperty]
    private string categoriaSeleccionada = "Cabello";

    [ObservableProperty]
    private string precio = "25.00";

    [ObservableProperty]
    private int duracionMinutos = 45;

    [ObservableProperty]
    private string descripcion = string.Empty;

    [ObservableProperty]
    private bool activo = true;

    public ObservableCollection<string> CategoriasDisponibles { get; } = new()
    {
        "Cabello",
        "Uñas",
        "Maquillaje",
        "Spa",
        "Faciales"
    };

    public AdminCreateServiceViewModel(
        IServicioRepository servicioRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _servicioRepository = servicioRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        CodigoServicio = $"SRV-{Random.Shared.Next(100, 999)}";
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            var esAdmin = !string.IsNullOrEmpty(rol) && rol.ToUpperInvariant().Contains("ADMIN");
            IsAuthorized = esAdmin;
            IsUnauthorized = !esAdmin;

            if (IsAuthorized)
            {
                var cats = await _servicioRepository.GetCategoriasAsync();
                var lista = cats.Where(c => c != "Todos").ToList();
                if (lista.Count > 0)
                {
                    CategoriasDisponibles.Clear();
                    foreach (var c in lista)
                    {
                        CategoriasDisponibles.Add(c);
                    }
                    CategoriaSeleccionada = CategoriasDisponibles.FirstOrDefault() ?? "Cabello";
                }
            }
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Nombre))
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Por favor ingrese el nombre del servicio.", "Aceptar");
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

        if (DuracionMinutos <= 0)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "La duración debe ser mayor a 0 minutos.", "Aceptar");
            }
            return;
        }

        try
        {
            IsBusy = true;

            var nuevoServicio = new Servicio
            {
                Nombre = Nombre.Trim(),
                CodigoServicio = !string.IsNullOrWhiteSpace(CodigoServicio) ? CodigoServicio.Trim() : $"SRV-{Random.Shared.Next(100, 999)}",
                Descripcion = Descripcion?.Trim() ?? string.Empty,
                Precio = precioDecimal,
                DuracionMinutos = DuracionMinutos,
                CategoriaNombre = CategoriaSeleccionada,
                CategoriaId = ObtenerCategoriaId(CategoriaSeleccionada),
                Activo = Activo
            };

            var creado = await _servicioRepository.CrearServicioAsync(nuevoServicio);

            if (creado != null)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", $"El servicio '{creado.Nombre}' fue creado exitosamente en el catálogo.", "Aceptar");
                }
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el servicio en el servidor. Verifique la conexión con la API.", "Aceptar");
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

    private static long ObtenerCategoriaId(string categoria)
    {
        return categoria.ToLowerInvariant() switch
        {
            "cabello" => 1,
            "uñas" or "unas" => 2,
            "maquillaje" => 3,
            "spa" => 4,
            "faciales" => 5,
            _ => 1
        };
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
