import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../data/models/service_dto.dart';
import '../../../data/models/stylist_dto.dart';
import '../../blocs/booking/booking_bloc.dart';
import '../../blocs/booking/booking_event.dart';
import '../../blocs/booking/booking_state.dart';
import 'booking_summary_screen.dart';

class SelectDateTimeScreen extends StatefulWidget {
  final ServiceDto service;

  const SelectDateTimeScreen({super.key, required this.service});

  @override
  State<SelectDateTimeScreen> createState() => _SelectDateTimeScreenState();
}

class _SelectDateTimeScreenState extends State<SelectDateTimeScreen> {
  DateTime _selectedDay = DateTime.now();

  @override
  void initState() {
    super.initState();
    context.read<BookingBloc>().add(BookingStylistsFetchRequested());
  }

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
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new, color: primaryColor, size: 20),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: const Text(
          'Seleccionar Horario',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: BlocBuilder<BookingBloc, BookingState>(
        builder: (context, state) {
          if (state is BookingLoading && state is! BookingAvailabilityLoaded) {
            return const Center(child: CircularProgressIndicator(color: primaryColor));
          }

          if (state is BookingError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(Icons.error_outline_rounded, color: Color(0xFFEF5350), size: 48),
                    const SizedBox(height: 12),
                    Text(state.message, textAlign: TextAlign.center, style: const TextStyle(color: subtitleColor)),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: () => context.read<BookingBloc>().add(BookingStylistsFetchRequested()),
                      style: ElevatedButton.styleFrom(backgroundColor: primaryColor, foregroundColor: Colors.black),
                      child: const Text('Reintentar'),
                    ),
                  ],
                ),
              ),
            );
          }

          if (state is BookingAvailabilityLoaded) {
            return SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 20.0, vertical: 12.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Banner del Servicio Seleccionado
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: surfaceColor,
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: const Color(0x33C5A059)),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.spa_rounded, color: primaryColor, size: 28),
                        const SizedBox(width: 14),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                widget.service.nombre,
                                style: const TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
                              ),
                              const SizedBox(height: 2),
                              Text(
                                '${widget.service.duracionFormateada} • ${widget.service.precioFormateado}',
                                style: const TextStyle(color: subtitleColor, fontSize: 13),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 24),

                  // Sección: Seleccionar Estilista
                  const Text(
                    '1. Selecciona tu Profesional',
                    style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 12),
                  SizedBox(
                    height: 110,
                    child: ListView.separated(
                      scrollDirection: Axis.horizontal,
                      itemCount: state.stylists.length,
                      separatorBuilder: (_, __) => const SizedBox(width: 12),
                      itemBuilder: (context, index) {
                        final stylist = state.stylists[index];
                        final isSelected = state.selectedStylist?.id == stylist.id;
                        return _stylistCard(stylist, isSelected, primaryColor, surfaceColor, textColor, subtitleColor);
                      },
                    ),
                  ),
                  const SizedBox(height: 24),

                  // Sección: Seleccionar Fecha (Horizontal Strip)
                  const Text(
                    '2. Selecciona la Fecha',
                    style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 12),
                  _buildDateSelector(primaryColor, surfaceColor, textColor, subtitleColor),
                  const SizedBox(height: 24),

                  // Sección: Franjas Horarias Disponibles
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        '3. Horarios Disponibles',
                        style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      if (state.selectedStylist != null)
                        Text(
                          DateFormat('EEEE d MMMM', 'es').format(_selectedDay),
                          style: const TextStyle(color: primaryColor, fontSize: 13),
                        ),
                    ],
                  ),
                  const SizedBox(height: 14),

                  if (state.slots.isEmpty)
                    Container(
                      width: double.infinity,
                      padding: const EdgeInsets.all(24),
                      decoration: BoxDecoration(
                        color: surfaceColor,
                        borderRadius: BorderRadius.circular(14),
                      ),
                      child: const Column(
                        children: [
                          Icon(Icons.event_busy_rounded, color: subtitleColor, size: 36),
                          SizedBox(height: 8),
                          Text(
                            'El profesional no labora o no tiene franjas disponibles en este día.',
                            textAlign: TextAlign.center,
                            style: TextStyle(color: subtitleColor, fontSize: 14),
                          ),
                        ],
                      ),
                    )
                  else
                    Wrap(
                      spacing: 10,
                      runSpacing: 10,
                      children: state.slots.map((slot) {
                        final isSelected = state.selectedSlot?.horaInicio == slot.horaInicio;
                        return _timeSlotChip(slot, isSelected, primaryColor, surfaceColor, cardColor, textColor, subtitleColor);
                      }).toList(),
                    ),

                  const SizedBox(height: 100),
                ],
              ),
            );
          }

          return const SizedBox.shrink();
        },
      ),
      bottomSheet: BlocBuilder<BookingBloc, BookingState>(
        builder: (context, state) {
          if (state is! BookingAvailabilityLoaded || state.selectedSlot == null) {
            return const SizedBox.shrink();
          }

          return Container(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 14),
            color: surfaceColor,
            child: SafeArea(
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('Cita Seleccionada', style: TextStyle(color: subtitleColor, fontSize: 11)),
                        Text(
                          '${DateFormat('d MMM').format(_selectedDay)} a las ${state.selectedSlot!.horaInicio}',
                          style: const TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.bold),
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton(
                    onPressed: () {
                      _showConfirmationModal(context, state);
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: primaryColor,
                      foregroundColor: Colors.black,
                      padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                    child: const Text('Confirmar', style: TextStyle(fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _stylistCard(
    StylistDto stylist,
    bool isSelected,
    Color primaryColor,
    Color surfaceColor,
    Color textColor,
    Color subtitleColor,
  ) {
    return GestureDetector(
      onTap: () {
        context.read<BookingBloc>().add(BookingStylistSelected(stylist));
      },
      child: Container(
        width: 120,
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: surfaceColor,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: isSelected ? primaryColor : const Color(0xFF2E2E38),
            width: isSelected ? 2 : 1,
          ),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            CircleAvatar(
              radius: 22,
              backgroundColor: const Color(0x2EC5A059),
              backgroundImage: (stylist.avatarUrl != null && stylist.avatarUrl!.isNotEmpty)
                  ? CachedNetworkImageProvider(stylist.avatarUrl!)
                  : null,
              child: (stylist.avatarUrl == null || stylist.avatarUrl!.isEmpty)
                  ? Text(
                      stylist.nombreCompleto[0].toUpperCase(),
                      style: TextStyle(color: primaryColor, fontWeight: FontWeight.bold),
                    )
                  : null,
            ),
            const SizedBox(height: 6),
            Text(
              stylist.nombreCompleto,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                color: isSelected ? primaryColor : textColor,
                fontSize: 12,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
              ),
            ),
            Text(
              stylist.especialidadPrincipal,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(color: subtitleColor, fontSize: 10),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDateSelector(Color primaryColor, Color surfaceColor, Color textColor, Color subtitleColor) {
    final now = DateTime.now();
    return SizedBox(
      height: 76,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: 14, // Próximos 14 días
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          final day = now.plusDays(index);
          final isSelected = day.year == _selectedDay.year &&
              day.month == _selectedDay.month &&
              day.day == _selectedDay.day;

          return GestureDetector(
            onTap: () {
              setState(() => _selectedDay = day);
              context.read<BookingBloc>().add(
                    BookingDateSelected(
                      day,
                      duracionMinutos: widget.service.duracionMinutos,
                    ),
                  );
            },
            child: Container(
              width: 58,
              padding: const EdgeInsets.symmetric(vertical: 10),
              decoration: BoxDecoration(
                color: isSelected ? primaryColor : surfaceColor,
                borderRadius: BorderRadius.circular(14),
                border: Border.all(
                  color: isSelected ? primaryColor : const Color(0xFF2E2E38),
                ),
              ),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    DateFormat('E', 'es').format(day).toUpperCase(),
                    style: TextStyle(
                      color: isSelected ? Colors.black : subtitleColor,
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '${day.day}',
                    style: TextStyle(
                      color: isSelected ? Colors.black : textColor,
                      fontSize: 17,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _timeSlotChip(
    TimeSlotDto slot,
    bool isSelected,
    Color primaryColor,
    Color surfaceColor,
    Color cardColor,
    Color textColor,
    Color subtitleColor,
  ) {
    if (!slot.disponible) {
      return Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        decoration: BoxDecoration(
          color: const Color(0xFF18181C),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: const Color(0xFF222228)),
        ),
        child: Text(
          slot.horaInicio,
          style: const TextStyle(
            color: Color(0xFF4A4A55),
            fontSize: 13,
            decoration: TextDecoration.lineThrough,
          ),
        ),
      );
    }

    return GestureDetector(
      onTap: () {
        context.read<BookingBloc>().add(BookingTimeSlotSelected(slot));
      },
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        decoration: BoxDecoration(
          color: isSelected ? primaryColor : surfaceColor,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isSelected ? primaryColor : const Color(0xFF333340),
            width: isSelected ? 1.5 : 1,
          ),
        ),
        child: Text(
          slot.horaInicio,
          style: TextStyle(
            color: isSelected ? Colors.black : textColor,
            fontSize: 13,
            fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
          ),
        ),
      ),
    );
  }

  void _showConfirmationModal(BuildContext context, BookingAvailabilityLoaded state) {
    showModalBottomSheet(
      context: context,
      backgroundColor: const Color(0xFF1E1E24),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      builder: (ctx) => Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Resumen de Pre-Reserva',
              style: TextStyle(color: Colors.white, fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            _summaryRow('Servicio', widget.service.nombre),
            _summaryRow('Profesional', state.selectedStylist?.nombreCompleto ?? 'Asignado por salón'),
            _summaryRow('Fecha', DateFormat('EEEE d MMMM yyyy', 'es').format(_selectedDay)),
            _summaryRow('Horario', '${state.selectedSlot!.horaInicio} - ${state.selectedSlot!.horaFin}'),
            _summaryRow('Total a Pagar', widget.service.precioFormateado),
            const SizedBox(height: 24),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () {
                  Navigator.of(ctx).pop();
                  if (state.selectedStylist != null && state.selectedSlot != null) {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => BookingSummaryScreen(
                          service: widget.service,
                          stylist: state.selectedStylist!,
                          selectedDate: _selectedDay,
                          selectedSlot: state.selectedSlot!,
                        ),
                      ),
                    );
                  }
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFFC5A059),
                  foregroundColor: Colors.black,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                ),
                child: const Text('Continuar al Resumen', style: TextStyle(fontWeight: FontWeight.bold)),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _summaryRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(color: Color(0xFFA0A0AB), fontSize: 13)),
          Text(value, style: const TextStyle(color: Colors.white, fontSize: 13, fontWeight: FontWeight.w600)),
        ],
      ),
    );
  }
}

extension DateTimeExtension on DateTime {
  DateTime plusDays(int days) {
    return add(Duration(days: days));
  }
}
