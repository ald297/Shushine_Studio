using CommunityToolkit.Mvvm.ComponentModel;

namespace ShushineStudio.Mobile.Presentation.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string? errorMessage;

    public bool IsNotBusy => !IsBusy;
}
