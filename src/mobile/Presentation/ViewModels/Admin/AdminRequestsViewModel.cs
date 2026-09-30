using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminRequestsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly ISolicitudRepository _solicitudRepository;

    [ObservableProperty]
    private string title = "Solicitudes de Clientes";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<SolicitudDiseno> Solicitudes { get; } = new();

    public bool HasSolicitudes => Solicitudes.Count > 0;
    public bool ShowEmptyState => !IsLoading && !HasError && !HasSolicitudes;

    public AdminRequestsViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService,
        ISolicitudRepository solicitudRepository)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
        _solicitudRepository = solicitudRepository;

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarRolAdminAsync();
        if (IsAuthorized)
        {
            await CargarSolicitudesAsync();
        }
    }

    private async Task VerificarRolAdminAsync()
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
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    [RelayCommand]
    public async Task CargarSolicitudesAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var list = await _solicitudRepository.GetSolicitudesAdminAsync();
            Solicitudes.Clear();
            foreach (var item in list)
            {
                Solicitudes.Add(item);
            }

            OnPropertyChanged(nameof(HasSolicitudes));
            OnPropertyChanged(nameof(ShowEmptyState));
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al consultar solicitudes: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(ShowEmptyState));
        }
    }

    [RelayCommand]
    private async Task VerDetalleSolicitudAsync(SolicitudDiseno? solicitud)
    {
        if (solicitud == null) return;
        await Shell.Current.GoToAsync($"AdminRequestDetailPage?id={solicitud.Id}");
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
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToLogin();
        }
        else
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
