using ShushineStudio.Mobile.Presentation.Views.Admin;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
using ShushineStudio.Mobile.Presentation.Views.Booking;
using ShushineStudio.Mobile.Presentation.Views.Catalog;
using ShushineStudio.Mobile.Presentation.Views.Chat;
using ShushineStudio.Mobile.Presentation.Views.Home;
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
		// ClientHomePage está registrada en el TabBar; ruta secundaria no necesaria
		Routing.RegisterRoute("ClientProductsPage", typeof(ClientProductsPage));
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
		// Rutas secundarias del Hub de Agenda Admin
		Routing.RegisterRoute("TimelineAgendaPage", typeof(TimelineAgendaPage));
		Routing.RegisterRoute("AdminAppointmentsPage", typeof(AdminAppointmentsPage));
		Routing.RegisterRoute("AdminAppointmentDetailPage", typeof(AdminAppointmentDetailPage));
		Routing.RegisterRoute("AdminCreateAppointmentPage", typeof(AdminCreateAppointmentPage));
		Routing.RegisterRoute("AdminClientsPage", typeof(AdminClientsPage));
		Routing.RegisterRoute("AdminClientDetailPage", typeof(AdminClientDetailPage));
		Routing.RegisterRoute("AdminCreateClientPage", typeof(AdminCreateClientPage));
		Routing.RegisterRoute("AdminProfessionalsPage", typeof(AdminProfessionalsPage));
		Routing.RegisterRoute("AdminProfessionalDetailPage", typeof(AdminProfessionalDetailPage));
		Routing.RegisterRoute("AdminCreateProfessionalPage", typeof(AdminCreateProfessionalPage));
		// Rutas secundarias del Hub de Catálogo Admin
		Routing.RegisterRoute("AdminCatalogPage", typeof(AdminCatalogPage));
		Routing.RegisterRoute("AdminServicesPage", typeof(AdminServicesPage));
		Routing.RegisterRoute("AdminServiceDetailPage", typeof(AdminServiceDetailPage));
		Routing.RegisterRoute("AdminCreateServicePage", typeof(AdminCreateServicePage));
		Routing.RegisterRoute("AdminProductsPage", typeof(AdminProductsPage));
		Routing.RegisterRoute("AdminProductDetailPage", typeof(AdminProductDetailPage));
		// Rutas secundarias del Hub de Mensajes Admin
		// AdminMessagesPage es raíz de tab — NO registrar como ruta secundaria
		Routing.RegisterRoute("AdminConversationPage", typeof(AdminConversationPage));
		Routing.RegisterRoute("AdminRequestsPage", typeof(AdminRequestsPage));
		Routing.RegisterRoute("AdminRequestDetailPage", typeof(AdminRequestDetailPage));
		Routing.RegisterRoute("AdminReviewsPage", typeof(AdminReviewsPage));
		Routing.RegisterRoute("AdminReportsPage", typeof(AdminReportsPage));
		Routing.RegisterRoute("AdminSettingsPage", typeof(AdminSettingsPage));
		Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));

		// Inicializar la aplicación en la pantalla de Login sin pestañas
		CurrentItem = LoginShellContent;
	}

	public void SwitchToRole(string role)
	{
		bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.ToUpperInvariant().Contains("ADMIN");
		if (isAdmin)
		{
			SwitchToAdmin();
		}
		else
		{
			SwitchToClient();
		}
	}

	public void SwitchToClient()
	{
		ClearNavigationStack();
		if (ClientTabBar != null)
		{
			if (ClientTabBar.Items.Count > 0)
			{
				ClientTabBar.CurrentItem = ClientTabBar.Items[0];
				if (ClientTabBar.CurrentItem?.Items?.Count > 0)
				{
					ClientTabBar.CurrentItem.CurrentItem = ClientTabBar.CurrentItem.Items[0];
				}
			}
			CurrentItem = ClientTabBar;
		}
	}

	public void SwitchToAdmin()
	{
		ClearNavigationStack();
		if (AdminTabBar != null)
		{
			if (AdminTabBar.Items.Count > 0)
			{
				AdminTabBar.CurrentItem = AdminTabBar.Items[0];
				if (AdminTabBar.CurrentItem?.Items?.Count > 0)
				{
					AdminTabBar.CurrentItem.CurrentItem = AdminTabBar.CurrentItem.Items[0];
				}
			}
			CurrentItem = AdminTabBar;
		}
	}

	public void SwitchToAdminTab(int index)
	{
		ClearNavigationStack();
		if (AdminTabBar != null && index >= 0 && index < AdminTabBar.Items.Count)
		{
			AdminTabBar.CurrentItem = AdminTabBar.Items[index];
			CurrentItem = AdminTabBar;
		}
	}

	public void SwitchToLogin()
	{
		ClearNavigationStack();
		if (ClientTabBar?.Items?.Count > 0)
		{
			ClientTabBar.CurrentItem = ClientTabBar.Items[0];
		}
		if (AdminTabBar?.Items?.Count > 0)
		{
			AdminTabBar.CurrentItem = AdminTabBar.Items[0];
		}
		CurrentItem = LoginShellContent;
	}

	private void ClearNavigationStack()
	{
		try
		{
			if (Navigation != null)
			{
				while (Navigation.ModalStack.Count > 0)
				{
					Navigation.PopModalAsync(false);
				}
				if (Navigation.NavigationStack.Count > 1)
				{
					Navigation.PopToRootAsync(false);
				}
			}

			// Limpiar pilas de navegación internas de cada pestaña de Cliente
			if (ClientTabBar?.Items != null)
			{
				foreach (var section in ClientTabBar.Items)
				{
					try
					{
						if (section.Navigation != null && section.Navigation.NavigationStack.Count > 1)
						{
							section.Navigation.PopToRootAsync(false);
						}
					}
					catch { }
				}
			}

			// Limpiar pilas de navegación internas de cada pestaña de Administrador
			if (AdminTabBar?.Items != null)
			{
				foreach (var section in AdminTabBar.Items)
				{
					try
					{
						if (section.Navigation != null && section.Navigation.NavigationStack.Count > 1)
						{
							section.Navigation.PopToRootAsync(false);
						}
					}
					catch { }
				}
			}
		}
		catch
		{
			// Si la pila ya está en la raíz, ignorar
		}
	}
}

