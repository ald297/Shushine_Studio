using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminProfessionalsViewModel : ObservableObject
{
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    private List<Estilista> _todosLosEstilistas = new();

    [ObservableProperty]
    private string title = "Profesionales";

    [ObservableProperty]
    private string subtitulo = "Equipo del salón";

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string filtroEstadoSeleccionado = "Todos"; // "Todos", "Activos", "Inactivos"

    [ObservableProperty]
    private int totalProfesionales;

    [ObservableProperty]
    private bool estaVacio;

    public ObservableCollection<Estilista> Estilistas { get; } = new();

    public AdminProfessionalsViewModel(
        IEstilistaRepository estilistaRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _estilistaRepository = estilistaRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
    }

    public async Task InicializarAsync()
    {
        await CargarPermisosAsync();
        if (IsAuthorized)
        {
            await CargarProfesionalesAsync();
        }
    }

    private async Task CargarPermisosAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            if (string.IsNullOrEmpty(rol) || !rol.ToUpperInvariant().Contains("ADMIN"))
            {
                IsUnauthorized = true;
                IsAuthorized = false;
                ErrorMessage = "Acceso denegado: Esta pantalla está reservada exclusivamente para administradores.";
                return;
            }

            IsUnauthorized = false;
            IsAuthorized = true;
        }
        catch
        {
            IsUnauthorized = true;
            IsAuthorized = false;
        }
    }

    [RelayCommand]
    public async Task CargarProfesionalesAsync()
    {
        if (IsBusy || !IsAuthorized) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            var lista = await _estilistaRepository.GetTodosEstilistasAsync();
            _todosLosEstilistas = lista?.ToList() ?? new List<Estilista>();

            AplicarFiltros();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al conectar con el servidor para consultar profesionales.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"[AdminProfessionalsViewModel] Error: {ex.Message}");
            Estilistas.Clear();
            EstaVacio = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnTextoBusquedaChanged(string value)
    {
        AplicarFiltros();
    }

    [RelayCommand]
    private void CambiarFiltroEstado(string estado)
    {
        if (FiltroEstadoSeleccionado == estado) return;
        FiltroEstadoSeleccionado = estado;
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        Estilistas.Clear();

        IEnumerable<Estilista> resultado = _todosLosEstilistas;

        // 1. Filtro por estado real
        if (FiltroEstadoSeleccionado == "Activos")
        {
            resultado = resultado.Where(e => e.Activo);
        }
        else if (FiltroEstadoSeleccionado == "Inactivos")
        {
            resultado = resultado.Where(e => !e.Activo);
        }

        // 2. Filtro de búsqueda local
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var query = TextoBusqueda.Trim().ToLowerInvariant();
            resultado = resultado.Where(e =>
                (!string.IsNullOrWhiteSpace(e.NombreCompleto) && e.NombreCompleto.ToLowerInvariant().Contains(query)) ||
                (!string.IsNullOrWhiteSpace(e.EspecialidadPrincipal) && e.EspecialidadPrincipal.ToLowerInvariant().Contains(query))
            );
        }

        foreach (var est in resultado)
        {
            Estilistas.Add(est);
        }

        TotalProfesionales = Estilistas.Count;
        EstaVacio = Estilistas.Count == 0 && !IsBusy;
    }

    [RelayCommand]
    private async Task SeleccionarEstilistaAsync(Estilista estilista)
    {
        if (estilista == null) return;

        await Shell.Current.GoToAsync("AdminProfessionalDetailPage", new Dictionary<string, object>
        {
            { "estilista", estilista },
            { "estilistaId", estilista.Id }
        });
    }

    [RelayCommand]
    private async Task NuevoProfesionalAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateProfessionalPage");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarALoginAsync()
    {
        await _authRepository.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
