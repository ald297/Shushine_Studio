import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../data/models/appointment_dto.dart';
import '../../../data/models/service_dto.dart';
import '../../../data/models/stylist_dto.dart';
import '../../blocs/booking/booking_bloc.dart';
import '../../blocs/booking/booking_event.dart';
import '../../blocs/booking/booking_state.dart';
import 'booking_success_screen.dart';

class BookingSummaryScreen extends StatefulWidget {
  final ServiceDto service;
  final StylistDto stylist;
  final DateTime selectedDate;
  final TimeSlotDto selectedSlot;

  const BookingSummaryScreen({
    super.key,
    required this.service,
    required this.stylist,
    required this.selectedDate,
    required this.selectedSlot,
  });

  @override
  State<BookingSummaryScreen> createState() => _BookingSummaryScreenState();
}

class _BookingSummaryScreenState extends State<BookingSummaryScreen> {
  final TextEditingController _notesController = TextEditingController();
  String _selectedPaymentMethod = 'Efectivo';

  @override
  void dispose() {
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    final double subtotal = widget.service.precioBase;
    final double iva = (subtotal * 0.13 * 100).round() / 100;
    final double total = subtotal + iva;

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
          'Resumen de Cita',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: BlocConsumer<BookingBloc, BookingState>(
        listener: (context, state) {
          if (state is BookingCreationSuccess) {
            Navigator.of(context).pushReplacement(
              MaterialPageRoute(
                builder: (_) => BookingSuccessScreen(appointment: state.appointment),
              ),
            );
          } else if (state is BookingCreationFailure) {
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
          final isSubmitting = state is BookingSubmitting;

          return SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Stylist & Service Card
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: surfaceColor,
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: primaryColor.withValues(alpha: 0.2)),
                  ),
                  child: Column(
                    children: [
                      Row(
                        children: [
                          CircleAvatar(
                            radius: 28,
                            backgroundColor: primaryColor.withValues(alpha: 0.2),
                            backgroundImage: widget.stylist.avatarUrl != null
                                ? NetworkImage(widget.stylist.avatarUrl!)
                                : null,
                            child: widget.stylist.avatarUrl == null
                                ? const Icon(Icons.person, color: primaryColor, size: 30)
                                : null,
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  widget.stylist.nombreCompleto,
                                  style: const TextStyle(
                                    color: textColor,
                                    fontSize: 17,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  widget.stylist.especialidadPrincipal,
                                  style: const TextStyle(
                                    color: primaryColor,
                                    fontSize: 13,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      const Divider(color: cardColor, height: 1),
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          const Icon(Icons.content_cut, color: primaryColor, size: 18),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Text(
                              widget.service.nombre,
                              style: const TextStyle(
                                color: textColor,
                                fontSize: 15,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ),
                          Text(
                            widget.service.duracionFormateada,
                            style: const TextStyle(color: subtitleColor, fontSize: 13),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 18),

                // Date & Time Box
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: surfaceColor,
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Column(
                    children: [
                      Row(
                        children: [
                          const Icon(Icons.calendar_month, color: primaryColor, size: 20),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Text(
                              DateFormat('EEEE d MMMM, yyyy', 'es').format(widget.selectedDate),
                              style: const TextStyle(
                                color: textColor,
                                fontSize: 14,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          const Icon(Icons.access_time_filled, color: primaryColor, size: 20),
                          const SizedBox(width: 12),
                          Text(
                            '${widget.selectedSlot.horaInicio} - ${widget.selectedSlot.horaFin}',
                            style: const TextStyle(
                              color: textColor,
                              fontSize: 14,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 22),

                // Payment Method Selector
                const Text(
                  'Método de Pago Preferente',
                  style: TextStyle(
                    color: textColor,
                    fontSize: 15,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    _paymentMethodChip('Efectivo', Icons.attach_money),
                    const SizedBox(width: 10),
                    _paymentMethodChip('Tarjeta', Icons.credit_card),
                    const SizedBox(width: 10),
                    _paymentMethodChip('Puntos', Icons.star),
                  ],
                ),

                const SizedBox(height: 22),

                // Client Notes
                const Text(
                  'Notas o requerimientos especiales',
                  style: TextStyle(
                    color: textColor,
                    fontSize: 15,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 10),
                TextField(
                  controller: _notesController,
                  maxLines: 2,
                  style: const TextStyle(color: textColor, fontSize: 14),
                  decoration: InputDecoration(
                    hintText: 'Ej. Alergia a tintes, cabello procesado...',
                    hintStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                    filled: true,
                    fillColor: surfaceColor,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: BorderSide.none,
                    ),
                  ),
                ),

                const SizedBox(height: 24),

                // Cost Breakdown
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: surfaceColor,
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: primaryColor.withValues(alpha: 0.2)),
                  ),
                  child: Column(
                    children: [
                      _costRow('Subtotal del servicio', '\$${subtotal.toStringAsFixed(2)}'),
                      const SizedBox(height: 8),
                      _costRow('IVA (13% El Salvador)', '\$${iva.toStringAsFixed(2)}'),
                      const SizedBox(height: 12),
                      const Divider(color: cardColor, height: 1),
                      const SizedBox(height: 12),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Total a Pagar',
                            style: TextStyle(
                              color: textColor,
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          Text(
                            '\$${total.toStringAsFixed(2)}',
                            style: const TextStyle(
                              color: primaryColor,
                              fontSize: 20,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 28),

                // Confirm CTA Button
                SizedBox(
                  width: double.infinity,
                  height: 54,
                  child: ElevatedButton(
                    onPressed: isSubmitting
                        ? null
                        : () {
                            final dateStr = DateFormat('yyyy-MM-dd').format(widget.selectedDate);
                            final req = CreateAppointmentRequest(
                              estilistaId: widget.stylist.id,
                              fechaCita: dateStr,
                              horaInicio: widget.selectedSlot.horaInicio,
                              servicioIds: [widget.service.id],
                              metodoPagoPreferente: _selectedPaymentMethod,
                              notasCliente: _notesController.text.trim().isEmpty
                                  ? null
                                  : _notesController.text.trim(),
                            );
                            context.read<BookingBloc>().add(BookingCreateSubmitted(req));
                          },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: primaryColor,
                      foregroundColor: Colors.black,
                      elevation: 0,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                      disabledBackgroundColor: primaryColor.withValues(alpha: 0.4),
                    ),
                    child: isSubmitting
                        ? const SizedBox(
                            width: 24,
                            height: 24,
                            child: CircularProgressIndicator(color: Colors.black, strokeWidth: 2.5),
                          )
                        : const Text(
                            'Confirmar Reserva',
                            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                          ),
                  ),
                ),
                const SizedBox(height: 30),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _paymentMethodChip(String method, IconData icon) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    final isSelected = _selectedPaymentMethod == method;

    return Expanded(
      child: GestureDetector(
        onTap: () => setState(() => _selectedPaymentMethod = method),
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 12),
          decoration: BoxDecoration(
            color: isSelected ? primaryColor.withValues(alpha: 0.15) : surfaceColor,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(
              color: isSelected ? primaryColor : Colors.transparent,
              width: 1.5,
            ),
          ),
          child: Column(
            children: [
              Icon(icon, color: isSelected ? primaryColor : const Color(0xFFA0A0AB), size: 20),
              const SizedBox(height: 4),
              Text(
                method,
                style: TextStyle(
                  color: isSelected ? primaryColor : Colors.white70,
                  fontSize: 12,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _costRow(String title, String amount) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(title, style: const TextStyle(color: Color(0xFFA0A0AB), fontSize: 14)),
        Text(amount, style: const TextStyle(color: Colors.white, fontSize: 14, fontWeight: FontWeight.w500)),
      ],
    );
  }
}
