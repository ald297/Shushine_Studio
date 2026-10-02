using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminReviewsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IResenaRepository _resenaRepository;

    [ObservableProperty]
    private string title = "Reseñas de Clientes";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private ObservableCollection<Resena> resenas = new();

    [ObservableProperty]
    private bool tieneResenas;

    [ObservableProperty]
    private bool noTieneResenas = true;

    public AdminReviewsViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService,
        IResenaRepository resenaRepository)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
        _resenaRepository = resenaRepository;

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarRolAdminAsync();
        if (IsAuthorized)
        {
            await CargarResenasAsync();
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
    public async Task CargarResenasAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var lista = await _resenaRepository.GetResenasAdminAsync();
            Resenas.Clear();
            foreach (var r in lista)
            {
                Resenas.Add(r);
            }

            TieneResenas = Resenas.Count > 0;
            NoTieneResenas = !TieneResenas;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al sincronizar reseñas: " + ex.Message;
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
