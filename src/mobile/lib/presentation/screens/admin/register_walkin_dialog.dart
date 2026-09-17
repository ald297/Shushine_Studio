import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../data/models/timeline_item_dto.dart';
import '../../blocs/timeline/timeline_bloc.dart';
import '../../blocs/timeline/timeline_event.dart';

class RegisterWalkinDialog extends StatefulWidget {
  final int defaultStylistId;
  final DateTime defaultDate;

  const RegisterWalkinDialog({
    super.key,
    required this.defaultStylistId,
    required this.defaultDate,
  });

  @override
  State<RegisterWalkinDialog> createState() => _RegisterWalkinDialogState();
}

class _RegisterWalkinDialogState extends State<RegisterWalkinDialog> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _notesController = TextEditingController();

  TimeOfDay _selectedTime = TimeOfDay.now();
  String _selectedPayment = 'Efectivo';
  int _selectedServiceId = 1;

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    final formattedHour = '${_selectedTime.hour.toString().padLeft(2, '0')}:${_selectedTime.minute.toString().padLeft(2, '0')}';

    return Dialog(
      backgroundColor: surfaceColor,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Registrar Walk-in',
                    style: TextStyle(
                      color: textColor,
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, color: subtitleColor, size: 20),
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ],
              ),
              const SizedBox(height: 14),

              // Client Name
              TextFormField(
                controller: _nameController,
                style: const TextStyle(color: textColor, fontSize: 14),
                decoration: InputDecoration(
                  labelText: 'Nombre Completo del Cliente',
                  labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                  filled: true,
                  fillColor: cardColor,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'El nombre es requerido';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 12),

              // Phone
              TextFormField(
                controller: _phoneController,
                keyboardType: TextInputType.phone,
                style: const TextStyle(color: textColor, fontSize: 14),
                decoration: InputDecoration(
                  labelText: 'Teléfono de Contacto (Opcional)',
                  labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                  filled: true,
                  fillColor: cardColor,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),
              const SizedBox(height: 12),

              // Service Dropdown
              DropdownButtonFormField<int>(
                initialValue: _selectedServiceId,
                dropdownColor: cardColor,
                style: const TextStyle(color: textColor, fontSize: 14),
                decoration: InputDecoration(
                  labelText: 'Servicio a Realizar',
                  labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                  filled: true,
                  fillColor: cardColor,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
                items: const [
                  DropdownMenuItem(value: 1, child: Text('Corte & Estilo Personalizado (\$25.00)')),
                  DropdownMenuItem(value: 2, child: Text('Balayage & Colorimetría (\$65.00)')),
                  DropdownMenuItem(value: 3, child: Text('Manicura & Pedicura Spa (\$22.00)')),
                  DropdownMenuItem(value: 4, child: Text('Tratamiento Capilar Keratina (\$45.00)')),
                ],
                onChanged: (val) {
                  if (val != null) setState(() => _selectedServiceId = val);
                },
              ),
              const SizedBox(height: 12),

              // Time Picker Row
              ListTile(
                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
                tileColor: cardColor,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                leading: const Icon(Icons.access_time, color: primaryColor),
                title: const Text('Hora de Inicio', style: TextStyle(color: subtitleColor, fontSize: 13)),
                trailing: Text(
                  formattedHour,
                  style: const TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.bold),
                ),
                onTap: () async {
                  final picked = await showTimePicker(
                    context: context,
                    initialTime: _selectedTime,
                  );
                  if (picked != null) setState(() => _selectedTime = picked);
                },
              ),
              const SizedBox(height: 12),

              // Payment Method
              DropdownButtonFormField<String>(
                initialValue: _selectedPayment,
                dropdownColor: cardColor,
                style: const TextStyle(color: textColor, fontSize: 14),
                decoration: InputDecoration(
                  labelText: 'Método de Pago',
                  labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                  filled: true,
                  fillColor: cardColor,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
                items: const [
                  DropdownMenuItem(value: 'Efectivo', child: Text('Efectivo')),
                  DropdownMenuItem(value: 'Tarjeta', child: Text('Tarjeta Débito/Crédito')),
                  DropdownMenuItem(value: 'Transferencia', child: Text('Transferencia Bancaria')),
                ],
                onChanged: (val) {
                  if (val != null) setState(() => _selectedPayment = val);
                },
              ),
              const SizedBox(height: 12),

              // Notes
              TextFormField(
                controller: _notesController,
                maxLines: 2,
                style: const TextStyle(color: textColor, fontSize: 14),
                decoration: InputDecoration(
                  labelText: 'Notas adicionales (Opcional)',
                  labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                  filled: true,
                  fillColor: cardColor,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // Submit Button
              SizedBox(
                width: double.infinity,
                height: 48,
                child: ElevatedButton(
                  onPressed: () {
                    if (_formKey.currentState!.validate()) {
                      final req = CreateWalkinRequest(
                        nombreCliente: _nameController.text.trim(),
                        telefonoCliente: _phoneController.text.trim().isEmpty ? null : _phoneController.text.trim(),
                        estilistaId: widget.defaultStylistId,
                        fechaCita: DateFormat('yyyy-MM-dd').format(widget.defaultDate),
                        horaInicio: formattedHour,
                        servicioIds: [_selectedServiceId],
                        metodoPagoPreferente: _selectedPayment,
                        notas: _notesController.text.trim().isEmpty ? null : _notesController.text.trim(),
                      );
                      context.read<TimelineBloc>().add(RegisterWalkinRequested(req));
                      Navigator.of(context).pop();
                    }
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: primaryColor,
                    foregroundColor: Colors.black,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                  child: const Text('Registrar en Agenda', style: TextStyle(fontWeight: FontWeight.bold)),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
