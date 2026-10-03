namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminAgendaHubPage : ContentPage
{
	public AdminAgendaHubPage()
	{
		InitializeComponent();
	}

	private async void OnAgendaTimelineTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("TimelineAgendaPage");
	}

	private async void OnCitasTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminAppointmentsPage");
	}

	private async void OnNuevaCitaTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminCreateAppointmentPage");
	}

	private async void OnWalkInTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("WalkInPage");
	}
}
