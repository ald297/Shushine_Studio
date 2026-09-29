using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

[QueryProperty(nameof(ConversacionId), "conversacionId")]
[QueryProperty(nameof(ClienteNombre), "clienteNombre")]
public partial class AdminConversationViewModel : ObservableObject
{
    private readonly IChatRepository _chatRepository;
    private CancellationTokenSource? _pollingCts;

    [ObservableProperty]
    private int conversacionId;

    [ObservableProperty]
    private string clienteNombre = "Cliente";

    [ObservableProperty]
    private string title = "Conversación";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isSending = false;

    [ObservableProperty]
    private string messageText = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<ChatMessageItem> Messages { get; } = new();

    public bool HasMessages => Messages.Count > 0;
    public bool ShowEmptyState => !IsLoading && !HasError && !HasMessages;
    public bool IsNotSending => !IsSending;

    public AdminConversationViewModel(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    partial void OnClienteNombreChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Title = value;
        }
    }

    partial void OnConversacionIdChanged(int value)
    {
        if (value > 0)
        {
            _ = IniciarConversacionAsync();
        }
    }

    public async Task IniciarConversacionAsync()
    {
        if (ConversacionId <= 0) return;
        await CargarMensajesAsync();
        IniciarPolling();
    }

    public async Task CargarMensajesAsync()
    {
        if (IsLoading || ConversacionId <= 0) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var items = await _chatRepository.ObtenerMensajesAdminAsync(ConversacionId, 0, 50);

            Messages.Clear();
            var ordenados = items.OrderBy(m => m.Timestamp).ToList();
            foreach (var m in ordenados)
            {
                Messages.Add(m);
            }

            NotificarCambios();
            _ = _chatRepository.MarcarMensajesLeidosAdminAsync(ConversacionId);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al cargar mensajes: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
            NotificarCambios();
        }
    }

    public void IniciarPolling()
    {
        DetenerPolling();
        if (ConversacionId <= 0) return;

        _pollingCts = new CancellationTokenSource();
        var token = _pollingCts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(4000, token);
                    if (token.IsCancellationRequested) break;

                    var nuevos = await _chatRepository.ObtenerMensajesAdminAsync(ConversacionId, 0, 50);
                    if (nuevos != null && nuevos.Count > 0)
                    {
                        var ordenados = nuevos.OrderBy(m => m.Timestamp).ToList();

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            var idsExistentes = Messages.Select(x => x.Id).ToHashSet();
                            bool huboNuevos = false;

                            foreach (var item in ordenados)
                            {
                                if (!idsExistentes.Contains(item.Id))
                                {
                                    Messages.Add(item);
                                    huboNuevos = true;
                                }
                            }

                            if (huboNuevos)
                            {
                                NotificarCambios();
                                _ = _chatRepository.MarcarMensajesLeidosAdminAsync(ConversacionId);
                            }
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
    private async Task EnviarRespuestaAsync()
    {
        var texto = MessageText?.Trim();
        if (string.IsNullOrWhiteSpace(texto)) return;
        if (IsSending || ConversacionId <= 0) return;

        try
        {
            IsSending = true;
            OnPropertyChanged(nameof(IsNotSending));
            HasError = false;
            ErrorMessage = string.Empty;

            var enviado = await _chatRepository.EnviarMensajeAdminAsync(ConversacionId, texto);
            if (enviado != null)
            {
                Messages.Add(enviado);
                MessageText = string.Empty;
                NotificarCambios();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al responder: " + ex.Message;
            HasError = true;
        }
        finally
        {
            IsSending = false;
            OnPropertyChanged(nameof(IsNotSending));
        }
    }

    private void NotificarCambios()
    {
        OnPropertyChanged(nameof(HasMessages));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        DetenerPolling();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void DismissError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
