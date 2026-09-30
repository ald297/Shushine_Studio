using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminMessagesViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IChatRepository _chatRepository;
    private CancellationTokenSource? _pollingCts;

    [ObservableProperty]
    private string title = "Mensajes";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasConversaciones = false;

    [ObservableProperty]
    private bool showEmptyState = false;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<ConversacionChat> Conversaciones { get; } = new();

    public AdminMessagesViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService,
        IChatRepository chatRepository)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
        _chatRepository = chatRepository;

        _ = VerificarRolAdminAsync();
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

            if (esAdmin)
            {
                await CargarConversacionesAsync();
            }
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    public async Task CargarConversacionesAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var lista = await _chatRepository.ObtenerConversacionesAdminAsync();

            Conversaciones.Clear();
            foreach (var item in lista)
            {
                Conversaciones.Add(item);
            }

            HasConversaciones = Conversaciones.Count > 0;
            ShowEmptyState = !HasConversaciones;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al cargar conversaciones: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void IniciarPolling()
    {
        DetenerPolling();
        if (!IsAuthorized) return;

        _pollingCts = new CancellationTokenSource();
        var token = _pollingCts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(6000, token);
                    if (token.IsCancellationRequested) break;

                    var lista = await _chatRepository.ObtenerConversacionesAdminAsync();
                    if (lista != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Conversaciones.Clear();
                            foreach (var item in lista)
                            {
                                Conversaciones.Add(item);
                            }
                            HasConversaciones = Conversaciones.Count > 0;
                            ShowEmptyState = !HasConversaciones;
                        });
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch
                {
                    // Ignorar error transitorio en sondeo silencioso
                }
            }
        }, token);
    }

    public void DetenerPolling()
    {
        try
        {
            _pollingCts?.Cancel();
            _pollingCts?.Dispose();
        }
        catch
        {
        }
        finally
        {
            _pollingCts = null;
        }
    }

    [RelayCommand]
    private async Task AbrirConversacionAsync(ConversacionChat conv)
    {
        if (conv == null) return;
        await Shell.Current.GoToAsync($"AdminConversationPage?conversacionId={conv.Id}&clienteNombre={Uri.EscapeDataString(conv.NombreCliente)}");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        DetenerPolling();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarALoginAsync()
    {
        DetenerPolling();
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
