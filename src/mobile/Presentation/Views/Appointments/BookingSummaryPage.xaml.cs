using ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

namespace ShushineStudio.Mobile.Presentation.Views.Appointments;

public partial class BookingSummaryPage : ContentPage
{
    private readonly BookingSummaryViewModel _viewModel;

    public BookingSummaryPage(BookingSummaryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
