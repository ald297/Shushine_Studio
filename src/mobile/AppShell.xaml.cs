using ShushineStudio.Mobile.Presentation.Views.Admin;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
using ShushineStudio.Mobile.Presentation.Views.Booking;

namespace ShushineStudio.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Registro de Rutas secundarias para navegación con Shell.Current.GoToAsync()
		Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
		Routing.RegisterRoute("ServiceDetailPage", typeof(ServiceDetailPage));
		Routing.RegisterRoute("StylistSelectionPage", typeof(StylistSelectionPage));
		Routing.RegisterRoute("SlotSelectionPage", typeof(SlotSelectionPage));
		Routing.RegisterRoute("BookingSummaryPage", typeof(BookingSummaryPage));
		Routing.RegisterRoute("BookingConfirmationPage", typeof(BookingConfirmationPage));
		Routing.RegisterRoute("TimelineAgendaPage", typeof(TimelineAgendaPage));
		Routing.RegisterRoute("AdminCatalogPage", typeof(AdminCatalogPage));
		Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));
		Routing.RegisterRoute("AdminEstilistasPage", typeof(AdminEstilistasPage));
		Routing.RegisterRoute("AdminProfilePage", typeof(AdminProfilePage));
		Routing.RegisterRoute("WalkInPage", typeof(WalkInPage));
	}
}

