using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class TimelineAgendaPage : ContentPage
{
    public TimelineAgendaPage(TimelineAgendaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
