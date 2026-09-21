import 'package:equatable/equatable.dart';

class AppointmentDto extends Equatable {
  final int id;
  final String codigoCita;
  final int? clienteId;
  final String? clienteNombre;
  final String? clienteTelefono;
  final int estilistaId;
  final String? estilistaNombre;
  final String fechaCita;
  final String horaInicio;
  final String horaFin;
  final String estado;
  final double subtotal;
  final double iva;
  final double total;
  final String? metodoPagoPreferente;
  final String? estadoPago;
  final String? notasCliente;
  final List<String> serviciosNombres;

  const AppointmentDto({
    required this.id,
    required this.codigoCita,
    this.clienteId,
    this.clienteNombre,
    this.clienteTelefono,
    required this.estilistaId,
    this.estilistaNombre,
    required this.fechaCita,
    required this.horaInicio,
    required this.horaFin,
    required this.estado,
    required this.subtotal,
    required this.iva,
    required this.total,
    this.metodoPagoPreferente,
    this.estadoPago,
    this.notasCliente,
    this.serviciosNombres = const [],
  });

  bool get esActiva => estado == 'Confirmed' || estado == 'InProgress' || estado == 'Pending';

  factory AppointmentDto.fromJson(Map<String, dynamic> json) {
    return AppointmentDto(
      id: json['id'] as int? ?? 0,
      codigoCita: json['codigoCita']?.toString() ?? '',
      clienteId: json['clienteId'] as int?,
      clienteNombre: json['clienteNombre']?.toString(),
      clienteTelefono: json['clienteTelefono']?.toString(),
      estilistaId: json['estilistaId'] as int? ?? 0,
      estilistaNombre: json['estilistaNombre']?.toString(),
      fechaCita: json['fechaCita']?.toString() ?? '',
      horaInicio: json['horaInicio']?.toString() ?? '',
      horaFin: json['horaFin']?.toString() ?? '',
      estado: json['estado']?.toString() ?? 'Confirmed',
      subtotal: (json['subtotal'] as num?)?.toDouble() ?? 0.0,
      iva: (json['iva'] as num?)?.toDouble() ?? 0.0,
      total: (json['total'] as num?)?.toDouble() ?? 0.0,
      metodoPagoPreferente: json['metodoPagoPreferente']?.toString(),
      estadoPago: json['estadoPago']?.toString(),
      notasCliente: json['notasCliente']?.toString(),
      serviciosNombres: (json['serviciosNombres'] as List<dynamic>?)
              ?.map((e) => e.toString())
              .toList() ??
          const [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'codigoCita': codigoCita,
      'clienteId': clienteId,
      'clienteNombre': clienteNombre,
      'clienteTelefono': clienteTelefono,
      'estilistaId': estilistaId,
      'estilistaNombre': estilistaNombre,
      'fechaCita': fechaCita,
      'horaInicio': horaInicio,
      'horaFin': horaFin,
      'estado': estado,
      'subtotal': subtotal,
      'iva': iva,
      'total': total,
      'metodoPagoPreferente': metodoPagoPreferente,
      'estadoPago': estadoPago,
      'notasCliente': notasCliente,
      'serviciosNombres': serviciosNombres,
    };
  }

  @override
  List<Object?> get props => [
        id,
        codigoCita,
        clienteId,
        clienteNombre,
        estilistaId,
        fechaCita,
        horaInicio,
        horaFin,
        estado,
        total,
      ];
}

class CreateAppointmentRequest extends Equatable {
  final int estilistaId;
  final String fechaCita;
  final String horaInicio;
  final List<int> servicioIds;
  final String metodoPagoPreferente;
  final String? notasCliente;

  const CreateAppointmentRequest({
    required this.estilistaId,
    required this.fechaCita,
    required this.horaInicio,
    required this.servicioIds,
    this.metodoPagoPreferente = 'Efectivo',
    this.notasCliente,
  });

  Map<String, dynamic> toJson() {
    return {
      'estilistaId': estilistaId,
      'fechaCita': fechaCita,
      'horaInicio': horaInicio.length == 5 ? '$horaInicio:00' : horaInicio,
      'servicioIds': servicioIds,
      'metodoPagoPreferente': metodoPagoPreferente,
      'notasCliente': notasCliente,
    };
  }

  @override
  List<Object?> get props => [
        estilistaId,
        fechaCita,
        horaInicio,
        servicioIds,
        metodoPagoPreferente,
        notasCliente,
      ];
}
