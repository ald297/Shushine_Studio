import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../core/di/injection_container.dart';
import '../../../data/models/user_dto.dart';
import '../../blocs/auth/auth_bloc.dart';
import '../../blocs/auth/auth_event.dart';
import '../../blocs/auth/auth_state.dart';
import '../../blocs/appointments/appointment_bloc.dart';
import '../../blocs/catalog/catalog_bloc.dart';
import '../../blocs/dashboard/dashboard_bloc.dart';
import '../../blocs/timeline/timeline_bloc.dart';
import '../admin/admin_dashboard_screen.dart';
import '../admin/stylist_timeline_screen.dart';
import '../appointments/my_appointments_screen.dart';
import '../auth/login_screen.dart';
import '../catalog/catalog_screen.dart';

class ProfileScreen extends StatelessWidget {
  final UserDto user;

  const ProfileScreen({super.key, required this.user});

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Scaffold(
      backgroundColor: backgroundColor,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        title: const Text(
          'Mi Perfil',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
        actions: [
          IconButton(
            icon: const Icon(Icons.logout_rounded, color: Color(0xFFEF5350)),
            tooltip: 'Cerrar Sesión',
            onPressed: () {
              _showLogoutDialog(context);
            },
          ),
        ],
      ),
      body: BlocListener<AuthBloc, AuthState>(
        listener: (context, state) {
          if (state is Unauthenticated) {
            Navigator.of(context).pushAndRemoveUntil(
              MaterialPageRoute(builder: (_) => const LoginScreen()),
              (route) => false,
            );
          }
        },
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 20.0, vertical: 12.0),
          child: Column(
            children: [
              // Header Card
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(24.0),
                decoration: BoxDecoration(
                  color: surfaceColor,
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(color: const Color(0x4DC5A059), width: 1),
                ),
                child: Column(
                  children: [
                    CircleAvatar(
                      radius: 42,
                      backgroundColor: const Color(0x26C5A059),
                      child: Text(
                        user.nombre.isNotEmpty ? user.nombre[0].toUpperCase() : 'U',
                        style: const TextStyle(
                          fontSize: 36,
                          fontWeight: FontWeight.bold,
                          color: primaryColor,
                        ),
                      ),
                    ),
                    const SizedBox(height: 14),
                    Text(
                      user.nombreCompleto,
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: textColor,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '@${user.login}',
                      style: const TextStyle(fontSize: 14, color: subtitleColor),
                    ),
                    const SizedBox(height: 12),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 6),
                      decoration: BoxDecoration(
                        color: const Color(0x2EC5A059),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: primaryColor, width: 1),
                      ),
                      child: Text(
                        user.rol.toUpperCase(),
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.bold,
                          color: primaryColor,
                          letterSpacing: 1.1,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),

              // Detalles de Cuenta
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(20.0),
                decoration: BoxDecoration(
                  color: surfaceColor,
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Información Personal',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: textColor,
                      ),
                    ),
                    const SizedBox(height: 16),
                    _profileItem(Icons.phone_outlined, 'Teléfono', user.telefono ?? 'No especificado', primaryColor, textColor, subtitleColor),
                    const Divider(color: cardColor, height: 24),
                    _profileItem(Icons.badge_outlined, 'Identificador', '#${user.id}', primaryColor, textColor, subtitleColor),
                    const Divider(color: cardColor, height: 24),
                    _profileItem(Icons.verified_user_outlined, 'Estado de Cuenta', user.activo ? 'Activa' : 'Inactiva', primaryColor, textColor, subtitleColor),
                  ],
                ),
              ),
              const SizedBox(height: 20),

              // Tarjeta de Beneficios y Fidelidad
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(20.0),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [
                      Color(0xFF2A2418),
                      surfaceColor,
                    ],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: const Color(0x66C5A059)),
                ),
                child: Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(12),
                      decoration: const BoxDecoration(
                        color: Color(0x33C5A059),
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(Icons.star_rounded, color: primaryColor, size: 28),
                    ),
                    const SizedBox(width: 16),
                    const Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Nivel Shushine Club',
                            style: TextStyle(color: subtitleColor, fontSize: 13),
                          ),
                          SizedBox(height: 2),
                          Text(
                            'Miembro Bronce • 0 pts',
                            style: TextStyle(
                              color: textColor,
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 32),

              // Botón de Explorar Catálogo
              ElevatedButton.icon(
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => BlocProvider<CatalogBloc>(
                        create: (_) => sl<CatalogBloc>(),
                        child: const CatalogScreen(),
                      ),
                    ),
                  );
                },
                icon: const Icon(Icons.auto_awesome, color: Colors.black, size: 20),
                label: const Text(
                  'Explorar Catálogo de Servicios',
                  style: TextStyle(color: Colors.black, fontSize: 16, fontWeight: FontWeight.bold),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: primaryColor,
                  minimumSize: const Size(double.infinity, 52),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  elevation: 2,
                ),
              ),
              const SizedBox(height: 12),

              // Botón de Mis Citas
              ElevatedButton.icon(
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => BlocProvider<AppointmentBloc>(
                        create: (_) => sl<AppointmentBloc>(),
                        child: const MyAppointmentsScreen(),
                      ),
                    ),
                  );
                },
                icon: const Icon(Icons.calendar_month, color: primaryColor, size: 20),
                label: const Text(
                  'Mis Citas Agendadas',
                  style: TextStyle(color: primaryColor, fontSize: 15, fontWeight: FontWeight.bold),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: surfaceColor,
                  minimumSize: const Size(double.infinity, 50),
                  side: BorderSide(color: primaryColor.withValues(alpha: 0.4), width: 1.2),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  elevation: 0,
                ),
              ),
              const SizedBox(height: 12),

              // Botón de Agenda Timeline (Estilistas / Admin)
              if (user.rol == 'ADMIN' || user.rol == 'RECEPCIONISTA') ...[
                ElevatedButton.icon(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => BlocProvider<TimelineBloc>(
                          create: (_) => sl<TimelineBloc>(),
                          child: const StylistTimelineScreen(),
                        ),
                      ),
                    );
                  },
                  icon: const Icon(Icons.view_timeline_outlined, color: Colors.white, size: 20),
                  label: const Text(
                    'Agenda del Salón (Timeline)',
                    style: TextStyle(color: Colors.white, fontSize: 15, fontWeight: FontWeight.bold),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: cardColor,
                    minimumSize: const Size(double.infinity, 50),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                    elevation: 0,
                  ),
                ),
                const SizedBox(height: 12),

                // Botón de Dashboard Gerencial (Admin)
                ElevatedButton.icon(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => BlocProvider<DashboardBloc>(
                          create: (_) => sl<DashboardBloc>(),
                          child: const AdminDashboardScreen(),
                        ),
                      ),
                    );
                  },
                  icon: const Icon(Icons.dashboard_rounded, color: primaryColor, size: 20),
                  label: const Text(
                    'Dashboard Gerencial & KPIs',
                    style: TextStyle(color: primaryColor, fontSize: 15, fontWeight: FontWeight.bold),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: cardColor,
                    minimumSize: const Size(double.infinity, 50),
                    side: BorderSide(color: primaryColor.withValues(alpha: 0.3), width: 1.2),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                    elevation: 0,
                  ),
                ),
                const SizedBox(height: 12),
              ],
              const SizedBox(height: 4),

              // Botón de Cerrar Sesión
              OutlinedButton.icon(
                onPressed: () => _showLogoutDialog(context),
                icon: const Icon(Icons.logout, color: Color(0xFFEF5350), size: 18),
                label: const Text(
                  'Cerrar Sesión',
                  style: TextStyle(color: Color(0xFFEF5350), fontSize: 15, fontWeight: FontWeight.w600),
                ),
                style: OutlinedButton.styleFrom(
                  side: const BorderSide(color: Color(0xFFEF5350), width: 1.2),
                  padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 24),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
              ),
              const SizedBox(height: 16),
            ],
          ),
        ),
      ),
    );
  }

  Widget _profileItem(
    IconData icon,
    String label,
    String value,
    Color primaryColor,
    Color textColor,
    Color subtitleColor,
  ) {
    return Row(
      children: [
        Icon(icon, color: primaryColor, size: 22),
        const SizedBox(width: 14),
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: TextStyle(color: subtitleColor, fontSize: 12)),
            const SizedBox(height: 2),
            Text(value, style: TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.w500)),
          ],
        ),
      ],
    );
  }

  void _showLogoutDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: const Color(0xFF1E1E24),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('¿Cerrar Sesión?', style: TextStyle(color: Colors.white)),
        content: const Text(
          '¿Está seguro de que desea salir de su cuenta?',
          style: TextStyle(color: Color(0xFFA0A0AB)),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancelar', style: TextStyle(color: Color(0xFFA0A0AB))),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              context.read<AuthBloc>().add(AuthLogoutRequested());
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFFD32F2F),
              foregroundColor: Colors.white,
            ),
            child: const Text('Salir'),
          ),
        ],
      ),
    );
  }
}
