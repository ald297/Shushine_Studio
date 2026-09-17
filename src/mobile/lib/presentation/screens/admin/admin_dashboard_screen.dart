import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../data/models/dashboard_metrics_dto.dart';
import '../../blocs/dashboard/dashboard_bloc.dart';
import '../../blocs/dashboard/dashboard_event.dart';
import '../../blocs/dashboard/dashboard_state.dart';

class AdminDashboardScreen extends StatefulWidget {
  const AdminDashboardScreen({super.key});

  @override
  State<AdminDashboardScreen> createState() => _AdminDashboardScreenState();
}

class _AdminDashboardScreenState extends State<AdminDashboardScreen> {
  @override
  void initState() {
    super.initState();
    context.read<DashboardBloc>().add(FetchDashboardMetricsRequested());
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const textColor = Color(0xFFF5F5F7);

    return Scaffold(
      backgroundColor: backgroundColor,
      appBar: AppBar(
        backgroundColor: surfaceColor,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new, color: primaryColor, size: 20),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: const Text(
          'Dashboard Gerencial',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh_rounded, color: primaryColor),
            tooltip: 'Actualizar métricas',
            onPressed: () {
              context.read<DashboardBloc>().add(FetchDashboardMetricsRequested());
            },
          ),
        ],
      ),
      body: BlocBuilder<DashboardBloc, DashboardState>(
        builder: (context, state) {
          if (state is DashboardLoading) {
            return const Center(child: CircularProgressIndicator(color: primaryColor));
          }

          if (state is DashboardError) {
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
                      onPressed: () => context.read<DashboardBloc>().add(FetchDashboardMetricsRequested()),
                      style: ElevatedButton.styleFrom(backgroundColor: primaryColor, foregroundColor: Colors.black),
                      child: const Text('Reintentar'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is DashboardLoaded) {
            final m = state.metrics;
            return _buildDashboardContent(m);
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildDashboardContent(DashboardMetricsDto m) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return RefreshIndicator(
      color: primaryColor,
      onRefresh: () async {
        context.read<DashboardBloc>().add(FetchDashboardMetricsRequested());
      },
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Banner Title
            Container(
              padding: const EdgeInsets.all(18),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [Color(0xFF2A2418), surfaceColor],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(18),
                border: Border.all(color: primaryColor.withValues(alpha: 0.3)),
              ),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: primaryColor.withValues(alpha: 0.15),
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(Icons.analytics_rounded, color: primaryColor, size: 28),
                  ),
                  const SizedBox(width: 14),
                  const Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Resumen Operativo y Financiero',
                          style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
                        ),
                        SizedBox(height: 2),
                        Text(
                          'Métricas en tiempo real sincronizadas con Supabase Pooler',
                          style: TextStyle(color: subtitleColor, fontSize: 12),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 18),

            // Revenue KPIs Row
            Row(
              children: [
                Expanded(
                  child: _kpiCard(
                    'Ingresos de Hoy',
                    '\$${m.ingresosHoy.toStringAsFixed(2)}',
                    Icons.attach_money,
                    const Color(0xFF4CAF50),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _kpiCard(
                    'Ingresos del Mes',
                    '\$${m.ingresosMes.toStringAsFixed(2)}',
                    Icons.trending_up,
                    primaryColor,
                  ),
                ),
              ],
            ),

            const SizedBox(height: 12),

            // Appointment Volume KPIs Row
            Row(
              children: [
                Expanded(
                  child: _kpiCard(
                    'Citas Hoy',
                    '${m.totalCitasHoy}',
                    Icons.calendar_today,
                    const Color(0xFF2196F3),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _kpiCard(
                    'Citas Semana',
                    '${m.totalCitasSemana}',
                    Icons.date_range,
                    Colors.indigoAccent,
                  ),
                ),
              ],
            ),

            const SizedBox(height: 12),

            // Salon Status KPIs Row
            Row(
              children: [
                Expanded(
                  child: _kpiCard(
                    'En Cabina Ahora',
                    '${m.citasEnProceso}',
                    Icons.chair,
                    const Color(0xFFFF9800),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _kpiCard(
                    'Estilistas Activos',
                    '${m.estilistasActivos}',
                    Icons.people,
                    Colors.tealAccent,
                  ),
                ),
              ],
            ),

            const SizedBox(height: 22),

            // Summary Breakdown Box
            const Text(
              'Estado de Operaciones',
              style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(18),
              decoration: BoxDecoration(
                color: surfaceColor,
                borderRadius: BorderRadius.circular(18),
              ),
              child: Column(
                children: [
                  _statusRow('Citas Completadas con éxito', '${m.citasCompletadas}', const Color(0xFF4CAF50)),
                  const SizedBox(height: 10),
                  _statusRow('Citas En Atención en Cabina', '${m.citasEnProceso}', const Color(0xFFFF9800)),
                  const SizedBox(height: 10),
                  _statusRow('Citas Canceladas', '${m.citasCanceladas}', const Color(0xFFE53935)),
                ],
              ),
            ),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }

  Widget _kpiCard(String title, String value, IconData icon, Color color) {
    const surfaceColor = Color(0xFF1E1E24);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: surfaceColor,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: color.withValues(alpha: 0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(title, style: const TextStyle(color: subtitleColor, fontSize: 12)),
              Icon(icon, color: color, size: 20),
            ],
          ),
          const SizedBox(height: 10),
          Text(
            value,
            style: const TextStyle(color: textColor, fontSize: 22, fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }

  Widget _statusRow(String label, String count, Color dotColor) {
    const textColor = Color(0xFFF5F5F7);

    return Row(
      children: [
        Container(
          width: 10,
          height: 10,
          decoration: BoxDecoration(color: dotColor, shape: BoxShape.circle),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Text(label, style: const TextStyle(color: textColor, fontSize: 13)),
        ),
        Text(
          count,
          style: const TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.bold),
        ),
      ],
    );
  }
}
