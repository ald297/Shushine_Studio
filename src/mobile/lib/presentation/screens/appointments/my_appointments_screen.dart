import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../data/models/appointment_dto.dart';
import '../../blocs/appointments/appointment_bloc.dart';
import '../../blocs/appointments/appointment_event.dart';
import '../../blocs/appointments/appointment_state.dart';

class MyAppointmentsScreen extends StatefulWidget {
  const MyAppointmentsScreen({super.key});

  @override
  State<MyAppointmentsScreen> createState() => _MyAppointmentsScreenState();
}

class _MyAppointmentsScreenState extends State<MyAppointmentsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    context.read<AppointmentBloc>().add(FetchMyAppointmentsRequested());
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Scaffold(
      backgroundColor: backgroundColor,
      appBar: AppBar(
        backgroundColor: backgroundColor,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new, color: primaryColor, size: 20),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: const Text(
          'Mis Citas',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: primaryColor,
          indicatorWeight: 3,
          labelColor: primaryColor,
          unselectedLabelColor: subtitleColor,
          tabs: const [
            Tab(text: 'Activas'),
            Tab(text: 'Historial'),
          ],
        ),
      ),
      body: BlocBuilder<AppointmentBloc, AppointmentState>(
        builder: (context, state) {
          if (state is AppointmentLoading) {
            return const Center(child: CircularProgressIndicator(color: primaryColor));
          }

          if (state is AppointmentError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline, color: Colors.redAccent, size: 48),
                    const SizedBox(height: 16),
                    Text(
                      state.message,
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: textColor, fontSize: 14),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: () => context
                          .read<AppointmentBloc>()
                          .add(FetchMyAppointmentsRequested()),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: primaryColor,
                        foregroundColor: Colors.black,
                      ),
                      child: const Text('Reintentar'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is AppointmentsLoaded) {
            return TabBarView(
              controller: _tabController,
              children: [
                _appointmentsList(state.activeAppointments, true),
                _appointmentsList(state.pastAppointments, false),
              ],
            );
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _appointmentsList(List<AppointmentDto> items, bool isActiveTab) {
    const primaryColor = Color(0xFFC5A059);
    const subtitleColor = Color(0xFFA0A0AB);

    if (items.isEmpty) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                isActiveTab ? Icons.calendar_today_outlined : Icons.history,
                color: subtitleColor,
                size: 54,
              ),
              const SizedBox(height: 16),
              Text(
                isActiveTab
                    ? 'No tienes citas activas'
                    : 'Aún no tienes historial de citas',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                isActiveTab
                    ? 'Explora el catálogo y reserva tu próximo cambio de look.'
                    : 'Las citas pasadas o completadas aparecerán aquí.',
                textAlign: TextAlign.center,
                style: const TextStyle(color: subtitleColor, fontSize: 13),
              ),
            ],
          ),
        ),
      );
    }

    return RefreshIndicator(
      color: primaryColor,
      backgroundColor: const Color(0xFF1E1E24),
      onRefresh: () async {
        context.read<AppointmentBloc>().add(RefreshAppointmentsRequested());
      },
      child: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: items.length,
        separatorBuilder: (_, __) => const SizedBox(height: 14),
        itemBuilder: (context, index) {
          final item = items[index];
          return _appointmentCard(item);
        },
      ),
    );
  }

  Widget _appointmentCard(AppointmentDto item) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    Color statusColor;
    String statusText;
    switch (item.estado) {
      case 'Confirmed':
        statusColor = const Color(0xFF4CAF50);
        statusText = 'Confirmada';
        break;
      case 'InProgress':
        statusColor = const Color(0xFFFF9800);
        statusText = 'En Proceso';
        break;
      case 'Completed':
        statusColor = const Color(0xFF2196F3);
        statusText = 'Completada';
        break;
      case 'Cancelled':
        statusColor = const Color(0xFFE53935);
        statusText = 'Cancelada';
        break;
      default:
        statusColor = primaryColor;
        statusText = item.estado;
    }

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surfaceColor,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: cardColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: primaryColor.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  item.codigoCita,
                  style: const TextStyle(
                    color: primaryColor,
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  statusText,
                  style: TextStyle(
                    color: statusColor,
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              const Icon(Icons.person_pin, color: primaryColor, size: 18),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  item.estilistaNombre ?? 'Estilista Shushine',
                  style: const TextStyle(
                    color: textColor,
                    fontSize: 15,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              Text(
                '\$${item.total.toStringAsFixed(2)}',
                style: const TextStyle(
                  color: primaryColor,
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              const Icon(Icons.access_time, color: subtitleColor, size: 16),
              const SizedBox(width: 8),
              Text(
                '${item.fechaCita} • ${item.horaInicio} - ${item.horaFin}',
                style: const TextStyle(color: subtitleColor, fontSize: 13),
              ),
            ],
          ),
          if (item.serviciosNombres.isNotEmpty) ...[
            const SizedBox(height: 12),
            Wrap(
              spacing: 6,
              runSpacing: 4,
              children: item.serviciosNombres
                  .map(
                    (s) => Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: cardColor,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        s,
                        style: const TextStyle(color: Colors.white70, fontSize: 11),
                      ),
                    ),
                  )
                  .toList(),
            ),
          ],
        ],
      ),
    );
  }
}
