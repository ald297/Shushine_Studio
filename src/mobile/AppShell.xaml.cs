using ShushineStudio.Mobile.Presentation.Views.Admin;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
using ShushineStudio.Mobile.Presentation.Views.Booking;
using ShushineStudio.Mobile.Presentation.Views.Chat;
using ShushineStudio.Mobile.Presentation.Views.Notifications;
using ShushineStudio.Mobile.Presentation.Views.Profile;
using ShushineStudio.Mobile.Presentation.Views.Reviews;

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
		Routing.RegisterRoute("AppointmentDetailPage", typeof(AppointmentDetailPage));
		Routing.RegisterRoute("CustomRequestPage", typeof(CustomRequestPage));
		Routing.RegisterRoute("WalkInPage", typeof(WalkInPage));
		Routing.RegisterRoute("EditProfilePage", typeof(EditProfilePage));
		Routing.RegisterRoute("SecurityPage", typeof(SecurityPage));
		Routing.RegisterRoute("PreferencesPage", typeof(PreferencesPage));
		Routing.RegisterRoute("AppInfoPage", typeof(AppInfoPage));
		Routing.RegisterRoute("LeaveReviewPage", typeof(LeaveReviewPage));
		Routing.RegisterRoute("MyReviewsPage", typeof(MyReviewsPage));
		Routing.RegisterRoute("ReviewDetailPage", typeof(ReviewDetailPage));
		Routing.RegisterRoute("ReviewSuccessPage", typeof(ReviewSuccessPage));
		Routing.RegisterRoute("NotificationCenterPage", typeof(NotificationCenterPage));
		Routing.RegisterRoute("NotificationDetailPage", typeof(NotificationDetailPage));
		Routing.RegisterRoute("AdminAppointmentsPage", typeof(AdminAppointmentsPage));
		Routing.RegisterRoute("AdminAppointmentDetailPage", typeof(AdminAppointmentDetailPage));
		Routing.RegisterRoute("AdminCreateAppointmentPage", typeof(AdminCreateAppointmentPage));
		Routing.RegisterRoute("AdminClientsPage", typeof(AdminClientsPage));
		Routing.RegisterRoute("AdminClientDetailPage", typeof(AdminClientDetailPage));
		Routing.RegisterRoute("AdminCreateClientPage", typeof(AdminCreateClientPage));
		Routing.RegisterRoute("AdminProfessionalsPage", typeof(AdminProfessionalsPage));
		Routing.RegisterRoute("AdminProfessionalDetailPage", typeof(AdminProfessionalDetailPage));
		Routing.RegisterRoute("AdminCreateProfessionalPage", typeof(AdminCreateProfessionalPage));
		Routing.RegisterRoute("AdminServicesPage", typeof(AdminServicesPage));
		Routing.RegisterRoute("AdminServiceDetailPage", typeof(AdminServiceDetailPage));
		Routing.RegisterRoute("AdminCreateServicePage", typeof(AdminCreateServicePage));
		Routing.RegisterRoute("AdminProductsPage", typeof(AdminProductsPage));
		Routing.RegisterRoute("AdminProductDetailPage", typeof(AdminProductDetailPage));
		Routing.RegisterRoute("AdminMessagesPage", typeof(AdminMessagesPage));
		Routing.RegisterRoute("AdminConversationPage", typeof(AdminConversationPage));
		Routing.RegisterRoute("AdminRequestsPage", typeof(AdminRequestsPage));
		Routing.RegisterRoute("AdminRequestDetailPage", typeof(AdminRequestDetailPage));
		Routing.RegisterRoute("AdminReviewsPage", typeof(AdminReviewsPage));
		Routing.RegisterRoute("AdminReportsPage", typeof(AdminReportsPage));
		Routing.RegisterRoute("AdminSettingsPage", typeof(AdminSettingsPage));

		// Inicializar la aplicación en la pantalla de Login sin pestañas
		CurrentItem = LoginShellContent;
	}

	public void SwitchToRole(string role)
	{
		bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.ToUpperInvariant().Contains("ADMIN");
		if (isAdmin)
		{
			CurrentItem = AdminTabBar;
		}
		else
		{
			CurrentItem = ClientTabBar;
		}
	}

	public void SwitchToLogin()
	{
		CurrentItem = LoginShellContent;
	}
}

