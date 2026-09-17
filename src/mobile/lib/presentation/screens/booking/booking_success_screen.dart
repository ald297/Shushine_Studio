import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../data/models/appointment_dto.dart';
import '../appointments/my_appointments_screen.dart';

class BookingSuccessScreen extends StatelessWidget {
  final AppointmentDto appointment;

  const BookingSuccessScreen({super.key, required this.appointment});

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
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 20),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const SizedBox(height: 30),

              // Success Icon Badge
              Container(
                width: 90,
                height: 90,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: primaryColor.withValues(alpha: 0.15),
                  border: Border.all(color: primaryColor, width: 2),
                ),
                child: const Icon(
                  Icons.check_circle_rounded,
                  color: primaryColor,
                  size: 54,
                ),
              ),

              const SizedBox(height: 24),

              const Text(
                '¡Cita Confirmada con Éxito!',
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: textColor,
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  letterSpacing: -0.5,
                ),
              ),

              const SizedBox(height: 8),

              const Text(
                'Tu espacio ha sido reservado en el salón. Presenta tu código al llegar.',
                textAlign: TextAlign.center,
                style: TextStyle(color: subtitleColor, fontSize: 14),
              ),

              const SizedBox(height: 28),

              // Appointment Code Card
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
                decoration: BoxDecoration(
                  color: surfaceColor,
                  borderRadius: BorderRadius.circular(18),
                  border: Border.all(color: primaryColor.withValues(alpha: 0.3)),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'CÓDIGO DE RESERVA',
                          style: TextStyle(
                            color: primaryColor,
                            fontSize: 11,
                            letterSpacing: 1.2,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          appointment.codigoCita,
                          style: const TextStyle(
                            color: textColor,
                            fontSize: 22,
                            fontWeight: FontWeight.bold,
                            letterSpacing: 1.0,
                          ),
                        ),
                      ],
                    ),
                    IconButton(
                      icon: const Icon(Icons.copy, color: primaryColor),
                      tooltip: 'Copiar código',
                      onPressed: () {
                        Clipboard.setData(ClipboardData(text: appointment.codigoCita));
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Código copiado al portapapeles'),
                            backgroundColor: primaryColor,
                            behavior: SnackBarBehavior.floating,
                          ),
                        );
                      },
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 24),

              // Details Box
              Container(
                padding: const EdgeInsets.all(20),
                decoration: BoxDecoration(
                  color: surfaceColor,
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Column(
                  children: [
                    _detailRow('Profesional', appointment.estilistaNombre ?? 'Estilista Shushine'),
                    const SizedBox(height: 12),
                    _detailRow('Fecha', appointment.fechaCita),
                    const SizedBox(height: 12),
                    _detailRow('Horario', '${appointment.horaInicio} - ${appointment.horaFin}'),
                    const SizedBox(height: 12),
                    _detailRow('Total', '\$${appointment.total.toStringAsFixed(2)}'),
                    const SizedBox(height: 12),
                    _detailRow('Método de Pago', appointment.metodoPagoPreferente ?? 'Efectivo'),
                  ],
                ),
              ),

              const SizedBox(height: 24),

              // Tip Box
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: cardColor,
                  borderRadius: BorderRadius.circular(14),
                ),
                child: const Row(
                  children: [
                    Icon(Icons.info_outline, color: primaryColor, size: 20),
                    SizedBox(width: 12),
                    Expanded(
                      child: Text(
                        'Por favor presentarse 10 minutos antes de la hora programada.',
                        style: TextStyle(color: subtitleColor, fontSize: 12),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 36),

              // Actions
              SizedBox(
                width: double.infinity,
                height: 52,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).pushReplacement(
                      MaterialPageRoute(builder: (_) => const MyAppointmentsScreen()),
                    );
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: primaryColor,
                    foregroundColor: Colors.black,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                  ),
                  child: const Text(
                    'Ver Mis Citas',
                    style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                  ),
                ),
              ),

              const SizedBox(height: 12),

              SizedBox(
                width: double.infinity,
                height: 52,
                child: OutlinedButton(
                  onPressed: () {
                    Navigator.of(context).popUntil((route) => route.isFirst);
                  },
                  style: OutlinedButton.styleFrom(
                    side: const BorderSide(color: cardColor),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                  ),
                  child: const Text(
                    'Volver al Catálogo',
                    style: TextStyle(color: textColor, fontSize: 15),
                  ),
                ),
              ),

              const SizedBox(height: 20),
            ],
          ),
        ),
      ),
    );
  }

  Widget _detailRow(String label, String value) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: const TextStyle(color: Color(0xFFA0A0AB), fontSize: 13)),
        Text(
          value,
          style: const TextStyle(
            color: Color(0xFFF5F5F7),
            fontSize: 14,
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }
}
