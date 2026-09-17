import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../data/models/timeline_item_dto.dart';
import '../../blocs/timeline/timeline_bloc.dart';
import '../../blocs/timeline/timeline_event.dart';
import '../../blocs/timeline/timeline_state.dart';
import 'register_walkin_dialog.dart';

class StylistTimelineScreen extends StatefulWidget {
  const StylistTimelineScreen({super.key});

  @override
  State<StylistTimelineScreen> createState() => _StylistTimelineScreenState();
}

class _StylistTimelineScreenState extends State<StylistTimelineScreen> {
  DateTime _selectedDate = DateTime.now();
  int? _selectedStylistId;

  @override
  void initState() {
    super.initState();
    context.read<TimelineBloc>().add(
          FetchTimelineRequested(date: _selectedDate, stylistId: _selectedStylistId),
        );
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
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
          'Agenda Timeline',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
        actions: [
          IconButton(
            icon: const Icon(Icons.calendar_today_rounded, color: primaryColor, size: 20),
            tooltip: 'Seleccionar fecha',
            onPressed: () async {
              final timelineBloc = context.read<TimelineBloc>();
              final picked = await showDatePicker(
                context: context,
                initialDate: _selectedDate,
                firstDate: DateTime(2025),
                lastDate: DateTime(2030),
                builder: (context, child) {
                  return Theme(
                    data: ThemeData.dark().copyWith(
                      colorScheme: const ColorScheme.dark(
                        primary: primaryColor,
                        onPrimary: Colors.black,
                        surface: surfaceColor,
                      ),
                    ),
                    child: child!,
                  );
                },
              );
              if (picked != null) {
                if (!mounted) return;
                setState(() => _selectedDate = picked);
                timelineBloc.add(
                  FetchTimelineRequested(date: picked, stylistId: _selectedStylistId),
                );
              }
            },
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: primaryColor,
        foregroundColor: Colors.black,
        icon: const Icon(Icons.person_add_alt_1),
        label: const Text('Nuevo Walk-in', style: TextStyle(fontWeight: FontWeight.bold)),
        onPressed: () {
          showDialog(
            context: context,
            builder: (_) => BlocProvider.value(
              value: context.read<TimelineBloc>(),
              child: RegisterWalkinDialog(
                defaultStylistId: _selectedStylistId ?? 1,
                defaultDate: _selectedDate,
              ),
            ),
          );
        },
      ),
      body: BlocConsumer<TimelineBloc, TimelineState>(
        listener: (context, state) {
          if (state is TimelineLoaded && state.successMessage != null) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.successMessage!),
                backgroundColor: primaryColor,
                behavior: SnackBarBehavior.floating,
              ),
            );
          } else if (state is TimelineError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: Colors.redAccent,
                behavior: SnackBarBehavior.floating,
              ),
            );
          }
        },
        builder: (context, state) {
          return Column(
            children: [
              // Date Header & Stylist Filters
              Container(
                color: surfaceColor,
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                child: Column(
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          DateFormat('EEEE, d MMMM yyyy', 'es').format(_selectedDate),
                          style: const TextStyle(
                            color: textColor,
                            fontSize: 15,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          decoration: BoxDecoration(
                            color: cardColor,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: const Text(
                            'Cabina Shushine',
                            style: TextStyle(color: primaryColor, fontSize: 12),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    SingleChildScrollView(
                      scrollDirection: Axis.horizontal,
                      child: Row(
                        children: [
                          _filterChip(null, 'Todos los estilistas'),
                          const SizedBox(width: 8),
                          _filterChip(1, 'Valeria Rivas'),
                          const SizedBox(width: 8),
                          _filterChip(2, 'Carlos Mendoza'),
                          const SizedBox(width: 8),
                          _filterChip(3, 'Sofía Castro'),
                        ],
                      ),
                    ),
                  ],
                ),
              ),

              // Timeline Content
              Expanded(
                child: _buildTimelineContent(state),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _filterChip(int? id, String label) {
    const primaryColor = Color(0xFFC5A059);
    final isSelected = _selectedStylistId == id;

    return ChoiceChip(
      label: Text(label),
      selected: isSelected,
      onSelected: (selected) {
        setState(() => _selectedStylistId = selected ? id : null);
        context.read<TimelineBloc>().add(
              FetchTimelineRequested(date: _selectedDate, stylistId: _selectedStylistId),
            );
      },
      selectedColor: primaryColor,
      backgroundColor: const Color(0xFF26262E),
      labelStyle: TextStyle(
        color: isSelected ? Colors.black : Colors.white70,
        fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
        fontSize: 12,
      ),
    );
  }

  Widget _buildTimelineContent(TimelineState state) {
    const primaryColor = Color(0xFFC5A059);
    const subtitleColor = Color(0xFFA0A0AB);

    if (state is TimelineLoading) {
      return const Center(child: CircularProgressIndicator(color: primaryColor));
    }

    if (state is TimelineLoaded) {
      if (state.items.isEmpty) {
        return const Center(
          child: Padding(
            padding: EdgeInsets.all(32),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.event_busy, color: subtitleColor, size: 54),
                SizedBox(height: 16),
                Text(
                  'No hay citas programadas para este día',
                  style: TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold),
                ),
                SizedBox(height: 8),
                Text(
                  'Los clientes con cita agendada o registros walk-in aparecerán aquí.',
                  textAlign: TextAlign.center,
                  style: TextStyle(color: subtitleColor, fontSize: 13),
                ),
              ],
            ),
          ),
        );
      }

      return RefreshIndicator(
        color: primaryColor,
        onRefresh: () async {
          context.read<TimelineBloc>().add(
                FetchTimelineRequested(date: _selectedDate, stylistId: _selectedStylistId),
              );
        },
        child: ListView.separated(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
          itemCount: state.items.length,
          separatorBuilder: (_, __) => const SizedBox(height: 14),
          itemBuilder: (context, index) {
            final item = state.items[index];
            return _timelineCard(item);
          },
        ),
      );
    }

    return const SizedBox.shrink();
  }

  Widget _timelineCard(TimelineItemDto item) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    Color statusColor;
    String statusLabel;
    switch (item.estado) {
      case 'Confirmed':
        statusColor = const Color(0xFF4CAF50);
        statusLabel = 'Confirmada';
        break;
      case 'InProgress':
        statusColor = const Color(0xFFFF9800);
        statusLabel = 'En Cabina';
        break;
      case 'Completed':
        statusColor = const Color(0xFF2196F3);
        statusLabel = 'Finalizada';
        break;
      case 'Cancelled':
        statusColor = const Color(0xFFE53935);
        statusLabel = 'Cancelada';
        break;
      default:
        statusColor = primaryColor;
        statusLabel = item.estado;
    }

    return Container(
      decoration: BoxDecoration(
        color: surfaceColor,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(
          color: item.estado == 'InProgress' ? primaryColor : cardColor,
          width: item.estado == 'InProgress' ? 1.5 : 1.0,
        ),
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Time & Status Header
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  const Icon(Icons.access_time_filled, color: primaryColor, size: 16),
                  const SizedBox(width: 6),
                  Text(
                    '${item.horaInicio} - ${item.horaFin}',
                    style: const TextStyle(
                      color: textColor,
                      fontSize: 14,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  if (item.esWalkin) ...[
                    const SizedBox(width: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                      decoration: BoxDecoration(
                        color: Colors.purple.withValues(alpha: 0.2),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: const Text(
                        'Walk-in',
                        style: TextStyle(color: Colors.purpleAccent, fontSize: 10, fontWeight: FontWeight.bold),
                      ),
                    ),
                  ],
                ],
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Text(
                  statusLabel,
                  style: TextStyle(color: statusColor, fontSize: 11, fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),

          // Client and Stylist
          Text(
            item.clienteNombre,
            style: const TextStyle(
              color: textColor,
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            'Estilista: ${item.estilistaNombre ?? "Asignado por salón"} • Total: \$${item.total.toStringAsFixed(2)}',
            style: const TextStyle(color: subtitleColor, fontSize: 12),
          ),

          if (item.servicios.isNotEmpty) ...[
            const SizedBox(height: 8),
            Wrap(
              spacing: 6,
              children: item.servicios
                  .map(
                    (s) => Chip(
                      label: Text(s, style: const TextStyle(fontSize: 11, color: Colors.white70)),
                      backgroundColor: cardColor,
                      padding: EdgeInsets.zero,
                      materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    ),
                  )
                  .toList(),
            ),
          ],

          const SizedBox(height: 12),
          const Divider(color: cardColor, height: 1),
          const SizedBox(height: 10),

          // State Transition Action Buttons (T065)
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              if (item.estado == 'Confirmed') ...[
                OutlinedButton(
                  onPressed: () {
                    context.read<TimelineBloc>().add(
                          UpdateAppointmentStatusRequested(
                            citaId: item.citaId,
                            nuevoEstado: 'Cancelled',
                            motivoCancelacion: 'Cancelado desde agenda',
                          ),
                        );
                  },
                  style: OutlinedButton.styleFrom(
                    side: const BorderSide(color: Colors.redAccent),
                    visualDensity: VisualDensity.compact,
                  ),
                  child: const Text('Cancelar', style: TextStyle(color: Colors.redAccent, fontSize: 12)),
                ),
                const SizedBox(width: 8),
                ElevatedButton.icon(
                  onPressed: () {
                    context.read<TimelineBloc>().add(
                          UpdateAppointmentStatusRequested(
                            citaId: item.citaId,
                            nuevoEstado: 'InProgress',
                          ),
                        );
                  },
                  icon: const Icon(Icons.play_arrow, size: 16),
                  label: const Text('Iniciar Atención', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: primaryColor,
                    foregroundColor: Colors.black,
                    visualDensity: VisualDensity.compact,
                  ),
                ),
              ] else if (item.estado == 'InProgress') ...[
                ElevatedButton.icon(
                  onPressed: () {
                    context.read<TimelineBloc>().add(
                          UpdateAppointmentStatusRequested(
                            citaId: item.citaId,
                            nuevoEstado: 'Completed',
                          ),
                        );
                  },
                  icon: const Icon(Icons.check_circle_outline, size: 16),
                  label: const Text('Finalizar Cita', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF4CAF50),
                    foregroundColor: Colors.white,
                    visualDensity: VisualDensity.compact,
                  ),
                ),
              ] else if (item.estado == 'Completed') ...[
                const Text('Cita Atendida', style: TextStyle(color: Color(0xFF4CAF50), fontSize: 12, fontWeight: FontWeight.bold)),
              ] else ...[
                const Text('Cita Cancelada', style: TextStyle(color: Colors.redAccent, fontSize: 12)),
              ],
            ],
          ),
        ],
      ),
    );
  }
}
