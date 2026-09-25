using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;

namespace ShushineStudio.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Registro de Rutas secundarias para navegación con Shell.Current.GoToAsync()
		Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
		Routing.RegisterRoute("SeleccionHorarioPage", typeof(SeleccionHorarioPage));
		Routing.RegisterRoute("BookingSummaryPage", typeof(BookingSummaryPage));
		Routing.RegisterRoute("ResumenReservaPage", typeof(BookingSummaryPage));
	}
}
